using BenchmarkDotNet.Attributes;
using SerializIt.Benchmarks.Contexts;
using SerializIt.Benchmarks.Model;
using SerializIt.Benchmarks.SystemText2;
using SerializIt.Benchmarks.SystemText3;
using System.Text.Json;
using Tinyhand;

namespace SerializIt.Benchmarks;

[MemoryDiagnoser]
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
    public string TinyhandJson()
    {
        return TinyhandSerializer.SerializeToString(_pokedex);
    }

    [Benchmark]
    public string SystemTextJsonSG()
    {
        return JsonSerializer.Serialize(_pokedex, PokedexJsonContext2.Default.Pokedex);
    }

    [Benchmark]
    public string SystemTextFormattedJsonSG()
    {
        return JsonSerializer.Serialize(_pokedex, PokedexJsonContext3.Default.Pokedex);
    }

    /* [Benchmark]
     public string SystemTextJson()
     {
         System.Text.Json.JsonSerializerOptions options = new()
         {
             WriteIndented = false,
         };
         return JsonSerializer.Serialize(_pokedex, options);
     }

     [Benchmark]
     public string SystemTextFormattedJson()
     {
         System.Text.Json.JsonSerializerOptions options = new()
         {
             WriteIndented = true,
         };
         return JsonSerializer.Serialize(_pokedex, options);
     }*/

    [Benchmark]
    public string SerializItJson()
    {
        PokedexJsonContext ctx = new();
        return ctx.Pokedex.SerializeDocument(_pokedex);
    }

    [Benchmark]
    public string SerializItFormattedJson()
    {
        PokedexFormattedJsonContext ctx = new();
        return ctx.Pokedex.SerializeDocument(_pokedex);
    }

}
