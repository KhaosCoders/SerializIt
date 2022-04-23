<p align="center">
<img src ="https://raw.githubusercontent.com/KhaosCoders/SerializIt/master/Art/Icon.png" width=200 />
</p>

# SerializIt
A super fast serializer using Roslyn SourceGenerators instead of reflection

**!THIS IS WORK IN PROGRESS!**

**Only supports serialization, no deserialization!**

## Supported Serializers
- JSON
- YAML
- XML

# Installation
No nuget package released until now

# Usage
Create a model you want to serialize. See the sample in this repo.

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

Call the `SerializeDocument` (or `SerializeElement`) method to serialize an instance

```C#
// var root = new RootElement();

var serializeIt = new YamlContext();
string text = serializeIt.RootElement.SerializeDocument(root);
```

Serialization should be super fast!

# Benchmarks

> Need to be created

# Features

This serializer supports the following features: 
- Formats: JSON, YAML, XML
- PrettyPrint (JSON / XML): On/Off 
- Custom Indentation - Defaults to Tabs
- Can skip defined properties
- Can skip properties if default
- Can order properties

## Pending improvements
[] Value Converters (serialize complex data types)  
[] Custom serializers (support custom formats)  
[] Serializer specific member options (like CDATA for XML or Inline for Yaml & Json)  
[] XML Attributes  
[] XML Namespaces  

# Support this <3
If you like my work, please support this project!  
Donate via [PayPal](https://www.paypal.com/donate?hosted_button_id=37PBGZPHXY8EC)