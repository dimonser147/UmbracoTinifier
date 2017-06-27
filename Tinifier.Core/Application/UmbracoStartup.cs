using System;
using Tinifier.Core.Infrastructure;
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
            MediaService.EmptiedRecycleBin += MediaService_EmptiedRecycleBin;

            // Clear dashboard.config before deleting
            InstalledPackage.BeforeDelete += InstalledPackage_BeforeDelete;
        }

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

        private void MediaServiceSaved(IMediaService sender, SaveEventArgs<IMedia> e)
        {
            foreach (var mediaItem in e.SavedEntities)
            {
                if (!string.IsNullOrEmpty(mediaItem.ContentType.Alias) && string.Equals(mediaItem.ContentType.Alias, "image", StringComparison.OrdinalIgnoreCase))
                {
                    _statisticService.UpdateStatistic();
                }
            }
        }

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

                ExtendDashboard.AddTabs();
            }
        }

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
    }
}

