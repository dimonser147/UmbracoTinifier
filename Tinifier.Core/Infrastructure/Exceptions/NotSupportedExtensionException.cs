using System;

namespace Tinifier.Core.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception occurs when file extension is not supported
    /// </summary>
    public class NotSupportedExtensionException : Exception
    {
        public NotSupportedExtensionException(string message) : base(message)
        {
        }
    }
}
