shader_type canvas_item;
uniform float fade;
uniform vec4 color;

void fragment() {
	COLOR = mix(vec4(0.0),color,fade);
}
