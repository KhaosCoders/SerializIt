using SerializIt.Generator.Serializers;
using System.Collections.Generic;

namespace SerializIt.Generator.Model
{
    internal record SerializationContext
    {
        public string ClassName { get; set; }
        public string SerializerNamespace { get; set; }
        public string ContextNamespace { get; set; }
        public string Accessability { get; set; }

        public IList<SerializeType> SerializeTypes { set; get; } = new List<SerializeType>();

        public ISerializer Serializer { get; set; }

        public SerializationContext(string className, string nsContext)
        {
            ClassName = className;
            ContextNamespace = nsContext;
            SerializerNamespace = "SerializIt.Serializers";
        }
    }
}
