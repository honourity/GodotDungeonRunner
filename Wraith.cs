using Godot;

public partial class Wraith : CharacterBody3D
{
	float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	Player _player;
	NavigationAgent3D _navigationAgent3D;

	double _pathUpdateRate = 0.5f;
	double _pathUpdateElapsed;
	
	public override void _Ready()
	{
		_navigationAgent3D = GetNode<NavigationAgent3D>("NavigationAgent3D");
		_player = GetParent().GetNode<Player>("Player");

		CallDeferred(MethodName.wait_for_map);
	}

	async void wait_for_map()
	{
		await ToSignal(GetTree(), "physics_frame");
		await ToSignal(GetTree(), "process_frame");
		
		await ToSignal(GetTree(), "physics_frame");
		await ToSignal(GetTree(), "process_frame");
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
}
