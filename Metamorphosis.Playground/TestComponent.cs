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
        public virtual void TestOptionalSignal()
        {
            Log("TestOptionalSignal called.");
        }

        [Startup]
        public void Startup()
        {
            Log("I'm Running!!!");
            TestOptionalSignal();
        }

        [Trigger]
        public void TestMe()
        {
            Log("This is a test!");
        }
    }
}
