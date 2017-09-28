using System;

namespace Tinifier.Core.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception occurs when concurrent tinifing images
    /// </summary>
    public class ConcurrentOptimizingException : Exception
    {
        public ConcurrentOptimizingException(string message) : base(message)
        {
        }
    }
}
