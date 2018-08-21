using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Xml;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.FileSystemProvider;
using Tinifier.Core.Repository.Section;
using Tinifier.Core.Services;
using Tinifier.Core.Services.History;
using Tinifier.Core.Services.ImageCropperInfo;
using Tinifier.Core.Services.Media;
using Tinifier.Core.Services.Settings;
using Tinifier.Core.Services.Statistic;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;
using Umbraco.Web.UI.JavaScript;

namespace Tinifier.Core.Application
{
    public class UmbracoStartup : ApplicationEventHandler
    {
        private readonly IStatisticService _statisticService;
        private readonly ISettingsService _settingsService;
        private readonly IImageService _imageService;
        private readonly IHistoryService _historyService;
        private readonly IImageCropperInfoService _imageCropperInfoService;
        private readonly ITSectionRepo _sectionRepo;
        private readonly IFileSystemProviderRepository _fileSystemProviderRepository;

        public UmbracoStartup()
        {
            _statisticService = new StatisticService();
            _settingsService = new SettingsService();
            _imageService = new ImageService();
            _historyService = new HistoryService();
            _sectionRepo = new TSectionRepo();
            _imageCropperInfoService = new ImageCropperInfoService();
            _fileSystemProviderRepository = new TFileSystemProviderRepository();
        }

        /// <summary>
        /// Add custom section and event handlers 
        /// </summary>
        /// <param name="umbraco">UmbracoApplicationBase</param>
        /// <param name="context">ApplicationContext</param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbraco, ApplicationContext context)
        {
            CreateTinifySection(context);
            SetFileSystemProvider();
            TreeControllerBase.MenuRendering += MenuRenderingHandler;
            MediaService.Saved += MediaService_Saved;
            MediaService.Saving += MediaService_Saving;
            ContentService.Saving += ContentService_Saving;

            MediaService.EmptiedRecycleBin += MediaService_EmptiedRecycleBin;
            InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;
            InstalledPackage.BeforeSave += InstalledPackage_BeforeSave;
            ServerVariablesParser.Parsing += Parsing;  
        }

        #region ImageCropper

        private void ContentService_Saving(IContentService sender, SaveEventArgs<IContent> e)
        {
            var settingService = _settingsService.GetSettings();
            if (settingService == null)
                return;

            foreach (var entity in e.SavedEntities)
            {
                var imageCroppers = entity.Properties.Where(x => x.PropertyType.PropertyEditorAlias ==
                                                                 Constants.PropertyEditors.ImageCropperAlias);

                foreach (var crop in imageCroppers)
                {
                    var key = string.Concat(entity.Name, "-", crop.Alias);
                    var imageCropperInfo = _imageCropperInfoService.Get(key);
                    var imagePath = crop.Value;

                    //Wrong object
                    if (imageCropperInfo == null && imagePath == null)
                        continue;

                    //Cropped file was Deleted
                    if (imageCropperInfo != null && imagePath == null)
                    {
                        _imageCropperInfoService.DeleteImageFromImageCropper(key, imageCropperInfo);
                        continue;
                    }

                    var json = JObject.Parse(imagePath.ToString());
                    var path = json.GetValue("src").ToString();

                    //republish existed content
                    if (imageCropperInfo != null && imageCropperInfo.ImageId == path)
                        continue;

                    //Cropped file was created or updated
                    _imageCropperInfoService.GetCropImagesAndTinify(key, imageCropperInfo, imagePath,
                        settingService.EnableOptimizationOnUpload, path);
                }
            }
        }

        #endregion ImageCropper

        #region Media
        private void MediaService_Saving(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            MediaSavingHelper.IsSavingInProgress = true;
            // reupload image issue https://goo.gl/ad8pTs
            HandleMedia(e.SavedEntities,
                    (m) => _historyService.Delete(m.Id.ToString()),
                    (m) => m.IsPropertyDirty(PackageConstants.UmbracoFileAlias));
        }

        private void MediaService_Saved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            MediaSavingHelper.IsSavingInProgress = false;
            // optimize on upload
            var settingService = _settingsService.GetSettings();
            if (settingService == null || settingService.EnableOptimizationOnUpload == false)
                return;

            HandleMedia(e.SavedEntities,
                (m) =>
                {
                    try
                    {
                        OptimizeOnUploadAsync(m.Id, e).GetAwaiter().GetResult();
                    }
                    catch (NotSupportedExtensionException)
                    { }
                });
        }

