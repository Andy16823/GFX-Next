using LibGFX.Core;
using LibGFX.Graphics.Shader;
using LibGFX.Graphics.Shapes;
using LibGFX.Math;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rect = LibGFX.Math.Rect;

namespace LibGFX.Graphics
{
    public interface IRenderDevice
    {
        void Init(Window window);
        void Dispose();
        void MakeCurrent();
        void SwapBuffers();
        void UseVsync(bool value);
        void EnableDepthTest();
        void DisableDepthTest();
        void SetViewport(Viewport viewport);
        void SetViewMatrix(Matrix4 matrix);
        void SetProjectionMatrix(Matrix4 matrix);
        void Clear(int mask);
        void ClearColor(float r, float g, float b, float a);
        void Flush();
        RenderTarget CreateRenderTarget(RenderTargetDescriptor constructorInfo);
        void BindRenderTarget(RenderTarget renderTarget); 
        void ResizeRenderTarget(RenderTarget renderTarget, int width, int height);
        void UnbindRenderTarget();
        int GetFramebufferIndex();
        void DisposeRenderTarget(RenderTarget renderTarget);
        void BuildShaderProgram(ShaderProgram shaderProgram);
        void DisposeShaderProgram(ShaderProgram shaderProgram);
        void AddShaderProgram(String name, ShaderProgram shaderProgram);
        void AddShape(Shape shape);
        void InitShape(Shape shape);
        void DisposeShape(Shape shape);
        ShaderProgram GetShaderProgram(String name);
        void BindShaderProgram(ShaderProgram shaderProgram);
        void UnbindShaderProgram();
        int GetUniformLocation(int program, String name);
        void InitializeTexture(Texture texture);
        void DisposeTexture(Texture texture);
        void DrawRenderTarget(RenderTarget renderTarget);
        void DrawRect(Math.Rect rect, Vector4 color, float borderWidth = 1.0f, float rotation = 0.0f);
        void FillRect(Math.Rect rect, Vector4 color, float rotation = 0.0f);
        void DrawTexture(Vector3 position, Vector3 rotation, Vector3 scale, Texture texture, Vector4 color);
        void DrawTexture(Vector3 position, Vector3 rotation, Vector3 scale, int textureId, Vector4 color);
        int GetError();
    }
}
