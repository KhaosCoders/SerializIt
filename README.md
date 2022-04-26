<p align="center">
<img src ="https://raw.githubusercontent.com/KhaosCoders/SerializIt/master/Art/Icon.png" width=200 />
</p>

# SerializIt
A super fast serializer using Roslyn SourceGenerators instead of reflection

![Build](https://github.com/KhaosCoders/SerializIt/actions/workflows/build.yml/badge.svg)
![Test](https://github.com/KhaosCoders/SerializIt/actions/workflows/test.yml/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/SerializIt.svg)](https://www.nuget.org/packages/SerializIt/)
<!-- [![Coverage](https://codecov.io/gh/KhaosCoders/SerializIt/branch/master/graph/badge.svg?token=xxx)](https://codecov.io/gh/KhaosCoders/SerializIt) -->

**Only supports serialization, no deserialization!**

## Supported Serializers

- JSON
- YAML
- (basic) XML

# Installation

You can find this package on [nuget.org](https://www.nuget.org/packages/SerializIt).

Install `SerializIt` using Package Manager Console:
```powershell
Install-Package SerializIt
```

Or a terminal:
```bash
dotnet add package SerializIt 
```

# Usage
Create a model you want to serialize.  
Then create a `partial` serialization context like this
```C#
using SerializIt;
using Samples.Model;

namespace Samples
{
    [Serializer(ESerializers.Yaml)]
    [YamlOptions(indentChars: "  ")]
    [SerializeType(typeof(RootElement))]
    [SerializeType(typeof(Container))]
    [SerializeType(typeof(Info))]
    internal partial class YamlContext { }
}
```

This context will be augmented with a serializer for each defined type.

Call the `SerializeDocument` (or `SerializeElement`) method to serialize an object instance.

```C#
// var root = new RootElement();

var serializeIt = new YamlContext();
string text = serializeIt.RootElement.SerializeDocument(root);
```

Take the [Samples](https://github.com/KhaosCoders/SerializIt/tree/main/Samples) project in this repo as an example.

Serialization should be super fast!

# Benchmarks

This Benchmark compares `SerializIt` **JSON**, **YAML** and **XML** serialization with other common libraries, like [Tinyhand](https://github.com/archi-Doc/Tinyhand), [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-6-0), [YamlDotNet](https://github.com/aaubry/YamlDotNet) and [System.Xml.Serialization](https://docs.microsoft.com/en-us/dotnet/standard/serialization/examples-of-xml-serialization). This benchmark was run on my HP Notebook (16GB Ram, Intel i7-8565U, SSD).

|                    Method |        Mean |     Error |    StdDev |      Median |     Gen 0 |    Gen 1 |   Gen 2 | Allocated |
|-------------------------- |------------:|----------:|----------:|------------:|----------:|---------:|--------:|----------:|
|              TinyhandJson |    428.7 us |  17.17 us |  48.98 us |    398.2 us |   49.8047 |   9.7656 |       - |    207 KB |
|          SystemTextJsonSG |    221.4 us |   3.41 us |   3.50 us |    220.3 us |   38.3301 |  38.3301 | 38.3301 |    121 KB |
| SystemTextFormattedJsonSG |    285.7 us |   3.32 us |   2.77 us |    285.2 us |   66.4063 |  66.4063 | 66.4063 |    208 KB |
|            SerializItJson |    194.2 us |   0.70 us |   0.58 us |    194.4 us |   41.5039 |  41.5039 | 41.5039 |    130 KB |
|   SerializItFormattedJson |    308.7 us |   2.59 us |   2.02 us |    309.5 us |   52.2461 |  52.2461 | 52.2461 |    164 KB |
|                YamlDotNet | 13,335.0 us | 111.03 us | 159.23 us | 13,327.2 us | 1125.0000 | 218.7500 | 31.2500 |  4,873 KB |
|            SerializItYaml |    206.9 us |   2.53 us |   2.25 us |    206.9 us |   41.5039 |  41.5039 | 41.5039 |    130 KB |
|             XmlSerializer |    628.0 us |   7.36 us |   6.89 us |    626.4 us |   54.6875 |  54.6875 | 54.6875 |    395 KB |
|             SerializItXml |    175.0 us |   2.55 us |   2.26 us |    174.3 us |   65.6738 |  65.6738 | 65.6738 |    207 KB |

## Conclusion

`SerializIt` is fast. But, if you want to serialize to and from **JSON** your better of with the [Source Generators](https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-source-generator/) in `System.Text.Json`. When you're after **YAML** or **XML** serialization however, `SerializIt` is your best option. It delivers much faster serialization to these formats, than the common libraries this benchmark was performed with.

## Remarks
`SerializIt` does only serialize object. There is no de-serialization for now. So if you need to de-serialize some **YAML** or **XML** you're again better of with the mentioned libraries, as they offer de-serialization, too. Moreover, the **XML** serializer comming with `SerializIt` is in *alpha* stage, at best. A lot of work needs to be done, to make it a helpfull serializer. 

# Features

This serializer supports the following features: 
- Formats: JSON, YAML, (basic) XML
- PrettyPrint (JSON / XML): On/Off 
- Custom Indentation - Defaults to Tabs
- Can skip defined properties
- Can skip properties with default value
- Can order properties

## Pending improvements

[] Value Converters (serialize complex data types, like `GUID` or `BitInteger`)  
[] Custom serializers (support custom formats)  
[] Serializer specific member options (like CDATA for XML or Inline for Yaml & Json)  
[] XML Namespaces  
[] XML Attributes  

# Support this <3

If you like my work, please support this project!  
Donate via [PayPal](https://www.paypal.com/donate?hosted_button_id=37PBGZPHXY8EC)
or become a [Sponsor on GitHub](https://github.com/sponsors/Khaos66)