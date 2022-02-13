using SerializIt;


namespace Samples.Model;

public class RootElement : Base
{
    [Order(1)]
    public string Name { get; set; }

    [Order(0)]
    public string Type { get; set; }
}
