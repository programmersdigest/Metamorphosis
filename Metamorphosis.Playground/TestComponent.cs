using Metamorphosis.Attributes;
using Metamorphosis.Logging;

namespace Metamorphosis.Playground
{
    [Component]
    public abstract class TestComponent
    {
        [Signal]
        public abstract void Log(object item, LogLevel logLevel = LogLevel.Info);

        [Startup]
        public void Startup()
        {
            Log("I'm Running!!!");
        }

        [Trigger]
        public void TestMe()
        {
            Log("This is a test!");
        }
    }
}
