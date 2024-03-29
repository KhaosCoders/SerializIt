﻿using SerializIt;
using Samples.Model;

namespace Samples
{
    [Serializer(ESerializers.Json, "Samples.JsonSerializers")]
    [JsonOptions(prettyPrint: true, indentChars: "  ", skipNullValues: false)]
    [SerializeType(typeof(RootElement))]
    [SerializeType(typeof(Container))]
    [SerializeType(typeof(Info))]
    internal partial class JsonContext
    {
    }
}
