using SerializIt.Benchmarks.Model;

namespace SerializIt.Benchmarks.SerializIt;

[Serializer(ESerializers.Json, ns: "BenchmarkFormattedJson")]
[JsonOptions(prettyPrint: true)]
[SerializeType(typeof(Pokedex))]
[SerializeType(typeof(Pokemon))]
[SerializeType(typeof(Next_Evolution))]
[SerializeType(typeof(Prev_Evolution))]
internal partial class PokedexFormattedJsonContext
{
}
