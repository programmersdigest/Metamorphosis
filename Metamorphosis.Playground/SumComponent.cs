using Metamorphosis.Attributes;
using System;

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

        [Trigger]
        public void Startup()
        {
            Console.WriteLine("To sum it up...");
        }
    }
}
