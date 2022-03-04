using Tinyhand;

namespace SerializIt.Benchmarks.Model
{
    [TinyhandObject]
    public partial class Pokedex
    {
        [Key(0)]
        public Pokemon[] Pokemon { get; set; }
    }

    [TinyhandObject]
    public partial class Pokemon
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Num { get; set; }
        [Key(2)]
        public string Name { get; set; }
        [Key(3)]
        public string Img { get; set; }
        [Key(4)]
        public string[] Type { get; set; }
        [Key(5)]
        public string Height { get; set; }
        [Key(6)]
        public string Weight { get; set; }
        [Key(7)]
        public string Candy { get; set; }
        [Key(8)]
        public int Candy_count { get; set; }
        [Key(9)]
        public string Egg { get; set; }
        [Key(10)]
        public float Spawn_chance { get; set; }
        [Key(11)]
        public float Avg_spawns { get; set; }
        [Key(12)]
        public string Spawn_time { get; set; }
        [Key(13)]
        public float[] Multipliers { get; set; }
        [Key(14)]
        public string[] Weaknesses { get; set; }
        [Key(15)]
        public Next_Evolution[] Next_evolution { get; set; }
        [Key(16)]
        public Prev_Evolution[] Prev_evolution { get; set; }
    }

    [TinyhandObject]
    public partial class Next_Evolution
    {
        [Key(0)]
        public string Num { get; set; }
        [Key(1)]
        public string Name { get; set; }
    }

    [TinyhandObject]
    public partial class Prev_Evolution
    {
        [Key(0)]
        public string Num { get; set; }
        [Key(1)]
        public string Name { get; set; }
    }
}
