using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using System.Linq;

public partial class MapGenerator : GridMap
{
	Node3D _player;
	int[] _meshLibrary;
	List<Move> _existingMoves;
	
	readonly Random _random = new();
	
	public override void _Ready()
	{
		_player = GetParentNode3D().GetNode<Node3D>("Player");
		
		_existingMoves = new List<Move>();
		var populatedCells = GetUsedCells();
		foreach (var cell in populatedCells)
		{
			var cellItem = GetCellItem(cell);
			var cellOrientation = GetCellItemOrientation(cell);
			//todo - double check if i can include InvalidCellItem cells as existing moves
			//if (cellItem == InvalidCellItem) continue;
			Debug.WriteLine("type:" + cellItem + " orientation:" + cellOrientation);
			_existingMoves.Add(new Move(MeshLibraryItemToMoveType(cellItem), Helpers.RawToOrientation(cellOrientation), cell));
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
		
		foreach (var cardinal in Enum.GetValues<Enums.Cardinal>())
		{
			var existingMove = _existingMoves.FirstOrDefault(m => m.Position == targetCell + Helpers.CardinalToRelativePosition(cardinal));
			
			if (existingMove == null) existingMove = new Move(Enums.MoveType.Empty, Enums.Orientation.Up, targetCell + Helpers.CardinalToRelativePosition(cardinal));
			
			legalMoves.AddRange(existingMove.LegalMoves.Where(m => m.RelativePosition == (Enums.Cardinal)(((int)cardinal + 4) % 8)));
		}
		
		legalMoves = FilterLegalMovesFromEveryDirection(legalMoves);
		
		SetCellItem(targetCell, legalMoves[_random.Next(0, legalMoves.Count)]);
	}

	List<LegalMove> FilterLegalMovesFromEveryDirection(List<LegalMove> legalMoves)
	{
		var set = new Dictionary<string, HashSet<Enums.Cardinal>>();
        
		Console.WriteLine("legal move count:" + legalMoves.Count);
		foreach (var move in legalMoves)
		{
			if (!set.ContainsKey(move.GetKey())) set[move.GetKey()] = new HashSet<Enums.Cardinal>();
            
			set[move.GetKey()].Add((Enums.Cardinal)(((int)move.RelativePosition + 4) % 8));
		}
        
		// Filter items which exist in all directions
		return legalMoves.Where(item => set[item.GetKey()].Count == Enum.GetValues<Enums.Cardinal>().Length).ToList();
	}
	
	int OrientationToRaw(Enums.Orientation orientation)
	{
		return orientation switch
		{
			Enums.Orientation.Up => 0,
			Enums.Orientation.Right => 22,
			Enums.Orientation.Down => 10,
			Enums.Orientation.Left => 16,
			_ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
		};
	}
		
	int MoveTypeToMeshLibraryItem(Enums.MoveType moveType)
	{
		switch (moveType)
		{
			case Enums.MoveType.Empty:
				return (int)InvalidCellItem;
			case Enums.MoveType.Floor:
				return MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "floor");
			case Enums.MoveType.Wall:
				return MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall");
			case Enums.MoveType.InsideCorner:
				return MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall-inside");
			case Enums.MoveType.OutsideCorner:
				return MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall-outside");
			default:
				throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null);
		}
	}

	Enums.MoveType MeshLibraryItemToMoveType(int meshLibraryItem)
	{
		var itemName = MeshLibrary.GetItemName(meshLibraryItem);
		switch (itemName)
		{
			case "":
				return Enums.MoveType.Empty;
			case "floor":
				return Enums.MoveType.Floor;
			case "wall":
				return Enums.MoveType.Wall;
			case "wall-inside":
				return Enums.MoveType.InsideCorner;
			case "wall-outside":
				return Enums.MoveType.OutsideCorner;
			default:
				throw new ArgumentOutOfRangeException(nameof(meshLibraryItem), meshLibraryItem, null);
		}
	}
	
	void SetCellItem(Vector3I position, LegalMove move)
	{
		SetCellItem(position, MoveTypeToMeshLibraryItem(move.Name), OrientationToRaw(move.Orientation));
	}
}
