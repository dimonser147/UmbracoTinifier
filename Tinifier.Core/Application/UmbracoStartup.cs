using System;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace Tinifier.Core.Application
{
    public class UmbracoStartup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbraco, ApplicationContext context)
        {
            // Create a new section
            CreateTinifySection(context);

            // Extend dropdownMenu with Tinify button
            TreeControllerBase.MenuRendering += MenuRenderingHandler;
        }

        private void CreateTinifySection(ApplicationContext context)
        {
            // Create a new section
            Section section = context.Services.SectionService.GetByAlias(PackageConstants.SectionAlias);
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
                menuItemSettingsButton.Icon = "umb-content";
                e.Menu.Items.Add(menuItemSettingsButton);
            }
        }
    }
}

