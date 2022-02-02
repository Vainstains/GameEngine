#version 330

out vec4 outputColor;

in vec2 texCoord;
uniform vec3 color;

void main()
{
    outputColor = vec4(color,1);
}