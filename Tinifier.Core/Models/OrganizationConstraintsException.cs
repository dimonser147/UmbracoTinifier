using System;
using System.Runtime.Serialization;

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
