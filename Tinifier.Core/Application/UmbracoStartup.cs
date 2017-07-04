using System;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Models.Db;
using Tinifier.Core.Services.Interfaces;
using Tinifier.Core.Services.Services;
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

        protected override void ApplicationStarted(UmbracoApplicationBase umbraco, ApplicationContext context)
        {
            // Set statistic before optimizing
            _statisticService.CreateStatistic();

            // Create a new section
            CreateTinifySection(context);

            // Extend dropdownMenu with Tinify button
            TreeControllerBase.MenuRendering += MenuRenderingHandler;

            // Save image handler for updating number of Images in database and tinifing on upload
            MediaService.Saved += MediaServiceSaving;

            // Delete image handler for updating number of Images in database
            MediaService.EmptiedRecycleBin += MediaService_EmptiedRecycleBin;

            // Clear dashboard.config before deleting
            InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;
        }

        // Remove section and clear tabs before deleting package
        private void InstalledPackage_BeforeDelete(InstalledPackage sender, EventArgs e)
        {
            if (string.Equals(sender.Data.Name, "tinifier", StringComparison.OrdinalIgnoreCase))
            {
                ExtendDashboard.ClearTabs();

                var section = ApplicationContext.Current.Services.SectionService.GetByAlias(PackageConstants.SectionAlias);

                if (section != null)
                {
                    ApplicationContext.Current.Services.SectionService.DeleteSection(section);
                }
            }
        }

        // Update statistic when image upload and optimize if flag is true
        private void MediaServiceSaving(IMediaService sender, SaveEventArgs<IMedia> eventArg)
        {
            foreach (var mediaItem in eventArg.SavedEntities)
            {
                if (!string.IsNullOrEmpty(mediaItem.ContentType.Alias) && string.Equals(mediaItem.ContentType.Alias, "image", StringComparison.OrdinalIgnoreCase))
                {
                    var settings = _settingsService.GetSettings();

                    if (settings != null)
                    {
                        if(settings.EnableOptimizationOnUpload)
                        {
                            try
                            {
                                OptimizeOnUpload(mediaItem.Id, eventArg);
                            }
                            catch(Infrastructure.Exceptions.NotSupportedException)
                            {
                                continue;
                            }
                        }                                       
                    }

                    _statisticService.UpdateStatistic();
                }
            }
        }

        // Update statistic when recycle bin is empty
        private void MediaService_EmptiedRecycleBin(IMediaService sender, RecycleBinEventArgs e)
        {
            var iMedias = ApplicationContext.Current.Services.MediaService.GetByIds(e.Ids);

            foreach (var mediaItem in iMedias)
            {
                if (!string.IsNullOrEmpty(mediaItem.ContentType.Alias) && string.Equals(mediaItem.ContentType.Alias, "image", StringComparison.OrdinalIgnoreCase))
                {
                    _statisticService.UpdateStatistic();
                }
            }
        }

        // Create a new section
        private void CreateTinifySection(ApplicationContext context)
        {
            var section = context.Services.SectionService.GetByAlias(PackageConstants.SectionAlias);

            if (section == null)
            {
                context.Services.SectionService.MakeNew(PackageConstants.SectionName,
                    PackageConstants.SectionAlias,
                    PackageConstants.SectionIcon);
                context.Services.UserService.AddSectionToAllUsers(PackageConstants.SectionAlias);

                ExtendDashboard.AddTabs();
            }
        }

        // Add menuItems to menu
        private void MenuRenderingHandler(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if (string.Equals(sender.TreeAlias, "media", StringComparison.OrdinalIgnoreCase))
            {
                var menuItemTinifyButton = new MenuItem("Tinifier_Button", "Tinify");
                menuItemTinifyButton.LaunchDialogView(PackageConstants.TinyTImageRoute, "Tinifier");
                menuItemTinifyButton.Icon = PackageConstants.MenuIcon;
                e.Menu.Items.Add(menuItemTinifyButton);

                var menuItemSettingsButton = new MenuItem("Tinifier_Settings", "Stats");
                menuItemSettingsButton.LaunchDialogView(PackageConstants.TinySettingsRoute, "Optimization Stats");
                menuItemSettingsButton.Icon = PackageConstants.MenuSettingsIcon;
                e.Menu.Items.Add(menuItemSettingsButton);
            }
        }

        // Call methods for tinifing when upload image
        private void OptimizeOnUpload(int mediaItemId, SaveEventArgs<IMedia> e)
        {
            TImage image;

            try
            {
                image = _imageService.GetImageById(mediaItemId);
            }
            catch (Infrastructure.Exceptions.NotSupportedException ex)
            {
                e.Messages.Add(new EventMessage("Validation", PackageConstants.NotSupported, EventMessageType.Error));
                throw ex;
            }

            var imageHistory = _historyService.GetHistoryForImage(image.Id);

            if (imageHistory == null)
            {
                _imageService.OptimizeImage(image);
            }
        }
    }
}

