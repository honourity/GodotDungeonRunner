using System;
using System.Collections.Generic;
using Godot;

public class Move
{
	public List<LegalMove> LegalMoves;
	public Vector3I Position;

	public Move(Enums.MoveType name, Enums.Orientation orientation, Vector3I position)
	{
		LegalMoves = GetLegalMoves(name, orientation, position);
		Position = position;
	}

	List<LegalMove> GetLegalMoves(Enums.MoveType moveType, Enums.Orientation baseOrientation, Vector3I sourceAbsolutePosition)
	{
		return moveType switch
		{
			Enums.MoveType.Empty => GetLegalMovesEmpty(baseOrientation, sourceAbsolutePosition),
			Enums.MoveType.Floor => GetLegalMovesFloor(baseOrientation, sourceAbsolutePosition),
			Enums.MoveType.Wall => GetLegalMovesWall(baseOrientation, sourceAbsolutePosition),
			Enums.MoveType.InsideCorner => GetLegalMovesInsideCorner(baseOrientation, sourceAbsolutePosition),
			Enums.MoveType.OutsideCorner => GetLegalMovesOutsideCorner(baseOrientation, sourceAbsolutePosition),
			_ => throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null)
		};
	}

	static List<LegalMove> GetLegalMovesOutsideCorner(Enums.Orientation baseOrientation, Vector3I sourceAbsolutePosition)
	{
		return new List<LegalMove>
		{
			new (Enums.Cardinal.North, Enums.MoveType.Wall, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.North, Enums.MoveType.InsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),

			new (Enums.Cardinal.NorthEast, Enums.MoveType.Empty, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),

			new (Enums.Cardinal.East, Enums.MoveType.Wall, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.East, Enums.MoveType.InsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),

			new (Enums.Cardinal.SouthEast, Enums.MoveType.Wall, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.InsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),

			new (Enums.Cardinal.South, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.OutsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.OutsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),

			new (Enums.Cardinal.SouthWest, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.Wall, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.InsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),

			new (Enums.Cardinal.West, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.West, Enums.MoveType.Wall, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.West, Enums.MoveType.OutsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.West, Enums.MoveType.OutsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
		};
	}

	static List<LegalMove> GetLegalMovesInsideCorner(Enums.Orientation baseOrientation, Vector3I sourceAbsolutePosition)
	{
		return new List<LegalMove>
		{
			new (Enums.Cardinal.North, Enums.MoveType.Empty, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthEast, Enums.MoveType.Empty, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.East, Enums.MoveType.Empty, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.SouthEast, Enums.MoveType.InsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.Wall, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.South, Enums.MoveType.OutsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.InsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.Wall, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),

			new (Enums.Cardinal.SouthWest, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.Wall, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.InsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),

			new (Enums.Cardinal.West, Enums.MoveType.Wall, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.West, Enums.MoveType.InsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.West, Enums.MoveType.OutsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
		
			new (Enums.Cardinal.NorthWest, Enums.MoveType.InsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthWest, Enums.MoveType.Wall, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
		};
	}

	static List<LegalMove> GetLegalMovesWall(Enums.Orientation baseOrientation, Vector3I sourceAbsolutePosition)
	{
		return new List<LegalMove>
		{
			new (Enums.Cardinal.North, Enums.MoveType.Empty, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.North, Enums.MoveType.Empty, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.North, Enums.MoveType.Empty, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.North, Enums.MoveType.Empty, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.NorthEast, Enums.MoveType.Wall, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthEast, Enums.MoveType.InsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.East, Enums.MoveType.Wall, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.East, Enums.MoveType.InsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.East, Enums.MoveType.OutsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.SouthEast, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.SouthEast, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.SouthEast, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.SouthEast, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.Wall, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.InsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.OutsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.South, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.South, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.South, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.South, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.OutsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.OutsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),

			new(Enums.Cardinal.SouthWest, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new(Enums.Cardinal.SouthWest, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new(Enums.Cardinal.SouthWest, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new(Enums.Cardinal.SouthWest, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new(Enums.Cardinal.SouthWest, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new(Enums.Cardinal.SouthWest, Enums.MoveType.Wall, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new(Enums.Cardinal.SouthWest, Enums.MoveType.InsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new(Enums.Cardinal.SouthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			
			new(Enums.Cardinal.West, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new(Enums.Cardinal.West, Enums.MoveType.InsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new(Enums.Cardinal.West, Enums.MoveType.OutsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			
			new(Enums.Cardinal.NorthWest, Enums.MoveType.Wall, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new(Enums.Cardinal.NorthWest, Enums.MoveType.InsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
		};
	}

	static List<LegalMove> GetLegalMovesFloor(Enums.Orientation baseOrientation, Vector3I sourceAbsolutePosition)
	{
		return new List<LegalMove>
		{
			new (Enums.Cardinal.North, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.North, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.North, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.North, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.North, Enums.MoveType.Wall, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.North, Enums.MoveType.OutsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.North, Enums.MoveType.OutsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.NorthEast, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.NorthEast, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.NorthEast, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.NorthEast, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthEast, Enums.MoveType.Wall, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthEast, Enums.MoveType.Wall, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthEast, Enums.MoveType.InsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthEast, Enums.MoveType.OutsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthEast, Enums.MoveType.OutsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthEast, Enums.MoveType.OutsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.East, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.East, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.East, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.East, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.East, Enums.MoveType.Wall, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.East, Enums.MoveType.OutsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.East, Enums.MoveType.OutsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.SouthEast, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.SouthEast, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.SouthEast, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.SouthEast, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.Wall, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.InsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.OutsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.OutsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthEast, Enums.MoveType.OutsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),

			new (Enums.Cardinal.South, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.South, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.South, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.South, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.OutsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.South, Enums.MoveType.OutsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.SouthWest, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.SouthWest, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.SouthWest, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.SouthWest, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.Wall, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.Wall, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.InsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.SouthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.West, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.West, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.West, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.West, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.West, Enums.MoveType.Wall, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.West, Enums.MoveType.OutsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.West, Enums.MoveType.OutsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			
			new (Enums.Cardinal.NorthWest, Enums.MoveType.Floor, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.NorthWest, Enums.MoveType.Floor, Enums.Orientation.Right, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.NorthWest, Enums.MoveType.Floor, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			// new (Enums.Cardinal.NorthWest, Enums.MoveType.Floor, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthWest, Enums.MoveType.Wall, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthWest, Enums.MoveType.Wall, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthWest, Enums.MoveType.InsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Down, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Left, baseOrientation, sourceAbsolutePosition),
			new (Enums.Cardinal.NorthWest, Enums.MoveType.OutsideCorner, Enums.Orientation.Up, baseOrientation, sourceAbsolutePosition),
		};
	}

	static List<LegalMove> GetLegalMovesEmpty(Enums.Orientation baseOrientation, Vector3I sourceAbsolutePosition)
	{
		var legalMoves = new List<LegalMove>();

		var cardinals = Enum.GetValues<Enums.Cardinal>();
		var moveTypes = Enum.GetValues<Enums.MoveType>();
		var orientations = Enum.GetValues<Enums.Orientation>();
		foreach (var a in cardinals)
		{
			foreach (var b in moveTypes)
			{
				foreach (var c in orientations)
				{
					legalMoves.Add(new LegalMove(a, b, c, baseOrientation, sourceAbsolutePosition));
				}
			}
		}

		return legalMoves;
	}
}