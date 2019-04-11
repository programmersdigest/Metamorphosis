using System.Collections.Generic;

namespace Metamorphosis.Modelling
{
    internal sealed class ComponentModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public IReadOnlyDictionary<string, EndpointModel> Endpoints { get; } = new Dictionary<string, EndpointModel>();
    }
}