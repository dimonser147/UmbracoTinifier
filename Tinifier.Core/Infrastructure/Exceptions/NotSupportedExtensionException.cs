using System;

namespace Tinifier.Core.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception occurs when file extension is not supported
    /// </summary>
    public class NotSupportedExtensionException : Exception
    {
        private const string _message = "Extension \"{0}\" is not supported. We support: {1}";

        public NotSupportedExtensionException(string extension = "unknown") 
            : base(string.Format(_message, extension, string.Join(", ", PackageConstants.SupportedExtensions)))
        { }
    }
}
