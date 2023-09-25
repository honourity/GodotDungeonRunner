using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : CharacterBody3D
{
	const float Speed = 5.0f;
	float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	ShaderMaterial _screenFlash;

	public override void _Ready()
	{
		_screenFlash = GetParentNode3D().GetNode<CanvasLayer>("CanvasLayer").GetNode<ColorRect>("ScreenFlash").Material as ShaderMaterial;
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

		// Apply gravity
		if (!IsOnFloor()) velocity.Y -= _gravity * (float)delta;

		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			DoShoot();
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
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

	void DoShoot()
	{
		_screenFlash.SetShaderParameter("toggle", 1.0);
		
		var timer = GetTree().CreateTimer(0.03);
		timer.Timeout += () => { _screenFlash.SetShaderParameter("toggle", 0.0); };
		
		//todo - do a raycast to determind what is hit and where
		//todo - place a decal if it hits gridmap stuff (which auto removes itself after x seconds)
	}
}
