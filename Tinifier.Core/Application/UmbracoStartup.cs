using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Repository.Section;
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

namespace Tinifier.Core.Application
{
    public class UmbracoStartup : ApplicationEventHandler
    {
        private readonly IStatisticService _statisticService;
        private readonly ISettingsService _settingsService;
        private readonly IImageService _imageService;
        private readonly IHistoryService _historyService;
        private readonly IMediaService _mediaService;
        private readonly IImageCropperInfoService _imageCropperInfoService;

        private readonly ITSectionRepo _sectionRepo;

        private static readonly object padlock = new object();

        public UmbracoStartup()
        {
            _statisticService = new StatisticService();
            _settingsService = new SettingsService();
            _imageService = new ImageService();
            _historyService = new HistoryService();
            _sectionRepo = new TSectionRepo();
            _imageCropperInfoService = new ImageCropperInfoService();
            _mediaService = ApplicationContext.Current.Services.MediaService;            
        }

        /// <summary>
        /// Add custom section and event handlers 
        /// </summary>
        /// <param name="umbraco">UmbracoApplicationBase</param>
        /// <param name="context">ApplicationContext</param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbraco, ApplicationContext context)
        {
            CreateTinifySection(context);
            TreeControllerBase.MenuRendering += MenuRenderingHandler;
            MediaService.Saved += MediaService_Saved;
            MediaService.Saving += MediaService_Saving;
            ContentService.Saving += ContentService_Saving;

            MediaService.EmptiedRecycleBin += MediaService_EmptiedRecycleBin;
            InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;
            InstalledPackage.BeforeSave += InstalledPackage_BeforeSave;
        }

        #region Media

        private void ContentService_Saving(IContentService sender, SaveEventArgs<IContent> e)
        {
            var entities = e.SavedEntities;

            var settingService = _settingsService.GetSettings();
            if (settingService == null || !settingService.EnableCropsOptimization)
            {
                _statisticService.UpdateStatistic();
                return;
            }
                
            foreach (var entity in entities)
            {
                var imageCroppers = entity.Properties.Where(x => x.PropertyType.PropertyEditorAlias == Constants.PropertyEditors.ImageCropperAlias);

                foreach (var crop in imageCroppers)
                {
                    var key = string.Concat(entity.Name, "-", crop.Alias);
                    var imageCropperInfo = _imageCropperInfoService.Get(key);
                    var imagePath = crop.Value;

                    //Delete
                    if(imageCropperInfo != null && imagePath == null)
                    {
                        _imageCropperInfoService.Delete(key);

                        var pathForFolder = imageCropperInfo.ImageId.Remove(imageCropperInfo.ImageId.LastIndexOf('/') + 1);
                        var histories = _historyService.GetHistoryByPath(pathForFolder);

                        foreach(var history in histories)
                        {
                            _historyService.Delete(history.ImageId);
                        }
                        
                        _statisticService.UpdateStatistic();
                    }

                    //Update
                    if(imageCropperInfo != null && imagePath != null)
                    {
                        var json = JObject.Parse(imagePath.ToString());
                        var path = json.GetValue("src").ToString();

                        if (string.IsNullOrEmpty(path))
                            throw new Infrastructure.Exceptions.EntityNotFoundException();

                        var fileExt = Path.GetExtension(path).ToUpper().Replace(".", string.Empty).Trim();
                        if (!PackageConstants.SupportedExtensions.Contains(fileExt))
                            throw new NotSupportedExtensionException(fileExt);

                        var pathForFolder = path.Remove(path.LastIndexOf('/') + 1);
                        var serverPathForFolder = System.Web.HttpContext.Current.Server.MapPath(pathForFolder);

                        var di = new DirectoryInfo(serverPathForFolder);
                        var files = di.GetFiles();

                        foreach (var file in files)
                        {
                            TImage image = new TImage
                            {
                                Id = Path.Combine(pathForFolder, file.Name),
                                Name = file.Name,
                                AbsoluteUrl = Path.Combine(pathForFolder, file.Name)
                            };

                            var imageHistory = _historyService.GetImageHistory(image.Id);
                            if (imageHistory != null && imageHistory.IsOptimized)
                                continue;

                            _imageService.OptimizeImageAsync(image).GetAwaiter().GetResult();
                        }

                        var histories = _historyService.GetHistoryByPath(pathForFolder);

                        foreach (var history in histories)
                        {
                            _historyService.Delete(history.ImageId);
                        }

                        _imageCropperInfoService.Update(key, path);
                        _statisticService.UpdateStatistic();
                    }

                    //Create
                    if(imageCropperInfo == null && imagePath != null)
                    {
                        var json = JObject.Parse(imagePath.ToString());
                        var path = json.GetValue("src").ToString();

                        if (string.IsNullOrEmpty(path))
                            throw new Infrastructure.Exceptions.EntityNotFoundException();

                        var fileExt = Path.GetExtension(path).ToUpper().Replace(".", string.Empty).Trim();
                        if (!PackageConstants.SupportedExtensions.Contains(fileExt))
                            throw new NotSupportedExtensionException(fileExt);

                        var pathForFolder = path.Remove(path.LastIndexOf('/') + 1);
                        var serverPathForFolder = System.Web.HttpContext.Current.Server.MapPath(pathForFolder);

                        var di = new DirectoryInfo(serverPathForFolder);
                        var files = di.GetFiles();

                        foreach (var file in files)
                        {
                            TImage image = new TImage
                            {
                                Id = Path.Combine(pathForFolder, file.Name),
                                Name = file.Name,
                                AbsoluteUrl = Path.Combine(pathForFolder, file.Name)
                            };

                            var imageHistory = _historyService.GetImageHistory(image.Id);
                            if (imageHistory != null && imageHistory.IsOptimized)
                                continue;

                            _imageService.OptimizeImageAsync(image).GetAwaiter().GetResult();
                        }

                        _imageCropperInfoService.Create(key, path);
                        _statisticService.UpdateStatistic();
                    }
                }
            }
        }

