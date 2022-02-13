using SerializIt;

namespace Samples.Model;

public class Base
{
    [Skip(true)]
    public IList<Container> Children { get; set; }
}
