namespace Cat.Core
{
    public enum HintType
    {
        PlaceBulb,
        AvoidBulb
    }

    public class HintMove
    {
        public int X;
        public int Y;
        public HintType Type;
        public string Reason;
    }
}
