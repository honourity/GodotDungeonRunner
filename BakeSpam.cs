using System.Diagnostics;
using Godot;

public partial class BakeSpam : NavigationRegion3D
{
	Player _player;
	Vector3 _lastBakePlayerPosition;
	bool _bakeFinished;
	Rid _map;
	double _minimumBakeRate = 5;
	double _elapsedBakeTime;
	
	public override void _Ready()
	{
		_player = GetParent().GetNode<Player>("Player");
		_map = GetWorld3D().NavigationMap;
		
		BakeFinished += BakePreviouslyFinished;
		BakeNavigationMesh();
	}

	public override void _PhysicsProcess(double delta)
	{
		_elapsedBakeTime += delta;
		if (_elapsedBakeTime < _minimumBakeRate) return;
		
		var closestPoint = NavigationServer3D.MapGetClosestPoint(_map, _player.GlobalPosition);
		
		if (_player.GlobalPosition.DistanceTo(closestPoint) > 1)
		{
			Bake();
		}
	}
	
	void Bake()
	{
		if (!_bakeFinished) return;
		
		_bakeFinished = false;
		_lastBakePlayerPosition = _player.GlobalPosition;
		
		BakeNavigationMesh();
	}

	void BakePreviouslyFinished()
	{
		_bakeFinished = true;
		_elapsedBakeTime = 0f;
	}
}
