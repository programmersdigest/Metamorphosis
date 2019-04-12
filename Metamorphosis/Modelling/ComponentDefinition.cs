using System;
using System.Collections.Generic;

namespace Metamorphosis.Modelling
{
    internal class ComponentDefinition
    {
        public string Name { get; set; }
        public Type BaseType { get; set; }
        public Type ProxyType { get; set; }
        public List<DependencyDefinition> Dependencies { get; set; }
        public List<SignalDefinition> Signals { get; set; }
        public object Instance { get; internal set; }
    }
}
