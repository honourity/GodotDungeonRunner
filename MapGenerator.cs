using System;
using System.Collections.Generic;
using Godot;
using System.Linq;

public partial class MapGenerator : GridMap
{
	Player _player;
	Vector3I _lastPlayerLocation;
	PackedScene _wraithScene;

	Vector3I PlayerPosition => LocalToMap(ToLocal(new Vector3(_player.GlobalPosition.X, 0, _player.GlobalPosition.Z)));
	readonly Random _random = new();
	readonly HashSet<Vector3I> _floorProcessedCells = new();
	readonly HashSet<Vector3I> _floorProcessedSecondPassCells = new();
	readonly HashSet<Vector3I> _wallProcessedCells = new();

	public AStarGrid2D AstarGrid;
	
	public override void _Ready()
	{
		_player = GetTree().CurrentScene.GetNode<Player>("Player");
		_wraithScene = (PackedScene)ResourceLoader.Load("res://wraith.tscn");
		//AstarGrid = new AStarGrid2D();
	}
	
	public override void _Process(double delta)
	{
		if (_lastPlayerLocation != _player.GlobalPosition)
		{
			ApplyFloorToCells(144);
			ApplyFloorToCellsSecondPass(121);
			ApplyWallsToCells(81);
			//UpdateAstarGrid();
		}
		
		_lastPlayerLocation = PlayerPosition;
	}

	void SpawnWraith(Vector3I cell)
	{
		var wraiths = GetTree().GetNodesInGroup("wraiths").Select(w => w as Wraith);
		var position = ToGlobal(MapToLocal(cell));

		if (position.DistanceTo(_player.GlobalPosition) > 8f &&
		    wraiths.Select(w => w?.GlobalPosition.DistanceTo(_player.GlobalPosition) < 20f).Count() < 5 &&
			_random.Next(0, 5) == 0)
		{
			var wraith = _wraithScene.Instantiate<Node3D>();
			GetTree().CurrentScene.AddChild(wraith);
			wraith.GlobalPosition = new Vector3(ToGlobal(MapToLocal(cell)).X + 0.5f, 1f, ToGlobal(MapToLocal(cell)).Z + 0.5f);	
		}
	}

	void UpdateAstarGrid()
	{
		var usedCells = GetUsedCells();
		var minGridmapX = usedCells.MinBy(v => v.X).X;
		var maxGridmapX = usedCells.MaxBy(v => v.X).X;
		var height = maxGridmapX - minGridmapX;
		var minGridmapZ = usedCells.MinBy(v => v.Z).Z;
		var maxGridmapZ = usedCells.MaxBy(v => v.Z).Z;
		var width = maxGridmapZ - minGridmapZ;

		AstarGrid.Region = new Rect2I(minGridmapX, minGridmapZ, width, height);
		AstarGrid.Update();

		foreach (var cell in usedCells)
		{
			AstarGrid.SetPointSolid(new Vector2I(cell.X, cell.Z));
		}
	}

	void ApplyWallsToCells(int distance)
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

			if (!_wallProcessedCells.Contains(cell))
			{
				_wallProcessedCells.Add(cell);
				ApplyWallsToTargetCell(cell);
				if (GetCellItem(cell) == MoveTypeToMeshLibraryItem(MoveType.Floor))
				{
					SpawnWraith(cell);
				}
			}

