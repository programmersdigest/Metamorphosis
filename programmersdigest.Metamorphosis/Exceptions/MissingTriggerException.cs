using System;

namespace programmersdigest.Metamorphosis.Exceptions
{
    public class MissingTriggerException : Exception
    {
        public MissingTriggerException(string message) : base(message) { }
    }
}
