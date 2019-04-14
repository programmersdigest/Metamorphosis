using programmersdigest.Metamorphosis.Attributes;
using System;

namespace programmersdigest.Metamorphosis.Playground
{
    [Component]
    public abstract class SumComponent
    {
        [Trigger]
        public int Add(int a, int b)
        {
            return a + b;
        }

        [Trigger]
        public void Startup()
        {
            Console.WriteLine("To sum it up...");
        }
    }
}
