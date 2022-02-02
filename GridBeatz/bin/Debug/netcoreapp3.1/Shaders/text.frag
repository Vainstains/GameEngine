#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform vec3 color;
uniform float alpha;

void main()
{
    outputColor = texture(texture0, texCoord) * vec4(color,alpha);
}