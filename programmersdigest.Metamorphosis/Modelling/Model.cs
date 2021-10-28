using System.Collections.Generic;

namespace programmersdigest.Metamorphosis.Modelling
{
    internal sealed class Model
    {
        public IReadOnlyList<string> Assemblies { get; }
        public IReadOnlyList<ComponentModel> Components { get; }
        public IReadOnlyList<ConnectionModel> Connections { get; }

        public Model(IReadOnlyList<string> assemblies, IReadOnlyList<ComponentModel> components, IReadOnlyList<ConnectionModel> connections)
        {
            Assemblies = assemblies;
            Components = components;
            Connections = connections;
        }
    }
}
