using LibGFX.Core;
using LibGFX.Graphics;
using LibGFX.Graphics.Lights;
using LibGFX.Math;
using NewGFXEditor.Shader;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGFX.Graphics.Materials;

namespace NewGFXEditor
{
    public struct ColorPickResult
    {
        public bool Success;
        public int Id;
        public Vector4 Color;
    }

    public class ColorIDPicker
    {
        /// <summary>
        /// Render target for the ID picking pass.
        /// </summary>
        public RenderTarget RenderTarget { get; set; }

        /// <summary>
        /// Pixel data from the render target.
        /// </summary>
        public byte[] PixelData { get; set; }

        /// <summary>
        /// Size of the result image.
        /// </summary>
        public Vector2i ResultSize { get; set; }

        private bool _isDepthTestEnabled = false;

        /// <summary>
        /// Converts an ID to a Vector4 color.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Vector4 IdToVec4(int id)
        {
            int r = (id & 0xFF);
            int g = (id >> 8) & 0xFF;
            int b = (id >> 16) & 0xFF;

            return new Vector4(r / 255f, g / 255f, b / 255f, 1.0f);
        }

        /// <summary>
        /// Converts a GUID to a uint32 value.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static uint GuidToUInt32(Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            // Nimm z. B. die ersten 4 Bytes für einen uint32-Wert
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Converts a Vector4 color to an ID.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public int Vec4ToId(Vector4 vec)
        {
            int r = (int)(vec.X * 255);
            int g = (int)(vec.Y * 255);
            int b = (int)(vec.Z * 255);
            return (r & 0xFF) | ((g & 0xFF) << 8) | ((b & 0xFF) << 16);
        }

        /// <summary>
        /// Gets the pixel color from the framebuffer data.
        /// </summary>
        /// <param name="framebufferData"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Vector4 GetPixelColor(byte[] framebufferData, int x, int y, int width, int height)
        {
            int index = ((height - y) * width + x) * 4;
            return BgraToRgba(framebufferData, index);
        }

