namespace SerializIt;

public struct YamlSerializerOptions : ISerializerOptions
{
    public bool AddPreamble { get; set; }
    public bool AddPostamble { get; set; }
    public bool UseParallel { get; set; }
    public string? IndentChars { get; set; }
}

public class YamlOptionsAttribute : OptionsAttribute
{
    public YamlSerializerOptions? YamlOptions
    {
        get => (YamlSerializerOptions)Options;
        set => Options = value;
    }

    public YamlOptionsAttribute(bool addPreamble = false,
                                bool addPostamble = false,
                                string? indentChars = default,
                                bool useParallel = false)
    {
        YamlOptions = new YamlSerializerOptions()
        {
            AddPreamble = addPreamble,
            AddPostamble = addPostamble,
            IndentChars = indentChars,
            UseParallel = useParallel
        };
    }
}
