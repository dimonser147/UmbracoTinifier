using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;

namespace Tinifier.Core.Repository.Section
{
    interface ITSectionRepo
    {
        /// <summary>
        /// assign tinifier section to all administrators
        /// </summary>
        void AssignTinifierToAdministrators();
    }
}
