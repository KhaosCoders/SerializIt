using System;
using System.IO;
using System.Reflection;

namespace SerializIt.Generator.Helpers;

internal static class Resources
{
    public static Lazy<string> SerializeTypeAttribute { get; } = new Lazy<string>(() => ReadResource("SerializIt.Generator.Templates.SerializeTypeAttribute.cs"));
    public static Lazy<string> SkipAttribute { get; } = new Lazy<string>(() => ReadResource("SerializIt.Generator.Templates.SkipAttribute.cs"));
    public static Lazy<string> OrderAttribute { get; } = new Lazy<string>(() => ReadResource("SerializIt.Generator.Templates.OrderAttribute.cs"));
    public static Lazy<string> ESerializers { get; } = new Lazy<string>(() => ReadResource("SerializIt.Generator.Templates.ESerializers.cs"));
    public static Lazy<string> SerializerAttribute { get; } = new Lazy<string>(() => ReadResource("SerializIt.Generator.Templates.SerializerAttribute.cs"));
    public static Lazy<string> IndentedWriter { get; } = new Lazy<string>(() => ReadResource("SerializIt.Generator.IndentedWriter.cs"));
    public static Lazy<string> ISerializerOptions { get; } = new Lazy<string>(() => ReadResource("SerializIt.Generator.Serializers.ISerializerOptions.cs"));
    public static Lazy<string> JsonSerializerOptions { get; } = new Lazy<string>(() => ReadResource("SerializIt.Generator.Serializers.JsonSerializerOptions.cs"));
    public static Lazy<string> YamlSerializerOptions { get; } = new Lazy<string>(() => ReadResource("SerializIt.Generator.Serializers.YamlSerializerOptions.cs"));
    public static Lazy<string> XmlSerializerOptions { get; } = new Lazy<string>(() => ReadResource("SerializIt.Generator.Serializers.XmlSerializerOptions.cs"));

    private static string ReadResource(string resource)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
        if (stream == null)
        {
            return null;
        }
        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
