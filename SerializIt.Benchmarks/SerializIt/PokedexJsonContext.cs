using SerializIt.Benchmarks.Model;

namespace SerializIt.Benchmarks.SerializIt;

[Serializer(ESerializers.Json, ns: "BenchmarkJson")]
[JsonOptions(prettyPrint: false)]
[SerializeType(typeof(Pokedex))]
[SerializeType(typeof(Pokemon))]
[SerializeType(typeof(Next_Evolution))]
[SerializeType(typeof(Prev_Evolution))]
internal partial class PokedexJsonContext
{
}
