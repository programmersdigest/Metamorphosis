using Metamorphosis.Attributes;

namespace Metamorphosis
{
    [Component]
    public abstract class Lifecycle
    {
        [Signal]
        protected virtual void Startup()
        {
        }

        [Signal]
        protected virtual void Shutdown()
        {
        }

        internal void SignalStartup()
        {
            Startup();
        }

        internal void SignalShutdown()
        {
            Shutdown();
        }
    }
}
