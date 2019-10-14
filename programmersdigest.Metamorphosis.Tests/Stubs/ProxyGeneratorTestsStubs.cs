using System.Collections.Generic;
using System.Reflection;
using programmersdigest.Metamorphosis.Attributes;
using programmersdigest.Metamorphosis.Modelling;

namespace programmersdigest.Metamorphosis.Tests.Stubs
{
    #region simple stubs

    [Component]
    public abstract class ProxyGeneratorTestsSimpleTriggerStub
    {
        internal static string ComponentName = "TriggerStub";

        [Trigger]
        public int Add(int a, int b)
        {
            return a + b;
        }

        internal static ComponentDefinition GetComponentDefinition()
        {
            return new ComponentDefinition
            {
                Name = ComponentName,
                BaseType = typeof(ProxyGeneratorTestsSimpleTriggerStub),
                Signals = new List<SignalDefinition>(),
                Dependencies = new List<DependencyDefinition>()
            };
        }
    }

    [Component]
    public abstract class ProxyGeneratorTestsSimpleSignalStub
    {
        [Signal]
        protected abstract int Add(int a, int b);

        internal static string ComponentName = "SignalStub";

        internal static ComponentDefinition GetComponentDefinition()
        {
            var connections = new List<ConnectionModel>
            {
                new ConnectionModel
                {
                    Signal = $"{ComponentName}.Add",
                    Trigger = $"{ProxyGeneratorTestsSimpleTriggerStub.ComponentName}.Add"
                }
            };

            var signals = new List<SignalDefinition>
            {
                new SignalDefinition
                {
                    SignalMethod = typeof(ProxyGeneratorTestsSimpleSignalStub).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance),
                    Connections = connections
                }
            };

