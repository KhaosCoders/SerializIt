using System;
using System.Reflection.Metadata;

namespace SerializIt;

internal struct JsonSerializerOptions : ISerializerOptions
{
    public bool PrettyPrint { get; set; }
    public bool UseSingleQuotes { get; set; }
    public string IndentChars { get; set; }
    public bool SkipNullValues { get; set; }
    public char Quotes => UseSingleQuotes ? '\'' : '"';
    public bool UseParallel { get; set; }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class JsonOptionsAttribute : Attribute
{
    public JsonSerializerOptions Options { get; set; }

    public JsonOptionsAttribute(bool prettyPrint = false,
                                bool useSingeQuotes = false,
                                string indentChars = default,
                                bool skipNullValues = true,
                                bool parallel = false)
    {
        Options = new JsonSerializerOptions()
        {
            PrettyPrint = prettyPrint,
            UseSingleQuotes = useSingeQuotes,
            IndentChars = indentChars,
            SkipNullValues = skipNullValues,
            UseParallel = parallel,
        };
    }
}