        /// <summary>
        /// Update number of images in statistic before removing from recyclebin
        /// </summary>
        /// <param name="sender">IMediaService</param>
        /// <param name="e">RecycleBinEventArgs</param>
        private void MediaService_EmptiedRecycleBin(IMediaService sender, RecycleBinEventArgs e)
        {
            foreach (var id in e.Ids)
            {
                _historyService.Delete(id.ToString());
            }
            if (e.Ids.Any())
                _statisticService.UpdateStatistic(e.Ids.Count());
        }

        private void HandleMedia(IEnumerable<IMedia> items, Action<IMedia> action, Func<IMedia, bool> predicate = null)
        {
            var isChanged = false;
            foreach (var item in items)
            {
                if (string.Equals(item.ContentType.Alias, PackageConstants.ImageAlias, StringComparison.OrdinalIgnoreCase))
                {
                    if (action != null && (predicate == null || predicate(item)))
                    {
                        action(item);
                        isChanged = true;
                    }
                }
            }
            if (isChanged)
                _statisticService.UpdateStatistic();
        }

        /// <summary>
        /// Call methods for tinifing when upload image
        /// </summary>
        /// <param name="mediaItemId">Media Item Id</param>
        /// <param name="e">CancellableEventArgs</param>
        private async System.Threading.Tasks.Task OptimizeOnUploadAsync(int mediaItemId, CancellableEventArgs e)
        {
            TImage image;

            try
            {
                image = _imageService.GetImage(mediaItemId);
            }
            catch (NotSupportedExtensionException ex)
            {
                e.Messages.Add(new EventMessage(PackageConstants.ErrorCategory, ex.Message,
                    EventMessageType.Error));
                throw;
            }

            var imageHistory = _historyService.GetImageHistory(image.Id);

            if (imageHistory == null)
                await _imageService.OptimizeImageAsync(image).ConfigureAwait(false);
        }

        #endregion

        #region Package

        public void InstalledPackage_BeforeSave(InstalledPackage sender, EventArgs e)
        {
            CheckFieldsDatabase();
        }

        /// <summary>
        /// Remove section and clear tabs in dashboard.config before deleting package
        /// </summary>
        /// <param name="sender">InstalledPackage</param>
        /// <param name="e">EventArgs</param>
        private void InstalledPackage_BeforeDelete(InstalledPackage sender, EventArgs e)
        {
            if (string.Equals(sender.Data.Name, PackageConstants.SectionAlias, StringComparison.OrdinalIgnoreCase))
            {
                DashboardExtension.ClearTabs();
                var section = ApplicationContext.Current.Services.SectionService.GetByAlias(PackageConstants.SectionAlias);

                if (section != null)
                    ApplicationContext.Current.Services.SectionService.DeleteSection(section);
            }
        }

