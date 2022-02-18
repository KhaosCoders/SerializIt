namespace SerializIt.Benchmarks.Model
{
    public class Pokedex
    {
        public Pokemon[] Pokemon { get; set; }
    }

    public class Pokemon
    {
        public int Id { get; set; }
        public string Num { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public string[] Type { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string Candy { get; set; }
        public int Candy_count { get; set; }
        public string Egg { get; set; }
        public float Spawn_chance { get; set; }
        public float Avg_spawns { get; set; }
        public string Spawn_time { get; set; }
        public float[] Multipliers { get; set; }
        public string[] Weaknesses { get; set; }
        public Next_Evolution[] Next_evolution { get; set; }
        public Prev_Evolution[] Prev_evolution { get; set; }
    }

    public class Next_Evolution
    {
        public string Num { get; set; }
        public string Name { get; set; }
    }

    public class Prev_Evolution
    {
        public string Num { get; set; }
        public string Name { get; set; }
    }
}
