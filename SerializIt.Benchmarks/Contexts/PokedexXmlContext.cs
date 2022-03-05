using SerializIt;
using SerializIt.Benchmarks.Model;

namespace SerializIt.Benchmarks.Contexts;

[Serializer(ESerializers.Xml, ns: "BenchmarkXml")]
[SerializeType(typeof(Pokedex))]
[SerializeType(typeof(Pokemon))]
[SerializeType(typeof(Next_Evolution))]
[SerializeType(typeof(Prev_Evolution))]
internal partial class PokedexXmlContext
{
}
