using System.Reflection.Emit;

namespace programmersdigest.Metamorphosis.Modelling
{
    internal class DependencyDefinition
    {
        public string Name { get; }
        public ComponentDefinition ComponentDefinition { get; set; } = null!;
        public FieldBuilder FieldBuilder { get; set; } = null!;

        public DependencyDefinition(string name)
        {
            Name = name;
        }
    }
}
