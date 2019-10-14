using System;

namespace programmersdigest.Metamorphosis.Exceptions
{
    public class MissingConnectionException : Exception
    {
        public MissingConnectionException(string message) : base(message) { }
    }
}
