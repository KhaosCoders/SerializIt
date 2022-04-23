using System;
using System.Collections.Generic;
using System.Linq;

namespace SerializIt.Generator.Helpers
{
    internal static class SerializerFactory
    {
        static readonly Dictionary<ESerializers, Type> serializers = new();

        public static Type? GetSerializerType(SerializerAttribute attr)
        {
            if (serializers.Count == 0)
            {
                FindSerializers();
            }

            return serializers.TryGetValue(attr?.Serializer ?? ESerializers.Json, out var type) ? type : null;
        }

        private static void FindSerializers()
        {
            var types = typeof(SerializerFactory).Assembly.GetTypes();
            types.Where(t => typeof(ISerializer).IsAssignableFrom(t))
                .ToList()
                .ForEach(t =>
                {
                    var attributes = t.GetCustomAttributes(typeof(SerializerAttribute), false);
                    if (attributes.Length > 0 && attributes[0] is SerializerAttribute seriAttr)
                    {
                        serializers.Add(seriAttr.Serializer, t);
                    }
                });
        }
    }
}
