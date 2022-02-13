using SerializIt;

namespace Samples.Model;

public class Container : Base
{
    [Skip]
    public int Id { get; set; }

    [Skip(true)]
    public int ChildCount => Children?.Count ?? 0;

    public Info Info { get; set; }
}
