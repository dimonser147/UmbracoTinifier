using System;

namespace Tinifier.Core.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception occurs when Request is not successfull
    /// </summary>
    public class NotSuccessfullRequestException : Exception
    {
        public NotSuccessfullRequestException(string message) : base(message)
        {
        }
    }
}
