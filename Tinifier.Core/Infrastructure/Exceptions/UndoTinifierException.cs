using System;

namespace Tinifier.Core.Infrastructure.Exceptions
{
    public class UndoTinifierException : Exception
    {
        public UndoTinifierException(string message)
            : base(message)
        { }
    }
}
