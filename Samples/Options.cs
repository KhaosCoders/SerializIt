using SerializIt;
using CommandLine;

namespace Samples;

internal class Options
{
    [Option('s', "serializer", Required = false, HelpText = "One of: Json, Yaml or Xml", Default = ESerializers.Json)]
    public ESerializers Serializer { get; set; }

    [Option('i', "iterations", Required = false, HelpText = "Number of times to run the serializer", Default = 1, Min = 1)]
    public int Iterations { get; set; }
}
