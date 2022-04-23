namespace SerializIt;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SerializerAttribute : Attribute
{
    public ESerializers Serializer { get; }
    public string? Namespace { get; }

    public SerializerAttribute(ESerializers serializer = default, string? ns = default)
    {
        Serializer = serializer;
        Namespace = ns;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SerializerOptionsAttribute : Attribute
{
    public Type? OptionsType { get; }

    public SerializerOptionsAttribute(Type optionsType)
    {
        OptionsType = optionsType;
    }
}
