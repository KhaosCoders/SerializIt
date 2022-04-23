namespace SerializIt;

public struct JsonSerializerOptions : ISerializerOptions
{
    public bool PrettyPrint { get; set; }
    public bool UseSingleQuotes { get; set; }
    public string? IndentChars { get; set; }
    public bool SkipNullValues { get; set; }
    public char Quotes => UseSingleQuotes ? '\'' : '"';
    public bool UseParallel { get; set; }
}

public class JsonOptionsAttribute : OptionsAttribute
{
    public JsonSerializerOptions? JsonOptions
    {
        get => (JsonSerializerOptions) Options;
        set => Options = value;
    }

    public JsonOptionsAttribute(bool prettyPrint = false,
                                bool useSingeQuotes = false,
                                string? indentChars = default,
                                bool skipNullValues = true,
                                bool parallel = false)
    {
        JsonOptions = new JsonSerializerOptions()
        {
            PrettyPrint = prettyPrint,
            UseSingleQuotes = useSingeQuotes,
            IndentChars = indentChars,
            SkipNullValues = skipNullValues,
            UseParallel = parallel,
        };
    }
}