        /// <summary>
        /// Converts BGRA pixel data to RGBA format.
        /// </summary>
        /// <param name="framebufferData"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector4 BgraToRgba(byte[] framebufferData, int index)
        {
            // BGRA -> RGBA Umwandlung
            byte b = framebufferData[index];     // Blau (BGRA[0])
            byte g = framebufferData[index + 1]; // Grün (BGRA[1])
            byte r = framebufferData[index + 2]; // Rot (BGRA[2])
            byte a = framebufferData[index + 3]; // Alpha (BGRA[3])

            // Umwandlung von [0, 255] nach [0.0, 1.0]
            return new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        /// <summary>
        /// Initializes the render target for ID picking.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="viewport"></param>
        public void Init(IRenderDevice renderer, Viewport viewport)
        {
            var renderTargetDescriptor = new RenderTargetDescriptor()
            {
                Width = viewport.Width,
                Height = viewport.Height,
                Border = 0
            };
            RenderTarget = renderer.CreateRenderTarget(renderTargetDescriptor);


            if (!renderer.ExistsShaderProgram("ColorIDShader"))
            {
                var shader = new ColorIDShader();
                renderer.BuildShaderProgram(shader);
                renderer.AddShaderProgram("ColorIDShader", shader);
            }
        }

        /// <summary>
        /// Disposes the render target.
        /// </summary>
        /// <param name="renderer"></param>
        public void Dispose(IRenderDevice renderer)
        {
            renderer.DisposeRenderTarget(RenderTarget);
        }

        /// <summary>
        /// Prepares the scene for picking by rendering each game element with a unique ID.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="viewport"></param>
        /// <param name="camera"></param>
        /// <param name="scene"></param>
        public void PrepareSceneForPicking(IRenderDevice renderer, Viewport viewport, Camera camera, BaseScene scene)
        {
            //Color Picker render pass
            int id = 1;
            this.StartIdRenderPass(renderer, viewport, camera);
            scene.ForEachElement(element =>
            {
                this.RenderGameElement(renderer, element, camera, id);
                id++;
            });
            this.EndIdRenderPass(renderer);
        }

        /// <summary>
        /// Starts the ID render pass.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="viewport"></param>
        /// <param name="camera"></param>
        public void StartIdRenderPass(IRenderDevice renderer, Viewport viewport, Camera camera)
        {
            var shader = renderer.GetShaderProgram("ColorIDShader");
            _isDepthTestEnabled = renderer.IsDepthTestEnabled();

            renderer.SetViewport(viewport);
            renderer.SetProjectionMatrix(camera.GetProjectionMatrix(viewport));
            renderer.SetViewMatrix(camera.GetViewMatrix());

            // Render the scene to the render target
            renderer.ResizeRenderTarget(RenderTarget, viewport.Width, viewport.Height);
            renderer.BindRenderTarget(RenderTarget);
            renderer.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

            renderer.EnableDepthTest();
            renderer.BindShaderProgram(shader);
        }

        /// <summary>
        /// Renders a game element with a unique ID to the picking framebuffer.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="element"></param>
        /// <param name="camera"></param>
        /// <param name="id"></param>
        public void RenderGameElement(IRenderDevice renderer, GameElement element, Camera camera, int id)
        {
            // Bind the uniforms for the specific element
            var meshes = element.GetMeshes();
            if (meshes == null)
                return;

            var colorId = IdToVec4(id);

            renderer.PrepareShader("colorId", colorId);
            foreach (var item in meshes)
            {
                var mesh = item.Item1;
                var material = item.Item2;

                renderer.DrawMesh(element.Transform, mesh, material);
            }
        }

        public void RenderMesh(IRenderDevice renderer, Transform transform, Mesh mesh, IMaterial material, int id)
        {
            // Bind the uniforms for the specific element
            var colorId = IdToVec4(id);
            renderer.PrepareShader("colorId", colorId);
            renderer.DrawMesh(transform, mesh, material);
        }

        /// <summary>
        /// Ends the ID render pass and unbinds the render target. Also sets the pixel data.
        /// </summary>
        /// <param name="renderer"></param>
        public void EndIdRenderPass(IRenderDevice renderer)
        {
            // Unbind the render target and restore the default framebuffer
            renderer.UnbindShaderProgram();
            renderer.UnbindRenderTarget();
            renderer.SetDepthTest(_isDepthTestEnabled);

            SetPixelData(renderer);
        }

        /// <summary>
        /// Sets the pixel data from the render target.
        /// </summary>
        /// <param name="renderer"></param>
        private void SetPixelData(IRenderDevice renderer)
        {
            var size = renderer.GetRenderTargetSize(RenderTarget);
            PixelData = renderer.GetRenderTargetData(RenderTarget, size.X, size.Y);
            this.ResultSize = size;
        }

        /// <summary>
        /// Converts the framebuffer data to a Bitmap.
        /// </summary>
        /// <returns></returns>
        public Bitmap FramebufferToBitmap()
        {
            var bitmap = Utils.ByteBGRAToBitmap(this.PixelData, ResultSize.X, ResultSize.Y);
            return bitmap;
        }

        public void PerformPick(int x, int y, out ColorPickResult result)
        {
            // Create the basic result
            result = new ColorPickResult();
            result.Success = false;
            result.Id = -1;
            result.Color = new Vector4(0, 0, 0, 0);

            var pixelColor = GetPixelColor(PixelData, x, y, ResultSize.X, ResultSize.Y);
            var id = Vec4ToId(pixelColor) - 1; // Subtract 1 to get the original ID

            if (id < 0)
            {
                return;
            }

            result.Success = true;
            result.Id = id;
            result.Color = pixelColor;
        }

        /// <summary>
        /// Performs the pick operation on the scene based on the pixel color at the given coordinates.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void PerformScenePick(BaseScene scene, int x, int y, out ColorPickResult result, out GameElement pickedElement)
        {
            this.PerformPick(x, y, out result);
            pickedElement = null;

            if(!result.Success)
            {
                return;
            }

            var elements = scene.GetAllElements().ToList();
            var id = result.Id;
            if (id >= elements.Count)
            {
                Console.WriteLine("ID out of range");
                return;
            }

            pickedElement = elements[id];
        }
    }
}
