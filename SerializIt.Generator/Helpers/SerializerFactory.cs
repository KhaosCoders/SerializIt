using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SerializIt.Generator.Helpers
{
    internal static class SerializerFactory
    {
        static readonly Dictionary<ESerializers, Type> _serializers = new();

        public static Type GetSerializerType(Attribute attr)
        {
            if (_serializers.Count == 0)
            {
                FindSerializers();
            }

            var property = attr.GetType().GetProperty(nameof(SerializerAttribute.Serializer), BindingFlags.Instance | BindingFlags.Public);
            var serializer = (ESerializers)property.GetValue(attr);

            if (_serializers.TryGetValue(serializer, out var type))
            {
                return type;
            }

            return null;
        }

        private static void FindSerializers()
        {
            var types = typeof(ESerializers).Assembly.GetTypes();
            types.Where(t => typeof(ISerializer).IsAssignableFrom(t))
                .ToList()
                .ForEach(t =>
                {
                    var attributes = t.GetCustomAttributes(typeof(SerializerAttribute), false);
                    if (attributes.Length > 0 && attributes[0] is SerializerAttribute seriAttr)
                    {
                        _serializers.Add(seriAttr.Serializer, t);
                    }
                });
        }
    }
}
