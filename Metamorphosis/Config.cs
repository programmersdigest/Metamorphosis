using Metamorphosis.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Metamorphosis
{
    [Component]
    public sealed class Config
    {
        private readonly Dictionary<string, JToken> _values = new Dictionary<string, JToken>();

        public Config(Dictionary<string, JToken> values)
        {
            _values = values;
        }

        [Capability]
        public object Get(string key, Type type, object defaultValue)
        {
            if (!_values.TryGetValue(key, out var value))
            {
                return defaultValue;
            }

            try
            {
                return value.ToObject(type);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
