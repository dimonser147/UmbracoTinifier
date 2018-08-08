using Umbraco.Core.Models;

namespace Tinifier.Core.Models.API
{
    public class OrganisableMediaModel
    {
        public Media Media { get; set; }
        public string[] DestinationPath { get; set; }
    }
}
