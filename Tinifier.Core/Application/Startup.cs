using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Tinifier.Core.Application
{
    public class Startup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbraco, ApplicationContext context)
        {
            // Gets a reference to the section (if already added)
            Section section = context.Services.SectionService.GetByAlias(PackageConstants.SectionAlias);
            if (section != null) return;

            // Add a new section
            context.Services.SectionService.MakeNew(PackageConstants.SectionName, 
                PackageConstants.SectionAlias, 
                PackageConstants.SectionIcon);

            context.Services.UserService.AddSectionToAllUsers(PackageConstants.SectionAlias);

        }

    }
}
