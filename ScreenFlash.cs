using Godot;

public partial class ScreenFlash : ColorRect
{
	ShaderMaterial _flashShader;
	double _flashTimeTotal = 0.2;
	double _flashTime;
	
	public override void _Ready()
	{
		_flashShader = Material as ShaderMaterial;
	}

	public override void _Process(double delta)
	{
		if (_flashTime < _flashTimeTotal)
		{
			_flashTime += delta;
			
			if (_flashTime >= _flashTimeTotal)
			{
				_flashShader.SetShaderParameter("fade", 0);
			}
			else
			{
				
				var fade = Mathf.Remap(_flashTime, 0, _flashTimeTotal, 1, 0);
				_flashShader.SetShaderParameter("fade", fade);
			}
		}
	}
	
	public void Flash(Color color)
	{
		_flashShader.SetShaderParameter("color", color);
		_flashShader.SetShaderParameter("fade", 1);
		_flashTime = 0;
	}
}
