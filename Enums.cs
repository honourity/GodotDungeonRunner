public class Enums
{
    public enum MoveType
    {
        Empty,
        Floor,
        Wall,
        InsideCorner,
        OutsideCorner,
        OutsideDoubleCorner
    }

    public enum Orientation
    {
        Up,
        Right,
        Down,
        Left
    }

    public enum Cardinal
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
}