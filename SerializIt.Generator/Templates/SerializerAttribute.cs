 using System;

namespace SerializIt;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class SerializerAttribute : Attribute
{
    public ESerializers Serializer { get; set; }
    public string Namespace { get; set; }

    public SerializerAttribute(ESerializers serializer = default, string ns = default)
    {
        Serializer = serializer;
        Namespace = ns;
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class SerializerOptionsAttribute : Attribute
{
    public Type OptionsType { get; set; }

    public SerializerOptionsAttribute(Type optionsType)
    {
        OptionsType = optionsType;
    }
}
