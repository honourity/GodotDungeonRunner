using System;
using Godot;

public static class Helpers
{
    public static Enums.Orientation RawToOrientation(int raw)
    {
        return raw switch
        {
            0 =>  Enums.Orientation.Up,
            22 => Enums.Orientation.Right,
            10 => Enums.Orientation.Down,
            16 => Enums.Orientation.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(raw), raw, null)
        };
    }
    
    public static Vector3I CardinalToRelativePosition(Enums.Cardinal cardinal)
    {
        switch (cardinal)
        {
            case Enums.Cardinal.North:
                return new Vector3I(1, 0, 0);
            case Enums.Cardinal.NorthEast:
                return new Vector3I(1, 0, 1);
            case Enums.Cardinal.East:
                return new Vector3I(0, 0, 1);
            case Enums.Cardinal.SouthEast:
                return new Vector3I(-1, 0, 1);
            case Enums.Cardinal.South:
                return new Vector3I(-1, 0, 0);
            case Enums.Cardinal.SouthWest:
                return new Vector3I(-1, 0, -1);
            case Enums.Cardinal.West:
                return new Vector3I(0, 0, -1);
            case Enums.Cardinal.NorthWest:
                return new Vector3I(1, 0, -1);
            default:
                throw new ArgumentOutOfRangeException(nameof(cardinal), cardinal, null);
        }
    }
}