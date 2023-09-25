shader_type canvas_item;
uniform float toggle;

void fragment() {
	COLOR = mix(vec4(0.0),vec4(1.0,0.0,0.0,1.0),toggle);
}
