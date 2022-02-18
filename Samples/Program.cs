using Samples;
using Samples.Model;
using CommandLine;

Parser.Default.ParseArguments<Options>(args).WithParsed(Run);

static void Run(Options o)
{
    Console.WriteLine($"{o.Serializer} serialized object:");

    var root = new RootElement()
    {
        Name = "test",
        Type = "root",
        Children = new List<Container>()
        {
            new Container()
            {
                Id = 1,
                Info = new() { Info1 = "test1", Info2 = 100 },
                Children = new List<Container>()
                {
                    new Container()
                    {
                        Id = 3,
                        Info = new() { Info1 = "test3", Info2 = 300 }
                    }
                }
            },
            new Container()
            {
                Id = 2,
                Info = new() { Info1 = "test2\r\nmultiline", Info2 = 200 }
            },
            new Container()
            {
                Id = 4,
                Info = null
            }
        }
    };

    switch (o.Serializer)
    {
        case SerializIt.ESerializers.Json:
            Console.WriteLine(new JsonContext().RootElement.SerializeDocument(root));
            break;
        case SerializIt.ESerializers.Yaml:
            Console.WriteLine(new YamlContext().RootElement.SerializeDocument(root));
            break;
        case SerializIt.ESerializers.Xml:
            Console.WriteLine(new XmlContext().RootElement.SerializeDocument(root));
            break;
    }
}
