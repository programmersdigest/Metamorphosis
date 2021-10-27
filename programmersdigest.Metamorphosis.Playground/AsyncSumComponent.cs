using programmersdigest.Metamorphosis.Attributes;
using System;
using System.Threading.Tasks;

namespace programmersdigest.Metamorphosis.Playground
{
    [Component]
    public abstract class AsyncSumComponent
    {
        [Trigger]
        public async Task<int> Add(int a, int b)
        {
            Console.WriteLine("Computing sum in 5 secs...");
            return await Task.Delay(5000).ContinueWith((t) => a + b);
        }
    }
}
