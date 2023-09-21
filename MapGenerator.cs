using System;
using System.Collections.Generic;
using Godot;
using System.Linq;

public partial class MapGenerator : GridMap
{
	Node3D _player;
	int[] _meshLibrary;
	List<ExistingMove> _existingMoves;
	
	//helper
	static List<LegalMove> _legalMovesForEmptyCell;
	
	readonly Random _random = new();
	
	public override void _Ready()
	{
		_player = GetParentNode3D().GetNode<Node3D>("Player");
		PopulateLegalMovesForEmptyCell();

		_existingMoves = new List<ExistingMove>();
		var populatedCells = GetUsedCells();
		foreach (var cell in populatedCells)
		{
			var cellItem = GetCellItem(cell);
			var cellOrientation = GetCellItemOrientation(cell);
			if (cellItem == InvalidCellItem) continue;
			_existingMoves.Add(new ExistingMove(MeshLibraryItemToMoveType(cellItem), RawToOrientation(cellOrientation), cell));
		}
	}
	
	public override void _Process(double delta)
	{
		var playerLocation = LocalToMap(ToLocal(new Vector3(_player.GlobalPosition.X, 0, _player.GlobalPosition.Z)));
		
		for (var x = -10; x < 11; x++)
		{
			for (var z = -10; z < 11; z++)
			{
				var targetCell = playerLocation + new Vector3I(x, 0, z);
				if (GetCellItem(targetCell) == InvalidCellItem)
				{
					PopulateCell(targetCell);
				}
			}
		}
	}
	
	void PopulateCell(Vector3I targetCell)
	{
		var legalMoves = new List<LegalMove>();
		
		foreach (var cardinal in Enum.GetValues<Cardinal>())
		{
			var existingMove = _existingMoves.FirstOrDefault(m => m.Position == targetCell + CardinalToRelativePosition(cardinal));
			legalMoves.AddRange(existingMove == null ? _legalMovesForEmptyCell.Where(m => m.RelativePosition == (Cardinal)(((int)cardinal + 4) % 8)) : existingMove.Move.LegalMoves.Where(m => m.RelativePosition == (Cardinal)(((int)cardinal + 4) % 8)));
		}

		SetCellItem(targetCell, legalMoves[_random.Next(0, legalMoves.Count)]);
	}

	class ExistingMove
	{
		public readonly Move Move;
		public readonly Vector3I Position;

		public ExistingMove(MoveType name, Orientation orientation, Vector3I position)
		{
			Move = new Move(name, orientation);
			Position = position;
		}
	}
	
	class Move
	{
		public MoveType Name;
		public Orientation Orientation;
		public List<LegalMove> LegalMoves;
		
		public Move(MoveType name, Orientation orientation)
		{
			Name = name;
			Orientation = orientation;
			LegalMoves = GetLegalMoves(name, orientation);
		}

