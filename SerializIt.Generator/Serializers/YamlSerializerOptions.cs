using System;

namespace SerializIt;

internal struct YamlSerializerOptions
{
    public bool AddPreamble { get; set; }
    public bool AddPostamble { get; set; }
    public string IndentChars { get; set; }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class YamlOptionsAttribute : Attribute
{
    public YamlSerializerOptions Options { get; set; }

    public YamlOptionsAttribute(bool addPreamble = false,
                                bool addPostamble = false,
                                string indentChars = default)
    {
        Options = new YamlSerializerOptions()
        {
            AddPreamble = addPreamble,
            AddPostamble = addPostamble,
            IndentChars = indentChars
        };
    }
}