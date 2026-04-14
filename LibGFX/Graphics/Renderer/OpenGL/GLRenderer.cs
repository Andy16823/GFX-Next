using FreeTypeSharp;
using LibGFX.Compute;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Shader;
using LibGFX.Graphics.Shapes;
using LibGFX.Math;
using Microsoft.VisualBasic;
using OpenTK.Compute.OpenCL;
using OpenTK.Core;
using OpenTK.Graphics.Egl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static FreeTypeSharp.FT;
using static FreeTypeSharp.FT_LOAD;
using static FreeTypeSharp.FT_Render_Mode_;

namespace LibGFX.Graphics.Renderer.OpenGL
{
    public class GLRenderer : IRenderDevice
    {
        public static int Backbuffer = 0;

        private CullMode _cullMode = CullMode.Back;
        private Dictionary<string, RenderShader> _programs;
        private Dictionary<string, Shape> _shapes;
        private Dictionary<Primitives.PrimitiveType, Mesh> _primitives;
        private IGLFWGraphicsContext _context;
        private Window _window;
        private Matrix4 _viewMatrix;
        private Matrix4 _projectionMatrix;
        private int _currentProgram;
        private bool _depthTestEnabled = false;
        private bool _blendEnabled = false;
        private int _srcBlendMode = (int) BlendingFactor.Zero;
        private int _destBlendMode = (int) BlendingFactor.Zero;
        private Viewport _viewport;

        public void Init(IGLFWGraphicsContext context)
        {
            _context = context;

            // Register default shaders
            _programs = new Dictionary<string, RenderShader>();
            RegisterRenderShader("ScreenShader", new ScreenShader());
            RegisterRenderShader("RectShader", new RectShader());
            RegisterRenderShader("SpriteShader", new SpriteShader());
            RegisterRenderShader("FontShader", new FontShader());
            RegisterRenderShader("MeshShader", new MeshShader());
            RegisterRenderShader("AnimatedMeshShader", new AnimatedMeshShader());
            RegisterRenderShader("LineShader", new LineShader());
            RegisterRenderShader("EnviromentShader", new EnviromentShader());
            RegisterRenderShader("InstancedShader3D", new InstancedShader3D());
            RegisterRenderShader("ProceduralSkyShader", new ProceduralSkyShader());
            RegisterRenderShader("InstancedShader2D", new InstancedShader2D());
            RegisterRenderShader("PBRMeshShader", new PBRMeshShader());
            RegisterRenderShader("LitSpriteShader", new LitSpriteShader());
            RegisterRenderShader("ShadowMapTest", new ShadowMapTest());
            RegisterRenderShader("DepthMeshShader", new DepthMeshShader());
            RegisterRenderShader("AnimatedDepthMeshShader", new AnimatedDepthMeshShader());
            RegisterRenderShader("DepthInstancedShader3D", new DepthInstancedShader3D());
            RegisterRenderShader("SolidMeshShader", new SolidMeshShader());
            RegisterRenderShader("AABBShader", new AABBShader());
            RegisterRenderShader("InfiniteGridShader", new InfiniteGridShader());
            RegisterRenderShader("InstancedShader3DArray", new InstancedShader3DArray());
            foreach (RenderShader program in _programs.Values)
            {
                program.Init(this);
            }

            // Register default shapes
            _shapes = new Dictionary<string, Shape>();
            AddShape(new FramebufferShape());
            AddShape(new RectShape());
            AddShape(new SpriteShape());
            AddShape(new LineShape());
            AddShape(new CubeShape());
            AddShape(new CubeWireShape());
            AddShape(new PlaneShape());
            foreach (var shape in _shapes.Values)
            {
                shape.Init(this);
            }

            // Register default primitives
            _primitives = new Dictionary<Primitives.PrimitiveType, Mesh>();
            _primitives.Add(Primitives.PrimitiveType.Cube, Primitives.Cube.GetMesh());
            _primitives.Add(Primitives.PrimitiveType.Sphere, Primitives.Sphere.GetMesh());
            _primitives.Add(Primitives.PrimitiveType.Quad, Primitives.Quad.GetMesh());
            foreach(var primitive in _primitives.Values)
            {
                primitive.Init(this);
            }
        }

        public void Init(Window window)
        {
            _window = window;
            Init(window.GetContext());
        }

        public void SetContext(IGLFWGraphicsContext context)
        {
            _context = context;
        }

        public IGLFWGraphicsContext GetContext()
        {
            return _context;
        }

        public void UseVsync(bool value)
        {
            _context.SwapInterval = value ? 1 : 0;
        }

        public void SetDepthMask(bool value)
        {
            GL.DepthMask(value);
        }

        public void Clear(RenderFlags.ClearFlags clearFlags)
        {
            GL.Clear(GLMappings.ToGL(clearFlags));
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
            return (int)GL.GetError();
        }

        public bool IsDepthTestEnabled()
        {
            //return GL.IsEnabled(EnableCap.DepthTest);
            return _depthTestEnabled;
        }

        public void SetDepthTest(bool value)
        {
            if (value)
            {
                EnableDepthTest();
            }
            else
            {
                DisableDepthTest();
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

        public void EnableBlend()
        {
            GL.Enable(EnableCap.Blend);
            _blendEnabled = true;
        }

        public bool BlendEnabled()
        {
            return _blendEnabled;
        }

        public (int srcFactor, int dstFactor) GetCurrentBlendMode()
        {
            return (_srcBlendMode, _destBlendMode);
        }

        public void SetBlendMode(int srcFactor, int dstFactor)
        {
            _srcBlendMode = srcFactor;
            _destBlendMode = dstFactor;
            GL.BlendFunc((BlendingFactor)_srcBlendMode, (BlendingFactor)_destBlendMode);
        }

        public void DisableBlend()
        {
            GL.Disable(EnableCap.Blend);
            _blendEnabled = false;
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

        public Matrix4 GetViewMatrix()
        {
            return _viewMatrix;
        }

        public Matrix4 GetProjectionMatrix()
        {
            return _projectionMatrix;
        }

        public RenderTarget2D CreateRenderTarget2D(int width, int height)
        {
            var frameBufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);

            var textureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureID, 0);

            var depthAttachment = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthAttachment);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, depthAttachment);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Failed to create framebuffer for render target.");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            RenderTarget2D renderTarget = new RenderTarget2D();
            renderTarget.FramebufferId = frameBufferId;
            renderTarget.TextureId = textureID;
            renderTarget.DepthAttachmentId = depthAttachment;
            renderTarget.Width = width;
            renderTarget.Height = height;
            return renderTarget;
        }

        public MSAARenderTarget2D CreateMSAARenderTarget2D(int width, int height, int samples = 0)
        {
            // Create the Texture Framebuffer for the sampling result
            var textureFbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, textureFbo);

