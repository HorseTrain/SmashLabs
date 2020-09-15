SHADER                0�      1�  �  %      #version 330 core

uniform sampler2D Texture0;
uniform sampler2D Texture1;
uniform sampler2D Texture2;
uniform sampler2D Texture3;
uniform sampler2D Texture4;
uniform sampler2D Texture5;
uniform sampler2D Texture6;
uniform sampler2D Texture7;

//Texture 0 -> Albeto Map
//Texture 1 -> ?
//Texture 4 -> Normal Map
//Texture 6 -> Prm Map

in vec3 position;
in vec3 normal;
in vec2 uv0;
in vec2 uv1;

out vec4 fragColor;

uniform int RenderMode;

void NormalDraw()
{
    fragColor = texture(Texture0,uv1);
}

void ShadowDraw()
{

}

void main()
{
    switch (RenderMode)
    {
        case 0: NormalDraw(); break;
        case 1: ShadowDraw(); break;
    }
} #version 330 core

layout(location = 0) in vec3 iPosition;
layout(location = 1) in vec3 iNormal;
layout(location = 2) in vec2 iUV0;
layout(location = 3) in vec2 iUV1;
layout(location = 4) in vec4 iVertexColor;
layout(location = 5) in ivec4 iVertexWeightIndicies;
layout(location = 6) in vec4 iVertexWeight;

uniform int RenderMode;

out vec3 position;
out vec3 normal;
out vec2 uv0;
out vec2 uv1;

layout(std140) uniform SceneTransforms
{
    mat4 CameraTransform;
};

layout(std140) uniform SkeletonBuffer
{
    mat4 TransformBuffer[300];
};

mat4 GetTransform()
{
    mat4 Out = mat4(0);

    for (int i = 0; i < 4; i++)
    {
        Out += TransformBuffer[iVertexWeightIndicies[i]] * iVertexWeight[i];
    }

    return Out;
}

void NormalDraw()
{
    mat4 Transform = GetTransform();
    mat4 DirectionTransform = transpose(inverse(Transform));

    position = iPosition;

    normal = iNormal.xyz;
    
    uv0 = iUV0;
    uv1 = iUV1;

    gl_Position = CameraTransform * TransformBuffer[0] * vec4(position,1);
};

void ShadowDraw()
{

};

void main()
{
    switch (RenderMode)
    {
        case 0: NormalDraw(); break;
        case 1: ShadowDraw(); break;
    }
}; 