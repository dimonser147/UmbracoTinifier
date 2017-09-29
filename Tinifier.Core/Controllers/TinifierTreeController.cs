using System.Net.Http.Formatting;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Services.Media;
using Tinifier.Core.Services.Settings;
using Umbraco.Web;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Tinifier.Core.Controllers
{
    [Tree(PackageConstants.SectionAlias, PackageConstants.TreeAlias, PackageConstants.TreeTitle)]
    [PluginController(PackageConstants.SectionName)]
    public class TinifierTreeController : TreeController
    {
        private readonly IImageService _imageService;
        private readonly ISettingsService _settingsService;

        public TinifierTreeController()
        {
            _imageService = new ImageService();
            _settingsService = new SettingsService();
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }

        /// <summary>
        /// Get nodes for custom trees
        /// </summary>
        /// <param name="id">Node Id</param>
        /// <param name="queryStrings">FormDataCollection</param>
        /// <returns>Nodes for custom tree</returns>
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            var settings = _settingsService.GetSettings();

            if (id == PackageConstants.FirstNodeId && settings != null && !settings.HideLeftPanel)
            {
                foreach (var timage in _imageService.GetTopOptimizedImages())
                {
                    nodes.Add(CreateTreeNode(timage.Id + string.Empty, id, queryStrings, timage.Name, PackageConstants.TreeIcon, false,
                        FormDataCollectionExtensions.GetValue<string>(queryStrings, PackageConstants.AppAlias) + PackageConstants.CustomTreeUrl + timage.Id));
                }
            }

            return nodes;
        }      
    }
}
