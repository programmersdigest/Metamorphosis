using programmersdigest.Metamorphosis.Attributes;
using programmersdigest.Metamorphosis.Logging;

namespace programmersdigest.Metamorphosis.Playground
{
    [Component]
    public abstract class TestComponent
    {
        private readonly string _injectedDependency;

        public TestComponent(string injectedDependency)
        {
            _injectedDependency = injectedDependency;
        }

        [Signal]
        protected abstract void Log(object item, LogLevel logLevel = LogLevel.Info);

        [Signal]
        protected virtual int Add(int a, int b)
        {
            return a - b;
        }

        [Signal]
        protected abstract string VariadicConcat(params string[] variadicParameter);

        [Trigger]
        public void Startup()
        {
            Log($"I'm running with an {_injectedDependency}!");

            var result = Add(17, 13);
            Log(result);

            var concat = VariadicConcat("Test", "123", "456");
            Log(concat);
        }

        [Trigger]
        public void Shutdown()
        {
            Log("I'm not running anymore...");
        }
    }
}
