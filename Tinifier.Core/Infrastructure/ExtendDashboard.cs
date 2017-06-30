using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Tinifier.Core.Infrastructure
{
    // Extend dashboard.config with our custom tabs and views
    public static class ExtendDashboard
    {
        public static void AddTabs()
        {
            var path = HttpContext.Current.Server.MapPath("~/config/dashboard.config");
            var doc = XDocument.Load(path);

            var restaurant = new XElement("section",
                new XAttribute("alias", "TinifierSettings"),
                new XElement("areas",
                    new XElement("area", "tinifier")),
                new XElement("tab", new XAttribute("caption", "Settings"),
                    new XElement("control", "/App_Plugins/Tinifier/BackOffice/Dashboards/settings.html")),
                new XElement("tab", new XAttribute("caption", "Statistic"),
                    new XElement("control", "/App_Plugins/Tinifier/BackOffice/Dashboards/statistic.html"))
            );
            doc.Root?.Add(restaurant);
            doc.Save(path);
        }

        public static void ClearTabs()
        {
            var path = HttpContext.Current.Server.MapPath("~/config/dashboard.config");
            var doc = new XmlDocument();
            doc.Load(path);
            var node = doc.SelectSingleNode("//section[@alias='TinifierSettings']");

            if (node != null)
            {
                var parent = node.ParentNode;
                parent.RemoveChild(node);
                var newXML = doc.OuterXml;
                doc.Save(path);
            }
        }
    }
}
