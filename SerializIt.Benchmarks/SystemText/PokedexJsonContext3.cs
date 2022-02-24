using SerializIt.Benchmarks.Model;
using System.Text.Json.Serialization;

namespace SerializIt.Benchmarks.SystemText3;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Pokedex))]
internal partial class PokedexJsonContext3 : JsonSerializerContext
{
}
