using Metamorphosis.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Metamorphosis.Modelling
{
    internal sealed class ProxyGenerator
    {

        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;

        private Dictionary<string, ComponentDefinition> _componentDefinitions;

        public ProxyGenerator()
        {
            var assemblyName = new AssemblyName("Metamorphosis.DynamicProxies");
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule("Metamorphosis.DynamicProxies");
        }

        public void GenerateProxyTypes(List<ComponentDefinition> componentDefinitions)
        {
            _componentDefinitions = componentDefinitions.ToDictionary(cd => cd.Name);

            foreach (var componentDefinition in componentDefinitions)
            {
                GenerateProxyTypesRecursive(componentDefinition, 0);
            }
        }

        private void GenerateProxyTypesRecursive(ComponentDefinition componentDefinition, int recursionDepth)
        {
            if (recursionDepth > _componentDefinitions.Count)
            {
                throw new InvalidOperationException($"The recursion depth is greater than the number of components. Is there a loop in the endpoint configuration?");
            }

            if (componentDefinition.ProxyType != null)
            {
                return;
            }

            foreach (var dependency in componentDefinition.Dependencies)
            {
                var dependencyComponentDefinition = _componentDefinitions[dependency.Name];
                GenerateProxyTypesRecursive(dependencyComponentDefinition, recursionDepth + 1);

                dependency.ComponentDefinition = dependencyComponentDefinition;
            }

            GenerateProxyType(componentDefinition);
        }

        private void GenerateProxyType(ComponentDefinition componentDefinition)
        {
            var proxyTypeBuilder = _moduleBuilder.DefineType(componentDefinition.Name,
                                                             TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed,
                                                             componentDefinition.BaseType);

            var dependenciesDictionary = GenerateDependencyFields(proxyTypeBuilder, componentDefinition.Dependencies);

            foreach (var signal in componentDefinition.Signals)
            {
                var connection = signal.Connections.First();   // We do not support multiple connections at this point.

                var receiver = dependenciesDictionary[connection.Receiver];
                var triggerMethod = receiver.ComponentDefinition.ProxyType.GetMethods()
                    .Single(m => m.Name == connection.SignalName &&
                                 m.GetCustomAttribute<TriggerAttribute>() != null &&
                                 m.IsMethodDefinitionCompatible(signal.SignalMethod)
                    );

                var signalMethodOverride = PrepareMethodOverride(proxyTypeBuilder, signal.SignalMethod);
                GenerateIL(signalMethodOverride, receiver.FieldBuilder, triggerMethod);

                proxyTypeBuilder.DefineMethodOverride(signalMethodOverride, signal.SignalMethod);
            }

            componentDefinition.ProxyType = proxyTypeBuilder.CreateTypeInfo();
        }

        private Dictionary<string, DependencyDefinition> GenerateDependencyFields(TypeBuilder typeBuilder, List<DependencyDefinition> dependencies)
        {
            var dependenciesDictionary = new Dictionary<string, DependencyDefinition>();

            foreach (var dependency in dependencies)
            {
                var fieldBuilder = typeBuilder.DefineField($"__proxy_{dependency.Name}",
                                                           dependency.ComponentDefinition.ProxyType, FieldAttributes.Private);
                dependency.FieldBuilder = fieldBuilder;
                dependenciesDictionary.Add(dependency.Name, dependency);
            }

            return dependenciesDictionary;
        }

        private MethodBuilder PrepareMethodOverride(TypeBuilder typeBuilder, MethodInfo baseMethod)
        {
            var signalMethodParameters = baseMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var signalMethodOverride = typeBuilder.DefineMethod(baseMethod.Name,
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                baseMethod.ReturnType, signalMethodParameters);

            if (baseMethod.IsGenericMethod)
            {
                var signalMethodGenericParameters = baseMethod.GetGenericArguments().ToList();
                var genericParameters = signalMethodOverride.DefineGenericParameters(signalMethodGenericParameters.Select(a => a.Name).ToArray());
                for (var i = 0; i < genericParameters.Length; i++)
                {
                    var baseTypeConstraint = signalMethodGenericParameters[i].GetGenericParameterConstraints().SingleOrDefault(c => !c.IsInterface);
                    if (baseTypeConstraint != null)
                    {
                        genericParameters[i].SetBaseTypeConstraint(baseTypeConstraint);
                    }

                    var interfaceConstraints = signalMethodGenericParameters[i].GetGenericParameterConstraints().Where(c => c.IsInterface).ToArray();
                    if (interfaceConstraints.Any())
                    {
                        genericParameters[i].SetInterfaceConstraints(interfaceConstraints);
                    }
                }
            }

            return signalMethodOverride;
        }

        private void GenerateIL(MethodBuilder methodBuilder, FieldBuilder receiverFieldBuilder, MethodInfo triggerMethod)
        {
            var ilGenerator = methodBuilder.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);                                  // Load "this".
            ilGenerator.Emit(OpCodes.Ldfld, receiverFieldBuilder);              // Load receiver from field.
            for (var i = 1; i <= triggerMethod.GetParameters().Length; i++)
            {
                ilGenerator.Emit(OpCodes.Ldarg, i);                             // Load all parameters.
            }
            ilGenerator.Emit(OpCodes.Callvirt, triggerMethod);                  // Call trigger method on receiver instance.
            ilGenerator.Emit(OpCodes.Ret);                                      // Return to caller.
        }
    }
}
