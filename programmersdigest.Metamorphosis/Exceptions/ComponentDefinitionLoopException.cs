using System;

namespace programmersdigest.Metamorphosis.Exceptions
{
    public class ComponentDefinitionLoopException : Exception
    {
        public ComponentDefinitionLoopException(string message) : base(message) { }
    }
}
