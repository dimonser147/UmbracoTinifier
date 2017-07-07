using System.Net.Http.Formatting;
using Tinifier.Core.Infrastructure;
using Tinifier.Core.Services.Media;
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

        public TinifierTreeController()
        {
            _imageService = new ImageService();
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            if (id == PackageConstants.FirstNodeId)
            {
                foreach (var timage in _imageService.GetOptimizedImages())
                {
                    nodes.Add(CreateTreeNode(timage.Id + "", id, queryStrings, timage.Name, PackageConstants.TreeIcon));
                }
            }

            return nodes;
        }      
    }
}
