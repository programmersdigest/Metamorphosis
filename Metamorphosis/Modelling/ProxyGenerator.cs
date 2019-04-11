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

            foreach (var requirement in componentDefinition.Requirements)
            {
                var sender = dependenciesDictionary[requirement.Sender];
                var senderMethod = sender.ComponentDefinition.ProxyType.GetMethods()
                    .Single(m => m.Name == requirement.Capability &&
                                 m.GetCustomAttribute<CapabilityAttribute>() != null &&
                                 m.IsMethodDefinitionCompatible(requirement.ReceiverMethod)
                    );

                var methodOverride = PrepareMethodOverride(proxyTypeBuilder, requirement.ReceiverMethod);
                GenerateIL(methodOverride, sender.FieldBuilder, senderMethod);

                proxyTypeBuilder.DefineMethodOverride(methodOverride, requirement.ReceiverMethod);
            }

            componentDefinition.ProxyType = proxyTypeBuilder.CreateTypeInfo();
        }

        private Dictionary<string, Dependency> GenerateDependencyFields(TypeBuilder typeBuilder, List<Dependency> dependencies)
        {
            var dependenciesDictionary = new Dictionary<string, Dependency>();

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
            var receiverMethodParameters = baseMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var methodOverride = typeBuilder.DefineMethod(baseMethod.Name,
                                                          MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                                                          baseMethod.ReturnType, receiverMethodParameters);
            if (baseMethod.IsGenericMethod)
            {
                var receiverMethodGenericParameters = baseMethod.GetGenericArguments().ToList();
                var genericParameters = methodOverride.DefineGenericParameters(receiverMethodGenericParameters.Select(a => a.Name).ToArray());
                for (var i = 0; i < genericParameters.Length; i++)
                {
                    var baseTypeConstraint = receiverMethodGenericParameters[i].GetGenericParameterConstraints().SingleOrDefault(c => !c.IsInterface);
                    if (baseTypeConstraint != null)
                    {
                        genericParameters[i].SetBaseTypeConstraint(baseTypeConstraint);
                    }

                    var interfaceConstraints = receiverMethodGenericParameters[i].GetGenericParameterConstraints().Where(c => c.IsInterface).ToArray();
                    if (interfaceConstraints.Any())
                    {
                        genericParameters[i].SetInterfaceConstraints(interfaceConstraints);
                    }
                }
            }

            return methodOverride;
        }

        private void GenerateIL(MethodBuilder methodBuilder, FieldBuilder senderFieldBuilder, MethodInfo senderMethod)
        {
            var ilGenerator = methodBuilder.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);                              // Load "this".
            ilGenerator.Emit(OpCodes.Ldfld, senderFieldBuilder);            // Load instance from field.
            for (var i = 1; i <= senderMethod.GetParameters().Length; i++)
            {
                ilGenerator.Emit(OpCodes.Ldarg, i);                         // Load all parameters.
            }
            ilGenerator.Emit(OpCodes.Callvirt, senderMethod);               // Call method on field instance.
            ilGenerator.Emit(OpCodes.Ret);                                  // Return to caller.
        }

    }
}
