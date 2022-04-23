namespace SerializIt;


[AttributeUsage(AttributeTargets.Class)]
public abstract class OptionsAttribute : Attribute
{
    public ISerializerOptions? Options { get; set; }
}
