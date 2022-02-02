#version 330 core
out vec4 FragColor;

//In order to calculate some basic lighting we need a few things per model basis, and a few things per fragment basis:
in vec2 texCoord;

struct PointLight {    
    vec3 position;
    
    float constant;
    float linear;
    float quadratic;  

    float power;  
    float radius;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
#define NR_POINT_LIGHTS 100
uniform PointLight pointLights[NR_POINT_LIGHTS];
uniform int numLights;

uniform sampler2D texture0;
uniform sampler2D metallicMap;
uniform sampler2D emissionMap;
uniform vec3 lightColor; //The color of the light.
uniform vec3 ambientColor;
uniform vec3 skyColor;
uniform vec3 lightPos; //The position of the light.
uniform vec3 viewPos;
uniform vec3 camViewDir;
//uniform vec3 modelPos;
uniform float lightPower;
uniform float shininess;
uniform float ambientPower;
uniform float emission;
uniform float orthographic;

//fog
uniform float fogStart;
uniform float fogEnd;


in vec3 Normal; //The normal of the fragment is calculated in the vertex shader.
in vec3 FragPos; //The fragment position.
in vec3 Color;

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 3*shininess);
    // attenuation
    float distance    = length(light.position - fragPos) * 20 / (light.radius);
    float attenuation = 1.0 / (light.constant + light.linear * distance + 
  			     light.quadratic * (distance * distance));    
    // combine results
    vec3 ambient  = light.ambient  * 1;
    vec3 diffuse  = light.diffuse  * diff;
    vec3 specular = light.specular * spec * (1.2 - dot(viewDir, normal));
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular)*1.1*light.power;
}

vec3 rgb2hsv(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float fresnel(vec3 Normal, vec3 ViewDir, float Power)
{
    return pow((1.0 - (dot(normalize(Normal), normalize(ViewDir)))), Power);
}

void main()
{
    //The ambient color is the color where the light does not directly hit the object.
    //You can think of it as an underlying tone throughout the object. Or the light coming from the scene/the sky (not the sun).
    float ambientStrength = ambientPower;
    vec3 ambient = ambientStrength * lightColor * ambientColor;

    //We calculate the light direction, and make sure the normal is normalized.
    vec3 norm = normalize((Normal*20));
    vec3 lightDir = normalize(lightPos * vec3(1.0,1.0,1.0)); //Note: The light is pointing from the light to the fragment
    
    //The specular light is the light that shines from the object, like light hitting metal.
    //The calculations are explained much more detailed in the web version of the tutorials.
    
    vec3 viewDir = (normalize(viewPos-FragPos)*(1-orthographic))+(camViewDir*orthographic);
    vec3 reflectDir = reflect(-lightDir, norm);
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float specularStrength = 0.6;
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 3*shininess*(1+fresnel(norm, viewDir, 2))); //The 32 is the shininess of the material.
    vec3 specular = specularStrength * spec * lightColor * Color*200 * (1.2 - dot(viewDir, norm) *0.2);

    //The diffuse part of the phong model.
    //This is the part of the light that gives the most, it is the color of the object where it is hit by light.
    float diff = (texture(emissionMap, texCoord).x + max((dot(norm, lightDir)+1)/2, 0.0) * max((dot(norm, lightDir)+0.5)/1.5, 0.0)*100); //We make sure the value is non negative with the max function.
    vec3 diffuse = diff * lightColor * Color;


    
    vec3 light = vec3(0,0,0);
    for(int i = 0; i < numLights; i++)
    {
        light += CalcPointLight(pointLights[i],norm, FragPos, viewDir)*2;
    }

    float power = lightPower;
    //At last we add all the light components together and multiply with the color of the object. Then we set the color
    //and makes sure the alpha value is 1
    vec3 result = (((((ambient + ((diffuse + specular) * (power/2)))+emission) * texture(texture0, texCoord).xyz))+(light*texture(texture0, texCoord).xyz*0.9))+(fresnel(norm, viewDir, 7)*ambient);
    float distance = length(FragPos-viewPos);


    //linear interpolation
    float fog_factor = (distance-fogStart)/(fogEnd-fogStart);
    fog_factor = clamp(fog_factor,0,1);
    
    vec4 col = vec4(mix(result*Color,skyColor,fog_factor), 1.0);

    vec3 hsv = rgb2hsv(col.xyz);

    hsv.y -= clamp(hsv.z-0.8,0,1);
    
    vec3 rgb = hsv2rgb(hsv) + (Color*emission);
    FragColor = vec4(rgb,1);
    //FragColor = vec4(result, 1.0) * (1 + texture(emissionMap, texCoord).x);
    
    //Note we still use the light color * object color from the last tutorial.
    //This time the light values are in the phong model (ambient, diffuse and specular)
}