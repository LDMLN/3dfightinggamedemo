shader_type spatial;

//world_vertex_coords -> sets the VERTEX to be handled in world space
//cull_disabled -> disables backface culling
render_mode cull_disabled;


const float ALPHA_CUTOFF = 0.1f;
//uniform float dissolve_height = 12.0f;
//noise to alter the shape of mesh
uniform sampler2D noise_texture;
uniform float noise_speed = 0.1f;

uniform float dissolve_cutoff = .05f;
//varying vec4 noise_color;

varying vec3 world_position;
uniform vec3 base_color: source_color;

void vertex() {
	//noise_color = texture(noise_texture, UV + TIME * noise_speed);
	//want to make this affected by 3D noise, but not sure how to right now...
	//will study and update this later.
	//world_position = VERTEX;
}

void fragment()
{
	//sets it equal to where the vertex is in world space...
	//coupled with the ALPHA_SCISSOR means we are cutting off all verticies below
	//0.5
	//if world_position.y > dissolve height... it is visible...
	//with the oneMinus here it flips that.
	vec4 noise_color = texture(noise_texture, UV + TIME * noise_speed);
	ALPHA = step(dissolve_cutoff, noise_color.r);
	ALBEDO = noise_color.rgb + base_color;
	ALPHA_SCISSOR_THRESHOLD = ALPHA_CUTOFF;
	//EMISSION = noise_color.rgb / 2.0f;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
