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
			_existingMoves.Add(new Move(MeshLibraryItemToMoveType(cellItem), Helpers.RawToOrientation(cellOrientation), cell));
		}
	}
	
	public override void _Process(double delta)
	{
		var playerLocation = LocalToMap(ToLocal(new Vector3(_player.GlobalPosition.X, 0, _player.GlobalPosition.Z)));
		
		for (var x = -1; x < 1; x++)
		{
			for (var z = -1; z < 1; z++)
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

		if (legalMoves.Count == 0)
		{
			Debug.WriteLine("No legal moves found!");
			return;
		}
		
		var chosenMove = legalMoves[_random.Next(0, legalMoves.Count)];
		_existingMoves.Add(new Move(chosenMove.Name, chosenMove.Orientation, targetCell));
		SetCellItem(targetCell, chosenMove);
	}

	List<LegalMove> FilterLegalMovesFromEveryDirection(List<LegalMove> legalMoves)
	{
		var set = new Dictionary<string, HashSet<Enums.Cardinal>>();
        
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
		return moveType switch
		{
			Enums.MoveType.Empty => (int)InvalidCellItem,
			Enums.MoveType.Floor => MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "floor"),
			Enums.MoveType.Wall => MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall"),
			Enums.MoveType.InsideCorner => MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall-inside"),
			Enums.MoveType.OutsideCorner => MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall-outside"),
			_ => throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null)
		};
	}

	Enums.MoveType MeshLibraryItemToMoveType(int meshLibraryItem)
	{
		var itemName = MeshLibrary.GetItemName(meshLibraryItem);
		return itemName switch
		{
			"" => Enums.MoveType.Empty,
			"floor" => Enums.MoveType.Floor,
			"wall" => Enums.MoveType.Wall,
			"wall-inside" => Enums.MoveType.InsideCorner,
			"wall-outside" => Enums.MoveType.OutsideCorner,
			_ => throw new ArgumentOutOfRangeException(nameof(meshLibraryItem), meshLibraryItem, null)
		};
	}
	
	void SetCellItem(Vector3I position, LegalMove move)
	{
		SetCellItem(position, MoveTypeToMeshLibraryItem(move.Name), OrientationToRaw(move.Orientation));
	}
}
