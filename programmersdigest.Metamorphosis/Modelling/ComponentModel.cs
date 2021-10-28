namespace programmersdigest.Metamorphosis.Modelling
{
    internal sealed class ComponentModel
    {
        public string Name { get; }
        public string Type { get; }

        public ComponentModel(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}