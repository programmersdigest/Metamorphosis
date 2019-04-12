using Metamorphosis.Attributes;
using Metamorphosis.Logging;

namespace Metamorphosis.Playground
{
    [Component]
    public abstract class TestComponent
    {
        [Signal]
        public abstract void Log(object item, LogLevel logLevel = LogLevel.Info);

        [Signal]
        public virtual int Add(int a, int b)
        {
            return a - b;
        }

        [Startup]
        public void Startup()
        {
            Log("I'm Running!!!");

            var result = Add(17, 13);
            Log(result);
        }
    }
}
