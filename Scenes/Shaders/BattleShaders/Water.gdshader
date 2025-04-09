shader_type spatial;

uniform vec4 water_color: source_color;
uniform sampler2D noise_texture;
uniform sampler2D noise_texture2;
uniform float wave_speed;

const float WATER_TRANSPARENCY = .85f;
const float WATER_ROUGHNESS =.5f;

varying vec4 noise_color;
varying vec4 noise_color_reverse;
varying vec4 combined_noise;

void vertex() {
	noise_color = texture(noise_texture, UV + TIME * wave_speed);
	noise_color_reverse = texture(noise_texture2, UV - TIME * wave_speed);
	combined_noise = pow(noise_color * noise_color_reverse, vec4(1.25f));
	VERTEX = combined_noise.rgb * NORMAL * 5.0f + VERTEX;
	// Called for every vertex the material is visible on.
	
}

//general idea for this shader came from the following two tutorials mixed together:
//https://www.youtube.com/watch?v=XjCh2cN3Mfg&ab_channel=Bramwell (mainly)
//I don't like visual shaders so wrote it as text shader
//https://www.youtube.com/watch?v=sCIr_58NN48&ab_channel=Heartbeast
void fragment() {
	//vec4 combined_noise = noise_color * noise_color_reverse;
	ALBEDO = water_color.rgb + combined_noise.rgb;
	ALPHA = WATER_TRANSPARENCY;
	ROUGHNESS = WATER_ROUGHNESS;
	//makes the overlaps brighter
	EMISSION = combined_noise.rgb;
	// Called for every pixel the material is visible on.
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
