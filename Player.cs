using Godot;
using System;

public partial class Player : CharacterBody3D
{
	const float Speed = 5.0f;
	float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	Gun _gun;
	ScreenFlash _screenFlash;

	public override void _Ready()
	{
		_gun = GetNode("ScreenEffects").GetNode<Gun>("Gun");
		_screenFlash = GetNode("ScreenEffects").GetNode<ScreenFlash>("ScreenFlash");
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

		if (!IsOnFloor()) velocity.Y -= _gravity * (float)delta;

		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) _gun.Shoot();

		var inputDirection = Input.GetVector("ui_home", "ui_end", "ui_up", "ui_down");
		var direction = (Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		//Get input rotation
		if (Input.IsActionPressed("ui_left"))
		{
			RotateY((float)(Math.PI * delta));
		}
		if (Input.IsActionPressed("ui_right"))
		{
			RotateY((float)(-Math.PI * delta));
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public void TakeDamage(int damage)
	{
		_screenFlash.Flash(new Color(0.8f, 0, 0), 1);
	}
}
