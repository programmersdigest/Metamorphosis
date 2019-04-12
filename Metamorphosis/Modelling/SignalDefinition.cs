using System.Collections.Generic;
using System.Reflection;

namespace Metamorphosis.Modelling
{
    internal class SignalDefinition
    {
        public MethodInfo SignalMethod { get; set; }
        public List<ConnectionModel> Connections { get; set; }
    }
}