            var textureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureID, 0);

            // Create the main Framebuffer for rendering
            var frameBufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);
            int colorAttachment, depthAttachment;

            if(samples == 0)
            {
                colorAttachment = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, colorAttachment);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Rgba8, width, height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, colorAttachment);

                depthAttachment = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthAttachment);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, width, height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, depthAttachment);
            }
            else
            {
                colorAttachment = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, colorAttachment);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Rgba8, width, height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, colorAttachment);

                depthAttachment = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthAttachment);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, width, height);
                GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, depthAttachment);
            }

            if(GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Failed to create framebuffer for render target.");
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            MSAARenderTarget2D renderTarget = new MSAARenderTarget2D(width, height);
            renderTarget.TextureId = textureID;
            renderTarget.TextureFbo = textureFbo;
            renderTarget.FramebufferId = frameBufferId;
            renderTarget.ColorAttachmentId  = colorAttachment;
            renderTarget.DepthAttachmentId = depthAttachment;
            renderTarget.Samples = samples;
            return renderTarget;
        }

        public DepthOnlyRenderTarget CreateDepthRenderTarget2D(int width, int height)
        {
            var frameBufferId = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);

            var depthTextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, depthTextureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            float[] borderColor = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTextureID, 0);
            GL.DrawBuffer(OpenTK.Graphics.OpenGL4.DrawBufferMode.None);
            GL.ReadBuffer(OpenTK.Graphics.OpenGL4.ReadBufferMode.None);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
            {
                throw new Exception("Failed to create framebuffer for depth render target.");
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            DepthOnlyRenderTarget renderTarget = new DepthOnlyRenderTarget();
            renderTarget.FramebufferId = frameBufferId;
            renderTarget.DepthTextureId = depthTextureID;
            renderTarget.Width = width;
            renderTarget.Height = height;
            return renderTarget;
        }

        public void ResolveRenderTarget(MSAARenderTarget2D renderTarget)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, renderTarget.FramebufferId);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, renderTarget.TextureFbo);
            GL.BlitFramebuffer(0, 0, renderTarget.Width, renderTarget.Height, 0, 0, renderTarget.Width, renderTarget.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }


        public void ResizeRenderTarget(RenderTarget2D renderTarget, int width, int height)
        {
            if(renderTarget.Width == width && renderTarget.Height == height)
            {
                return;
            }

            renderTarget.Width = width;
            renderTarget.Height = height;

            // Resize the texture
            GL.BindTexture(TextureTarget.Texture2D, renderTarget.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Resize the depth attachment buffer
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderTarget.DepthAttachmentId);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, width, height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        }

        public void ResizeRenderTarget(MSAARenderTarget2D renderTarget, int width, int height)
        {
            if(renderTarget.Width == width && renderTarget.Height == height)
            {
                return;
            }

            renderTarget.Width = width;
            renderTarget.Height = height;

            // Resize the texture
            GL.BindTexture(TextureTarget.Texture2D, renderTarget.TextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            if(renderTarget.Samples == 0)
            {
                // Resize the color Attachment
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderTarget.ColorAttachmentId);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Rgba8, width, height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                // Resize the depth attachment buffer
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderTarget.DepthAttachmentId);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, width, height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            }
            else
            {
                // Resize the color Attachment
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderTarget.ColorAttachmentId);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, renderTarget.Samples, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Rgba8, width, height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

                // Resize the depth attachment buffer
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderTarget.DepthAttachmentId);
                GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, renderTarget.Samples, OpenTK.Graphics.OpenGL4.RenderbufferStorage.Depth24Stencil8, width, height);
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            }
        }

        public void ResizeRenderTarget(DepthOnlyRenderTarget renderTarget, int width, int height)
        {
            if(renderTarget.Width == width && renderTarget.Height == height)
            {
                return;
            }
            renderTarget.Width = width;
            renderTarget.Height = height;
            GL.BindTexture(TextureTarget.Texture2D, renderTarget.DepthTextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void BindRenderTarget(IRenderTarget renderTarget)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget.FramebufferId);
        }

        public void UnbindRenderTarget()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public int GetCurrentRenderTargetID()
        {
            int currentFramebuffer;
            GL.GetInteger(GetPName.FramebufferBinding, out currentFramebuffer);
            return currentFramebuffer;
        }

        public Vector2i GetRenderTargetSize(MSAARenderTarget2D renderTarget)
        {
            int width, height;
            GL.GetTextureLevelParameter(renderTarget.TextureId, 0, GetTextureParameter.TextureWidth, out width);
            GL.GetTextureLevelParameter(renderTarget.TextureId, 0, GetTextureParameter.TextureHeight, out height);
            return new Vector2i(width, height);
        }

        public byte[] GetRenderTargetData(MSAARenderTarget2D renderTarget)
        {
            var renderTargetSize = GetRenderTargetSize(renderTarget);
            return GetRenderTargetData(renderTarget, renderTargetSize.X, renderTargetSize.Y);
        }

        public byte[] GetRenderTargetData(MSAARenderTarget2D renderTarget, int width, int height)
        {
            var oldRenderTarget = GetCurrentRenderTargetID();

            byte[] data = new byte[width * height * 4];
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget.FramebufferId);
            GL.ReadPixels(0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, data);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, oldRenderTarget);

            return data;
        }

        public int GetFramebufferIndex()
        {
            int currentFramebuffer = 0;
            GL.GetInteger(GetPName.FramebufferBinding, out currentFramebuffer);
            Debug.WriteLine($"Current framebuffer binding: {currentFramebuffer}");
            return currentFramebuffer;
        }

        public void BuildRenderShader(RenderShader shaderProgram)
        {
            CompileShader(shaderProgram.VertexShader, ShaderType.VertexShader);
            CompileShader(shaderProgram.FragmentShader, ShaderType.FragmentShader);
            if (shaderProgram.GeometryShader != null)
            {
                CompileShader(shaderProgram.GeometryShader, ShaderType.GeometryShader);
            }

            shaderProgram.ProgramID = GL.CreateProgram();
            GL.AttachShader(shaderProgram.ProgramID, shaderProgram.VertexShader.ShaderID);
            GL.AttachShader(shaderProgram.ProgramID, shaderProgram.FragmentShader.ShaderID);
            if (shaderProgram.GeometryShader != null)
            {
                GL.AttachShader(shaderProgram.ProgramID, shaderProgram.GeometryShader.ShaderID);
            }
            GL.LinkProgram(shaderProgram.ProgramID);

            GL.GetProgram(shaderProgram.ProgramID, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                GL.GetProgramInfoLog(shaderProgram.ProgramID, out string log);
                Debug.WriteLine($"Shader Program Linking Failed: {log}");
                throw new Exception($"Shader Program Linking Failed: {log}");
            }
            else
            {
                Debug.WriteLine($"Shader Program {shaderProgram.ProgramID} created with error {GL.GetError()}");
            }
            GL.DeleteShader(shaderProgram.VertexShader.ShaderID);
            GL.DeleteShader(shaderProgram.FragmentShader.ShaderID);
            if (shaderProgram.GeometryShader != null)
            {
                GL.DeleteShader(shaderProgram.GeometryShader.ShaderID);
            }
        }

        public void CompileShader(Shader.Shader shader, ShaderType type)
        {
            shader.ShaderID = GL.CreateShader(type);
            GL.ShaderSource(shader.ShaderID, shader.Source);
            GL.CompileShader(shader.ShaderID);
            Debug.WriteLine($"Compiled shader with error {GetError()}");
        }
        public void DisposeRenderShader(RenderShader shaderProgram)
        {
            Debug.WriteLine($"Disposing shader program {shaderProgram.ProgramID}");
            GL.DeleteProgram(shaderProgram.ProgramID);
            shaderProgram.ProgramID = 0;
            Debug.WriteLine($"ShaderProgram {shaderProgram.GetType().ToString()} deleted");
        }

        public void RegisterRenderShader(string name, RenderShader shaderProgram)
        {
            _programs.Add(name, shaderProgram);
        }

        public bool IsRenderShaderRegistered(string name)
        {
            return _programs.ContainsKey(name);
        }

        public Mesh GetPrimitiveMesh(Primitives.PrimitiveType type)
        {
            return _primitives[type];
        }

        public RenderShader GetRenderShader(string name)
        {
            return _programs[name];
        }

        public Dictionary<String, RenderShader> GetAllRenderShaders()
        {
            return _programs;
        }

        public T GetRenderShader<T>() where T : RenderShader
        {
            foreach(var shader in _programs.Values)
            {
                if(shader is T)
                {
                    return (T)shader;
                }
            }
            throw new Exception($"RenderShader of type {typeof(T).ToString()} not found");
        }

        public void BuildComputeShader(ComputeShader shader)
        {
            int shaderId = GL.CreateShader(ShaderType.ComputeShader);
            GL.ShaderSource(shaderId, shader.ShaderSource);
            GL.CompileShader(shaderId);
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int success);
            if(success == 0)
            {
                GL.GetShaderInfoLog(shaderId, out string log);
                Debug.WriteLine($"Compute Shader Compilation Failed: {log}");
                throw new Exception($"Compute Shader Compilation Failed: {log}");
            }

            shader.ProgramID = GL.CreateProgram();
            GL.AttachShader(shader.ProgramID, shaderId);
            GL.LinkProgram(shader.ProgramID);
            GL.GetProgram(shader.ProgramID, GetProgramParameterName.LinkStatus, out success);
            if(success == 0)
            {
                GL.GetProgramInfoLog(shader.ProgramID, out string log);
                Debug.WriteLine($"Compute Shader Program Linking Failed: {log}");
                throw new Exception($"Compute Shader Program Linking Failed: {log}");
            }
            GL.DeleteShader(shaderId);
            Debug.WriteLine($"Compute Shader Program {shader.ProgramID} created with error {GetError()}");
        }

        public void DisposeComputeShader(ComputeShader shader)
        {
            Debug.WriteLine($"Disposing compute shader program {shader.ProgramID}");
            GL.DeleteProgram(shader.ProgramID);
            shader.ProgramID = 0;
            Debug.WriteLine($"Compute Shader Program deleted");
        }

        public void AddShape(Shape shape)
        {
            _shapes.Add(shape.GetShapeName(), shape);
        }

        public Shape GetShape(string name)
        {
            if (_shapes.TryGetValue(name, out var shape))
            {
                return shape;
            }
            else
            {
                throw new Exception($"Shape {name} not found");
            }
        }

        public T GetShape<T>() where T : Shape
        {
            foreach (var shape in _shapes.Values)
            {
                if (shape is T)
                {
                    return (T)shape;
                }
            }
            throw new Exception($"Shape of type {typeof(T).ToString()} not found");
        }

        public void InitShape(Shape shape)
        {
            shape.VertexArray = GL.GenVertexArray();
            GL.BindVertexArray(shape.VertexArray);

            var vBufferHint = BufferUsageHint.StaticDraw;
            if (shape.DynamicVertices())
            {
                vBufferHint = BufferUsageHint.DynamicDraw;
            }

            var vertices = shape.GetVertices();
            shape.VertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, shape.VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (nint)(vertices.Length * sizeof(float)), vertices, vBufferHint);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            if (shape.HasUvCoords())
            {
                var uvBufferHint = BufferUsageHint.StaticDraw;
                if (shape.DynamicUVCoords())
                {
                    uvBufferHint = BufferUsageHint.DynamicDraw;
                }

                var uvcoords = shape.GetUVCoords();
                shape.TextureBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, shape.TextureBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (nint)(uvcoords.Length * sizeof(float)), uvcoords, uvBufferHint);
                GL.EnableVertexAttribArray(1);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            }

            if (shape.HasNormals())
            {
                var nBufferHint = BufferUsageHint.StaticDraw;
                if (shape.DynamicNormals())
                {
                    nBufferHint = BufferUsageHint.DynamicDraw;
                }

                var normals = shape.GetNormals();
                shape.NormalBuffer = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, shape.NormalBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, (nint)(normals.Length * sizeof(float)), normals, nBufferHint);
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
                GL.BufferData(BufferTarget.ArrayBuffer, (nint)(tangents.Length * sizeof(float)), tangents, tBufferHint);
                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 0, 0);
            }

            var indicies = shape.GetIndices();
            shape.IndexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, shape.IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (nint)(indicies.Length * sizeof(uint)), indicies, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            Debug.WriteLine($"Created shape {shape.GetShapeName()} with error {GetError()}");
        }

        public void DrawShape(Shape shape)
        {
            if (shape.VertexArray != 0)
            {
                GL.BindVertexArray(shape.VertexArray);
                GL.DrawElements(BeginMode.Triangles, shape.GetIndexCount(), DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }
            else
            {
                Debug.WriteLine($"Shape {shape.GetShapeName()} is not initialized");
            }
        }

        public void DrawShape(Transform transform, Shape shape)
        {
            if(shape.VertexArray != 0)
            {
                var m_mat = transform.GetMatrix();

                GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), true, ref _projectionMatrix);
                GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), true, ref _viewMatrix);
                GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), true, ref m_mat);

                GL.BindVertexArray(shape.VertexArray);
                GL.DrawElements(PrimitiveType.Triangles, shape.GetIndexCount(), DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }
            else
            {
                Debug.WriteLine($"Shape {shape.GetShapeName()} is not initialized");
            }
        }

        public void DisposeShape(Shape shape)
        {
            if (shape.VertexBuffer != 0)
            {
                GL.DeleteBuffer(shape.VertexBuffer);
                shape.VertexBuffer = 0;
            }

            if (shape.TextureBuffer != 0)
            {
                GL.DeleteBuffer(shape.TextureBuffer);
                shape.TextureBuffer = 0;
            }

            if (shape.NormalBuffer != 0)
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
            // Dispose all shaders
            foreach (var shader in _programs)
            {
                shader.Value.Dispose(this);
            }

            // Dispose all shapes
            foreach (var shape in _shapes)
            {
                shape.Value.Dispose(this);
            }

            // Dispose all primitives
            foreach (var primitive in _primitives)
            {
                primitive.Value.Dispose(this);
            }
        }

        public void BindShaderProgram(IShaderProgram shaderProgram)
        {
            if (_currentProgram == shaderProgram.ProgramID)
            {
                return;
            }
            GL.UseProgram(shaderProgram.ProgramID);
            _currentProgram = shaderProgram.ProgramID;
        }

        public void UnbindShaderProgram()
        {
            GL.UseProgram(0);
            _currentProgram = 0;
        }

        public int GetUniformLocation(int program, string name)
        {
            return GL.GetUniformLocation(program, name);
        }

        public void LoadTexture(Texture texture)
        {
            LoadTexture(texture, TextureParameters.Default);
        }

        public void LoadTexture(Texture texture, TextureParameters textureOptions)
        {
            // Validate texture
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture), "Texture cannot be null");    
            }

            // Check if texture is already initialized
            if (texture.IsInitialized)
            {
                throw new Exception("Texture is already initialized");
            }

            // Load texture data into OpenGL
            texture.TextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, GLMappings.ToGL(textureOptions.WrapS));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, GLMappings.ToGL(textureOptions.WrapT));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, GLMappings.ToGL(textureOptions.MinFilter));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, GLMappings.ToGL(textureOptions.MagFilter));
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture.Width, texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, texture.TextureData);
            if (textureOptions.GenerateMipmaps)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
            Debug.WriteLine($"Texture loaded with error {GetError()}");
        }

        public int CreateArrayTexture(int width, int height, int layers, int mipLevels)
        {
            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, textureId);
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, mipLevels, SizedInternalFormat.Rgba8, width, height, layers);
            GL.BindTexture(TextureTarget.Texture2DArray, 0);
            return textureId;
        }

        public void SetArrayTextureData(int textureId, int layer, int level, Texture texture)
        {
            GL.BindTexture(TextureTarget.Texture2DArray, textureId);
            GL.TexSubImage3D(TextureTarget.Texture2DArray, level, 0, 0, layer, texture.Width, texture.Height, 1, PixelFormat.Rgba, PixelType.UnsignedByte, texture.TextureData);
            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        public void SetArrayTextureParameters(int textureId, TextureParameters textureParameters)
        {
            GL.BindTexture(TextureTarget.Texture2DArray, textureId);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, GLMappings.ToGL(textureParameters.WrapS));
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, GLMappings.ToGL(textureParameters.WrapT));
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, GLMappings.ToGL(textureParameters.MinFilter));
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, GLMappings.ToGL(textureParameters.MagFilter));
            GL.BindTexture(TextureTarget.Texture2DArray, 0);
        }

        public void DisposeTexture(Texture texture)
        {
            if (texture != null)
            {
                if (texture.IsInitialized)
                {
                    this.DisposeTexture(texture.TextureId);
                    texture.TextureId = 0;
                }
            }
        }

        public void DisposeTexture(int textureId)
        {
            GL.DeleteTexture(textureId);
            Debug.WriteLine($"Disposed texture with ID {textureId}");
        }

        public void LoadCubemap(Cubemap cubemap)
        {
            if(cubemap.IsInitialized)
            {
                throw new Exception("Cubemap is already initialized");
            }

            cubemap.TextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, cubemap.TextureId);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            for (int i = 0; i < 6; i++)
            {
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, cubemap.Width, cubemap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, cubemap.Faces[i]);
            }
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
            Debug.WriteLine($"Cubemap loaded with error {GetError()}");
        }

        public void DrawPrimitive(Transform tansform, Primitives.PrimitiveType type, Vector4 color)
        {
            //TODO: Currently we are using an mesh for the rendering, maybe we can optimize this later since the mesh for this dont have an material
            var shader = _programs["SolidMeshShader"];
            var mesh = _primitives[type];
            if(shader != null && mesh != null)
            {
                this.BindShaderProgram(shader);
                var m_mat = tansform.GetMatrix();
                GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), true, ref _projectionMatrix);
                GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), true, ref _viewMatrix);
                GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), true, ref m_mat);
                GL.Uniform4(GetUniformLocation(_currentProgram, "solidColor"), color);
                GL.BindVertexArray(mesh.RenderData.VertexArray);
                GL.DrawElements(BeginMode.Triangles, mesh.RenderData.IndexCount, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
                this.UnbindShaderProgram();
            }
        }

        public void DrawCubemap(Transform transform, Cubemap cubemap, Vector4 color)
        {
            var m_mat = transform.GetMatrix();

            var shape = _shapes["CubeShape"];
            GL.DepthMask(false);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), false, ref m_mat);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, cubemap.TextureId);
            GL.Uniform1(GetUniformLocation(_currentProgram, "skybox"), 0);
            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Triangles, shape.GetIndexCount(), DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DepthMask(true);
        }

        public void DrawVertexArray(Transform transform, int vertexBuffer, int vertexCount, RenderFlags.PrimitiveTypes primitiveTypes)
        {
            var m_mat = transform.GetMatrix();
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), true, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), true, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), true, ref m_mat);
            GL.BindVertexArray(vertexBuffer);
            GL.DrawElements(GLMappings.ToBeginMode(primitiveTypes), vertexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void DisposeCubemap(Cubemap cubemap)
        {
            if(!cubemap.IsInitialized)
            {
                throw new Exception("Cubemap is not initialized");
            }
            GL.DeleteTexture(cubemap.TextureId);
            cubemap.TextureId = 0;
            Debug.WriteLine($"Disposed cubemap");
        }

        public void DrawRenderTarget(RenderTarget2D renderTarget)
        {
            this.DrawRenderTarget(renderTarget.TextureId);
        }

        public void DrawRenderTarget(MSAARenderTarget2D renderTarget)
        {
            this.DrawRenderTarget(renderTarget.TextureId);
        }

        public void DrawRenderTarget(int textureId)
        {
            // Ensure Blending is enabled
            this.EnableBlend();
            this.SetBlendMode((int)BlendingFactor.SrcAlpha, (int)BlendingFactor.OneMinusSrcAlpha);

            // Draw Frambuffer
            var shape = _shapes["FramebufferShape"];
            if (shape != null)
            {
                var depthTest = IsDepthTestEnabled();
                DisableDepthTest();
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.Uniform1(GetUniformLocation(_currentProgram, "screenTexture"), 0);
                GL.BindVertexArray(shape.VertexArray);
                GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
                GL.BindTexture(TextureTarget.Texture2D, 0);
                SetDepthTest(depthTest);
            }

            // Disable blending if it was disabled
            this.DisableBlend();
        }

        public void DrawFullScreenQuad()
        {
            var shape = _shapes["FramebufferShape"];
            if (shape != null)
            {
                GL.BindVertexArray(shape.VertexArray);
                GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
            }
        }

        public void DrawRenderTarget(RenderTarget2D renderTarget, int framebuffer)
        {
            this.DrawRenderTarget(renderTarget.TextureId, framebuffer);
        }

        public void DrawRenderTarget(MSAARenderTarget2D renderTarget, int framebuffer)
        {
            this.DrawRenderTarget(renderTarget.TextureId, framebuffer);
        }

        public void DrawRenderTarget(int textureId, int framebuffer)
        {
            this.BindShaderProgram(this.GetRenderShader("ScreenShader"));
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            this.DrawRenderTarget(textureId);
            this.UnbindShaderProgram();
        }

        public void DrawLine(Vector3 start, Vector3 end, Vector4 color)
        {
            var shape = _shapes["LineShape"];
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.Uniform4(GetUniformLocation(_currentProgram, "vertexColor"), color);

            GL.BindBuffer(BufferTarget.ArrayBuffer, shape.VertexBuffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, 6 * sizeof(float), new float[] { start.X, start.Y, start.Z, end.X, end.Y, end.Z });
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Lines, 2, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void DrawRect(Rect rect, Vector4 color, float borderWidth = 1.0f, float rotation = 0.0f)
        {
            var shape = _shapes["RectShape"];
            var aspect = rect.Width / rect.Height;

            var mt_mat = Matrix4.CreateTranslation(rect.X, rect.Y, 0.0f);
            var mr_mat = Matrix4.CreateRotationZ(Math.MathUtils.ToRadians(rotation));
            var ms_mat = Matrix4.CreateScale(rect.Width, rect.Height, 0.0f);
            var m_mat = ms_mat * mr_mat * mt_mat;// mt_mat * mr_mat * ms_mat;

            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), false, ref m_mat);
            GL.Uniform4(GetUniformLocation(_currentProgram, "vertexColor"), color);
            GL.Uniform1(GetUniformLocation(_currentProgram, "aspect"), aspect);
            GL.Uniform1(GetUniformLocation(_currentProgram, "borderWidth"), borderWidth);
            GL.Uniform1(GetUniformLocation(_currentProgram, "wireframe"), 1);

            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void FillRect(Rect rect, Vector4 color, float rotation = 0)
        {
            var shape = _shapes["RectShape"];
            var aspect = rect.Width / rect.Height;

            var mt_mat = Matrix4.CreateTranslation(rect.X, rect.Y, 0.0f);
            var mr_mat = Matrix4.CreateRotationZ(Math.MathUtils.ToRadians(rotation));
            var ms_mat = Matrix4.CreateScale(rect.Width, rect.Height, 0.0f);
            var m_mat = ms_mat * mr_mat * mt_mat;// mt_mat * mr_mat * ms_mat;

            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), false, ref m_mat);
            GL.Uniform4(GetUniformLocation(_currentProgram, "vertexColor"), color);
            GL.Uniform1(GetUniformLocation(_currentProgram, "aspect"), aspect);
            GL.Uniform1(GetUniformLocation(_currentProgram, "borderWidth"), 0.0f);
            GL.Uniform1(GetUniformLocation(_currentProgram, "wireframe"), 0);

            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void DrawRect3D(Transform transform, Vector4 color, float borderWidth = 1.0f)
        {
            var shape = _shapes["RectShape"];
            var m_mat = transform.GetMatrix();
            var aspect = transform.Scale.X / transform.Scale.Y;

            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), false, ref m_mat);

            GL.Uniform4(GetUniformLocation(_currentProgram, "vertexColor"), color);
            GL.Uniform1(GetUniformLocation(_currentProgram, "aspect"), aspect);
            GL.Uniform1(GetUniformLocation(_currentProgram, "borderWidth"), borderWidth);
            GL.Uniform1(GetUniformLocation(_currentProgram, "wireframe"), 1);

            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void FillRect3D(Transform transform, Vector4 color)
        {
            var shape = _shapes["RectShape"];
            var m_mat = transform.GetMatrix();
            var aspect = transform.Scale.X / transform.Scale.Y;

            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), false, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), false, ref m_mat);

            GL.Uniform4(GetUniformLocation(_currentProgram, "vertexColor"), color);
            GL.Uniform1(GetUniformLocation(_currentProgram, "aspect"), aspect);
            GL.Uniform1(GetUniformLocation(_currentProgram, "borderWidth"), 0.0f);
            GL.Uniform1(GetUniformLocation(_currentProgram, "wireframe"), 0);

            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void DrawTexture(Transform transform, Texture texture, Vector4 color)
        {
            if (texture.IsInitialized)
            {
                DrawTexture(transform, texture.TextureId, color);
            }
        }

        public void DrawTexture(Transform transform, int texture, Vector4 color)
        {
            DrawTexture(transform, texture, color, Texture.DefaultUVTransform);
        }

        public void DrawTexture(Transform transform, int textureId, Vector4 color, Vector4 uvTransform)
        {
            DrawTexture(transform, textureId, color, uvTransform, Texture.DefaultUVScale);
        }

        public void DrawTexture(Transform transform, int textureId, Vector4 color, Vector4 uvTransform, Vector2 uvScale)
        {
            if (!_shapes.TryGetValue("SpriteShape", out var shape) || shape == null)
            {
                throw new Exception("Shape 'SpriteShape' is missing or invalid.");
            }
            var m_mat = transform.GetMatrix();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Uniform1(GetUniformLocation(_currentProgram, "textureSampler"), 0);

            GL.Uniform4(GetUniformLocation(_currentProgram, "uvTransform"), uvTransform);
            GL.Uniform2(GetUniformLocation(_currentProgram, "uvScale"), uvScale);

            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), true, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), true, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), true, ref m_mat);
            GL.Uniform4(GetUniformLocation(_currentProgram, "vertexColor"), color);

            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public Font LoadFont(string path, int fontsize = 42)
        {
            if (!Path.Exists(path))
            {
                throw new FileNotFoundException($"Font file not found: {path}");
            }

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
                    int left = face->glyph->bitmap_left;
                    int top = face->glyph->bitmap_top;
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
                        advance = (int)face->glyph->advance.x,
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
            GL.BufferData(BufferTarget.ArrayBuffer, 0, nint.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            font.GLBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, font.GLBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 0, nint.Zero, BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribIPointer(1, 1, VertexAttribIntegerType.Int, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            return font;
        }

        public void DrawString2D(string text, Vector2 position, Font font, Vector4 color, float scale = 1.0f, FontAlignment fontAlignment = FontAlignment.BottomLeft)
        {
            // Create position & scale data
            float x = position.X;
            float y = position.Y;
            var offset = font.GetAlignmentOffset(text, fontAlignment, scale);

            // Create lists for the buffers
            var vertices = new List<float>();
            var glypheTextures = new List<int>();

            // Bind the array texture and pass the font data to the shader
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, font.TextureId);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), false, ref _projectionMatrix);
            GL.Uniform4(GetUniformLocation(_currentProgram, "vertexColor"), color);

            // Build the new buffer data
            foreach (var c in text)
            {
                if (font.Characters.TryGetValue(c, out var character))
                {
                    var uv = Font.GetGlyphUV(character, font.TextureWidth, font.TextureHeight);
                    float xpos = x + character.bearing.X * scale + offset.X;
                    float ypos = y - (character.size.Y - character.bearing.Y) * scale + offset.Y;
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

                    float advance = character.advance / 64.0f * scale;
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
            if(mesh.IsInitialized)
            {
                throw new Exception("Mesh is already initialized");
            }

            // Create the vertex array object
            var vertexSize = Marshal.SizeOf<Vertex>(); // Der Abstand zwischen den Elementen der Struktur
            var vec3Size = Marshal.SizeOf<Vector3>();

            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            // Positions
            int positionBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Positions.Count * vec3Size, mesh.Positions.ToArray(), BufferUsageHint.DynamicDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            // Create the vertex buffer object
            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Vertices.Count * vertexSize, mesh.Vertices.ToArray(), BufferUsageHint.DynamicDraw);

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
                PositionBuffer = positionBuffer,
                VertexBuffer = vbo,
                IndexBuffer = ibo,
                IndexCount = mesh.Indices.Count
            };
            mesh.RenderData = renderData;
        }


        public void DrawMesh(Transform transform, Mesh mesh)
        {
            if(!mesh.IsInitialized)
            {
                throw new Exception("Mesh is not initialized");
            }

            var m_mat = mesh.GetTransform() * transform.GetMatrix();

            // Bind the shader uniforms
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), true, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), true, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), true, ref m_mat);

            // Draw the mesh    
            GL.BindVertexArray(mesh.RenderData.VertexArray);
            GL.DrawElements(BeginMode.Triangles, mesh.RenderData.IndexCount, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }

        public void DrawAABB(AABB aabb, Vector4 color)
        {
            var depthTest = IsDepthTestEnabled();
            var shape = _shapes["CubeWireShape"];
            var shader = _programs["AABBShader"];
            var m_mat = Matrix4.Identity;

            this.DisableDepthTest();
            this.BindShaderProgram(shader);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), true, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), true, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), true, ref m_mat);
            GL.Uniform3(GetUniformLocation(_currentProgram, "min"), aabb.Min);
            GL.Uniform3(GetUniformLocation(_currentProgram, "max"), aabb.Max);
            GL.Uniform4(GetUniformLocation(_currentProgram, "solidColor"), color);
            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Lines, shape.GetIndexCount(), DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            this.SetDepthTest(depthTest);
        }

        public void DrawGrid(Camera camera, Vector4 color)
        {
            float gridSize = 1.0f;
            float majorStep = 5.0f;
            float fadeStart = 40.0f;
            float fadeEnd = 200.0f;
            float lineWidthWorld = 0.02f;
            float planeY = 0f;
            float quadHalfsize = 2000f;

            var scale = Matrix4.CreateScale(quadHalfsize);
            var trans = Matrix4.Identity;
            var m_mat = scale;

            var shader = this.GetRenderShader("InfiniteGridShader");
            this.BindShaderProgram(shader);

            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), true, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), true, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "m_mat"), true, ref m_mat);

            GL.Uniform3(GetUniformLocation(_currentProgram, "u_CameraPos"), camera.Transform.Position);
            GL.Uniform1(GetUniformLocation(_currentProgram, "u_GridSize"), gridSize);
            GL.Uniform1(GetUniformLocation(_currentProgram, "u_MainStep"), System.Math.Max(1, majorStep));
            GL.Uniform1(GetUniformLocation(_currentProgram, "u_FadeStart"), fadeStart);
            GL.Uniform1(GetUniformLocation(_currentProgram, "u_FadeEnd"), fadeEnd);
            GL.Uniform1(GetUniformLocation(_currentProgram, "u_LineWidth"), lineWidthWorld);

            GL.Uniform4(GetUniformLocation(_currentProgram, "u_GridColor"), color);
            GL.Uniform4(GetUniformLocation(_currentProgram, "u_AxisColorX"), 1f, 0.3f, 0.3f, 1f);
            GL.Uniform4(GetUniformLocation(_currentProgram, "u_AxisColorZ"), 0.3f, 0.3f, 1f, 1f);

            this.EnableBlend();
            this.SetBlendMode((int)BlendingFactor.SrcAlpha, (int)BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(false);

            var shape = _shapes["PlaneShape"];
            GL.BindVertexArray(shape.VertexArray);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // restore states
            GL.DepthMask(true);
            this.DisableBlend();
            GL.UseProgram(0);
        }

        public void DisposeMesh(Mesh mesh)
        {
            if(!mesh.IsInitialized)
            {
                throw new Exception("Mesh is not initialized");
            }

            Debug.WriteLine($"Disposing Mesh {mesh.Name}");
            GL.DeleteVertexArray(mesh.RenderData.VertexArray);
            GL.DeleteBuffer(mesh.RenderData.VertexBuffer);
            GL.DeleteBuffer(mesh.RenderData.IndexBuffer);
            Debug.WriteLine($"Mesh {mesh.Name} disposed");

            mesh.RenderData = new RenderData();
        }

        public void LoadInstanceContainer(RenderInstanceContainer container)
        {
            if(container.IsInitialized)
            {
                throw new Exception("Instance container is already initialized.");
            }

            Debug.WriteLine($"Loading instance container");
            container.InstanceVAO = GL.GenVertexArray();
            container.TransformInstanceBuffer = GL.GenBuffer();
            container.ExtraInstanceBuffer = GL.GenBuffer();
            container.UVInstanceBuffer = GL.GenBuffer();
            Debug.WriteLine($"Instance container loaded");
        }

        public void BindMeshForInstance(RenderInstanceContainer container, Mesh mesh)
        {
            if (!container.IsInitialized)
            {
                throw new Exception("Instance container is not initialized.");
            }

            if (!mesh.IsInitialized)
            {
                throw new Exception("Mesh is not initialized.");
            }

            var positionSize = Marshal.SizeOf<Vector3>();
            var vertexSize = Marshal.SizeOf<Vertex>();

            GL.BindVertexArray(container.InstanceVAO);

            // Positions
            GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.RenderData.PositionBuffer);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, positionSize, nint.Zero);

            // Other Vertex Data
            GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.RenderData.VertexBuffer);            

            // Texture Coordinates (2 floats)
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf<Vertex>("TexCoord"));

            // Normals (3 floats)
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf<Vertex>("Normal"));

            // Tangents (3 floats)
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf<Vertex>("Tangent"));

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.RenderData.IndexBuffer);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            container.Mesh = mesh;
        }


        private void SetInstanceBuffers(RenderInstanceContainer container)
        {

            // Get the instance buffers
            var buffers = container.GetInstancesBuffers();
            var matrixSize = Marshal.SizeOf<Matrix4>();
            var vec4Size = Marshal.SizeOf<Vector4>();

            // Set the transform instance buffer
            int transformSize = matrixSize * buffers.Item1.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, container.TransformInstanceBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, transformSize, buffers.Item1, BufferUsageHint.DynamicDraw);

            // Set the extra instance buffer
            int extraSize = vec4Size * buffers.Item2.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, container.ExtraInstanceBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, extraSize, buffers.Item2, BufferUsageHint.DynamicDraw);

            // Set the uv instance buffer
            int uvSize = vec4Size * buffers.Item3.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, container.UVInstanceBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, uvSize, buffers.Item3, BufferUsageHint.DynamicDraw);

            // Unbind the buffers
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void LoadInstances(RenderInstanceContainer container)
        {
            if (!container.IsInitialized)
            {
                throw new Exception("Instance container is not initialized.");
            }

            SetInstanceBuffers(container);
        }

        public int AddRenderInstance(RenderInstanceContainer container, Transform transform)
        {
            var newInstanceId = container.Instances.Count;

            var newInstance = new RenderInstance();
            newInstance.Transform = transform;
            newInstance.Visible = true;
            container.Instances.Add(newInstance);

            SetInstanceBuffers(container);

            return newInstanceId;
        }

        public void UpdateInstance(RenderInstanceContainer container, int instanceIndex)
        {
            if (!container.IsInitialized)
            {
                throw new Exception("Instance container is not initialized.");
            }

            if (instanceIndex < 0 || instanceIndex >= container.Instances.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceIndex), "Instance index is out of range.");
            }

            var matrixSize = Marshal.SizeOf<Matrix4>();
            var vec4Size = Marshal.SizeOf<Vector4>();

            // Update the instance transform
            var transform = container.Instances[instanceIndex].Transform.GetMatrix();
            GL.BindBuffer(BufferTarget.ArrayBuffer, container.TransformInstanceBuffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, instanceIndex * matrixSize, matrixSize, ref transform);

            // Update the instance extras
            var extras = container.Instances[instanceIndex].GetExtras();
            GL.BindBuffer(BufferTarget.ArrayBuffer, container.ExtraInstanceBuffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, instanceIndex * vec4Size, vec4Size, ref extras);

            // Update the instance UVs
            var uvs = container.Instances[instanceIndex].UVTransform;
            GL.BindBuffer(BufferTarget.ArrayBuffer, container.UVInstanceBuffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, instanceIndex * vec4Size, vec4Size, ref uvs);

            // Unbind the buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void DrawInstances(RenderInstanceContainer container)
        {
            var meshMatrix = container.Mesh.GetTransform();

            // Bind the shader uniforms
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "p_mat"), true, ref _projectionMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "v_mat"), true, ref _viewMatrix);
            GL.UniformMatrix4(GetUniformLocation(_currentProgram, "mesh_matrix"), true, ref meshMatrix);

            // Bind the instance buffers
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, container.TransformInstanceBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, container.ExtraInstanceBuffer);
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 2, container.UVInstanceBuffer);

            // Draw the mesh    
            GL.BindVertexArray(container.InstanceVAO);
            GL.DrawElementsInstanced(PrimitiveType.Triangles, container.Mesh.RenderData.IndexCount, DrawElementsType.UnsignedInt, nint.Zero, container.Instances.Count);

            GL.BindVertexArray(0);
        }

        public void DisposeInstanceContainer(RenderInstanceContainer container)
        {
            if(!container.IsInitialized)
            {
                throw new Exception("Instance container is not initialized.");
            }

            Debug.WriteLine($"Disposing Instance Container");
            GL.DeleteVertexArray(container.InstanceVAO);
            GL.DeleteBuffer(container.TransformInstanceBuffer);
            GL.DeleteBuffer(container.ExtraInstanceBuffer);
            GL.DeleteBuffer(container.UVInstanceBuffer);
            Debug.WriteLine($"Disposed Instance Container");
        }

        public int CreateBuffer()
        {
            return GL.GenBuffer();
        }

        public int CreateVertexBuffer<T>(T[] data, bool dynamic = false) where T : unmanaged
        {
            int bufferId = GL.GenBuffer();
            SetVertexBufferData(bufferId, data, dynamic);
            return bufferId;
        }

        public int CreateElementBuffer(int[] data, bool dynamic = false)
        {
            int bufferId = GL.GenBuffer();
            SetElementBufferData(bufferId, data, dynamic);
            return bufferId;
        }

        public void BindBuffer(RenderFlags.GFXBufferTarget target, int buffer)
        {
            GL.BindBuffer(GLMappings.ToBufferTarget(target), buffer);
        }

        public void SetBufferSize(int buffer, int size, RenderFlags.GFXBufferTarget target, RenderFlags.GFXBufferUsageHint bufferUsageHint)
        {
            GL.BindBuffer(GLMappings.ToBufferTarget(target), buffer);
            GL.BufferData(GLMappings.ToBufferTarget(target), size, nint.Zero, GLMappings.ToBufferUsageHint(bufferUsageHint));
            GL.BindBuffer(GLMappings.ToBufferTarget(target), 0);
        }

        public void BindVertexBuffer(int buffer)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
        }

        public void BindElementBuffer(int buffer)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer);
        }

        public void SetBufferData<T>(int buffer, T[] data, RenderFlags.GFXBufferTarget target, RenderFlags.GFXBufferUsageHint bufferUsageHint) where T : unmanaged
        {
            int dataSize = Unsafe.SizeOf<T>();
            GL.BindBuffer(GLMappings.ToBufferTarget(target), buffer);
            GL.BufferData(GLMappings.ToBufferTarget(target), data.Length * dataSize, data, GLMappings.ToBufferUsageHint(bufferUsageHint));
            GL.BindBuffer(GLMappings.ToBufferTarget(target), 0);
        }

        public void SetVertexBufferData<T>(int buffer, T[] data, bool dynamic = false) where T : unmanaged
        {
            int dataSize = Unsafe.SizeOf<T>();
            var bufferUsageHint = dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw;
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * dataSize, data, bufferUsageHint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void SetElementBufferData(int buffer, int[] data, bool dynamic = false)
        {
            int dataSize = sizeof(int);
            var bufferUsageHint = dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw;
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * dataSize, data, bufferUsageHint);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void UpdateBufferData<T>(int buffer, T[] data, int offset, RenderFlags.GFXBufferTarget target) where T : unmanaged
        {
            int dataSize = Unsafe.SizeOf<T>();
            GL.BindBuffer(GLMappings.ToBufferTarget(target), buffer);
            GL.BufferSubData(GLMappings.ToBufferTarget(target), offset * dataSize, data.Length * dataSize, data);
            GL.BindBuffer(GLMappings.ToBufferTarget(target), 0);
        }

        public void UpdateVertexBufferData<T>(int buffer, T[] data, int offset) where T : unmanaged
        {
            int dataSize = Unsafe.SizeOf<T>();
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, offset * dataSize, data.Length * dataSize, data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void UpdateElementBufferData(int buffer, int[] data, int offset)
        {
            int dataSize = sizeof(int);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, buffer);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, offset * dataSize, data.Length * dataSize, data);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public T[] GetBufferData<T>(int buffer, int length, RenderFlags.GFXBufferTarget target) where T : unmanaged
        {
            int dataSize = Unsafe.SizeOf<T>();
            T[] data = new T[length];
            GL.BindBuffer(GLMappings.ToBufferTarget(target), buffer);
            GL.GetBufferSubData(GLMappings.ToBufferTarget(target), IntPtr.Zero, length * dataSize, data);
            GL.BindBuffer(GLMappings.ToBufferTarget(target), 0);
            return data;
        }

        public void DisposeBuffer(int buffer)
        {
            GL.DeleteBuffer(buffer);
        }

        public void BindShaderStorageBuffer(int binding, int buffer)
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, binding, buffer);
        }

        public void UnbindShaderStorageBuffer(int binding)
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, binding, 0);
        }

        public void BindBufferBase(RenderFlags.GFXBufferTarget target, int binding, int buffer)
        {
            GL.BindBufferBase(GLMappings.ToBufferRangeTarget(target), binding, buffer);
        }

        public void UnbindBufferBase(RenderFlags.GFXBufferTarget target, int binding)
        {
            GL.BindBufferBase(GLMappings.ToBufferRangeTarget(target), binding, 0);
        }

        public int CreateVertexArray()
        {
            int bufferId = GL.GenVertexArray();
            return bufferId;
        }

        public void DisposeVertexArray(int value)
        {
            GL.DeleteVertexArray(value);
        }

        public void BindVertexArray(int value)
        {
            GL.BindVertexArray(value);
        }

        public void EnableVertexArrayAttribute(int index)
        {
            GL.EnableVertexAttribArray(index);
        }

        public void SetVertexArrayAttribute(int index, int size, RenderFlags.RenderDataTypes type, bool normalized, int stride, nint pointer)
        {
            GL.VertexAttribPointer(index, size, GLMappings.GetVertexAttribPointerType(type), normalized, stride, pointer);
        }

        public void DispatchCompute(int numGroupsX, int numGroupsY, int numGroupsZ)
        {
            GL.DispatchCompute(numGroupsX, numGroupsY, numGroupsZ);
        }

        public void MemoryBarrier(MemoryBarrierFlags barriers)
        {
            GL.MemoryBarrier(barriers);
        }

        public IntPtr MapBufferRange(RenderFlags.GFXBufferTarget target, int offset, int length, MapBufferAccessMask access)
        {
            return GL.MapBufferRange(GLMappings.ToBufferTarget(target), offset, length, access);
        }

        public void UnmapBuffer(RenderFlags.GFXBufferTarget target)
        {
            GL.UnmapBuffer(GLMappings.ToBufferTarget(target));
        }

        public void PrepareShader(string location, bool value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.Uniform1(locationId, value ? 1 : 0);
        }

        public void PrepareShader(string location, float value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.Uniform1(locationId, value);
        }

        public void PrepareShader(string location, int value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.Uniform1(locationId, value);
        }

        public void PrepareShader(string location, Vector2 value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.Uniform2(locationId, value);
        }

        public void PrepareShader(string location, Vector3 value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.Uniform3(locationId, value);
        }

        public void PrepareShader(string location, Vector4 value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.Uniform4(locationId, value);
        }

        public void PrepareShader(string location, bool transpose, Matrix4 value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.UniformMatrix4(locationId, transpose, ref value);
        }

        public void PrepareShader(string location, int count, float[] value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.Uniform1(locationId, count, value);
        }

        public void PrepareShaderVec2Array(string location, int count, float[] value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.Uniform2(locationId, count, value);
        }

        public void PrepareShaderVec3Array(string location, int count, float[] value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.Uniform3(locationId, count, value);
        }

        public void PrepareShaderVec4Array(string location, int count, float[] value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.Uniform4(locationId, count, value);
        }

        public void PrepareShader(string uniformName, bool transpose, Matrix4[] matrices)
        {
            var locationId = GetUniformLocation(_currentProgram, uniformName);
            GL.ProgramUniformMatrix4(_currentProgram, locationId, matrices.Length, transpose, ref matrices[0].Row0.X);
        }

        public void PrepareShader(string location, TextureUnit textureUnit, int value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, value);
            GL.Uniform1(locationId, textureUnit - TextureUnit.Texture0);
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public void PrepareShader(string location, TextureUnit textureUnit, Texture texture)
        {
            PrepareShader(location, textureUnit, texture.TextureId);
        }

        public void PrepareShader(string location, int textureUnit, int texture)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            var unit = TextureUnit.Texture0 + textureUnit;
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.Uniform1(locationId, textureUnit);
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public void PrepareShaderArrayTexture(String location, int textureUnit, int value)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            var unit = TextureUnit.Texture0 + textureUnit;
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2DArray, value);
            GL.Uniform1(locationId, textureUnit);
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public void PrepareShader(string location, int textureUnit, Texture texture)
        {
            PrepareShader(location, textureUnit, texture.TextureId);
        }

        public void PrepareShader(String location, int textureUnit, Cubemap cubemap)
        {
            var locationId = GetUniformLocation(_currentProgram, location);
            var unit = TextureUnit.Texture0 + textureUnit;
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, cubemap.TextureId);
            GL.Uniform1(locationId, textureUnit);
            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public void DeleteFramebuffer(int framebuffer)
        {
            GL.DeleteFramebuffer(framebuffer);
        }

        public void DeleteTexture(int texture)
        {
            GL.DeleteTexture(texture);
        }

        public void DeleteRenderbuffer(int renderbuffer)
        {
            GL.DeleteRenderbuffer(renderbuffer);
        }

        public CullMode GetCullMode()
        {
            return _cullMode;
        }

        public void SetCullMode(CullMode mode)
        {
            _cullMode = mode;
            switch (mode)
            {
                case CullMode.Front:
                    GL.CullFace(TriangleFace.Front);
                    break;
                case CullMode.Back:
                    GL.CullFace(TriangleFace.Back);
                    break;
                case CullMode.FrontAndBack:
                    GL.CullFace(TriangleFace.FrontAndBack);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), "Invalid cull mode.");
            }
        }
    }
}
