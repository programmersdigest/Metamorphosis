using Metamorphosis.Attributes;
using Metamorphosis.Logging;

namespace Metamorphosis.Playground
{
    [Component]
    public abstract class TestComponent
    {
        [Signal]
        protected abstract void Log(object item, LogLevel logLevel = LogLevel.Info);

        [Signal]
        protected virtual int Add(int a, int b)
        {
            return a - b;
        }

        [Trigger]
        public void Startup()
        {
            Log("I'm Running!!!");

            var result = Add(17, 13);
            Log(result);
        }

        [Trigger]
        public void Shutdown()
        {
            Log("I'm not running anymore...");
        }
    }
}
