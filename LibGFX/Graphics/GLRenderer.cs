using LibGFX.Core;
using LibGFX.Graphics.Shader;
using LibGFX.Graphics.Shapes;
using LibGFX.Math;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    public class GLRenderer : IRenderDevice
    {
        private Dictionary<String, ShaderProgram> _programs;
        private Dictionary<String, Shape> _shapes;
        private IGLFWGraphicsContext _context;
        private Window _window;
        private Matrix4 _viewMatrix;
        private Matrix4 _projectionMatrix;
        private int _currentProgram;

        public void Init(Window window)
        {
            _window = window;
            _context = _window.GetContext();

            _programs = new Dictionary<String, ShaderProgram>();
            this.AddShaderProgram("ScreenShader", new ScreenShader());
            this.AddShaderProgram("RectShader", new RectShader());
            this.AddShaderProgram("SpriteShader", new SpriteShader());
            foreach (ShaderProgram program in _programs.Values)
            {
                this.BuildShaderProgram(program);
            }

            _shapes = new Dictionary<String, Shape>();
            this.AddShape(new FramebufferShape());
            this.AddShape(new RectShape());
            this.AddShape(new SpriteShape());
            foreach (var shape in _shapes.Values)
            {
                this.InitShape(shape);
            }

            GL.Enable(EnableCap.Multisample);
        }

        public void UseVsync(bool value)
        {
            _context.SwapInterval = value ? 1 : 0;
        }

        public void Clear(int mask)
        {
            GL.Clear((ClearBufferMask)mask);
        }

        public void ClearColor(float r, float g, float b, float a)
        {
            GL.ClearColor(r, g, b, a);
        }

        public void Flush()
        {
            GL.Flush();
        }

        public void MakeCurrent()
        {
            _context.MakeCurrent();
        }

        public void SwapBuffers()
        {
            _context.SwapBuffers();
        }

        public int GetError()
        {
            return (int) GL.GetError();
        }

        public void EnableDepthTest()
        {
            GL.Enable(EnableCap.DepthTest);
        }

        public void DisableDepthTest()
        {
            GL.Disable(EnableCap.DepthTest);
        }

        public void SetViewport(Viewport viewport)
        {
            GL.Viewport(0, 0, viewport.Width, viewport.Height);
        }

        public void SetViewMatrix(Matrix4 matrix)
        {
            _viewMatrix = matrix;
        }

        public void SetProjectionMatrix(Matrix4 matrix)
        {
            _projectionMatrix = matrix;
        }

        public RenderTarget CreateRenderTarget(RenderTargetDescriptor constructorInfo)
        {
            RenderTarget renderTarget = new RenderTarget();

            renderTarget.FramebufferID = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget.FramebufferID);

            renderTarget.TextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, renderTarget.TextureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, constructorInfo.Width, constructorInfo.Height, constructorInfo.Border, PixelFormat.Rgba, PixelType.UnsignedByte, 0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, renderTarget.TextureID, 0);

            renderTarget.RenderBufferID = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderTarget.RenderBufferID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, constructorInfo.Width, constructorInfo.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderTarget.RenderBufferID);

            if(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == FramebufferErrorCode.FramebufferComplete)
            {
                Debug.WriteLine("Framebuffer Created");
            }
            else
            {
                Debug.WriteLine("Error while creating Framebuffer");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            return renderTarget;
        }

        public void BindRenderTarget(RenderTarget renderTarget)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget.FramebufferID);
        }

        public void ResizeRenderTarget(RenderTarget renderTarget, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, renderTarget.TextureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderTarget.RenderBufferID);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public void UnbindRenderTarget()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public int GetFramebufferIndex()
        {
            int currentFramebuffer = 0;
            GL.GetInteger(GetPName.FramebufferBinding, out currentFramebuffer);
            Debug.WriteLine($"Current framebuffer binding: {currentFramebuffer}");
            return currentFramebuffer;
        }

        public void DisposeRenderTarget(RenderTarget renderTarget)
        {
            GL.DeleteTexture(renderTarget.TextureID);
            GL.DeleteRenderbuffer(renderTarget.RenderBufferID);
            GL.DeleteFramebuffer(renderTarget.FramebufferID);
            Debug.WriteLine("RenderTarget disposed");
        }

        public void BuildShaderProgram(ShaderProgram shaderProgram)
        {
            this.CompileShader(shaderProgram.VertexShader, ShaderType.VertexShader);
            this.CompileShader(shaderProgram.FragmentShader, ShaderType.FragmentShader);

            shaderProgram.ProgramID = GL.CreateProgram();
            GL.AttachShader(shaderProgram.ProgramID, shaderProgram.VertexShader.ShaderID);
            GL.AttachShader(shaderProgram.ProgramID, shaderProgram.FragmentShader.ShaderID);
            GL.LinkProgram(shaderProgram.ProgramID);

            GL.GetProgram(shaderProgram.ProgramID, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                GL.GetProgramInfoLog(shaderProgram.ProgramID, out string log);
                Debug.WriteLine($"Shader Program Linking Failed: {log}");
            }
            else
            {
                Debug.WriteLine($"Shader Program {shaderProgram.ProgramID} created with error {GL.GetError()}");
            }

            GL.DeleteShader(shaderProgram.VertexShader.ShaderID);
            GL.DeleteShader(shaderProgram.FragmentShader.ShaderID);
        }

        public void CompileShader(Shader.Shader shader, ShaderType type)
        {
            shader.ShaderID = GL.CreateShader(type);
            GL.ShaderSource(shader.ShaderID, shader.Source);
            GL.CompileShader(shader.ShaderID);
            Debug.WriteLine($"Compiled shader with error {this.GetError()}");
        }
        public void DisposeShaderProgram(ShaderProgram shaderProgram)
        {
            GL.DeleteProgram(shaderProgram.ProgramID);
            Debug.WriteLine($"ShaderProgram {shaderProgram.GetType().ToString()} deleted");
        }

        public void AddShaderProgram(string name, ShaderProgram shaderProgram)
        {
            _programs.Add(name, shaderProgram);
        }

        public ShaderProgram GetShaderProgram(string name)
        {
            return _programs[name];
        }

        public void AddShape(Shape shape)
        {
            _shapes.Add(shape.GetShapeName(), shape);
        }

        public void InitShape(Shape shape)
        {
            shape.VertexArray = GL.GenVertexArray();
            GL.BindVertexArray(shape.VertexArray);

            var vBufferHint = BufferUsageHint.StaticDraw;
            if(shape.DynamicVertices())
            {
                vBufferHint = BufferUsageHint.DynamicDraw;
            }

            var vertices = shape.GetVertices();
            shape.VertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, shape.VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, vBufferHint);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            if(shape.HasUvCoords())
            {
                var uvBufferHint = BufferUsageHint.StaticDraw;
                if (shape.DynamicUVCoords())
                {
                    uvBufferHint = BufferUsageHint.DynamicDraw;
                }

                var uvcoords = shape.GetUVCoords();
                shape.TextureBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, shape.TextureBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvcoords.Length * sizeof(float)), uvcoords, uvBufferHint);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            }

            if(shape.HasNormals())
            {
                var nBufferHint = BufferUsageHint.StaticDraw;
                if (shape.DynamicNormals())
                {
                    nBufferHint = BufferUsageHint.DynamicDraw;
                }

                var normals = shape.GetNormals();
                shape.NormalBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, shape.NormalBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(normals.Length * sizeof(float)), normals, nBufferHint);
                GL.EnableVertexAttribArray(2);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
            }

            if (shape.HasTangents())
            {
                var tBufferHint = BufferUsageHint.StaticDraw;
                if (shape.DynamicTangents())
                {
                    tBufferHint = BufferUsageHint.DynamicDraw;
                }

                var tangents = shape.GetTangents();
                shape.TangentBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, shape.TangentBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tangents.Length * sizeof(float)), tangents, tBufferHint);
                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 0, 0);
            }

            var indicies = shape.GetIndices();  
            shape.IndexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, shape.IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indicies.Length * sizeof(uint)), indicies, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            Debug.WriteLine($"Created shape {shape.GetShapeName()} with error {this.GetError()}");
        }

        public void DisposeShape(Shape shape)
        {
            if(shape.VertexBuffer != 0)
            {
                GL.DeleteBuffer(shape.VertexBuffer);
                shape.VertexBuffer = 0;
            }

            if (shape.TextureBuffer != 0)
            {
                GL.DeleteBuffer(shape.TextureBuffer);
                shape.TextureBuffer = 0;
            }

            if(shape.NormalBuffer != 0)
            {
                GL.DeleteBuffer(shape.NormalBuffer);
                shape.NormalBuffer = 0;
            }

            if (shape.TangentBuffer != 0)
            {
                GL.DeleteBuffer(shape.TangentBuffer);
                shape.TangentBuffer = 0;
            }

            if (shape.VertexArray != 0)
            {
                GL.DeleteVertexArray(shape.VertexArray);
                shape.VertexArray = 0;
            }

            Debug.WriteLine($"Disposed shape {shape.GetShapeName()}");
        }

        public void Dispose()
        {
            foreach (var shader in _programs)
            {
                this.DisposeShaderProgram(shader.Value);
            }

            foreach (var shape in _shapes)
            {
                this.DisposeShape(shape.Value);
            }
        }

        public void BindShaderProgram(ShaderProgram shaderProgram)
        {
            GL.UseProgram(shaderProgram.ProgramID);
            _currentProgram = shaderProgram.ProgramID;
        }

        public void UnbindShaderProgram()
        {
            GL.UseProgram(0);
            _currentProgram = 0;
        }

        public int GetUniformLocation(int program, String name)
        {
            return GL.GetUniformLocation(program, name);
        }

        public void InitializeTexture(Texture texture)
        {
            if(texture.Flags == TextureFlags.Loaded || texture.Flags == TextureFlags.Disposed)
            {
                texture.TextureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,(int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,(int)TextureMagFilter.Linear);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture.Width, texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, texture.TextureData);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                Debug.WriteLine($"Texture loaded with error {this.GetError()}");
                texture.Flags = TextureFlags.Initialized;
            }
        }

        public void DisposeTexture(Texture texture)
        {
            if (texture.Flags == TextureFlags.Initialized)
            {
                GL.DeleteTexture(texture.TextureId);
                texture.Flags = TextureFlags.Disposed;
                texture.TextureId = 0;
                Debug.WriteLine($"Disposed texture");
            }
        }

        public void DrawRenderTarget(RenderTarget renderTarget)
        {
            var shape = _shapes["FramebufferShape"];
            if (shape != null)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, renderTarget.TextureID);
                GL.Uniform1(this.GetUniformLocation(_currentProgram, "screenTexture"), 0);
                GL.BindVertexArray(shape.VertexArray);
                GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

        public void DrawRect(Math.Rect rect, Vector4 color, float borderWidth = 1.0f, float rotation = 0.0f)
        {
            var shape = _shapes["RectShape"];
            var aspect = rect.Width / rect.Height;

            var mt_mat = Matrix4.CreateTranslation(rect.X, rect.Y, 0.0f);
            var mr_mat = Matrix4.CreateRotationZ(Math.Math.ToRadians(rotation));
            var ms_mat = Matrix4.CreateScale(rect.Width, rect.Height, 0.0f);
            var m_mat = mt_mat * mr_mat * ms_mat;

            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "m_mat"), false, ref m_mat);
            GL.Uniform4(this.GetUniformLocation(_currentProgram, "vertexColor"), color);
            GL.Uniform1(this.GetUniformLocation(_currentProgram, "aspect"), aspect);
            GL.Uniform1(this.GetUniformLocation(_currentProgram, "borderWidth"), borderWidth);
            GL.Uniform1(this.GetUniformLocation(_currentProgram, "wireframe"), 1);

            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void FillRect(Math.Rect rect, Vector4 color, float rotation = 0)
        {
            var shape = _shapes["RectShape"];
            var aspect = rect.Width / rect.Height;

            var mt_mat = Matrix4.CreateTranslation(rect.X, rect.Y, 0.0f);
            var mr_mat = Matrix4.CreateRotationZ(Math.Math.ToRadians(rotation));
            var ms_mat = Matrix4.CreateScale(rect.Width, rect.Height, 0.0f);
            var m_mat = mt_mat * mr_mat * ms_mat;

            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "m_mat"), false, ref m_mat);
            GL.Uniform4(this.GetUniformLocation(_currentProgram, "vertexColor"), color);
            GL.Uniform1(this.GetUniformLocation(_currentProgram, "aspect"), aspect);
            GL.Uniform1(this.GetUniformLocation(_currentProgram, "borderWidth"), 0.0f);
            GL.Uniform1(this.GetUniformLocation(_currentProgram, "wireframe"), 0);

            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void DrawTexture(Vector3 position, Vector3 rotation, Vector3 scale, Texture texture, Vector4 color)
        {
            if(texture.Flags == TextureFlags.Initialized)
            {
                this.DrawTexture(position, rotation, scale, texture.TextureId, color);
            }
        }

        public void DrawTexture(Vector3 position, Vector3 rotation, Vector3 scale, int texture, Vector4 color)
        {
            if (!_shapes.TryGetValue("SpriteShape", out var shape) || shape == null)
            {
                throw new Exception("Shape 'SpriteShape' is missing or invalid.");
            }

            this.DrawTexture(position, rotation, scale, texture, color, shape.GetUVCoords());
        }

        public void DrawTexture(Vector3 position, Vector3 rotation, Vector3 scale, int texture, Vector4 color, float[] uvbuffer)
        {
            if (!_shapes.TryGetValue("SpriteShape", out var shape) || shape == null)
            {
                throw new Exception("Shape 'SpriteShape' is missing or invalid.");
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, shape.TextureBuffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr) 0, (IntPtr) (uvbuffer.Length * sizeof(float)), uvbuffer);
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvbuffer.Length * sizeof(float)), uvbuffer, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            var mt_mat = Matrix4.CreateTranslation(position);
            var mr_mat = Matrix4.CreateRotationX(Math.Math.ToRadians(rotation.X)) * Matrix4.CreateRotationY(Math.Math.ToRadians(rotation.Y)) * Matrix4.CreateRotationZ(Math.Math.ToRadians(rotation.Z));
            var ms_mat = Matrix4.CreateScale(scale);
            var m_mat = mt_mat * mr_mat * ms_mat;

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Uniform1(this.GetUniformLocation(_currentProgram, "textureSampler"), 0);

            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "m_mat"), false, ref m_mat);
            GL.Uniform4(this.GetUniformLocation(_currentProgram, "vertexColor"), color);

            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }


    }
}
