using SerializIt;

namespace Samples.Model
{
    public class Info
    {
        public string Info1 { get; set; }
        public int Info2 { get; set; }

        [Skip]
        public bool Info3 { get; set; }

        public string[] Infos { get; set; }
    }
}
