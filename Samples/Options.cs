using SerializIt;
using CommandLine;

namespace Samples;

internal class Options
{
    [Option('s', "serializer", Required = false, HelpText = "One of: Json, Yaml or Xml", Default = ESerializers.Json)]
    public ESerializers Serializer { get; set; }
}
