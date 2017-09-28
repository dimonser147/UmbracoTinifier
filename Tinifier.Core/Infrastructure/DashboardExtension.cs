using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Tinifier.Core.Infrastructure
{
    public static class DashboardExtension
    {
        /// <summary>
        /// Extend dashboard.config with our custom tabs and views
        /// </summary>
        public static void AddTabs()
        {
            var path = HttpContext.Current.Server.MapPath(PackageConstants.PathToDashboard);
            var doc = XDocument.Load(path);

            var restaurant = new XElement(PackageConstants.Section,
                new XAttribute(PackageConstants.Alias, PackageConstants.TinifierSettings),
                new XElement(PackageConstants.AreasAlias,
                    new XElement(PackageConstants.AreaAlias, PackageConstants.SectionAlias)),
                new XElement(PackageConstants.TabAlias, new XAttribute(PackageConstants.CaptionAlias, PackageConstants.Settings),
                    new XElement(PackageConstants.ControlAlias, PackageConstants.SettingsTabRoute)),
                new XElement(PackageConstants.TabAlias, new XAttribute(PackageConstants.CaptionAlias, PackageConstants.Statistic),
                    new XElement(PackageConstants.ControlAlias, PackageConstants.StatisticTabRoute))
            );
            if (doc.Root != null) doc.Root.Add(restaurant);
            doc.Save(path);
        }

        /// <summary>
        /// Clear custom tabs and views from dashboard.config
        /// </summary>
        public static void ClearTabs()
        {
            var path = HttpContext.Current.Server.MapPath(PackageConstants.PathToDashboard);
            var doc = new XmlDocument();
            doc.Load(path);
            var node = doc.SelectSingleNode(PackageConstants.PathToSectionDashboard);

            if (node != null)
            {
                var parent = node.ParentNode;
                if (parent != null) parent.RemoveChild(node);
                doc.Save(path);
            }
        }
    }
}
