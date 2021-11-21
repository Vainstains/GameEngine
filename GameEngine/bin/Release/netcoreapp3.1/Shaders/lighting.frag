#version 330 core
out vec4 FragColor;

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
in vec2 texCoord;

uniform sampler2D texture0;
uniform sampler2D metallicMap;
uniform sampler2D emissionMap;
uniform vec3 lightColor; //The color of the light.
uniform vec3 ambientColor;
uniform vec3 lightPos; //The position of the light.
uniform vec3 viewPos;
//uniform vec3 modelPos;
uniform float lightPower;
uniform float shininess;
uniform float ambientPower;
uniform float emission;

//fog
uniform float fogStart;
uniform float fogEnd;

in vec3 Normal; //The normal of the fragment is calculated in the vertex shader.
in vec3 FragPos; //The fragment position.
in vec3 Color;

void main()
{
    //The ambient color is the color where the light does not directly hit the object.
    //You can think of it as an underlying tone throughout the object. Or the light coming from the scene/the sky (not the sun).
    float ambientStrength = ambientPower;
    vec3 ambient = ambientStrength * lightColor * ambientColor;

    //We calculate the light direction, and make sure the normal is normalized.
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos * vec3(1.0,1.0,1.0)); //Note: The light is pointing from the light to the fragment
    
    //The specular light is the light that shines from the object, like light hitting metal.
    //The calculations are explained much more detailed in the web version of the tutorials.
    float specularStrength = 0.6;
    vec3 viewDir = normalize(viewPos-FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 3*shininess); //The 32 is the shininess of the material.
    vec3 specular = specularStrength * spec * lightColor * Color*500;

    //The diffuse part of the phong model.
    //This is the part of the light that gives the most, it is the color of the object where it is hit by light.
    float diff = texture(emissionMap, texCoord).x + max((dot(norm, lightDir)+1)/2, 0.0) * max((dot(norm, lightDir)+0.5)/1.5, 0.0)*100; //We make sure the value is non negative with the max function.
    vec3 diffuse = diff * lightColor * Color;


    




    float power = lightPower;
    //At last we add all the light components together and multiply with the color of the object. Then we set the color
    //and makes sure the alpha value is 1
    vec3 result = ((ambient + ((diffuse + specular) * (power/2)))+emission) * texture(texture0, texCoord).xyz;
    float distance = length(FragPos-viewPos);


    //linear interpolation
    float fog_factor = (distance-fogStart)/(fogEnd-fogStart);
    fog_factor = clamp(fog_factor,0,1);
    FragColor = vec4(mix(result*Color,vec3(0.8,0.9,1),fog_factor), 1.0);
    //FragColor = vec4(result, 1.0) * (1 + texture(emissionMap, texCoord).x);
    
    //Note we still use the light color * object color from the last tutorial.
    //This time the light values are in the phong model (ambient, diffuse and specular)
}