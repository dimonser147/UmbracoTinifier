using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Tinifier.Core.Application
{
    public class Startup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbraco, ApplicationContext context)
        {
            // create a new section
            Section section = context.Services.SectionService.GetByAlias(PackageConstants.SectionAlias);
            if (section == null)
            {
                context.Services.SectionService.MakeNew(PackageConstants.SectionName,
                    PackageConstants.SectionAlias,
                    PackageConstants.SectionIcon);
                context.Services.UserService.AddSectionToAllUsers(PackageConstants.SectionAlias);
            }

            // extend an existing media type
            var cts = ApplicationContext.Current.Services.ContentTypeService;
            IMediaType imageType = cts.GetMediaType("Image");
            if (imageType != null)
            {
                // add group
                const string group = "Tinifier";
                if (!imageType.PropertyGroups.Contains(group))
                {
                    imageType.AddPropertyGroup(group);
                }
                // add properties
                IDataTypeService dataTypeService = ApplicationContext.Current.Services.DataTypeService;
                IDataTypeDefinition[] dataTypeDefinitions = dataTypeService.GetAllDataTypeDefinitions().ToArray();
                // is optimized
                IDataTypeDefinition booleanProperty = dataTypeDefinitions.FirstOrDefault(p => p.Name.ToLower() == "true/false");
                string isOptimizedKey = "Tinifier_IsOptimized";
                if (!imageType.PropertyTypeExists(isOptimizedKey.ToLower()) && booleanProperty != null)
                {
                    imageType.AddPropertyType(new PropertyType(booleanProperty)
                    {
                        Name = isOptimizedKey,
                        Alias = isOptimizedKey.ToLower(),
                    }, group);
                }
                // tiny png respoon
                IDataTypeDefinition textProperty = dataTypeDefinitions.FirstOrDefault(p => p.Name.ToLower() == "textstring");
                string messageKey = "Tinifier_Message";
                if (!imageType.PropertyTypeExists(messageKey.ToLower()) && textProperty != null)
                {
                    imageType.AddPropertyType(new PropertyType(textProperty)
                    {
                        Name = messageKey,
                        Alias = messageKey.ToLower(),
                    }, group);
                }
                cts.Save(imageType);
            }
        }

    }
}
