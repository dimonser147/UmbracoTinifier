using System.Web;
using System.Xml.Linq;

namespace Tinifier.Core.Infrastructure
{
    public class ExtendDashboard
    {
        public void AddTabs()
        {
            var doc = XDocument.Load(HttpContext.Current.Server.MapPath("~/config/dashboard.config"));

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
            doc.Save(HttpContext.Current.Server.MapPath("~/config/dashboard.config"));
        }
    }
}
