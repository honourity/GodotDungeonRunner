using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 3f;
	
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor()) velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		//if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDirection = Input.GetVector("ui_home", "ui_end", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDirection.X, 0, inputDirection.Y)).Normalized();
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
}
