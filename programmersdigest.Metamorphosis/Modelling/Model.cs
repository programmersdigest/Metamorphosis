using System.Collections.Generic;

namespace programmersdigest.Metamorphosis.Modelling
{
    internal sealed class Model
    {
        public IReadOnlyList<string> Assemblies { get; } = new List<string>();
        public IReadOnlyList<ComponentModel> Components { get; } = new List<ComponentModel>();
        public IReadOnlyList<ConnectionModel> Connections { get; } = new List<ConnectionModel>();
    }
}