        private void MediaService_Saving(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            // reupload image issue https://goo.gl/ad8pTs
            HandleMedia(e.SavedEntities,
                    (m) => _historyService.Delete(m.Id.ToString()),
                    (m) => m.IsPropertyDirty(PackageConstants.UmbracoFileAlias));
        }

        private void MediaService_Saved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
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
            if (e.Ids.Count() > 0)
                _statisticService.UpdateStatistic();
        }


        private void HandleMedia(IEnumerable<IMedia> items, Action<IMedia> action, Func<IMedia, bool> predicate = null)
        {
            bool isChanged = false;
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

            var imageHistory = _historyService.GetImageHistory(image.Id.ToString());

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
                    var checkColumn = new Sql(@"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE 
                                                TABLE_NAME = 'TinifierImagesStatistic' AND COLUMN_NAME = 'TotalSavedBytes'");
                    var checkHidePanel = new Sql(@"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE
                                                TABLE_NAME = 'TinifierUserSettings' AND COLUMN_NAME = 'HideLeftPanel'");
                    var checkMetaData = new Sql(@"SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE 
                                                TABLE_NAME = 'TinifierUserSettings' AND COLUMN_NAME = 'PreserveMetadata'");
                    int? exists = ApplicationContext.Current.DatabaseContext.Database.ExecuteScalar<int?>(checkColumn);
                    int? hidePanel = ApplicationContext.Current.DatabaseContext.Database.ExecuteScalar<int?>(checkHidePanel);
                    int? metaData = ApplicationContext.Current.DatabaseContext.Database.ExecuteScalar<int?>(checkMetaData);

                    if (exists == null || exists == -1)
                        ApplicationContext.Current.DatabaseContext.Database.Execute
                            (new Sql("ALTER TABLE TinifierImagesStatistic ADD COLUMN TotalSavedBytes bigint"));

                    if (hidePanel == null || hidePanel == -1)
                        ApplicationContext.Current.DatabaseContext.Database.Execute
                            (new Sql("ALTER TABLE TinifierUserSettings ADD COLUMN HideLeftPanel bit not null default(0)"));

                    if(metaData == null || metaData == -1)
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

                var menuItemSettingsButton = new MenuItem(PackageConstants.StatsButton, PackageConstants.StatsButtonCaption);
                menuItemSettingsButton.LaunchDialogView(PackageConstants.TinySettingsRoute, PackageConstants.StatsDialogCaption);
                menuItemSettingsButton.Icon = PackageConstants.MenuSettingsIcon;
                e.Menu.Items.Add(menuItemSettingsButton);
            }
        }

    }
}

