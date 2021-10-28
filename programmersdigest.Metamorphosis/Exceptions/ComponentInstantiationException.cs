using System;

namespace programmersdigest.Metamorphosis.Exceptions
{
    public class ComponentInstantiationException : Exception
    {
        public ComponentInstantiationException(string message) : base(message) { }
    }
}
