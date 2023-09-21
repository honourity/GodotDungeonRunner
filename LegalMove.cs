using Godot;

public class LegalMove
{
    public Enums.MoveType Name;
    public Enums.Orientation Orientation;
    public Enums.Cardinal RelativePosition;
    public Vector3I SourceAbsolutePosition;

    public LegalMove(Enums.Cardinal relativePosition, Enums.MoveType name, Enums.Orientation orientation, Enums.Orientation baseOrientation,
        Vector3I sourceAbsolutePosition)
    {
        RelativePosition = relativePosition;
        Name = name;
        SourceAbsolutePosition = sourceAbsolutePosition;
        Orientation = (Enums.Orientation)(((int)baseOrientation + (int)orientation) % 4);
    }

    //movetype, orientation, and position is the key
    public string GetKey()
    {
        return Name.ToString()
               + Orientation.ToString()
               + (SourceAbsolutePosition + Helpers.CardinalToRelativePosition(RelativePosition));
    }
}