namespace Data
{
    public class Motor
    {

        public Motor() { }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public int MaxPosition { get; set; }
        public bool Stopped { get; set; }

    }
}
