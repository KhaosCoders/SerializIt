using SerializIt;
using Samples.Model;

namespace Samples
{
    [Serializer(ESerializers.Xml, "Samples.XmlSerializers")]
    [XmlOptions(true, indentChars: "  ")]
    [SerializeType(typeof(RootElement))]
    [SerializeType(typeof(Container))]
    [SerializeType(typeof(Info))]
    internal partial class XmlContext
    {
    }
}
