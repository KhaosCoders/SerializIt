using SerializIt;
using Samples.Model;

namespace Samples
{
    [Serializer(ESerializers.Yaml, "Samples.YamlSerializers")]
    [YamlOptions(indentChars: "  ")]
    [SerializeType(typeof(RootElement))]
    [SerializeType(typeof(Container))]
    [SerializeType(typeof(Info))]
    internal partial class YamlContext
    {
    }
}
