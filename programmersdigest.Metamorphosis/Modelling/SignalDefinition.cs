using System.Collections.Generic;
using System.Reflection;

namespace programmersdigest.Metamorphosis.Modelling
{
    internal class SignalDefinition
    {
        public MethodInfo SignalMethod { get; }
        public IReadOnlyCollection<ConnectionModel> Connections { get; }

        public SignalDefinition(MethodInfo signalMethod, IReadOnlyCollection<ConnectionModel> connections)
        {
            SignalMethod = signalMethod;
            Connections = connections;
        }
    }
}
