using System.Threading.Tasks;
using programmersdigest.Metamorphosis.Attributes;
using programmersdigest.Metamorphosis.Logging;

namespace programmersdigest.Metamorphosis.Playground
{
    [Component]
    public abstract class AsyncTestComponent
    {
        [Signal]
        protected abstract void Log(object item, LogLevel logLevel = LogLevel.Info);

        [Signal]
        protected virtual async Task<int> Add(int a, int b)
        {
            return await Task.Run(() => a - b);
        }

        [Trigger]
        public async void Startup()
        {
            Log("I'm running async!");

            var result = await Add(29, 13).ConfigureAwait(false);
            Log(result);
        }

        [Trigger]
        public void Shutdown()
        {
            Log("I'm not running anymore...");
        }
    }
}
