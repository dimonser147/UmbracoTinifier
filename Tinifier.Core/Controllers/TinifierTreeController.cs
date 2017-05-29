using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using Tinifier.Core.Application;
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
        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            return new MenuItemCollection();
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            TreeNodeCollection nodes = new TreeNodeCollection();

            if (id == "-1")
            {
                foreach (TImage timage in TestRepo.GetAll())
                {
                    nodes.Add(CreateTreeNode(timage.Id + "", id, queryStrings, timage.Name, "icon-umb-media"));
                }
            }

            return nodes;
        }
       
    }
}
