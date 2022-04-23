using System;

namespace SerializIt;

[AttributeUsage(AttributeTargets.Property)]
public class OrderAttribute : Attribute
{
    public int Order { get; set; }

    public OrderAttribute(int order)
    {
        Order = order;
    }
}
