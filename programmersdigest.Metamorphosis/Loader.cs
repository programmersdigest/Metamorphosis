using programmersdigest.Metamorphosis.Modelling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace programmersdigest.Metamorphosis
{
    internal sealed class Loader
    {
        private readonly string _modelFilename;
        private readonly Func<ConstructorInfo[], object[]> _resolveConstructorParametersCallback;
        private IReadOnlyList<ComponentDefinition> _componentDefinitions;

        public Loader(string modelFilename, Func<ConstructorInfo[], object[]> resolveConstructorParametersCallback = null)
        {
            _modelFilename = modelFilename;
            _resolveConstructorParametersCallback = resolveConstructorParametersCallback;
        }

        public void Init()
        {
            Model model;

            using (var stream = new FileStream(_modelFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var textReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                var serializer = new JsonSerializer();
                model = serializer.Deserialize<Model>(jsonReader);
            }

            foreach (var assemblyFilename in model.Assemblies)
            {
                if (AppDomain.CurrentDomain.GetAssemblies().Any(a => a.GetName().Name == assemblyFilename))
                {
                    continue;
                }

                if (File.Exists(assemblyFilename + ".dll"))
                {
                    Assembly.LoadFrom(assemblyFilename + ".dll");
                }
                else
                {
                    Assembly.LoadFrom(assemblyFilename + ".so");
                }
            }

            var componentDefinitionGenerator = new ComponentDefinitionGenerator();
            _componentDefinitions = componentDefinitionGenerator.GenerateComponentDefinitions(model);

            var proxyGenerator = new ProxyGenerator();
            proxyGenerator.GenerateProxyTypes(_componentDefinitions);

            foreach (var componentDefinition in _componentDefinitions)
            {
                CreateProxyInstancesRecursive(componentDefinition);
            }
        }

        public void Run()
        {
            var lifetimeService = _componentDefinitions.FirstOrDefault(cd => cd.BaseType == typeof(Lifecycle))?.Instance as Lifecycle;
            lifetimeService?.SignalStartup();

            Console.WriteLine("Press \"q\" to quit.");
            while (Console.ReadKey(true).KeyChar != 'q')
            {
            }

            Console.WriteLine("Shutting down.");

            lifetimeService?.SignalShutdown();
            DisposeProxyInstancesRecursive();
        }

        private void CreateProxyInstancesRecursive(ComponentDefinition componentDefinition)
        {
            if (componentDefinition.Instance != null)
            {
                return;
            }

            var constructorParameters = _resolveConstructorParametersCallback?.Invoke(componentDefinition.ProxyType.GetConstructors()) ?? Array.Empty<object>();
            var instance = Activator.CreateInstance(componentDefinition.ProxyType, constructorParameters);

            foreach (var dependency in componentDefinition.Dependencies)
            {
                CreateProxyInstancesRecursive(dependency.ComponentDefinition);

                var field = instance.GetType().GetField($"__proxy_{dependency.Name}", BindingFlags.Instance | BindingFlags.NonPublic);
                field.SetValue(instance, dependency.ComponentDefinition.Instance);
            }

            componentDefinition.Instance = instance;
        }

        private void DisposeProxyInstancesRecursive()
        {
            var disposalList = new List<ComponentDefinition>();
            CollectDependenciesRecursive(_componentDefinitions, disposalList);

            disposalList.Reverse();
            foreach (var disposable in disposalList.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }

        private void CollectDependenciesRecursive(IReadOnlyList<ComponentDefinition> components, List<ComponentDefinition> disposalList)
        {
            foreach (var component in components)
            {
                var dependencies = component.Dependencies.Select(d => d.ComponentDefinition).ToList();
                CollectDependenciesRecursive(dependencies, disposalList);

                if (!disposalList.Contains(component))
                {
                    disposalList.Add(component);
                }
            }
        }
    }
}
