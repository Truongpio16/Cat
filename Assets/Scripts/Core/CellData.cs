namespace Cat.Core
{
    public class CellData
    {
        public CellType Type;
        public int WallNumber;
        public bool HasBulb;
        public bool IsLit;
        public bool HasConflict;

        public CellData(CellType type = CellType.Empty, int wallNumber = -1)
        {
            Type = type;
            WallNumber = wallNumber;
            HasBulb = false;
        }
    }
}
