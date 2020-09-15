using GenericRenderer.Assets.Enums;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Maybe should not be in the generic renderer namespace?
//Ideas: Make this a virtual class?

namespace GenericRenderer.Assets
{
    public class MaterialAttribute
    {
        public ParamDataType Type { get; set; }
        public string UniformName { get; set; }
        public object Data { get; set; }
    }

    public class RenderMaterial
    {
        public string Name { get; set; }
        string currentShader { get; set; }
        public Dictionary<string, RenderShader> Shaders { get; set; } = new Dictionary<string, RenderShader>();
        public RenderShader CurrentShader => Shaders[currentShader];
        public RenderTexture[] Textures { get; set; } = new RenderTexture[15];
        public List<MaterialAttribute> Attributes { get; set; } = new List<MaterialAttribute>();

        public void UseShader (string name)
        {
            if (Shaders.ContainsKey(name))
                currentShader = name;
        }

        public void UseMaterial()
        {
            UseTextures();

            CurrentShader.Use();
        }

        public void UseTextures()
        {
            for (int i = 0; i < Textures.Length; i++)
            {
                if (Textures[i] != null)
                {
                    CurrentShader.UniformInt("Texture" + i, i);

                    Textures[i].Use(TextureUnit.Texture0 + i);
                }
            }
        }

        public static void BindNullMaterial()
        {
            GL.UseProgram(0);
        }
    }
}