			// Enqueue neighboring cells with incremented distance.
			queue.Enqueue(cell + new Vector3I(1, 0, 0));
			queue.Enqueue(cell + new Vector3I(-1, 0, 0));
			queue.Enqueue(cell + new Vector3I(0, 0, 1));
			queue.Enqueue(cell + new Vector3I(0, 0, -1));
		}
	}

	void ApplyFloorToCells(int distance)
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

			if (!_floorProcessedCells.Contains(cell))
			{
				_floorProcessedCells.Add(cell);
				ApplyFloorToTargetCell(cell);
			}

			// Enqueue neighboring cells with incremented distance.
			queue.Enqueue(cell + new Vector3I(1, 0, 0));
			queue.Enqueue(cell + new Vector3I(-1, 0, 0));
			queue.Enqueue(cell + new Vector3I(0, 0, 1));
			queue.Enqueue(cell + new Vector3I(0, 0, -1));
		}
	}
	
	void ApplyFloorToCellsSecondPass(int distance)
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

			if (!_floorProcessedSecondPassCells.Contains(cell))
			{
				_floorProcessedSecondPassCells.Add(cell);
				ApplyFloorToTargetCellSecondPass(cell);
			}

			// Enqueue neighboring cells with incremented distance.
			queue.Enqueue(cell + new Vector3I(1, 0, 0));
			queue.Enqueue(cell + new Vector3I(-1, 0, 0));
			queue.Enqueue(cell + new Vector3I(0, 0, 1));
			queue.Enqueue(cell + new Vector3I(0, 0, -1));
		}
	}

	int GetCellDistance(Vector3I cell)
	{
		return (cell - PlayerPosition).LengthSquared();
	}

	void ApplyWallsToTargetCell(Vector3I cell)
	{
		if (IsNorthWallConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.Wall), OrientationToRaw(Orientation.Right));
		}
		else if (IsSouthWallConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.Wall), OrientationToRaw(Orientation.Left));
		}
		else if (IsEastWallConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.Wall), OrientationToRaw(Orientation.Down));
		}
		else if (IsWestWallConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.Wall), OrientationToRaw(Orientation.Up));
		}
		else if (IsNorthWallEndCapConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.Wall), OrientationToRaw(Orientation.Right));
		}
		else if (IsEastWallEndCapConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.Wall), OrientationToRaw(Orientation.Down));
		}
		else if (IsSouthWallEndCapConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.Wall), OrientationToRaw(Orientation.Left));
		}
		else if (IsWestWallEndCapConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.Wall), OrientationToRaw(Orientation.Up));
		}
		else if (IsNorthEastInsideCornerConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.InsideCorner), OrientationToRaw(Orientation.Right));
		}
		else if (IsSouthEastInsideCornerConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.InsideCorner), OrientationToRaw(Orientation.Down));
		}
		else if (IsSouthWestInsideCornerConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.InsideCorner), OrientationToRaw(Orientation.Left));
		}
		else if (IsNorthWestInsideCornerConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.InsideCorner), OrientationToRaw(Orientation.Up));
		}
		else if (IsDiagonalDoubleCornerUpConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.OutsideDoubleCorner), OrientationToRaw(Orientation.Up));
		}
		else if (IsDiagonalDoubleCornerRightConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.OutsideDoubleCorner), OrientationToRaw(Orientation.Right));
		}
		else if (IsNorthEastOutsideCornerConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.OutsideCorner), OrientationToRaw(Orientation.Right));
		}
		else if (IsSouthEastOutsideCornerConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.OutsideCorner), OrientationToRaw(Orientation.Down));
		}
		else if (IsSouthWestOutsideCornerConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.OutsideCorner), OrientationToRaw(Orientation.Left));
		}
		else if (IsNorthWestOutsideCornerConfiguration(cell))
		{
			SetCellItem(cell, MoveTypeToMeshLibraryItem(MoveType.OutsideCorner), OrientationToRaw(Orientation.Up));
		}
	}

	bool IsDiagonalDoubleCornerUpConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(NorthWest(cell)) == InvalidCellItem
		       && GetCellItem(SouthEast(cell)) == InvalidCellItem;
	}
	bool IsDiagonalDoubleCornerRightConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(NorthEast(cell)) == InvalidCellItem
		       && GetCellItem(SouthWest(cell)) == InvalidCellItem;
	}

	bool IsNorthEastOutsideCornerConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(North(cell)) != InvalidCellItem
		       && GetCellItem(East(cell)) != InvalidCellItem
		       && GetCellItem(NorthEast(cell)) == InvalidCellItem;
	}
	bool IsSouthEastOutsideCornerConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(East(cell)) != InvalidCellItem
		       && GetCellItem(South(cell)) != InvalidCellItem
		       && GetCellItem(SouthEast(cell)) == InvalidCellItem;
	}
	bool IsSouthWestOutsideCornerConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(South(cell)) != InvalidCellItem
		       && GetCellItem(West(cell)) != InvalidCellItem
		       && GetCellItem(SouthWest(cell)) == InvalidCellItem;
	}
	bool IsNorthWestOutsideCornerConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(North(cell)) != InvalidCellItem
		       && GetCellItem(West(cell)) != InvalidCellItem
		       && GetCellItem(NorthWest(cell)) == InvalidCellItem;
	}

	bool IsSouthWestInsideCornerConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(West(cell)) == InvalidCellItem
		       && GetCellItem(South(cell)) == InvalidCellItem
		       && GetCellItem(North(cell)) != InvalidCellItem
		       && GetCellItem(NorthEast(cell)) != InvalidCellItem
		       && GetCellItem(East(cell)) != InvalidCellItem;
	}
	bool IsSouthEastInsideCornerConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(West(cell)) != InvalidCellItem
		       && GetCellItem(NorthWest(cell)) != InvalidCellItem
		       && GetCellItem(North(cell)) != InvalidCellItem
		       && GetCellItem(South(cell)) == InvalidCellItem
		       && GetCellItem(East(cell)) == InvalidCellItem;
	}
	bool IsNorthWestInsideCornerConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(West(cell)) == InvalidCellItem
		       && GetCellItem(North(cell)) == InvalidCellItem
		       && GetCellItem(South(cell)) != InvalidCellItem
		       && GetCellItem(SouthEast(cell)) != InvalidCellItem
		       && GetCellItem(East(cell)) != InvalidCellItem;
	}
	bool IsNorthEastInsideCornerConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(West(cell)) != InvalidCellItem
		       && GetCellItem(South(cell)) != InvalidCellItem
		       && GetCellItem(SouthWest(cell)) != InvalidCellItem
		       && GetCellItem(North(cell)) == InvalidCellItem
		       && GetCellItem(East(cell)) == InvalidCellItem;
	}

	bool IsEastWallConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(North(cell)) != InvalidCellItem
		       && GetCellItem(South(cell)) != InvalidCellItem
		       && GetCellItem(East(cell)) == InvalidCellItem
		       && GetCellItem(West(cell)) != InvalidCellItem
		       && GetCellItem(NorthWest(cell)) != InvalidCellItem
		       && GetCellItem(SouthWest(cell)) != InvalidCellItem;
	}
	bool IsWestWallConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(North(cell)) != InvalidCellItem
		       && GetCellItem(South(cell)) != InvalidCellItem
		       && GetCellItem(West(cell)) == InvalidCellItem
		       && GetCellItem(East(cell)) != InvalidCellItem
		       && GetCellItem(NorthEast(cell)) != InvalidCellItem
		       && GetCellItem(SouthEast(cell)) != InvalidCellItem;
	}
	bool IsNorthWallConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(West(cell)) != InvalidCellItem
		       && GetCellItem(East(cell)) != InvalidCellItem
		       && GetCellItem(North(cell)) == InvalidCellItem
		       && GetCellItem(South(cell)) != InvalidCellItem
		       && GetCellItem(SouthEast(cell)) != InvalidCellItem
		       && GetCellItem(SouthWest(cell)) != InvalidCellItem;
	}
	bool IsSouthWallConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(West(cell)) != InvalidCellItem
		       && GetCellItem(East(cell)) != InvalidCellItem
		       && GetCellItem(South(cell)) == InvalidCellItem
		       && GetCellItem(North(cell)) != InvalidCellItem
		       && GetCellItem(NorthEast(cell)) != InvalidCellItem
		       && GetCellItem(NorthWest(cell)) != InvalidCellItem;
	}

	bool IsNorthWallEndCapConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(North(cell)) == InvalidCellItem
		       && GetCellItem(East(cell)) != InvalidCellItem
		       && GetCellItem(West(cell)) != InvalidCellItem
		       && GetCellItem(NorthEast(cell)) != InvalidCellItem
		       && GetCellItem(NorthWest(cell)) != InvalidCellItem;
	}
	bool IsEastWallEndCapConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(East(cell)) == InvalidCellItem
		       && GetCellItem(North(cell)) != InvalidCellItem
		       && GetCellItem(South(cell)) != InvalidCellItem
		       && GetCellItem(NorthEast(cell)) != InvalidCellItem
		       && GetCellItem(SouthEast(cell)) != InvalidCellItem;
	}
	bool IsSouthWallEndCapConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(South(cell)) == InvalidCellItem
		       && GetCellItem(East(cell)) != InvalidCellItem
		       && GetCellItem(West(cell)) != InvalidCellItem
		       && GetCellItem(SouthEast(cell)) != InvalidCellItem
		       && GetCellItem(SouthWest(cell)) != InvalidCellItem;
	}
	bool IsWestWallEndCapConfiguration(Vector3I cell)
	{
		return GetCellItem(cell) != InvalidCellItem
		       && GetCellItem(West(cell)) == InvalidCellItem
		       && GetCellItem(North(cell)) != InvalidCellItem
		       && GetCellItem(South(cell)) != InvalidCellItem
		       && GetCellItem(NorthWest(cell)) != InvalidCellItem
		       && GetCellItem(SouthWest(cell)) != InvalidCellItem;
	}
	
	void ApplyFloorToTargetCell(Vector3I cell)
	{
		if (IsIsolatedFloorConfiguration(cell))
		{
			return;
		}
		if (IsSingleWidthFloorConfiguration(cell))
		{
			SetCellItemMiddleware(cell, MoveType.Floor, Orientation.Up);
			return;
		}
		if (IsDiagonalGapFloorConfiguration(cell))
		{
			SetCellItemMiddleware(cell, MoveType.Floor, Orientation.Up);
			return;
		}
		if (IsChokePointFloorConfiguration(cell))
		{
			SetCellItemMiddleware(cell, MoveType.Floor, Orientation.Up);
			return;
		}
		
		if (_random.Next(0, 2) == 0)
		{
			SetCellItemMiddleware(cell, MoveType.Floor, Orientation.Up);
		}
	}

	//this method is necessary for configurations which are very sensitive to order of floor block placements
	// so are cleaned up after normal floor placement has finished
	void ApplyFloorToTargetCellSecondPass(Vector3I cell)
	{
		if (IsTetrisChokePointLeftFloorConfiguration(cell))
		{
			SetCellItemMiddleware(cell, MoveType.Floor, Orientation.Up);
		}
		if (IsTetrisChokePointRightFloorConfiguration(cell))
		{
			SetCellItemMiddleware(cell, MoveType.Floor, Orientation.Up);
		}
	}

	void SetCellItemMiddleware(Vector3I cell, MoveType moveType, Orientation orientation)
	{
		SetCellItem(cell, MoveTypeToMeshLibraryItem(moveType), OrientationToRaw(orientation));
	}
	
	bool IsIsolatedFloorConfiguration(Vector3I cell)
	{
		return GetCellItem(North(cell)) == InvalidCellItem
		       && GetCellItem(South(cell)) == InvalidCellItem
		       && GetCellItem(East(cell)) == InvalidCellItem
		       && GetCellItem(West(cell)) == InvalidCellItem;
	}
	bool IsSingleWidthFloorConfiguration(Vector3I cell)
	{
		return (GetCellItem(North(cell)) != InvalidCellItem && GetCellItem(North(North(cell))) == InvalidCellItem)
		       || (GetCellItem(East(cell)) != InvalidCellItem && GetCellItem(East(East(cell))) == InvalidCellItem)
		       || (GetCellItem(South(cell)) != InvalidCellItem && GetCellItem(South(South(cell))) == InvalidCellItem)
		       || (GetCellItem(West(cell)) != InvalidCellItem && GetCellItem(West(West(cell))) == InvalidCellItem);
	}

	bool IsDiagonalGapFloorConfiguration(Vector3I cell)
	{
		return (GetCellItem(NorthEast(cell)) != InvalidCellItem && GetCellItem(NorthEast(NorthEast(cell))) == InvalidCellItem)
		       || (GetCellItem(SouthEast(cell)) != InvalidCellItem && GetCellItem(SouthEast(SouthEast(cell))) == InvalidCellItem)
		       || (GetCellItem(SouthWest(cell)) != InvalidCellItem && GetCellItem(SouthWest(SouthWest(cell))) == InvalidCellItem)
		       || (GetCellItem(NorthWest(cell)) != InvalidCellItem && GetCellItem(NorthWest(NorthWest(cell))) == InvalidCellItem);
	}

	bool IsChokePointFloorConfiguration(Vector3I cell)
	{
		return (GetCellItem(North(cell)) != InvalidCellItem
		        && GetCellItem(NorthEast(cell)) != InvalidCellItem
		        && GetCellItem(East(cell)) != InvalidCellItem
		        && GetCellItem(East(NorthEast(cell))) == InvalidCellItem)
		       || (GetCellItem(East(cell)) != InvalidCellItem
		           && GetCellItem(SouthEast(cell)) != InvalidCellItem
		           && GetCellItem(South(cell)) != InvalidCellItem
		           && GetCellItem(East(SouthEast(cell))) == InvalidCellItem)
		       || (GetCellItem(South(cell)) != InvalidCellItem
		           && GetCellItem(SouthWest(cell)) != InvalidCellItem
		           && GetCellItem(West(cell)) != InvalidCellItem
		           && GetCellItem(West(SouthWest(cell))) == InvalidCellItem)
		       || (GetCellItem(West(cell)) != InvalidCellItem
		           && GetCellItem(NorthWest(cell)) != InvalidCellItem
		           && GetCellItem(North(cell)) != InvalidCellItem
		           && GetCellItem(West(NorthWest(cell))) == InvalidCellItem);
	}
	
	bool IsTetrisChokePointLeftFloorConfiguration(Vector3I cell)
	{
		return (GetCellItem(West(cell)) != InvalidCellItem
		        && GetCellItem(West(West(cell))) != InvalidCellItem
		        && GetCellItem(SouthWest(cell)) != InvalidCellItem
		        && GetCellItem(West(SouthWest(cell))) == InvalidCellItem
		        && GetCellItem(South(cell)) != InvalidCellItem)
			|| (GetCellItem(West(cell)) != InvalidCellItem
			    && GetCellItem(NorthWest(cell)) != InvalidCellItem
			    && GetCellItem(North(NorthWest(cell))) == InvalidCellItem
			    && GetCellItem(North(North(cell))) != InvalidCellItem
			    && GetCellItem(North(cell)) != InvalidCellItem)
			|| (GetCellItem(North(cell)) != InvalidCellItem
			    && GetCellItem(NorthEast(cell)) != InvalidCellItem
			    && GetCellItem(NorthEast(East(cell))) == InvalidCellItem
			    && GetCellItem(East(East(cell))) != InvalidCellItem
			    && GetCellItem(East(cell)) != InvalidCellItem)
			|| (GetCellItem(East(cell)) != InvalidCellItem
			    && GetCellItem(South(cell)) != InvalidCellItem
			    && GetCellItem(SouthEast(cell)) != InvalidCellItem
			    && GetCellItem(South(South(cell))) != InvalidCellItem
			    && GetCellItem(South(SouthEast(cell))) == InvalidCellItem);
	}
	bool IsTetrisChokePointRightFloorConfiguration(Vector3I cell)
	{
		return (GetCellItem(North(cell)) != InvalidCellItem
		        && GetCellItem(NorthWest(cell)) != InvalidCellItem
		        && GetCellItem(NorthWest(West(cell))) == InvalidCellItem
		        && GetCellItem(West(West(cell))) != InvalidCellItem
		        && GetCellItem(West(cell)) != InvalidCellItem)
		       || (GetCellItem(West(cell)) != InvalidCellItem
		           && GetCellItem(SouthWest(cell)) != InvalidCellItem
		           && GetCellItem(South(cell)) != InvalidCellItem
		           && GetCellItem(South(South(cell))) != InvalidCellItem
		           && GetCellItem(SouthWest(South(cell))) == InvalidCellItem)
		       || (GetCellItem(South(cell)) != InvalidCellItem
		           && GetCellItem(SouthEast(cell)) != InvalidCellItem
		           && GetCellItem(SouthEast(East(cell))) == InvalidCellItem
		           && GetCellItem(East(East(cell))) != InvalidCellItem
		           && GetCellItem(East(cell)) != InvalidCellItem)
		       || (GetCellItem(East(cell)) != InvalidCellItem
		           && GetCellItem(NorthEast(cell)) != InvalidCellItem
		           && GetCellItem(North(cell)) != InvalidCellItem
		           && GetCellItem(North(North(cell))) != InvalidCellItem
		           && GetCellItem(NorthEast(North(cell))) == InvalidCellItem);
	}

	Vector3I North(Vector3I cell)
	{
		return cell + new Vector3I(1, 0, 0);
	}
	Vector3I South(Vector3I cell)
	{
		return cell + new Vector3I(-1, 0, 0);
	}
	Vector3I East(Vector3I cell)
	{
		return cell + new Vector3I(0, 0, 1);
	}
	Vector3I West(Vector3I cell)
	{
		return cell + new Vector3I(0, 0, -1);
	}
	Vector3I NorthEast(Vector3I cell)
	{
		return cell + new Vector3I(1, 0, 1);
	}
	Vector3I SouthEast(Vector3I cell)
	{
		return cell + new Vector3I(-1, 0, 1);
	}
	Vector3I SouthWest(Vector3I cell)
	{
		return cell + new Vector3I(-1, 0, -1);
	}
	Vector3I NorthWest(Vector3I cell)
	{
		return cell + new Vector3I(1, 0, -1);
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
		
	int MoveTypeToMeshLibraryItem(MoveType moveType)
	{
		return moveType switch
		{
			MoveType.Empty => (int)InvalidCellItem,
			MoveType.Floor => MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "floor"),
			MoveType.Wall => MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall"),
			MoveType.InsideCorner => MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall-inside"),
			MoveType.OutsideCorner => MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall-outside"),
			MoveType.OutsideDoubleCorner => MeshLibrary.GetItemList().FirstOrDefault(i => MeshLibrary.GetItemName(i) == "wall-outside-double"),
			_ => throw new ArgumentOutOfRangeException(nameof(moveType), moveType, null)
		};
	}

	MoveType MeshLibraryItemToMoveType(int meshLibraryItem)
	{
		var itemName = MeshLibrary.GetItemName(meshLibraryItem);
		return itemName switch
		{
			"" => MoveType.Empty,
			"floor" => MoveType.Floor,
			"wall" => MoveType.Wall,
			"wall-inside" => MoveType.InsideCorner,
			"wall-outside" => MoveType.OutsideCorner,
			"wall-outside-double" => MoveType.OutsideDoubleCorner,
			_ => throw new ArgumentOutOfRangeException(nameof(meshLibraryItem), meshLibraryItem, null)
		};
	}
	
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
}
