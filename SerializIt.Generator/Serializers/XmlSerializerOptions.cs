using System;

namespace SerializIt;

internal struct XmlSerializerOptions
{
    public bool PrettyPrint { get; set; }
    public bool LowercaseTags { get; set; }
    public bool AddDocTag { get; set; }
    public string IndentChars { get; set; }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class XmlOptionsAttribute : Attribute
{
    public XmlSerializerOptions Options { get; set; }

    public XmlOptionsAttribute(bool prettyPrint = false,
                               bool lowercaseTags = false,
                               bool addDocTag = true,
                               string indentChars = default)
    {
        Options = new XmlSerializerOptions()
        {
            PrettyPrint = prettyPrint,
            LowercaseTags = lowercaseTags,
            AddDocTag = addDocTag,
            IndentChars = indentChars
        };
    }
}