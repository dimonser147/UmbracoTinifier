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
            // Create a new section
            CreateTinifySection(context);

            // Add properties
            IDataTypeService dataTypeService = ApplicationContext.Current.Services.DataTypeService;
            IDataTypeDefinition[] dataTypeDefinitions = dataTypeService.GetAllDataTypeDefinitions().ToArray();

            // Extend an existing media type
            var cts = ApplicationContext.Current.Services.ContentTypeService;
            IMediaType imageType = cts.GetMediaType("Image");

            if (imageType != null)
            {
                // Add group
                const string group = "Tinifier";

                if (!imageType.PropertyGroups.Contains(group))
                {
                    imageType.AddPropertyGroup(group);
                }

                // Is optimized checkbox
                CreateIsOptimizedCheckbox(dataTypeDefinitions, imageType, group);
                CreateTinifierOriginSizeLabel(dataTypeDefinitions, imageType, group);
                CreateTinifierOptimizedSize(dataTypeDefinitions, imageType, group);
                CreateTinifierOccuredAt(dataTypeDefinitions, imageType, group);
                CreateTinifierMessageBox(dataTypeDefinitions, imageType, group);
                cts.Save(imageType);
            }

            // Extend dropdownMenu with Tinify button
            TreeControllerBase.MenuRendering += MenuRenderingHandler;
        }

        private void CreateTinifySection(ApplicationContext context)
        {
            // Create a new section
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
            // Is optimized
            IDataTypeDefinition booleanProperty = dataTypeDefinitions.FirstOrDefault(p => string.Equals(p.Name, "true/false", StringComparison.OrdinalIgnoreCase));
            string isOptimizedKey = "IsOptimized";
            if (!imageType.PropertyTypeExists(isOptimizedKey.ToLower()) && booleanProperty != null)
            {
                imageType.AddPropertyType(new PropertyType(booleanProperty)
                {
                    Name = isOptimizedKey,
                    Alias = isOptimizedKey.ToLower(),
                }, group);
            }
        }

        private void CreateTinifierOriginSizeLabel(IDataTypeDefinition[] dataTypeDefinitions, IMediaType imageType, string group)
        {
            // Origin Size
            IDataTypeDefinition labelProperty = dataTypeDefinitions.FirstOrDefault(p => string.Equals(p.Name, "label", StringComparison.OrdinalIgnoreCase));
            string messageKey = "Origin Size";
            if (!imageType.PropertyTypeExists(messageKey.ToLower()) && labelProperty != null)
            {
                imageType.AddPropertyType(new PropertyType(labelProperty)
                {
                    Name = messageKey,
                    Alias = messageKey.ToLower(),
                }, group);
            }
        }

        private void CreateTinifierOptimizedSize(IDataTypeDefinition[] dataTypeDefinitions, IMediaType imageType, string group)
        {
            // Optimized Size
            IDataTypeDefinition textProperty = dataTypeDefinitions.FirstOrDefault(p => string.Equals(p.Name, "label", StringComparison.OrdinalIgnoreCase));
            string messageKey = "Optimized Size";
            if (!imageType.PropertyTypeExists(messageKey.ToLower()) && textProperty != null)
            {
                imageType.AddPropertyType(new PropertyType(textProperty)
                {
                    Name = messageKey,
                    Alias = messageKey.ToLower(),

                }, group);
            }
        }

        private void CreateTinifierOccuredAt(IDataTypeDefinition[] dataTypeDefinitions, IMediaType imageType, string group)
        {
            // When optimized
            IDataTypeDefinition textProperty = dataTypeDefinitions.FirstOrDefault(p => string.Equals(p.Name, "label", StringComparison.OrdinalIgnoreCase));
            string messageKey = "Occured At";
            if (!imageType.PropertyTypeExists(messageKey.ToLower()) && textProperty != null)
            {
                imageType.AddPropertyType(new PropertyType(textProperty)
                {
                    Name = messageKey,
                    Alias = messageKey.ToLower(),

                }, group);
            }
        }

        private void CreateTinifierMessageBox(IDataTypeDefinition[] dataTypeDefinitions, IMediaType imageType, string group)
        {
            // Tiny png respoon
            IDataTypeDefinition textProperty = dataTypeDefinitions.FirstOrDefault(p => string.Equals(p.Name, "label", StringComparison.OrdinalIgnoreCase));
            string messageKey = "Error Message";
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
            // Get imageId and create menuItem
            var timageId = int.Parse(HttpContext.Current.Request.QueryString["id"]);

            if (string.Equals(sender.TreeAlias, "media", StringComparison.OrdinalIgnoreCase))
            {
                var menu = new MenuItem("Tinify_Button", "Tinify");
                menu.LaunchDialogView(PackageConstants.TinyTImageRoute, "Tinifier");
                menu.Icon = PackageConstants.MenuIcon;
                e.Menu.Items.Add(menu);
            }
        }
    }
}

