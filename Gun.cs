using Godot;

public partial class Gun : CanvasLayer
{
	ShaderMaterial _screenFlash;
	PackedScene _bulletDecalScene;
	RayCast3D _cameraRayCast;
	CharacterBody3D _player;
	Node _map;
	
	public override void _Ready()
	{
		_screenFlash = GetNode<ColorRect>("ScreenFlash").Material as ShaderMaterial;
		_cameraRayCast = GetParent().GetNode<Camera3D>("Camera").GetNode<RayCast3D>("RayCast3D");
		_player = GetParent <CharacterBody3D>();
		_bulletDecalScene = GD.Load<PackedScene>("res://bullet_decal.tscn");
		_map = GetParent().GetParent().GetNode("GridMap");
	}

	public override void _Process(double delta)
	{
	}
	
	public void Shoot()
	{
		_screenFlash.SetShaderParameter("toggle", 1.0);
		
		var timer = GetTree().CreateTimer(0.03);
		timer.Timeout += () => { _screenFlash.SetShaderParameter("toggle", 0); };

		if (!_cameraRayCast.IsColliding()) return;
		
		//todo - instantiate a decal in world space at the raycast hit position angled to raycast hit normal
		var bulletDecal = _bulletDecalScene.Instantiate() as Node3D;
		_map.AddChild(bulletDecal);

		bulletDecal.GlobalPosition = _cameraRayCast.GetCollisionPoint();
		bulletDecal.Rotation = _cameraRayCast.GetCollisionNormal().Rotated(Vector3.Up, Mathf.Pi * 0.5f);
	}
}
