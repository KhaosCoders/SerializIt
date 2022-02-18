using BenchmarkDotNet.Attributes;
using SerializIt.Benchmarks.Model;
using System.Text.Json;

namespace SerializIt.Benchmarks;

public class PokedexBenchmark
{
    private readonly Pokedex _pokedex;

    public PokedexBenchmark()
    {
        string data = File.ReadAllText(@"Data\pokedex.json");
        System.Text.Json.JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };
        _pokedex = JsonSerializer.Deserialize<Pokedex>(data, options) ?? new();
    }

    [Benchmark]
    public string SerializIt()
    {
        PokedexJsonContext ctx = new();
        return ctx.Pokedex.SerializeDocument(_pokedex);
    }
}
