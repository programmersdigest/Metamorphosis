using System;

namespace programmersdigest.Metamorphosis.Exceptions
{
    public class InvalidModelException : Exception
    {
        public InvalidModelException(string message) : base(message) { }
    }
}
