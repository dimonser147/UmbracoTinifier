using System;

namespace Tinifier.Core.Infrastructure.Exceptions
{
    public class NotSupportedException : Exception
    {
        public NotSupportedException(string message) : base(message)
        {
        }
    }
}
