using System;
using Tinifier.Core.Infrastructure;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace Tinifier.Core.Application
{
    public class UmbracoStartupPro : ApplicationEventHandler
    {

        /// <summary>
        /// Add custom section and event handlers 
        /// </summary>
        /// <param name="umbraco">UmbracoApplicationBase</param>
        /// <param name="context">ApplicationContext</param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbraco, ApplicationContext context)
        {
            base.ApplicationStarted(umbraco, context);

            TreeControllerBase.MenuRendering += MenuRenderingHandler;
        }

        /// <summary>
        /// Extend dropdownMenu with Organize Images button
        /// </summary>
        /// <param name="sender">TreeControllerBase</param>
        /// <param name="e">EventArgs</param>
        private void MenuRenderingHandler(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if (string.Equals(sender.TreeAlias, PackageConstants.MediaAlias, StringComparison.OrdinalIgnoreCase))
            {
                var menuItemOrganizeImagesButton = new MenuItem(ProPackageConstants.OrganizeImagesButton, ProPackageConstants.OrganizeImagesCaption);
                menuItemOrganizeImagesButton.LaunchDialogView(ProPackageConstants.OrganizeImagesRoute, ProPackageConstants.OrganizeImagesCaption);
                e.Menu.Items.Add(menuItemOrganizeImagesButton);
            }
        }
    }
}