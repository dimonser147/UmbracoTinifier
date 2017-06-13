using System.Net.Http.Formatting;
using Tinifier.Core.Models;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace Tinifier.Core.Controllers
{
    [Tree(PackageConstants.SectionAlias, "timages", "Optimized Images")]
    [PluginController(PackageConstants.SectionName)]
    public class TinifierTreeController : TreeController
    {
        private TImageRepository _repo;

        public TinifierTreeController()
        {
            _repo = new TImageRepository();
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            TreeNodeCollection nodes = new TreeNodeCollection();

            if (id == "-1")
            {
                foreach (TImage timage in _repo.GetAllItems())
                {
                    nodes.Add(CreateTreeNode(timage.Id + "", id, queryStrings, timage.Name, "icon-umb-media"));
                }
            }

            return nodes;
        }      
    }
}
