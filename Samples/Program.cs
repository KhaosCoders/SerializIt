using Samples;
using Samples.Model;
using CommandLine;
using System.Windows;

Parser.Default.ParseArguments<Options>(args).WithParsed(Run);

static void Run(Options o)
{
    Console.WriteLine($"{o.Serializer} serialized object (x{o.Iterations}):");

    var root = new RootElement()
    {
        Name = "test",
        Type = "root",
        Children = new List<Container>()
        {
            new Container()
            {
                Id = 1,
                Info = new() { Info1 = "test1", Info2 = 100, Infos = Array.Empty<string>() },
                Children = new List<Container>()
                {
                    new Container()
                    {
                        Id = 3,
                        Info = new() { Info1 = "test3", Info2 = 300, Infos = new string[]{ "A", "B", "C" } }
                    }
                }
            },
            new Container()
            {
                Id = 2,
                Info = new() { Info1 = "test2\r\nmultiline", Info2 = 200, Infos = new string[]{ "0" } }
            },
            new Container()
            {
                Id = 4,
                Info = null
            }
        }
    };

    JetBrains.Profiler.Api.MeasureProfiler.StartCollectingData();
    JetBrains.Profiler.Api.MemoryProfiler.GetSnapshot("start");

    switch (o.Serializer)
    {
        case SerializIt.ESerializers.Json:
            for (int x = 0; x < o.Iterations; x++)
            {
                var text = new JsonContext().RootElement.SerializeDocument(root);
                if (o.Iterations == 1)
                {
                    Console.WriteLine(text);
                }
            }
            break;
        case SerializIt.ESerializers.Yaml:
            for (int x = 0; x < o.Iterations; x++)
            {
                var text = new YamlContext().RootElement.SerializeDocument(root);
                if (o.Iterations == 1)
                {
                    Console.WriteLine(text);
                }
            }
            break;
        case SerializIt.ESerializers.Xml:
            for (int x = 0; x < o.Iterations; x++)
            {
                var text = new XmlContext().RootElement.SerializeDocument(root);
                if (o.Iterations == 1)
                {
                    Console.WriteLine(text);
                }
            }
            break;
    }

    JetBrains.Profiler.Api.MeasureProfiler.StopCollectingData();
    JetBrains.Profiler.Api.MemoryProfiler.GetSnapshot("end");
}
