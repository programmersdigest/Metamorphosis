using System;

namespace programmersdigest.Metamorphosis.Exceptions
{
    public class MissingProxyFieldException : Exception
    {
        public MissingProxyFieldException(string message) : base(message) { }
    }
}