        private void CheckFieldsDatabase()
        {
            var logger = LoggerResolver.Current.Logger;
            var dbContext = ApplicationContext.Current.DatabaseContext;
            var dbHelper = new DatabaseSchemaHelper(dbContext.Database, logger, dbContext.SqlSyntax);

            if (dbHelper.TableExist(PackageConstants.DbStateTable))
            {
                dbHelper.DropTable(PackageConstants.DbStateTable);
                dbHelper.CreateTable(false, typeof(TState));
            }

            var tables = new Dictionary<string, Type>
            {
                { PackageConstants.DbStatisticTable, typeof(TImageStatistic) },
            };

            for (var i = 0; i < tables.Count; i++)
            {
                if (dbHelper.TableExist(tables.ElementAt(i).Key))
                {
                    var checkColumn = new Sql("SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TinifierImagesStatistic' AND COLUMN_NAME = 'TotalSavedBytes'");
                    var checkHidePanel = new Sql("SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TinifierUserSettings' AND COLUMN_NAME = 'HideLeftPanel'");
                    var checkMetaData = new Sql(@"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE 
                                                TABLE_NAME = 'TinifierUserSettings' AND COLUMN_NAME = 'PreserveMetadata'");
                    var exists = ApplicationContext.Current.DatabaseContext.Database.ExecuteScalar<int?>(checkColumn);
                    var hidePanel = ApplicationContext.Current.DatabaseContext.Database.ExecuteScalar<int?>(checkHidePanel);
                    var metaData = ApplicationContext.Current.DatabaseContext.Database.ExecuteScalar<int?>(checkMetaData);


                    if (exists == null || exists == -1)
                    {
                        var sql = new Sql("ALTER TABLE TinifierImagesStatistic ADD COLUMN TotalSavedBytes INTEGER NULL");
                        ApplicationContext.Current.DatabaseContext.Database.Execute(sql);
                    }

                    if (hidePanel == null || hidePanel == -1)
                    {
                        var sql = new Sql("ALTER TABLE TinifierUserSettings ADD COLUMN HideLeftPanel BIT NOT NULL");
                        ApplicationContext.Current.DatabaseContext.Database.Execute(sql);
                    }

                    if (metaData == null || metaData == -1)
                        ApplicationContext.Current.DatabaseContext.Database.Execute
                            (new Sql("ALTER TABLE TinifierUserSettings ADD COLUMN PreserveMetadata bit not null default(0)"));

                }
            }
        }

        /// <summary>
        /// Create a new section
        /// </summary>
        /// <param name="context">ApplicationContext</param>
        private void CreateTinifySection(ApplicationContext context)
        {
            var section = context.Services.SectionService.GetByAlias(PackageConstants.SectionAlias);

            if (section == null)
            {
                context.Services.SectionService.MakeNew(PackageConstants.SectionName,
                    PackageConstants.SectionAlias,
                    PackageConstants.SectionIcon);

                DashboardExtension.AddTabs();
            }

            _sectionRepo.AssignTinifierToAdministrators();
        }

        #endregion

        /// <summary>
        /// Extend dropdownMenu with Tinify and Stats buttons
        /// </summary>
        /// <param name="sender">TreeControllerBase</param>
        /// <param name="e">EventArgs</param>
        private void MenuRenderingHandler(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if (string.Equals(sender.TreeAlias, PackageConstants.MediaAlias, StringComparison.OrdinalIgnoreCase))
            {
                var menuItemTinifyButton = new MenuItem(PackageConstants.TinifierButton, PackageConstants.TinifierButtonCaption);
                menuItemTinifyButton.LaunchDialogView(PackageConstants.TinyTImageRoute, PackageConstants.SectionName);
                menuItemTinifyButton.Icon = PackageConstants.MenuIcon;
                e.Menu.Items.Add(menuItemTinifyButton);

            /*    var menuItemUndoTinifyButton = new MenuItem(PackageConstants., PackageConstants.);
                menuItemUndoTinifyButton.LaunchDialogView(PackageConstants., PackageConstants.);
                menuItemUndoTinifyButton.Icon = PackageConstants.;
                e.Menu.Items.Add(menuItemUndoTinifyButton);*/

                var menuItemSettingsButton = new MenuItem(PackageConstants.StatsButton, PackageConstants.StatsButtonCaption);
                menuItemSettingsButton.LaunchDialogView(PackageConstants.TinySettingsRoute, PackageConstants.StatsDialogCaption);
                menuItemSettingsButton.Icon = PackageConstants.MenuSettingsIcon;
                e.Menu.Items.Add(menuItemSettingsButton);

                var menuItemOrganizeImagesButton = new MenuItem(PackageConstants.OrganizeImagesButton, PackageConstants.OrganizeImagesCaption);
                menuItemOrganizeImagesButton.LaunchDialogView(PackageConstants.OrganizeImagesRoute, PackageConstants.OrganizeImagesCaption);
                e.Menu.Items.Add(menuItemOrganizeImagesButton);
            }
        }

        private void Parsing(object sender, Dictionary<string, object> dictionary)
        {
            var umbracoPath = WebConfigurationManager.AppSettings["umbracoPath"];

            var apiRoot =$"{umbracoPath.Substring(1)}/backoffice/api/";

            var urls = dictionary["umbracoUrls"] as Dictionary<string, object>;
            urls["tinifierApiRoot"] = apiRoot;
        }

        private void SetFileSystemProvider()
        {
            var path = HostingEnvironment.MapPath("~/config/FileSystemProviders.config");
            var doc = new XmlDocument();
            doc.Load(path);
            var node = doc.SelectSingleNode("//Provider");

            var nodeType = node?.Attributes?.GetNamedItem("type");
            if (nodeType != null)
            {
                _fileSystemProviderRepository.Delete();
                _fileSystemProviderRepository.Create(nodeType.Value);
            }
        }
    } 
}
