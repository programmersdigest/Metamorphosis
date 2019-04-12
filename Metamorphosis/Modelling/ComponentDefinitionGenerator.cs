using Metamorphosis.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Metamorphosis.Modelling
{
    internal sealed class ComponentDefinitionGenerator
    {
        private readonly Dictionary<string, Type> _loadedTypes;

        public ComponentDefinitionGenerator()
        {
            _loadedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetTypes().Where(t => t.GetCustomAttribute<ComponentAttribute>() != null))
                .ToDictionary(t => t.FullName);
        }

        public List<ComponentDefinition> GenerateComponentDefinitions(Model model)
        {
            return model.Components.Select(c => GenerateComponentDefinition(c, model.Connections))
                .ToList();
        }

        private ComponentDefinition GenerateComponentDefinition(ComponentModel componentModel, IReadOnlyList<ConnectionModel> connections)
        {
            var name = componentModel.Name;
            var baseType = _loadedTypes[componentModel.Type];

            var signals = baseType.GetMethods()
                .Where(m => m.GetCustomAttribute<SignalAttribute>() != null)
                .Select(m => new SignalDefinition
                {
                    SignalMethod = m,
                    Connections = connections.Where(c => c.Sender == componentModel.Name && c.SignalName == m.Name)
                                             .ToList()
                })
                .ToList();

            var dependencies = signals.SelectMany(r => r.Connections)
                .GroupBy(c => c.Receiver)
                .Select(g => new DependencyDefinition
                {
                    Name = g.Key
                })
                .ToList();

            return new ComponentDefinition
            {
                Name = name,
                BaseType = baseType,
                Signals = signals,
                Dependencies = dependencies
            };
        }
    }
}
