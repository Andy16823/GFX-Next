using FreeTypeSharp;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics.Shader;
using LibGFX.Graphics.Shapes;
using LibGFX.Math;
using Microsoft.VisualBasic;
using OpenTK.Compute.OpenCL;
using OpenTK.Core;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static FreeTypeSharp.FT;
using static FreeTypeSharp.FT_LOAD;
using static FreeTypeSharp.FT_Render_Mode_;

namespace LibGFX.Graphics
{
    public class GLRenderer : IRenderDevice
    {
        private Dictionary<String, ShaderProgram> _programs;
        private Dictionary<String, Shape> _shapes;
        private Dictionary<String, Light> _lights;
        private IGLFWGraphicsContext _context;
        private Window _window;
        private Matrix4 _viewMatrix;
        private Matrix4 _projectionMatrix;
        private int _currentProgram;
        private bool _depthTestEnabled = false;
        private Viewport _viewport;

        public void Init(Window window)
        {
            _window = window;
            _context = _window.GetContext();

            _programs = new Dictionary<String, ShaderProgram>();
            this.AddShaderProgram("ScreenShader", new ScreenShader());
            this.AddShaderProgram("RectShader", new RectShader());
            this.AddShaderProgram("SpriteShader", new SpriteShader());
            this.AddShaderProgram("FontShader", new FontShader());
            this.AddShaderProgram("MeshShader", new MeshShader());  
            this.AddShaderProgram("AnimatedMeshShader", new AnimatedMeshShader());
            this.AddShaderProgram("LineShader", new LineShader());
            foreach (ShaderProgram program in _programs.Values)
            {
                this.BuildShaderProgram(program);
            }

            _shapes = new Dictionary<String, Shape>();
            this.AddShape(new FramebufferShape());
            this.AddShape(new RectShape());
            this.AddShape(new SpriteShape());
            this.AddShape(new LineShape());
            foreach (var shape in _shapes.Values)
            {
                this.InitShape(shape);
            }

            _lights = new Dictionary<String, Light>();

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

        public bool IsDepthTestEnabled()
        {
            //return GL.IsEnabled(EnableCap.DepthTest);
            return _depthTestEnabled;
        }

        public void SetDepthTest(bool value)
        {
            if(value)
            {
                this.EnableDepthTest();
            }
            else
            {
                this.DisableDepthTest();
            }
        }

        public void EnableDepthTest()
        {
            _depthTestEnabled = true;
            GL.Enable(EnableCap.DepthTest);
        }

        public void DisableDepthTest()
        {
            _depthTestEnabled = false;
            GL.Disable(EnableCap.DepthTest);
        }

        public void EnableAlphaBlend()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public void EnableAdditiveBlend()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
        }