		List<LegalMove> GetLegalMoves(MoveType moveType, Orientation baseOrientation)
		{
			return moveType switch
			{
				MoveType.Empty => GetLegalMovesEmpty(baseOrientation),
				MoveType.Floor => GetLegalMovesFloor(baseOrientation),
				MoveType.Wall => GetLegalMovesWall(baseOrientation),
				MoveType.InsideCorner => GetLegalMovesInsideCorner(baseOrientation),
				MoveType.OutsideCorner => GetLegalMovesOutsideCorner(baseOrientation),
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
		List<LegalMove> GetLegalMovesOutsideCorner(Orientation baseOrientation)
		{
			return GetLegalMovesEmpty(baseOrientation);
		}

		//todo - this is test data
		List<LegalMove> GetLegalMovesInsideCorner(Orientation baseOrientation)
		{
			return GetLegalMovesEmpty(baseOrientation);
		}

		//todo - this is test data
		List<LegalMove> GetLegalMovesWall(Orientation baseOrientation)
		{
			return GetLegalMovesEmpty(baseOrientation);
		}

		//todo - this is test data
		List<LegalMove> GetLegalMovesFloor(Orientation baseOrientation)
		{
			return GetLegalMovesEmpty(baseOrientation);
		}

		List<LegalMove> GetLegalMovesEmpty(Orientation baseOrientation)
		{
			return _legalMovesForEmptyCell;
		}
	}

	void PopulateLegalMovesForEmptyCell()
	{
		var legalMoves = new List<LegalMove>();
			
		var cardinals = Enum.GetValues<Cardinal>();
		var moveTypes = Enum.GetValues<MoveType>();
		var orientations = Enum.GetValues<Orientation>();
		foreach (var a in cardinals)
		{
			foreach (var b in moveTypes)
			{
				foreach (var c in orientations)
				{
					legalMoves.Add(new LegalMove(a, b, c, 0));
				}
			}
		}
			
		_legalMovesForEmptyCell = legalMoves;
	}
	
	class LegalMove
	{
		public MoveType Name;
		public Orientation Orientation;
		public Cardinal RelativePosition;
		
		public LegalMove(Cardinal relativePosition, MoveType name, Orientation orientation, Orientation baseOrientation)
		{
			RelativePosition = relativePosition;
			Name = name;
			Orientation = (Orientation)(((int)baseOrientation + (int)orientation) % 4);
		}
	}
	
	int OrientationToRaw(Orientation orientation)
	{
		return orientation switch
		{
			Orientation.Up => 0,
			Orientation.Right => 22,
			Orientation.Down => 10,
			Orientation.Left => 16,
			_ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
		};
	}
		
	Orientation RawToOrientation(int raw)
	{
		return raw switch
		{
			0 => Orientation.Up,
			22 => Orientation.Right,
			10 => Orientation.Down,
			16 => Orientation.Left,
			_ => throw new ArgumentOutOfRangeException(nameof(raw), raw, null)
		};
	}

	Vector3I CardinalToRelativePosition(Cardinal cardinal)
	{
		switch (cardinal)
		{
			case Cardinal.North:
				return new Vector3I(1, 0, 0);
			case Cardinal.NorthEast:
				return new Vector3I(1, 0, 1);
			case Cardinal.East:
				return new Vector3I(0, 0, 1);
			case Cardinal.SouthEast:
				return new Vector3I(-1, 0, 1);
			case Cardinal.South:
				return new Vector3I(-1, 0, 0);
			case Cardinal.SouthWest:
				return new Vector3I(-1, 0, -1);
			case Cardinal.West:
				return new Vector3I(0, 0, -1);
			case Cardinal.NorthWest:
				return new Vector3I(1, 0, -1);
			default:
				throw new ArgumentOutOfRangeException(nameof(cardinal), cardinal, null);
		}
	}
	
	int MoveTypeToMeshLibraryItem(MoveType moveType)
	{
		switch (moveType)
		{
			case MoveType.Empty:
				return (int)InvalidCellItem;
			case MoveType.Floor:
				return MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "floor");
			case MoveType.Wall:
				return MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall");
			case MoveType.InsideCorner:
				return MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall-inside");
			case MoveType.OutsideCorner:
				return MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall-outside");
			default:
				throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null);
		}
	}

	MoveType MeshLibraryItemToMoveType(int meshLibraryItem)
	{
		var itemName = MeshLibrary.GetItemName(meshLibraryItem);
		switch (itemName)
		{
			case "":
				return MoveType.Empty;
			case "floor":
				return MoveType.Floor;
			case "wall":
				return MoveType.Wall;
			case "wall-inside":
				return MoveType.InsideCorner;
			case "wall-outside":
				return MoveType.OutsideCorner;
			default:
				throw new ArgumentOutOfRangeException(nameof(meshLibraryItem), meshLibraryItem, null);
		}
	}
	
	enum MoveType
	{
		Empty,
		Floor,
		Wall,
		InsideCorner,
		OutsideCorner,
	}
	
	enum Orientation
	{
		Up,
		Right,
		Down,
		Left
	}
	
	enum Cardinal
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
	
	void SetCellItem(Vector3I position, LegalMove move)
	{
		SetCellItem(position, MoveTypeToMeshLibraryItem(move.Name), OrientationToRaw(move.Orientation));
	}
}
