using BenchmarkDotNet.Attributes;
using SerializIt.Benchmarks.Contexts;
using SerializIt.Benchmarks.Model;
using SerializIt.Benchmarks.SystemText2;
using SerializIt.Benchmarks.SystemText3;
using System.Text.Json;
using System.Xml.Serialization;
using Tinyhand;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SerializIt.Benchmarks;

[MemoryDiagnoser]
public class PokedexBenchmark
{
    private readonly Pokedex _pokedex;
    private YamlDotNet.Serialization.ISerializer _yamlSerializer;
    private XmlSerializer _xmlSerializer;

    public PokedexBenchmark()
    {
        string data = File.ReadAllText(@"Data\pokedex.json");
        System.Text.Json.JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };
        _pokedex = JsonSerializer.Deserialize<Pokedex>(data, options) ?? new();

        _yamlSerializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _xmlSerializer = new XmlSerializer(typeof(Pokedex));
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
        PokedexYamlContext ctx = new();
        return ctx.Pokedex.SerializeDocument(_pokedex);
    }

    [Benchmark]
    public string SerializItFormattedJson()
    {
        PokedexFormattedJsonContext ctx = new();
        return ctx.Pokedex.SerializeDocument(_pokedex);
    }

    [Benchmark]
    public string YamlDotNet()
    {
        return _yamlSerializer.Serialize(_pokedex);
    }

    [Benchmark]
    public string SerializItYaml()
    {
        PokedexYamlContext ctx = new();
        return ctx.Pokedex.SerializeDocument(_pokedex);
    }

    [Benchmark]
    public string XmlSerializer()
    {
        var writer = new StringWriter();
        _xmlSerializer.Serialize(writer, _pokedex);
        writer.Close();
        return writer.ToString();
    }

    [Benchmark]
    public string SerializItXml()
    {
        PokedexXmlContext ctx = new();
        return ctx.Pokedex.SerializeDocument(_pokedex);
    }

}
