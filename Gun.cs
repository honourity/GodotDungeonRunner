using Godot;

public partial class Gun : ColorRect
{
	ScreenFlash _screenFlash;
	PackedScene _bulletDecalScene;
	RayCast3D _cameraRayCast;
	CharacterBody3D _player;
	Node _map;

	public override void _Ready()
	{
		_cameraRayCast = GetParent().GetParent().GetNode<Camera3D>("Camera").GetNode<RayCast3D>("RayCast3D");
		_player = GetParent().GetParent<CharacterBody3D>();
		_bulletDecalScene = GD.Load<PackedScene>("res://bullet_decal.tscn");
		_map = GetParent().GetParent().GetParent().GetNode("GridMap");
		_screenFlash = GetParent().GetNode<ScreenFlash>("ScreenFlash");
	}

	public override void _Process(double delta)
	{
	}
	
	public void Shoot()
	{
		_screenFlash.Flash(new Color(0.8f, 0.8f, 0.8f), 0.2d);
		
		if (!_cameraRayCast.IsColliding()) return;
		
		var bulletDecal = _bulletDecalScene.Instantiate() as Node3D;
		_map.AddChild(bulletDecal);
		bulletDecal.GlobalPosition = _cameraRayCast.GetCollisionPoint();
		bulletDecal.Rotation = _cameraRayCast.GetCollisionNormal().Rotated(Vector3.Up, Mathf.Pi * 0.5f);
	}
}
