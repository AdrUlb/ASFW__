#version 330 core

in vec2 TexCoord;
in vec4 TexTint;

out vec4 FragColor;

uniform sampler2D tex;

void main()
{
	FragColor = texture(tex, TexCoord) * TexTint;
}
