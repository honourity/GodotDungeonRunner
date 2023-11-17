using Godot;

public partial class Wraith : CharacterBody3D
{
	float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	Player _player;
	NavigationAgent3D _navigationAgent3D;

	double _pathUpdateRate = 0.5f;
	double _pathUpdateElapsed;

	int _health = 3;

	Sprite3D _sprite;
	double _takeHitDuration = 0.05f;
	double _takeHitTime;
	
	public override void _Ready()
	{
		AddToGroup("wraiths");
		
		_navigationAgent3D = GetNode<NavigationAgent3D>("NavigationAgent3D");
		_player = GetTree().CurrentScene.GetNode<Player>("Player");
		_sprite = GetNode<Sprite3D>("Sprite3D");
		
		CallDeferred(MethodName.wait_for_map);
	}

	async void wait_for_map()
	{
		await ToSignal(GetTree(), "physics_frame");
		await ToSignal(GetTree(), "process_frame");
		
		await ToSignal(GetTree(), "physics_frame");
		await ToSignal(GetTree(), "process_frame");
	}

	public override void _Process(double delta)
	{
		_takeHitTime += delta;

		if (_takeHitTime >= _takeHitDuration)
		{
			_sprite.Modulate = new Color(1f, 1f, 1f);
			_sprite.Shaded = true;
		}

		if (_health <= 0)
		{
			Free();
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (GlobalPosition.DistanceTo(_player.GlobalPosition) < _navigationAgent3D.TargetDesiredDistance) return;
		
		_pathUpdateElapsed += delta;
		if (_pathUpdateElapsed > _pathUpdateRate)
		{
			_navigationAgent3D.TargetPosition = _player.GlobalPosition;
			_pathUpdateElapsed = 0;
		}

		var currentPosition = GlobalPosition;
		var nextPosition = _navigationAgent3D.GetNextPathPosition();
		
		var directionWithVelocity = nextPosition - currentPosition;
		directionWithVelocity = directionWithVelocity.Normalized() * 5f;

		Velocity = directionWithVelocity;
		MoveAndSlide();
	}

	public void TakeHit()
	{
		_sprite.Modulate = new Color(1f, 0, 0);
		_sprite.Shaded = false;
		_takeHitTime = 0f;

		_health--;
	}
}
