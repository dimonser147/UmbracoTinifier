using System;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Infrastructure.Exceptions;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Services.History;
using Tinifier.Core.Services.Media;
using Tinifier.Core.Services.Settings;
using Tinifier.Core.Services.Statistic;
using umbraco.cms.businesslogic.packager;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
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

        public UmbracoStartup()
        {
            _statisticService = new StatisticService();
            _settingsService = new SettingsService();
            _imageService = new ImageService();
            _historyService = new HistoryService();
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
            MediaService.Saved += MediaServiceSaving;
            MediaService.EmptiedRecycleBin += MediaService_EmptiedRecycleBin;
            InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;
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

        /// <summary>
        /// Update statistic when image upload and image optimizing during upload
        /// </summary>
        /// <param name="sender">IMediaService</param>
        /// <param name="eventArg">SaveEventArgs</param>
        private void MediaServiceSaving(IMediaService sender, SaveEventArgs<IMedia> eventArg)
        {
            var settings = _settingsService.GetSettings();

            foreach (var mediaItem in eventArg.SavedEntities)
            {
                if (!string.IsNullOrEmpty(mediaItem.ContentType.Alias) 
                    && string.Equals(mediaItem.ContentType.Alias, PackageConstants.ImageAlias, StringComparison.OrdinalIgnoreCase))
                {
                    if (settings != null && settings.EnableOptimizationOnUpload)
                    {
                        try
                        {
                            OptimizeOnUpload(mediaItem.Id, eventArg);
                        }
                        catch(NotSupportedExtensionException)
                        {
                            _statisticService.UpdateStatistic();
                            continue;
                        }
                    }
                    else
                    {
                        _statisticService.UpdateStatistic();
                    }                                                          
                }
            }
        }

        /// <summary>
        /// Update number of images in statistic before removing from recyclebin
        /// </summary>
        /// <param name="sender">IMediaService</param>
        /// <param name="e">RecycleBinEventArgs</param>
        private void MediaService_EmptiedRecycleBin(IMediaService sender, RecycleBinEventArgs e)
        {
            var iMedias = ApplicationContext.Current.Services.MediaService.GetByIds(e.Ids);

            foreach (var mediaItem in iMedias)
            {
                if (!string.IsNullOrEmpty(mediaItem.ContentType.Alias) 
                    && string.Equals(mediaItem.ContentType.Alias, PackageConstants.ImageAlias, StringComparison.OrdinalIgnoreCase))
                {
                    _statisticService.UpdateStatistic(0);
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
                context.Services.UserService.AddSectionToAllUsers(PackageConstants.SectionAlias);

                DashboardExtension.AddTabs();
            }
        }

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

        /// <summary>
        /// Call methods for tinifing when upload image
        /// </summary>
        /// <param name="mediaItemId">Media Item Id</param>
        /// <param name="e">CancellableEventArgs</param>
        private void OptimizeOnUpload(int mediaItemId, CancellableEventArgs e)
        {
            TImage image;

            try
            {
                image = _imageService.GetImage(mediaItemId);
            }
            catch (NotSupportedExtensionException)
            {
                e.Messages.Add(new EventMessage(PackageConstants.ErrorCategory, PackageConstants.NotSupported,
                    EventMessageType.Error));
                throw;
            }

            var imageHistory = _historyService.GetImageHistory(image.Id);

            if (imageHistory == null)
                    _imageService.OptimizeImage(image);               
            }                
        }
}

