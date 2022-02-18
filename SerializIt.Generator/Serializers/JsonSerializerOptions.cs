using System;

namespace SerializIt;

internal struct JsonSerializerOptions
{
    public bool PrettyPrint { get; set; }
    public bool UseSingleQuotes { get; set; }
    public string IndentChars { get; set; }
    public bool SkipNullValues { get; set; }
    public char Quotes => UseSingleQuotes ? '\'' : '"';
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class JsonOptionsAttribute : Attribute
{
    public JsonSerializerOptions Options { get; set; }

    public JsonOptionsAttribute(bool prettyPrint = false,
                                bool useSingeQuotes = false,
                                string indentChars = default,
                                bool skipNullValues = true)
    {
        Options = new JsonSerializerOptions()
        {
            PrettyPrint = prettyPrint,
            UseSingleQuotes = useSingeQuotes,
            IndentChars = indentChars,
            SkipNullValues = skipNullValues
        };
    }
}