using Metamorphosis.Attributes;

namespace Metamorphosis.Playground
{
    [Component]
    public abstract class SumComponent
    {
        [Trigger]
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
