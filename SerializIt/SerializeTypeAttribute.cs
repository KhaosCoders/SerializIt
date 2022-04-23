using System;

namespace SerializIt;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SerializeTypeAttribute : Attribute
{
    public Type SerializeType { get; set; }

    public SerializeTypeAttribute(Type typeToSerialize)
    {
        SerializeType = typeToSerialize;
    }
}
