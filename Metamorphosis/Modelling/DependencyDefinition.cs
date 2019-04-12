using System.Reflection.Emit;

namespace Metamorphosis.Modelling
{
    internal class DependencyDefinition
    {
        public string Name { get; set; }
        public ComponentDefinition ComponentDefinition { get; set; }
        public FieldBuilder FieldBuilder { get; set; }
    }
}
