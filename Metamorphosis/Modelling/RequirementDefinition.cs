using System.Collections.Generic;
using System.Reflection;

namespace Metamorphosis.Modelling
{
    internal class RequirementDefinition
    {
        public MethodInfo ReceiverMethod { get; set; }
        public List<ConnectionModel> Connections { get; set; }
    }
}
