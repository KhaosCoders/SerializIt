using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializIt.Benchmarks.Model
{
    [Serializer(ESerializers.Json)]
    [SerializeType(typeof(Pokedex))]
    [SerializeType(typeof(Pokemon))]
    [SerializeType(typeof(Next_Evolution))]
    [SerializeType(typeof(Prev_Evolution))]
    internal partial class PokedexJsonContext
    {
    }
}
