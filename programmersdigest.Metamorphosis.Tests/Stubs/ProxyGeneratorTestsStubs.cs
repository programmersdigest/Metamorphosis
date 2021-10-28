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
            return new ComponentDefinition(
                ComponentName,
                typeof(ProxyGeneratorTestsSimpleTriggerStub),
                new List<SignalDefinition>(),
                new List<DependencyDefinition>());
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
                new ConnectionModel($"{ComponentName}.Add", $"{ProxyGeneratorTestsSimpleTriggerStub.ComponentName}.Add")
            };

            var signals = new List<SignalDefinition>
            {
                new SignalDefinition(
                    typeof(ProxyGeneratorTestsSimpleSignalStub).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)!,
                    connections)
            };

            return new ComponentDefinition(
                ComponentName,
                typeof(ProxyGeneratorTestsSimpleSignalStub),
                signals,
                new List<DependencyDefinition>
                {
                    new DependencyDefinition(ProxyGeneratorTestsSimpleTriggerStub.ComponentName)
                });
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
                new ConnectionModel($"{ComponentName}.Subtract", $"{ProxyGeneratorTestsLoopStubA.ComponentName}.Subtract")
            };

            var signals = new List<SignalDefinition>
            {
                new SignalDefinition(
                    typeof(ProxyGeneratorTestsLoopStubB).GetMethod("Subtract", BindingFlags.NonPublic | BindingFlags.Instance)!,
                    connections)
            };

            return new ComponentDefinition(
                ComponentName,
                typeof(ProxyGeneratorTestsLoopStubB),
                signals,
                new List<DependencyDefinition>
                {
                    new DependencyDefinition(ProxyGeneratorTestsLoopStubB.ComponentName)
                });
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
                new ConnectionModel($"{ComponentName}.Add", $"{ProxyGeneratorTestsLoopStubA.ComponentName}.Add")
            };

            var signals = new List<SignalDefinition>
            {
                new SignalDefinition(
                    typeof(ProxyGeneratorTestsLoopStubB).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)!,
                    connections)
            };

            return new ComponentDefinition(
                ComponentName,
                typeof(ProxyGeneratorTestsLoopStubB),
                signals,
                new List<DependencyDefinition>
                {
                    new DependencyDefinition(ProxyGeneratorTestsLoopStubA.ComponentName)
                });
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
            return new ComponentDefinition(
                ComponentName,
                typeof(ProxyGeneratorTestsMissingTriggerStubA),
                new List<SignalDefinition>(),
                new List<DependencyDefinition>());
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
                new ConnectionModel($"{ComponentName}.Add", $"{ProxyGeneratorTestsMissingTriggerStubA.ComponentName}.Add")
            };

            var signals = new List<SignalDefinition>
            {
                new SignalDefinition(
                    typeof(ProxyGeneratorTestsMissingTriggerStubB).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)!,
                    connections)
            };

            return new ComponentDefinition(
                ComponentName,
                typeof(ProxyGeneratorTestsMissingTriggerStubB),
                signals,
                new List<DependencyDefinition>
                {
                    new DependencyDefinition(ProxyGeneratorTestsMissingTriggerStubA.ComponentName)
                });
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
            return new ComponentDefinition(
                ComponentName,
                typeof(ProxyGeneratorTestsMissingConnectionStubA),
                new List<SignalDefinition>(),
                new List<DependencyDefinition>());
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
                new SignalDefinition(
                    typeof(ProxyGeneratorTestsMissingConnectionStubB).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)!,
                    new List<ConnectionModel>())
            };

            return new ComponentDefinition(
                ComponentName,
                typeof(ProxyGeneratorTestsMissingConnectionStubB),
                signals,
                new List<DependencyDefinition>
                {
                    new DependencyDefinition(ProxyGeneratorTestsMissingConnectionStubA.ComponentName)
                });
        }
    }

    #endregion
}
