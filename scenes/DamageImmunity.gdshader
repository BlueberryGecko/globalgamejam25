shader_type canvas_item;
uniform sampler2D SCREEN_TEXTURE: hint_screen_texture, repeat_disable, filter_nearest;
uniform vec3 modulateColor;
uniform bool enabled;

void vertex() {

}

vec3 convolve(sampler2D tex, vec2 uv, vec2 pixelSize) {
	float gauss[9] = {
		2.0/33.0, 4.0/33.0, 2.0/33.0, 4.0/33.0, 9.0/33.0, 4.0/33.0, 2.0/33.0, 4.0/33.0, 2.0/33.0
	};
	vec3 conv = vec3(0);
	for (int i = 0; i < 3; i++) {
		for (int j = 0; j < 3; j++) {
			conv += texture(tex, uv + vec2(float(i - 1), float(j - 1)) * pixelSize).rgb * gauss[i * 3 + j];
		}
	}
	return conv;
}


void fragment() {
	if (enabled) {
		vec3 pixels[9];
		// [0, 1, 2]
		// [3, 4, 5]
		// [6, 7, 8]
		for (int row = 0; row < 3; row++) {
			for (int col = 0; col < 3; col++) {
				vec2 uv = UV + vec2(float(col - 1), float(row - 1)) * TEXTURE_PIXEL_SIZE;
				pixels[row * 3 + col] = convolve(TEXTURE, uv, TEXTURE_PIXEL_SIZE);
			}
		}
		vec3 gx = (
			pixels[0] * -1.0 + pixels[3] * -2.0 + pixels[6] * -1.0
			+ pixels[2] * 1.0 + pixels[5] * 2.0 + pixels[8] * 1.0
		);
		vec3 gy = (
			pixels[0] * -1.0 + pixels[1] * -2.0 + pixels[2] * -1.0
			+ pixels[6] * 1.0 + pixels[7] * 2.0 + pixels[8] * 1.0
		);
		vec3 sobel = sqrt(gx * gx + gy * gy);
		float m = max(max(sobel.x, sobel.y), sobel.z);
		//sobel = convolve(TEXTURE, UV, TEXTURE_PIXEL_SIZE);
		COLOR = vec4((vec3(m, m, m) * modulateColor).rgb, m) * texture(TEXTURE, UV)*2.0 + texture(TEXTURE, UV)*0.5;
	}
}