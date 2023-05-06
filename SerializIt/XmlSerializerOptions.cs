namespace SerializIt;

public struct XmlSerializerOptions : ISerializerOptions
{
    public bool PrettyPrint { get; set; }
    public bool LowercaseTags { get; set; }
    public bool AddDocTag { get; set; }
    public string? IndentChars { get; set; }
    public bool UseParallel { get; set; }
}

public class XmlOptionsAttribute : OptionsAttribute
{
    public XmlSerializerOptions? XmlOptions
    {
        get => (XmlSerializerOptions)Options!;
        set => Options = value;
    }

    public XmlOptionsAttribute(bool prettyPrint = false,
                               bool lowercaseTags = false,
                               bool addDocTag = true,
                               string? indentChars = default,
                               bool useParallel = false)
    {
        XmlOptions = new XmlSerializerOptions()
        {
            PrettyPrint = prettyPrint,
            LowercaseTags = lowercaseTags,
            AddDocTag = addDocTag,
            IndentChars = indentChars,
            UseParallel = useParallel
        };
    }
}
