using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tinifier.Core.Models
{
    [Serializable]
    internal class OrganizationConstraintsException : Exception
    {
        public OrganizationConstraintsException()
        {
        }

        public OrganizationConstraintsException(string message) : base(message)
        {
        }

        public OrganizationConstraintsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OrganizationConstraintsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
