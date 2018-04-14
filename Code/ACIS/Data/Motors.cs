namespace Data
{
    public class Motor
    {

        public Motor() { }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public int MaxPosition { get; set; }

    }

    public enum Motors
    {
        X_AXIS_TOP,
        X_AXIS_BOTTOM,
        Z_AXIS_TOP,
        Z_AXIS_BOTTOM,
        Y_AXIS
    }
}
