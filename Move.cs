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

		// legalMoves.Add(new LegalMove(Cardinal.North, MoveType.Empty, Orientation.Up));
		// legalMoves.Add(new LegalMove(Cardinal.North, MoveType.Empty, Orientation.Right));
		// legalMoves.Add(new LegalMove(Cardinal.North, MoveType.Empty, Orientation.Down));
		// legalMoves.Add(new LegalMove(Cardinal.North, MoveType.Empty, Orientation.Left));
		// legalMoves.Add(new LegalMove(Cardinal.NorthEast, MoveType.Empty, Orientation.Up));
		// legalMoves.Add(new LegalMove(Cardinal.NorthEast, MoveType.Empty, Orientation.Right));
		// legalMoves.Add(new LegalMove(Cardinal.NorthEast, MoveType.Empty, Orientation.Down));
		// legalMoves.Add(new LegalMove(Cardinal.NorthEast, MoveType.Empty, Orientation.Left));
	}

	//todo - this is test data
	static List<LegalMove> GetLegalMovesOutsideCorner(Enums.Orientation baseOrientation, Vector3I sourceAbsolutePosition)
	{
		return GetLegalMovesEmpty(baseOrientation, sourceAbsolutePosition);
	}

	//todo - this is test data
	static List<LegalMove> GetLegalMovesInsideCorner(Enums.Orientation baseOrientation, Vector3I sourceAbsolutePosition)
	{
		return GetLegalMovesEmpty(baseOrientation, sourceAbsolutePosition);
	}

	//todo - this is test data
	static List<LegalMove> GetLegalMovesWall(Enums.Orientation baseOrientation, Vector3I sourceAbsolutePosition)
	{
		return GetLegalMovesEmpty(baseOrientation, sourceAbsolutePosition);
	}

	//todo - this is test data
	static List<LegalMove> GetLegalMovesFloor(Enums.Orientation baseOrientation, Vector3I sourceAbsolutePosition)
	{
		return GetLegalMovesEmpty(baseOrientation, sourceAbsolutePosition);
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