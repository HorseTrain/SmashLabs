SHADER                0�      1�  }  �      #version 330 core

uniform sampler2D Texture0;
uniform sampler2D Texture1;
uniform sampler2D Texture2;
uniform sampler2D Texture3;
uniform sampler2D Texture4;
uniform sampler2D Texture5;
uniform sampler2D Texture6;
uniform sampler2D Texture7;

in vec3 position;
in vec3 normal;
in vec2 uv0;
in vec2 uv1;

out vec4 fragColor;

void main()
{
    fragColor = texture(Texture0,uv0);
} #version 330 core

layout(location = 0) in vec3 iPosition;
layout(location = 1) in vec3 iNormal;
layout(location = 2) in vec2 iUV0;
layout(location = 3) in vec3 iUV1;
layout(location = 4) in vec4 iVertexColor;
layout(location = 5) in ivec4 iVertexWeightIndicies;
layout(location = 6) in vec4 iVertexWeight;

uniform int RenderMode;

out vec3 position;
out vec3 normal;
out vec2 uv0;
out vec2 uv1;

layout(std140) uniform SceneBuffer
{
    mat4 CameraTransform;
};

layout(std140) uniform SkeletonBuffer
{
    mat4 TransformBuffer[300];
};

void NormalDraw()
{
    position = iPosition;

    normal = iVertexWeight.xyz;

    gl_Position = vec4(position/10.0f,1);
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