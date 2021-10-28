﻿using programmersdigest.Metamorphosis.Attributes;
using programmersdigest.Metamorphosis.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace programmersdigest.Metamorphosis.Modelling
{
    internal sealed class ProxyGenerator
    {
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;

        private Dictionary<string, ComponentDefinition> _componentDefinitions = null!;

        public ProxyGenerator()
        {
            var assemblyName = new AssemblyName("Metamorphosis.DynamicProxies");
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule("Metamorphosis.DynamicProxies");
        }

        public void GenerateProxyTypes(IReadOnlyList<ComponentDefinition> componentDefinitions)
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
                throw new ComponentDefinitionLoopException($"The recursion depth is greater than the number of components. Is there a loop in the endpoint configuration?");
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

            foreach (var baseTypeConstructor in componentDefinition.BaseType.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                GeneratePassthroughConstructor(proxyTypeBuilder, baseTypeConstructor);
            }

            var dependenciesDictionary = GenerateDependencyFields(proxyTypeBuilder, componentDefinition.Dependencies);

            foreach (var signal in componentDefinition.Signals)
            {
                if (signal.Connections.Count <= 0)
                {
                    if (signal.SignalMethod.IsAbstract)
                    {
                        throw new MissingConnectionException($"{componentDefinition.Name}.{signal.SignalMethod.Name} is mandatory. Please define a connection.");
                    }
                }
                else
                {
                    if (signal.Connections.Count > 1 && signal.SignalMethod.ReturnType != typeof(void))
                    {
                        throw new InvalidOperationException($"{componentDefinition.Name}.{signal.SignalMethod.Name} cannot be used with multiple connections because it expects a single return value.");
                    }

                    var signalMethodOverride = PrepareMethodOverride(proxyTypeBuilder, signal.SignalMethod);
                    GenerateMethodOverrideIL(signalMethodOverride, signal, dependenciesDictionary);
                    proxyTypeBuilder.DefineMethodOverride(signalMethodOverride, signal.SignalMethod);
                }
            }

            componentDefinition.ProxyType = proxyTypeBuilder.CreateTypeInfo()!;
        }

        private static Dictionary<string, DependencyDefinition> GenerateDependencyFields(TypeBuilder typeBuilder, IReadOnlyCollection<DependencyDefinition> dependencies)
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

        private static MethodBuilder PrepareMethodOverride(TypeBuilder typeBuilder, MethodInfo baseMethod)
        {
            var signalMethodParameters = baseMethod.GetParameters();
            var signalMethodParameterTypes = signalMethodParameters.Select(p => p.ParameterType).ToArray();
            var signalMethodOverride = typeBuilder.DefineMethod(baseMethod.Name,
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual,
                baseMethod.ReturnType, signalMethodParameterTypes);

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

            for (var i = 0; i < signalMethodParameters.Length; ++i)
            {
                var parameter = signalMethodParameters[i];
                var parameterBuilder = signalMethodOverride.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0)
                {
                    parameterBuilder.SetConstant(parameter.RawDefaultValue);
                }

                foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData()))
                {
                    parameterBuilder.SetCustomAttribute(attribute);
                }
            }

            foreach (var attribute in BuildCustomAttributes(baseMethod.GetCustomAttributesData()))
            {
                signalMethodOverride.SetCustomAttribute(attribute);
            }

            return signalMethodOverride;
        }

        private static void GenerateMethodOverrideIL(MethodBuilder methodBuilder, SignalDefinition signal, Dictionary<string, DependencyDefinition> dependencies)
        {
            var ilGenerator = methodBuilder.GetILGenerator();

            foreach (var connection in signal.Connections)
            {
                var receiver = dependencies[connection.Receiver];
                var triggerMethod = receiver.ComponentDefinition.ProxyType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .SingleOrDefault(m => m.Name == connection.SignalName &&
                                 m.GetCustomAttribute<TriggerAttribute>() != null &&
                                 m.IsMethodDefinitionCompatible(signal.SignalMethod)
                    );

                if (triggerMethod == null)
                {
                    var triggerName = $"{connection.Receiver}.{connection.TriggerName}";
                    throw new MissingTriggerException($"The trigger {triggerName} cannot be found. Does the component implement this trigger? Is the trigger method public?");
                }

                ilGenerator.Emit(OpCodes.Ldarg_0);                                  // Load "this".
                ilGenerator.Emit(OpCodes.Ldfld, receiver.FieldBuilder);             // Load receiver from field.
                for (var i = 1; i <= triggerMethod.GetParameters().Length; i++)
                {
                    ilGenerator.Emit(OpCodes.Ldarg, i);                             // Load all parameters.
                }
                ilGenerator.Emit(OpCodes.Callvirt, triggerMethod);                  // Call trigger method on receiver instance.
            }

            ilGenerator.Emit(OpCodes.Ret);                                      // Return to caller.
        }

        private static void GeneratePassthroughConstructor(TypeBuilder typeBuilder, ConstructorInfo baseTypeConstructor)
        {
            var parameters = baseTypeConstructor.GetParameters();
            if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
            {
                throw new InvalidOperationException("Variadic constructors are not supported");
            }

            var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
            var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
            var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.Public, baseTypeConstructor.CallingConvention,
                parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
            for (var i = 0; i < parameters.Length; ++i)
            {
                var parameter = parameters[i];
                var parameterBuilder = constructor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
                if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0)
                {
                    parameterBuilder.SetConstant(parameter.RawDefaultValue);
                }

                foreach (var attribute in BuildCustomAttributes(parameter.GetCustomAttributesData()))
                {
                    parameterBuilder.SetCustomAttribute(attribute);
                }
            }

            foreach (var attribute in BuildCustomAttributes(baseTypeConstructor.GetCustomAttributesData()))
            {
                constructor.SetCustomAttribute(attribute);
            }

            var emitter = constructor.GetILGenerator();
            emitter.Emit(OpCodes.Nop);

            // Load `this` and call base constructor with arguments
            emitter.Emit(OpCodes.Ldarg_0);
            for (var i = 1; i <= parameters.Length; ++i)
            {
                emitter.Emit(OpCodes.Ldarg, i);
            }
            emitter.Emit(OpCodes.Call, baseTypeConstructor);

            emitter.Emit(OpCodes.Ret);
        }

        private static CustomAttributeBuilder[] BuildCustomAttributes(IEnumerable<CustomAttributeData> customAttributes)
        {
            return customAttributes.Select(attribute =>
            {
                var attributeArgs = attribute.ConstructorArguments.Select(a => a.Value).ToArray();
                var namedPropertyInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
                var namedPropertyValues = attribute.NamedArguments.Where(a => a.MemberInfo is PropertyInfo).Select(a => a.TypedValue.Value).ToArray();
                var namedFieldInfos = attribute.NamedArguments.Select(a => a.MemberInfo).OfType<FieldInfo>().ToArray();
                var namedFieldValues = attribute.NamedArguments.Where(a => a.MemberInfo is FieldInfo).Select(a => a.TypedValue.Value).ToArray();
                return new CustomAttributeBuilder(attribute.Constructor, attributeArgs, namedPropertyInfos, namedPropertyValues, namedFieldInfos, namedFieldValues);
            }).ToArray();
        }
    }
}
