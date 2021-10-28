using System;
using System.Collections.Generic;

namespace programmersdigest.Metamorphosis.Modelling
{
    internal sealed class ComponentDefinition
    {
        internal string Name { get; }
        internal Type BaseType { get; }
        internal IReadOnlyCollection<SignalDefinition> Signals { get; }
        internal IReadOnlyCollection<DependencyDefinition> Dependencies { get; }
        internal Type ProxyType { get; set; } = null!;
        internal object? Instance { get; set; } = null;

        internal ComponentDefinition(string name, Type baseType, IReadOnlyCollection<SignalDefinition> signals, IReadOnlyCollection<DependencyDefinition> dependencies)
        {
            Name = name;
            BaseType = baseType;
            Signals = signals;
            Dependencies = dependencies;
        }
    }
}
