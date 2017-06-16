using System;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace Tinifier.Core.Application
{
    public class UmbracoStartup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbraco, ApplicationContext context)
        {
            // create a new section
            CreateTinifySection(context);

            // add properties
            IDataTypeService dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            IDataTypeDefinition[] dataTypeDefinitions = dataTypeService.GetAllDataTypeDefinitions().ToArray();

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

                // Is optimized checkbox
                CreateIsOptimizedCheckbox(dataTypeDefinitions, imageType, group);
                CreateTinifierMessageBox(dataTypeDefinitions, imageType, group);

                cts.Save(imageType);
            }

            // extend dropdownMenu with Tinify button
            TreeControllerBase.MenuRendering += MenuRenderingHandler;
        }

        private void CreateTinifySection(ApplicationContext context)
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
        }

        private void CreateIsOptimizedCheckbox(IDataTypeDefinition[] dataTypeDefinitions, IMediaType imageType, string group)
        {
            // is optimized
            IDataTypeDefinition booleanProperty = dataTypeDefinitions.FirstOrDefault(p => string.Equals(p.Name, "true/false", StringComparison.OrdinalIgnoreCase));
            string isOptimizedKey = "Tinifier_IsOptimized";
            if (!imageType.PropertyTypeExists(isOptimizedKey.ToLower()) && booleanProperty != null)
            {
                imageType.AddPropertyType(new PropertyType(booleanProperty)
                {
                    Name = isOptimizedKey,
                    Alias = isOptimizedKey.ToLower(),
                }, group);
            }
        }

        private void CreateTinifierMessageBox(IDataTypeDefinition[] dataTypeDefinitions, IMediaType imageType, string group)
        {
            // tiny png respoon
            IDataTypeDefinition textProperty = dataTypeDefinitions.FirstOrDefault(p => string.Equals(p.Name, "textstring", StringComparison.OrdinalIgnoreCase));
            string messageKey = "Tinifier_Message";
            if (!imageType.PropertyTypeExists(messageKey.ToLower()) && textProperty != null)
            {
                imageType.AddPropertyType(new PropertyType(textProperty)
                {
                    Name = messageKey,
                    Alias = messageKey.ToLower(),

                }, group);
            }
        }

        private void MenuRenderingHandler(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            // get imageId and create menuItem
            var timageId = int.Parse(HttpContext.Current.Request.QueryString["id"]);

            if (string.Equals(sender.TreeAlias, "media", StringComparison.OrdinalIgnoreCase))
            {
                var menu = new MenuItem("Tinify_Button", "Tinify");
                menu.Icon = "umb-developer";
                
               // menu.NavigateToRoute("/media/media/edit/" + timageId);
                menu.AdditionalData.Add("actionUrl", PackageConstants.TinyTImageRoute + timageId);

               // menu.AdditionalData.Add("actionView", "Views/media/edit.html");
                e.Menu.Items.Add(menu);
            }
        }
    }
}

