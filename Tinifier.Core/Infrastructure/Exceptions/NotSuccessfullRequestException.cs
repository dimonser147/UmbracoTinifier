using System;

namespace Tinifier.Core.Infrastructure.Exceptions
{
    public class NotSuccessfullRequestException : Exception
    {
        public NotSuccessfullRequestException(string message) : base(message)
        {
        }
    }
}
