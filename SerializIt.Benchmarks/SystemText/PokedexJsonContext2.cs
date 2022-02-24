using SerializIt.Benchmarks.Model;
using System.Text.Json.Serialization;

namespace SerializIt.Benchmarks.SystemText2;

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(Pokedex))]
internal partial class PokedexJsonContext2 : JsonSerializerContext
{
}
