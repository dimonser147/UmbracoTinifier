using System;
using System.Web;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Services.Interfaces;
using Tinifier.Core.Services.Realization;
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
        private IStatisticService _statisticService;

        public UmbracoStartup()
        {
            _statisticService = new StatisticService();
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbraco, ApplicationContext context)
        {
            // Create a new section
            CreateTinifySection(context);

            // Extend dropdownMenu with Tinify button
            TreeControllerBase.MenuRendering += MenuRenderingHandler;

            // Save image handler for updating number of Images in database
            MediaService.Saved += MediaServiceSaved;

            // Delete image handler for updating number of Images in database
            MediaService.Deleted += MediaServiceDeleted;
        }

        private void MediaServiceSaved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            foreach (var mediaItem in e.SavedEntities)
            {
                if (!string.IsNullOrEmpty(mediaItem.ContentType.Alias) && string.Equals(mediaItem.ContentType.Alias, "image", StringComparison.OrdinalIgnoreCase))
                {
                    _statisticService.UpdateNumberOfImages();
                }
            }
        }

        private void MediaServiceDeleted(IMediaService sender, DeleteEventArgs<IMedia> e)
        {
            foreach (var mediaItem in e.DeletedEntities)
            {
                if (!string.IsNullOrEmpty(mediaItem.ContentType.Alias) && string.Equals(mediaItem.ContentType.Alias, "image", StringComparison.OrdinalIgnoreCase))
                {
                    _statisticService.DecNumberOfImages();
                }
            }
        }

        private void CreateTinifySection(ApplicationContext context)
        {
            // Create a new section
            var section = context.Services.SectionService.GetByAlias(PackageConstants.SectionAlias);

            if (section == null)
            {
                context.Services.SectionService.MakeNew(PackageConstants.SectionName,
                    PackageConstants.SectionAlias,
                    PackageConstants.SectionIcon);
                context.Services.UserService.AddSectionToAllUsers(PackageConstants.SectionAlias);
            }
        }

        private void MenuRenderingHandler(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            // Get imageId and create menuItem
            var timageId = int.Parse(HttpContext.Current.Request.QueryString["id"]);

            if (string.Equals(sender.TreeAlias, "media", StringComparison.OrdinalIgnoreCase))
            {
                var menuItemTinifyButton = new MenuItem("Tinifier_Button", "Tinify");
                menuItemTinifyButton.LaunchDialogView(PackageConstants.TinyTImageRoute, "Tinifier");
                menuItemTinifyButton.Icon = PackageConstants.MenuIcon;
                e.Menu.Items.Add(menuItemTinifyButton);

                var menuItemSettingsButton = new MenuItem("Tinifier_Settings", "Settings");
                menuItemSettingsButton.LaunchDialogView(PackageConstants.TinySettingsRoute, "Tinifier Settings");
                menuItemSettingsButton.Icon = PackageConstants.MenuSettingsIcon;
                e.Menu.Items.Add(menuItemSettingsButton);
            }
        }
    }
}

