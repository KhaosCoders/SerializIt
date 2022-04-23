using System;

namespace SerializIt;

[AttributeUsage(AttributeTargets.Property)]
public class SkipAttribute : Attribute
{
    public bool SkipIfDefault { get; set; }

    public SkipAttribute(bool onlyIfDefault = false)
    {
        SkipIfDefault = onlyIfDefault;
    }
}
