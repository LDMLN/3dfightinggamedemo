//this overall shader came from a tutorial I found on youtube located here:
//https://www.youtube.com/watch?v=zdcvmBKVil8&ab_channel=MerliGameDev
shader_type spatial;
render_mode cull_disabled;
uniform sampler2D noise_sampler: filter_linear_mipmap, repeat_enable;
uniform sampler2D gradient_sampler: filter_linear, source_color;
uniform float emission_strength = 5.0;

//this came from the godotshaders website, linked here:
//https://godotshaders.com/snippet/fresnel/
float fresnel(float amount, vec3 normal, vec3 view)
{
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0 )), amount);
}



void fragment() {
	float noise = texture(noise_sampler, UV - vec2(0.0, TIME)).x;
	vec3 gradient = texture(gradient_sampler, vec2(UV.y, 0.0)).rgb;
	ALBEDO = gradient;
	EMISSION = (gradient + step(0.25, fresnel(2.5, NORMAL, VIEW)) * gradient) * emission_strength;
	ALPHA_SCISSOR_THRESHOLD = 0.5;
	ALPHA = step(-0.1, noise - UV.y);
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
