using GenericRenderer.Game;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericRenderer.Assets
{
    public class ShaderSource
    {
        public int Handler { get; private set; }
        public ShaderType Type { get; set; }
        public string Source { get; set; }
        public void Compile()
        {
            Handler = GL.CreateShader(Type);

            GL.ShaderSource(Handler, Source);
            GL.CompileShader(Handler);

            string Error = GL.GetShaderInfoLog(Handler);

            if (Error != "")
            {
                Console.WriteLine(Error);
            }
        }
    }
    public class RenderShader
    {
        public static Dictionary<string, RenderShader> ShaderCollection { get; set; } = new Dictionary<string, RenderShader>();
        public int Handler { get; private set; }
        public int Name { get; set; }
        public List<ShaderSource> Sources { get; set; } = new List<ShaderSource>();
        public Dictionary<string, int> Uniforms { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> UniformBlocks { get; set; } = new Dictionary<string, int>();
        public bool Compiled { get; private set; } = false;

        public int GetShaderUniform(string name)
        {
            Use();

            if (!Uniforms.ContainsKey(name))
            {
                Uniforms.Add(name, GL.GetUniformLocation(Handler, name));
            }

            return Uniforms[name];
        }

        public int GetSahderUniformBlock(string name)
        {
            Use();

            if (!UniformBlocks.ContainsKey(name))
            {
                UniformBlocks.Add(name, GL.GetUniformBlockIndex(Handler, name));
            }

            return UniformBlocks[name];
        }

        public void UniformFloat(string name,float data)
        {
            GL.Uniform1(GetShaderUniform(name), data);
        }
        public void UniformInt(string name, int data)
        {
            GL.Uniform1(GetShaderUniform(name), data);
        }
        public void UniformVector4(string name, Vector4 data)
        {
            GL.Uniform4(GetShaderUniform(name),data);
        }

        public void Compile()
        {
            if (!Compiled)
            {
                Handler = GL.CreateProgram();

                foreach (ShaderSource source in Sources)
                {
                    source.Compile();

                    GL.AttachShader(Handler, source.Handler);
                }

                GL.LinkProgram(Handler);
                GL.ValidateProgram(Handler);

                Compiled = true;
            }
        }
        public void Use()
        {
            Compile();

            GL.UseProgram(Handler);
        }
        ~RenderShader()
        {
            if (Window.MainWindow != null)
            {
                if (Window.MainWindow.Active)
                {
                    GL.DeleteProgram(Handler);
                }
            }
        }
    }
}
