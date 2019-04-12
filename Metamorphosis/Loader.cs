using Metamorphosis.Attributes;
using Metamorphosis.Modelling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Metamorphosis
{
    internal sealed class Loader
    {
        private readonly string _modelFilename;
        private Dictionary<string, object> _components = new Dictionary<string, object>();

        public Loader(string modelFilename)
        {
            _modelFilename = modelFilename;
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
            var componentDefinitions = componentDefinitionGenerator.GenerateComponentDefinitions(model);

            var proxyGenerator = new ProxyGenerator();
            proxyGenerator.GenerateProxyTypes(componentDefinitions);

            foreach (var componentDefinition in componentDefinitions)
            {
                CreateProxyInstancesRecursive(componentDefinition);

                _components[componentDefinition.Name] = componentDefinition.Instance;
            }
        }

        public void Run()
        {
            foreach (var component in _components.Values)
            {
                var startupMethod = component.GetType().GetMethods()
                    .SingleOrDefault(m => m.GetCustomAttribute<StartupAttribute>() != null);
                startupMethod?.Invoke(component, null);
            }

            Console.WriteLine("Press \"q\" to quit.");
            while (Console.ReadKey(true).KeyChar != 'q')
            {
            }
        }

        private void CreateProxyInstancesRecursive(ComponentDefinition componentDefinition)
        {
            if (componentDefinition.Instance != null)
            {
                return;
            }

            var instance = Activator.CreateInstance(componentDefinition.ProxyType);

            foreach (var dependency in componentDefinition.Dependencies)
            {
                CreateProxyInstancesRecursive(dependency.ComponentDefinition);

                var field = instance.GetType().GetField($"__proxy_{dependency.Name}", BindingFlags.Instance | BindingFlags.NonPublic);
                field.SetValue(instance, dependency.ComponentDefinition.Instance);
            }

            componentDefinition.Instance = instance;
        }
    }
}
