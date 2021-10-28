using programmersdigest.Metamorphosis.Attributes;
using programmersdigest.Metamorphosis.Logging;

namespace programmersdigest.Metamorphosis.Playground
{
    [Component]
    public abstract class VariadicConcatComponent
    {
        [Trigger]
        public string VariadicConcat(params string[] variadicParameter)
        {
            return string.Join(" - ", variadicParameter);
        }
    }
}