        public void EnableMultiplicativeBlend()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.DstColor, BlendingFactor.Zero);
        }

        public void EnableScreenBlend()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.OneMinusDstColor, BlendingFactor.One);
        }

        public (int srcFactor, int dstFactor) GetCurrentBlendMode()
        {
            int src, dst;
            GL.GetInteger(GetPName.BlendSrc, out src);
            GL.GetInteger(GetPName.BlendDst, out dst);

            string srcName = ((BlendingFactor)src).ToString();
            string dstName = ((BlendingFactor)dst).ToString();

            Debug.WriteLine($"Current Blend Mode: {srcName}, {dstName}");
            return (src, dst);
        }

        public void SetBlendMode(int srcFactor, int dstFactor)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc((BlendingFactor) srcFactor, (BlendingFactor) dstFactor);
        }

        public void ResetBlendMode()
        {
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.Zero);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
        }

        public void DisableBlend()
        {
            GL.Disable(EnableCap.Blend);
        }

        public void SetViewport(Viewport viewport)
        {
            _viewport = viewport;
            GL.Viewport(0, 0, viewport.Width, viewport.Height);
        }

        public Viewport GetViewport()
        {
            return _viewport;
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

        public void LoadMaterial(Material material)
        {
            if(material.Flags == MaterialFlags.None)
            {
                if(material.BaseColor != null)
                {
                    this.LoadTexture(material.BaseColor);
                }

                if (material.Normal != null)
                {
                    this.LoadTexture(material.Normal);
                }

                if (material.Metallic != null)
                {
                    this.LoadTexture(material.Metallic);
                }

                if (material.Roughness != null)
                {
                    this.LoadTexture(material.Roughness);
                }

                if (material.AmbientOcclusion != null)
                {
                    this.LoadTexture(material.AmbientOcclusion);
                }

                if (material.Emissive != null)
                {
                    this.LoadTexture(material.Emissive);
                }

                if (material.Height != null)
                {
                    this.LoadTexture(material.Height);
                }

                material.Flags = MaterialFlags.Loaded;
            }
        }

        public void DisposeMaterial(Material material)
        {
            Debug.WriteLine($"Disposing material {material.Name}");
            if (material.Flags == MaterialFlags.Loaded)
            {
                if (material.BaseColor != null)
                {
                    this.DisposeTexture(material.BaseColor);
                }
                if (material.Normal != null)
                {
                    this.DisposeTexture(material.Normal);
                }
                if (material.Metallic != null)
                {
                    this.DisposeTexture(material.Metallic);
                }
                if (material.Roughness != null)
                {
                    this.DisposeTexture(material.Roughness);
                }
                if (material.AmbientOcclusion != null)
                {
                    this.DisposeTexture(material.AmbientOcclusion);
                }
                if (material.Emissive != null)
                {
                    this.DisposeTexture(material.Emissive);
                }
                if (material.Height != null)
                {
                    this.DisposeTexture(material.Height);
                }
                material.Flags = MaterialFlags.Disposed;
            }
            Debug.WriteLine($"Disposed material {material.Name}");
        }

        public void LoadTexture(Texture texture)
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
                var depthTest = this.IsDepthTestEnabled();
                this.DisableDepthTest();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, renderTarget.TextureID);
                GL.Uniform1(this.GetUniformLocation(_currentProgram, "screenTexture"), 0);
                GL.BindVertexArray(shape.VertexArray);
                GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                this.SetDepthTest(depthTest);
            }
        }

        public void DrawLine(Vector3 start, Vector3 end, Vector4 color)
        {
            var shape = _shapes["LineShape"];
            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.Uniform4(this.GetUniformLocation(_currentProgram, "vertexColor"), color);

            GL.BindBuffer(BufferTarget.ArrayBuffer, shape.VertexBuffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, (6 * sizeof(float)), new float[] { start.X, start.Y, start.Z, end.X, end.Y, end.Z });
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Lines, 2, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
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

        public void DrawTexture(Transform transform, Texture texture, Vector4 color)
        {
            if(texture.Flags == TextureFlags.Initialized)
            {
                this.DrawTexture(transform, texture.TextureId, color);
            }
        }

        public void DrawTexture(Transform transform, int texture, Vector4 color)
        {
            if (!_shapes.TryGetValue("SpriteShape", out var shape) || shape == null)
            {
                throw new Exception("Shape 'SpriteShape' is missing or invalid.");
            }

            this.DrawTexture(transform, texture, color, shape.GetUVCoords());
        }

        public void DrawTexture(Transform transform, int texture, Vector4 color, float[] uvbuffer)
        {
            if (!_shapes.TryGetValue("SpriteShape", out var shape) || shape == null)
            {
                throw new Exception("Shape 'SpriteShape' is missing or invalid.");
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, shape.TextureBuffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr) 0, (IntPtr) (uvbuffer.Length * sizeof(float)), uvbuffer);
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(uvbuffer.Length * sizeof(float)), uvbuffer, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            //var mt_mat = Matrix4.CreateTranslation(position);
            //var mr_mat = Matrix4.CreateRotationX(Math.Math.ToRadians(rotation.X)) * Matrix4.CreateRotationY(Math.Math.ToRadians(rotation.Y)) * Matrix4.CreateRotationZ(Math.Math.ToRadians(rotation.Z));
            //var ms_mat = Matrix4.CreateScale(scale);
            var m_mat = transform.GetMatrix();

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

        public Font LoadFont(String path, int fontsize = 42)
        {
            Font font = new Font();
            int cellWidth = fontsize * 2;
            int cellHeight = fontsize * 2;
            int numGlyphes = 128;
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            unsafe
            {
                FT_LibraryRec_* lib;
                FT_FaceRec_* face;
                var error = FT_Init_FreeType(&lib);

                error = FT_New_Face(lib, (byte*)Marshal.StringToHGlobalAnsi(path), 0, &face);
                error = FT_Set_Char_Size(face, 0, 16 * fontsize, 300, 300);

                int arrayTextureId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2DArray, arrayTextureId);
                GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.R8, cellWidth, cellHeight, numGlyphes);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                for (int i = 0; i < numGlyphes; i++)
                {
                    char c = (char)i;
                    var glyphIndex = FT_Get_Char_Index(face, c);

                    if (FT_Load_Glyph(face, glyphIndex, FT_LOAD_DEFAULT) != FT_Error.FT_Err_Ok)
                    {
                        Debug.WriteLine($"Error while loading glype for char \"{c}\"");
                        continue;
                    }

                    if (FT_Render_Glyph(face->glyph, FT_RENDER_MODE_NORMAL) != FT_Error.FT_Err_Ok)
                    {
                        Debug.WriteLine($"Error while render glype for char \"{c}\"");
                        continue;
                    }

                    int width = (int)face->glyph->bitmap.width;
                    int height = (int)face->glyph->bitmap.rows;
                    int left = (int)face->glyph->bitmap_left;
                    int top = (int)face->glyph->bitmap_top;
                    int paddingX = width - cellWidth;
                    int paddingY = height - cellHeight;

                    byte[] cellBuffer = new byte[cellWidth * cellHeight];
                    for (int row = 0; row < height; row++)
                    {
                        for (int col = 0; col < width; col++)
                        {
                            int destX = col;
                            int destY = row;
                            if (destX < cellWidth && destY < cellHeight)
                            {
                                cellBuffer[destY * cellWidth + destX] = face->glyph->bitmap.buffer[row * face->glyph->bitmap.pitch + col];
                            }
                        }
                    }
                    GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, cellWidth, cellHeight, 1, PixelFormat.Red, PixelType.UnsignedByte, cellBuffer);

                    var gfxChar = new Character()
                    {
                        textureId = i,
                        size = new Vector2(width, height),
                        bearing = new Vector2(left, top),
                        advance = (Int32)face->glyph->advance.x,
                        padding = new Vector2(paddingX, paddingY)
                    };

                    font.Characters.Add(c, gfxChar);
                    Debug.WriteLine($"Loaded char {c}");
                }

                font.TextureId = arrayTextureId;
                font.TextureWidth = cellWidth;
                font.TextureHeight = cellHeight;

                GL.BindTexture(TextureTarget.Texture2DArray, 0);
                FT_Done_Face(face);
                FT_Done_FreeType(lib);
            }

            font.VAO = GL.GenVertexArray();
            GL.BindVertexArray(font.VAO);

            font.VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, font.VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            font.GLBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, font.GLBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribIPointer(1, 1, VertexAttribIntegerType.Int, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            return font;
        }

        public void DrawString2D(String text, Vector2 position, Font font, Vector4 color, float scale = 1.0f)
        {
            // Create position & scale data
            float x = position.X;
            float y = position.Y;

            // Create lists for the buffers
            var vertices = new List<float>();
            var glypheTextures = new List<int>();

            // Bind the array texture and pass the font data to the shader
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, font.TextureId);
            GL.UniformMatrix4(this.GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.Uniform4(this.GetUniformLocation(_currentProgram, "vertexColor"), color);

            // Build the new buffer data
            foreach (var c in text)
            {
                if(font.Characters.TryGetValue(c, out var character))
                {
                    var uv = Font.GetGlyphUV(character, font.TextureWidth, font.TextureHeight);
                    float xpos = x + character.bearing.X * scale;
                    float ypos = y - (character.size.Y - character.bearing.Y) * scale;
                    float w = character.size.X * scale;
                    float h = character.size.Y * scale;

                    float[] vertexdata = {
                        xpos,     ypos + h,   uv.u0, uv.v0, //0.0f, 0.0f,
                        xpos,     ypos,       uv.u0, uv.v1, //0.0f, 1.0f,
                        xpos + w, ypos,       uv.u1, uv.v1, //1.0f, 1.0f,

                        xpos,     ypos + h,   uv.u0, uv.v0, //0.0f, 0.0f,
                        xpos + w, ypos,       uv.u1, uv.v1, //1.0f, 1.0f,
                        xpos + w, ypos + h,   uv.u1, uv.v0, //1.0f, 0.0f
                    };

                    int[] glyphelayerdata = Enumerable.Repeat(character.textureId, 6).ToArray();

                    vertices.AddRange(vertexdata);
                    glypheTextures.AddRange(glyphelayerdata);

                    float advance = (character.advance / 64.0f) * scale;
                    x += advance;
                }
            }

            // Pass the new buffer data
            var verticesArr = vertices.ToArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, font.VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesArr.Length * sizeof(float), verticesArr, BufferUsageHint.DynamicDraw);

            var layerIds = glypheTextures.ToArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, font.GLBO);
            GL.BufferData(BufferTarget.ArrayBuffer, layerIds.Length * sizeof(int), layerIds, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Render the buffers
            int vertexCount = verticesArr.Length / 4;
            GL.BindVertexArray(font.VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);

            // Reset to default values
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        public void DisposeFont(Font font)
        {
            Debug.WriteLine($"Disposing Font");
            GL.DeleteVertexArray(font.VAO);
            GL.DeleteBuffer(font.VBO);
            GL.DeleteBuffer(font.GLBO);
            GL.DeleteTexture(font.TextureId);
            Debug.WriteLine("Font disposed");
        }

        public void LoadMesh(Mesh mesh)
        {
            // Create the vertex array object
            var vertexSize = Marshal.SizeOf<Vertex>(); // Der Abstand zwischen den Elementen der Struktur
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            // Create the vertex buffer object
            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Vertices.Count * vertexSize, mesh.Vertices.ToArray(), BufferUsageHint.DynamicDraw);

            // Position (3 floats)
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexSize, IntPtr.Zero);

            // Texture Coordinates (2 floats)
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf<Vertex>("TexCoord"));

            // Normals (3 floats)
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf<Vertex>("Normal"));

            // Tangents (3 floats)
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf<Vertex>("Tangent"));

            // BoneIDs (4 integers, use VertexAttribIPointer for integer attributes)
            GL.EnableVertexAttribArray(4);
            GL.VertexAttribIPointer(4, 4, VertexAttribIntegerType.Int, vertexSize, Marshal.OffsetOf<Vertex>("BoneIDs"));

            // BoneWeights (4 floats)
            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf<Vertex>("BoneWeights"));

            // Create the index buffer object
            int ibo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.Indices.Count * sizeof(uint), mesh.Indices.ToArray(), BufferUsageHint.DynamicDraw);

            // Unbind the buffers
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Create the render data object
            var renderData = new RenderData
            {
                VertexArray = vao,
                VertexBuffer = vbo,
                TextureBuffer = 0,
                NormalBuffer = 0,
                TangentBuffer = 0,
                IndexBuffer = ibo
            };
            mesh.RenderData = renderData;
        }


        public void DrawMesh(Transform transform, Mesh mesh)
        {
            // Create the model matrix
            //var mt_mat = Matrix4.CreateTranslation(position);
            //var mr_mat = Matrix4.CreateRotationX(Math.Math.ToRadians(rotation.X)) * Matrix4.CreateRotationY(Math.Math.ToRadians(rotation.Y)) * Matrix4.CreateRotationZ(Math.Math.ToRadians(rotation.Z));
            //var ms_mat = Matrix4.CreateScale(scale);
            var m_mat = transform.GetMatrix();

            // Bind the shader uniforms
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), false, ref m_mat);
            GL.Uniform4(GetUniformLocation(_currentProgram, "vertexColor"), mesh.Material.DiffuseColor);

            // Bind the BaseColor texture
            GL.ActiveTexture(TextureUnit.Texture0);
            if (mesh.Material.BaseColor != null && mesh.Material.BaseColor.Flags == TextureFlags.Initialized)
            {
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.BaseColor.TextureId);
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            GL.Uniform1(GetUniformLocation(_currentProgram, "textureSampler"), 0);

            // Bind the Normal texture
            GL.ActiveTexture(TextureUnit.Texture1);
            if (mesh.Material.Normal != null  && mesh.Material.Normal.Flags == TextureFlags.Initialized)
            {
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.Normal.TextureId);
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            GL.Uniform1(GetUniformLocation(_currentProgram, "normalSampler"), 1);

            // Reset the active texture unit
            GL.ActiveTexture(TextureUnit.Texture0);

            // Draw the mesh    
            GL.BindVertexArray(mesh.RenderData.VertexArray);
            GL.DrawElements(BeginMode.Triangles, mesh.Indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void DisposeMesh(Mesh mesh)
        {
            Debug.WriteLine($"Disposing Mesh {mesh.Name}");
            GL.DeleteVertexArray(mesh.RenderData.VertexArray);
            GL.DeleteBuffer(mesh.RenderData.VertexBuffer);
            GL.DeleteBuffer(mesh.RenderData.TextureBuffer);
            GL.DeleteBuffer(mesh.RenderData.NormalBuffer);
            GL.DeleteBuffer(mesh.RenderData.TangentBuffer);
            GL.DeleteBuffer(mesh.RenderData.IndexBuffer);
            Debug.WriteLine($"Mesh {mesh.Name} disposed");
        }

        public void AddLightSource(string name, Light light)
        {
            _lights.Add(name, light);
        }

        public void RemoveLightSource(string name)
        {
            _lights.Remove(name);
        }

        public IEnumerable<Light> GetAllLightSources()
        {
            return _lights.Values;
        }

        public T GetLightSource<T>() where T : Light
        {
            return _lights.Values.OfType<T>().FirstOrDefault();
        }

        public void PrepareShader(string location, bool value)
        {
            var locationId = this.GetUniformLocation(_currentProgram, location);
            GL.Uniform1(locationId, value ? 1 : 0);
        }

        public void PrepareShader(string location, float value)
        {
            var locationId = this.GetUniformLocation(_currentProgram, location);
            GL.Uniform1(locationId, value);
        }

        public void PrepareShader(string location, int value)
        {
            var locationId = this.GetUniformLocation(_currentProgram, location);
            GL.Uniform1(locationId, value);
        }

        public void PrepareShader(string location, Vector2 value)
        {
            var locationId = this.GetUniformLocation(_currentProgram, location);
            GL.Uniform2(locationId, value);
        }

        public void PrepareShader(string location, Vector3 value)
        {
            var locationId = this.GetUniformLocation(_currentProgram, location);
            GL.Uniform3(locationId, value);
        }

        public void PrepareShader(string location, Vector4 value)
        {
            var locationId = this.GetUniformLocation(_currentProgram, location);
            GL.Uniform4(locationId, value);
        }

        public void PrepareShader(string location, bool transpose, Matrix4 value)
        {
            var locationId = this.GetUniformLocation(_currentProgram, location);
            GL.UniformMatrix4(locationId, transpose, ref value);
        }

        public void PrepareShader(String location, int count, float[] value)
        {
            var locationId = this.GetUniformLocation(_currentProgram, location);
            GL.UniformMatrix4(locationId, count, false, value);
        }

        public void PrepareShader(String uniformName, bool transpose, Matrix4[] matrices)
        {
            var locationId = this.GetUniformLocation(_currentProgram, uniformName);
            GL.ProgramUniformMatrix4(_currentProgram, locationId, matrices.Length, transpose, ref matrices[0].Row0.X);
        }

        public void PrepareShader(String location, TextureUnit textureUnit, Texture texture)
        {
            var locationId = this.GetUniformLocation(_currentProgram, location);
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);
            GL.Uniform1(locationId, textureUnit - TextureUnit.Texture0);
            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}