            return new ComponentDefinition
            {
                Name = ComponentName,
                BaseType = typeof(ProxyGeneratorTestsSimpleSignalStub),
                Signals = signals,
                Dependencies = new List<DependencyDefinition>
                {
                    new DependencyDefinition
                    {
                        Name = ProxyGeneratorTestsSimpleTriggerStub.ComponentName
                    }
                }
            };
        }
    }

    #endregion

    #region loop stubs

    [Component]
    public abstract class ProxyGeneratorTestsLoopStubA
    {
        internal static string ComponentName = "TriggerLoopStub";

        [Trigger]
        public int Add(int a, int b)
        {
            return a + b;
        }

        [Signal]
        public abstract int Subtract(int a, int b);

        internal static ComponentDefinition GetComponentDefinition()
        {
            var connections = new List<ConnectionModel>
            {
                new ConnectionModel
                {
                    Signal = $"{ComponentName}.Subtract",
                    Trigger = $"{ProxyGeneratorTestsLoopStubA.ComponentName}.Subtract"
                }
            };

            var signals = new List<SignalDefinition>
            {
                new SignalDefinition
                {
                    SignalMethod = typeof(ProxyGeneratorTestsLoopStubB).GetMethod("Subtract", BindingFlags.NonPublic | BindingFlags.Instance),
                    Connections = connections
                }
            };

            return new ComponentDefinition
            {
                Name = ComponentName,
                BaseType = typeof(ProxyGeneratorTestsLoopStubB),
                Signals = signals,
                Dependencies = new List<DependencyDefinition>
                {
                    new DependencyDefinition
                    {
                        Name = ProxyGeneratorTestsLoopStubB.ComponentName
                    }
                }
            };
        }
    }

    [Component]
    public abstract class ProxyGeneratorTestsLoopStubB
    {
        [Signal]
        protected abstract int Add(int a, int b);

        [Trigger]
        public int Subtract(int a, int b)
        {
            return a - b;
        }

        internal static string ComponentName = "SignalLoopStub";

        internal static ComponentDefinition GetComponentDefinition()
        {
            var connections = new List<ConnectionModel>
            {
                new ConnectionModel
                {
                    Signal = $"{ComponentName}.Add",
                    Trigger = $"{ProxyGeneratorTestsLoopStubA.ComponentName}.Add"
                }
            };

            var signals = new List<SignalDefinition>
            {
                new SignalDefinition
                {
                    SignalMethod = typeof(ProxyGeneratorTestsLoopStubB).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance),
                    Connections = connections
                }
            };

            return new ComponentDefinition
            {
                Name = ComponentName,
                BaseType = typeof(ProxyGeneratorTestsLoopStubB),
                Signals = signals,
                Dependencies = new List<DependencyDefinition>
                {
                    new DependencyDefinition
                    {
                        Name = ProxyGeneratorTestsLoopStubA.ComponentName
                    }
                }
            };
        }
    }

    #endregion

    #region missing trigger stubs

    [Component]
    public abstract class ProxyGeneratorTestsMissingTriggerStubA
    {
        internal static string ComponentName = "MissingTriggerStub";

        [Trigger]
        public int Subtract(int a, int b)
        {
            return a - b;
        }

        internal static ComponentDefinition GetComponentDefinition()
        {
            return new ComponentDefinition
            {
                Name = ComponentName,
                BaseType = typeof(ProxyGeneratorTestsMissingTriggerStubA),
                Signals = new List<SignalDefinition>(),
                Dependencies = new List<DependencyDefinition>()
            };
        }
    }

    [Component]
    public abstract class ProxyGeneratorTestsMissingTriggerStubB
    {
        [Signal]
        protected abstract int Add(int a, int b);

        internal static string ComponentName = "MissingTriggerSignalStub";

        internal static ComponentDefinition GetComponentDefinition()
        {
            var connections = new List<ConnectionModel>
            {
                new ConnectionModel
                {
                    Signal = $"{ComponentName}.Add",
                    Trigger = $"{ProxyGeneratorTestsMissingTriggerStubA.ComponentName}.Add"
                }
            };

            var signals = new List<SignalDefinition>
            {
                new SignalDefinition
                {
                    SignalMethod = typeof(ProxyGeneratorTestsMissingTriggerStubB).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance),
                    Connections = connections
                }
            };

            return new ComponentDefinition
            {
                Name = ComponentName,
                BaseType = typeof(ProxyGeneratorTestsMissingTriggerStubB),
                Signals = signals,
                Dependencies = new List<DependencyDefinition>
                {
                    new DependencyDefinition
                    {
                        Name = ProxyGeneratorTestsMissingTriggerStubA.ComponentName
                    }
                }
            };
        }
    }

    #endregion

    #region missing connection stubs

    [Component]
    public abstract class ProxyGeneratorTestsMissingConnectionStubA
    {
        internal static string ComponentName = "MissingConnectionTriggerStub";

        [Trigger]
        public int Add(int a, int b)
        {
            return a + b;
        }

        internal static ComponentDefinition GetComponentDefinition()
        {
            return new ComponentDefinition
            {
                Name = ComponentName,
                BaseType = typeof(ProxyGeneratorTestsMissingConnectionStubA),
                Signals = new List<SignalDefinition>(),
                Dependencies = new List<DependencyDefinition>()
            };
        }
    }

    [Component]
    public abstract class ProxyGeneratorTestsMissingConnectionStubB
    {
        [Signal]
        protected abstract int Add(int a, int b);

        internal static string ComponentName = "MissingConnectionSignalStub";

        internal static ComponentDefinition GetComponentDefinition()
        {
            var signals = new List<SignalDefinition>
            {
                new SignalDefinition
                {
                    SignalMethod = typeof(ProxyGeneratorTestsMissingConnectionStubB).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance),
                    Connections = new List<ConnectionModel>()
                }
            };

            return new ComponentDefinition
            {
                Name = ComponentName,
                BaseType = typeof(ProxyGeneratorTestsMissingConnectionStubB),
                Signals = signals,
                Dependencies = new List<DependencyDefinition>
                {
                    new DependencyDefinition
                    {
                        Name = ProxyGeneratorTestsMissingConnectionStubA.ComponentName
                    }
                }
            };
        }
    }

    #endregion
}
