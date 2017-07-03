using System;

namespace Tinifier.Core.Infrastructure.Exceptions
{
    public class ConcurrentOptimizingException : Exception
    {
        public ConcurrentOptimizingException(string message) : base(message)
        {
        }
    }
}
