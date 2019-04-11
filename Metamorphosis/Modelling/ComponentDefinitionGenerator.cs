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
            return model.Components.Select(GenerateComponentDefinition).ToList();
        }

        private ComponentDefinition GenerateComponentDefinition(ComponentModel componentModel)
        {
            var name = componentModel.Name;
            var baseType = _loadedTypes[componentModel.Type];

            var requirements = baseType.GetMethods()
                .Where(m => m.IsAbstract && m.GetCustomAttribute<RequirementAttribute>() != null)
                .Select(m => new Requirement
                {
                    ReceiverMethod = m,
                    Sender = componentModel.Endpoints[m.Name].Sender,
                    Capability = componentModel.Endpoints[m.Name].Capability
                })
                .ToList();

            var dependencies = requirements.GroupBy(r => r.Sender)
                .Select(g => new Dependency
                {
                    Name = g.Key
                })
                .ToList();

            return new ComponentDefinition
            {
                Name = name,
                BaseType = baseType,
                Requirements = requirements,
                Dependencies = dependencies
            };
        }
    }
}
