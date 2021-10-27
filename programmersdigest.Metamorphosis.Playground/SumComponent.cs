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
            Console.WriteLine("Computing sum...");
            return a + b;
        }
    }
}
