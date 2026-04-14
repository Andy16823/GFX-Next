using LibGFX.Compute;
using LibGFX.Core;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Shader;
using LibGFX.Graphics.Shapes;
using LibGFX.Math;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rect = LibGFX.Math.Rect;

namespace LibGFX.Graphics
{
    public enum CullMode
    {
        Front,
        Back,
        None,
        FrontAndBack
    }

    public interface IRenderDevice
    {
        void Init(IGLFWGraphicsContext context);
        void SetContext(IGLFWGraphicsContext context);
        IGLFWGraphicsContext GetContext();
        void Init(Window window);
        void Dispose();
        void MakeCurrent();
        void SwapBuffers();
        void UseVsync(bool value);
        void SetDepthMask(bool value);
        bool IsDepthTestEnabled();
        void SetDepthTest(bool value);
        void EnableDepthTest();
        void DisableDepthTest();
        void EnableBlend();
        bool BlendEnabled();
        (int srcFactor, int dstFactor) GetCurrentBlendMode();
        void SetBlendMode(int srcFactor, int dstFactor);
        void DisableBlend();
        CullMode GetCullMode();
        void SetCullMode(CullMode mode);
        void SetViewport(Viewport viewport);
        Viewport GetViewport();
        void SetViewMatrix(Matrix4 matrix);
        void SetProjectionMatrix(Matrix4 matrix);
        Matrix4 GetViewMatrix();
        Matrix4 GetProjectionMatrix();
        void Clear(RenderFlags.ClearFlags clearFlags);
        void ClearColor(float r, float g, float b, float a);
        void Flush();
        RenderTarget2D CreateRenderTarget2D(int width, int height);
        MSAARenderTarget2D CreateMSAARenderTarget2D(int width, int height, int samples = 0);
        DepthOnlyRenderTarget CreateDepthRenderTarget2D(int width, int height);
        void ResolveRenderTarget(MSAARenderTarget2D renderTarget);
        void ResizeRenderTarget(RenderTarget2D renderTarget, int width, int height);
        void ResizeRenderTarget(MSAARenderTarget2D renderTarget, int width, int height);
        void ResizeRenderTarget(DepthOnlyRenderTarget renderTarget, int width, int height);
        void BindRenderTarget(IRenderTarget renderTarget); 
        void UnbindRenderTarget();
        int GetCurrentRenderTargetID();
        Vector2i GetRenderTargetSize(MSAARenderTarget2D renderTarget);
        byte[] GetRenderTargetData(MSAARenderTarget2D renderTarget);
        byte[] GetRenderTargetData(MSAARenderTarget2D renderTarget, int width, int height);
        int GetFramebufferIndex();
        void BuildRenderShader(RenderShader shaderProgram);
        void DisposeRenderShader(RenderShader shaderProgram);
        void RegisterRenderShader(String name, RenderShader shaderProgram);
        void BuildComputeShader(ComputeShader computeShader);
        void DisposeComputeShader(ComputeShader computeShader);
        void AddShape(Shape shape);
        Shape GetShape(String name);
        T GetShape<T>() where T : Shape;
        void InitShape(Shape shape);
        void DrawShape(Shape shape);
        void DrawShape(Transform transform, Shape shape);
        void DisposeShape(Shape shape);
        bool IsRenderShaderRegistered(String name);
        Mesh GetPrimitiveMesh(Primitives.PrimitiveType type);
        RenderShader GetRenderShader(String name);
        Dictionary<String, RenderShader> GetAllRenderShaders();
        T GetRenderShader<T>() where T : RenderShader;
        void BindShaderProgram(IShaderProgram shaderProgram);
        void UnbindShaderProgram();
        int GetUniformLocation(int program, String name);
        void LoadTexture(Texture texture);
        void LoadTexture(Texture texture, TextureParameters textureOptions);
        int CreateArrayTexture(int width, int height, int layers, int mipLevels);
        void SetArrayTextureData(int textureId, int layer, int level, Texture texture);
        void SetArrayTextureParameters(int textureId, TextureParameters textureParameters);
        void LoadCubemap(Cubemap cubemap);
        void DisposeTexture(Texture texture);
        void DisposeTexture(int textureId);
        void DisposeCubemap(Cubemap cubemap);
        void DrawRenderTarget(RenderTarget2D renderTarget);
        void DrawRenderTarget(MSAARenderTarget2D renderTarget);
        void DrawRenderTarget(int textureId);
        void DrawFullScreenQuad();
        void DrawRenderTarget(RenderTarget2D renderTarget, int framebuffer);
        void DrawRenderTarget(MSAARenderTarget2D renderTarget, int framebuffer);
        void DrawRenderTarget(int textureId, int framebuffer);
        void DrawLine(Vector3 start, Vector3 end, Vector4 color);
        void DrawRect(Math.Rect rect, Vector4 color, float borderWidth = 1.0f, float rotation = 0.0f);
        void FillRect(Math.Rect rect, Vector4 color, float rotation = 0.0f);
        void DrawRect3D(Transform transform, Vector4 color, float borderWidth = 1.0f);
        void FillRect3D(Transform transform, Vector4 color);
        void DrawTexture(Transform transform, Texture texture, Vector4 color);
        void DrawTexture(Transform transform, int textureId, Vector4 color);
        void DrawTexture(Transform transform, int textureId, Vector4 color, Vector4 uvTransform);
        void DrawTexture(Transform transform, int textureId, Vector4 color, Vector4 uvTransform, Vector2 uvScale);
        void DrawPrimitive(Transform tansform, Primitives.PrimitiveType type, Vector4 color);
        void DrawCubemap(Transform transform, Cubemap cubemap, Vector4 color);
        void DrawVertexArray(Transform transform, int vertexBuffer, int vertexCount, RenderFlags.PrimitiveTypes primitiveTypes);
        Font LoadFont(String path, int fontsize = 48);
        void DrawString2D(String text, Vector2 position, Font font, Vector4 color, float scale = 1.0f, FontAlignment alignment = FontAlignment.BottomLeft);
        void DisposeFont(Font font);
        void LoadMesh(Mesh mesh);
        void DrawMesh(Transform transform, Mesh mesh);
        void DisposeMesh(Mesh mesh);
        void DrawAABB(AABB aabb, Vector4 color);
        void DrawGrid(Camera camera, Vector4 color);
        void LoadInstanceContainer(RenderInstanceContainer container);
        void BindMeshForInstance(RenderInstanceContainer container, Mesh mesh);
        void LoadInstances(RenderInstanceContainer container);
        int AddRenderInstance(RenderInstanceContainer container, Transform transform);
        void UpdateInstance(RenderInstanceContainer container, int instanceIndex);
        void DrawInstances(RenderInstanceContainer container);
        void DisposeInstanceContainer(RenderInstanceContainer container);
        int CreateBuffer();
        int CreateVertexBuffer<T>(T[] data, bool dynamic = false) where T : unmanaged;
        int CreateElementBuffer(int[] data, bool dynamic = false);
        void BindBuffer(RenderFlags.GFXBufferTarget target, int buffer);
        void SetBufferSize(int buffer, int size, RenderFlags.GFXBufferTarget target, RenderFlags.GFXBufferUsageHint bufferUsageHint);
        void BindVertexBuffer(int buffer);
        void BindElementBuffer(int buffer);
        void SetBufferData<T>(int buffer, T[] data, RenderFlags.GFXBufferTarget target, RenderFlags.GFXBufferUsageHint bufferUsageHint) where T : unmanaged;
        void SetVertexBufferData<T>(int buffer, T[] data, bool dynamic = false) where T : unmanaged;
        void SetElementBufferData(int buffer, int[] data, bool dynamic = false);
        void UpdateBufferData<T>(int buffer, T[] data, int offset, RenderFlags.GFXBufferTarget target) where T : unmanaged;
        void UpdateVertexBufferData<T>(int buffer, T[] data, int offset) where T : unmanaged;
        void UpdateElementBufferData(int buffer, int[] data, int offset);
        T[] GetBufferData<T>(int buffer, int length, RenderFlags.GFXBufferTarget target) where T : unmanaged;
        void DisposeBuffer(int buffer);
        [Obsolete("Use BindBufferBase with RenderFlags.GFXBufferTarget.ShaderStorageBuffer instead.")]
        void BindShaderStorageBuffer(int binding, int buffer);
        [Obsolete("Use UnbindBufferBase with RenderFlags.GFXBufferTarget.ShaderStorageBuffer instead.")]
        void UnbindShaderStorageBuffer(int binding);
        void BindBufferBase(RenderFlags.GFXBufferTarget target, int binding, int buffer);
        void UnbindBufferBase(RenderFlags.GFXBufferTarget target, int binding);
        int CreateVertexArray();
        void DisposeVertexArray(int value);
        void BindVertexArray(int value);
        void EnableVertexArrayAttribute(int index);
        void SetVertexArrayAttribute(int index, int size, RenderFlags.RenderDataTypes type, bool normalized, int stride, nint pointer);
        void DispatchCompute(int numGroupsX, int numGroupsY, int numGroupsZ);
        void MemoryBarrier(MemoryBarrierFlags barriers); // TODO: Change to internal render flags
        IntPtr MapBufferRange(RenderFlags.GFXBufferTarget target, int offset, int length, MapBufferAccessMask access); // TODO: Change to internal render flags
        void UnmapBuffer(RenderFlags.GFXBufferTarget target);
        void PrepareShader(String location, bool value);
        void PrepareShader(String location, float value);
        void PrepareShader(String location, int value);
        void PrepareShader(String location, Vector2 value);
        void PrepareShader(String location, Vector3 value);
        void PrepareShader(String location, Vector4 value);
        void PrepareShader(String location, bool transpose, Matrix4 value);
        void PrepareShader(String location, int count, float[] value);
        void PrepareShaderVec2Array(String location, int count, float[] value);
        void PrepareShaderVec3Array(String location, int count, float[] value);
        void PrepareShaderVec4Array(String location, int count, float[] value);
        void PrepareShader(String uniformName, bool transpose, Matrix4[] matrices);
        void PrepareShader(String location, TextureUnit textureUnit, int value);
        void PrepareShader(String location, TextureUnit textureUnit, Texture texture);
        void PrepareShader(String location, int textureUnit, int value);
        void PrepareShaderArrayTexture(String location, int textureUnit, int value);
        void PrepareShader(String location, int textureUnit, Texture texture);
        void PrepareShader(String location, int textureUnit, Cubemap cubemap);
        public void DeleteFramebuffer(int framebuffer);
        public void DeleteTexture(int texture);
        public void DeleteRenderbuffer(int renderbuffer);
        int GetError();
    }
}
