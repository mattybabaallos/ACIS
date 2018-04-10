namespace Data
{
    public class Motor
    {
        int Id;
        string Name;
        int position;
        int MaxPosition;
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
