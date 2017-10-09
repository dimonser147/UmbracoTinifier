using System;

namespace Tinifier.Core.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception occurs when entity not found 
    /// </summary>
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
            : base(PackageConstants.ImageNotExists)
        { }

        public EntityNotFoundException(string message) 
            : base(message)
        { }
    }
}
