using System;
using System.Collections.Generic;
using Godot;
using System.Linq;

public partial class MapGenerator2 : GridMap
{
	Node3D _player;
	Vector3I PlayerPosition => LocalToMap(ToLocal(new Vector3(_player.GlobalPosition.X, 0, _player.GlobalPosition.Z)));
	readonly Random _random = new();
	readonly HashSet<Vector3I> _populatedCells = new(); // To keep track of processed cells
	
	public override void _Ready()
	{
		_player = GetParentNode3D().GetNode<Node3D>("Player");
	}
	
	public override void _Process(double delta)
	{
		PopulateCloseCells(10);
	}
	
	void PopulateCloseCells(int distance)
	{
		var queue = new Queue<Vector3I>();
		var visitedCells = new HashSet<Vector3I>();
		queue.Enqueue(PlayerPosition);

		while (queue.Count > 0)
		{
			var cell = queue.Dequeue();
			if (GetCellDistance(cell) > distance) continue;
			if (visitedCells.Contains(cell)) continue;
			visitedCells.Add(cell);

			if (!_populatedCells.Contains(cell))
			{
				_populatedCells.Add(cell);
				PopulateTargetCell(cell);
			}

			// Enqueue neighboring cells with incremented distance.
			queue.Enqueue(cell + new Vector3I(1, 0, 0));
			queue.Enqueue(cell + new Vector3I(-1, 0, 0));
			queue.Enqueue(cell + new Vector3I(0, 0, 1));
			queue.Enqueue(cell + new Vector3I(0, 0, -1));
			// queue.Enqueue(new Cell(cell.Position + new Vector3I(1, 0, 1), cell.Distance + 1));
			// queue.Enqueue(new Cell(cell.Position + new Vector3I(1, 0, -1), cell.Distance + 1));
			// queue.Enqueue(new Cell(cell.Position + new Vector3I(-1, 0, 1), cell.Distance + 1));
			// queue.Enqueue(new Cell(cell.Position + new Vector3I(-1, 0, -1), cell.Distance + 1));
		}
	}

	int GetCellDistance(Vector3I cell)
	{
		return (cell - PlayerPosition).LengthSquared();
	}
	
	void PopulateTargetCell(Vector3I cell)
	{
		if (_random.Next(0, 2) == 0)
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(Enums.MoveType.Floor), OrientationToRaw(Enums.Orientation.Up));
		}
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
}
