using LibGFX.Assets;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Shader;
using LibGFX.Math;
using LibGFX.Physics;
using Microsoft.VisualBasic.Devices;
using NewGFXEditor.Shader;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace NewGFXEditor.Editor
{
    /// <summary>
    /// Enum to represent the active axis of the gizmo.
    /// </summary>
    public enum GizmoActiveAxis
    {
        None,
        X,
        Y,
        Z
    }

    /// <summary>
    /// Gizmo types for different transformation modes.
    /// </summary>
    public enum GizmoType
    {
        Translation,
        Rotation,
        Scale
    }

    /// <summary>
    /// Class representing a gizmo for 3D transformations.
    /// </summary>
    public class Gizmo
    {
        /// <summary>
        /// Represents a method that handles an event when a gizmo is moved to a new position.
        /// </summary>
        /// <param name="newPosition"></param>
        public delegate void GizmoPositionDelegate(Vector3 newPosition);

        /// <summary>
        /// Represents a method that handles an event when a gizmo is scaled by a specified factor.
        /// </summary>
        /// <param name="scaleFactor">The factor by which the gizmo is scaled. A value greater than 1 increases the size; a value between 0 and 1
        /// decreases it.</param>
        public delegate void GizmoScaledDelegate(float scaleFactor);

        /// <summary>
        /// Represents the method that handles the event when a gizmo is rotated.
        /// </summary>
        /// <param name="angleDegrees">The rotation of the gizmo, expressed as a quaternion in degrees.</param>

        public delegate void GizmoRotatedDelegate(float rotationFactor);

        /// <summary>
        /// Gets or sets the static mesh model used to represent the gizmo in the scene.
        /// </summary>
        public StaticMeshModel GizmoModel { get; set; }

        /// <summary>
        /// Gets or sets the transformation applied to the object, including position, rotation, and scale information.
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>
        /// Gets or sets the shader program used for rendering the gizmo.
        /// </summary>
        public ShaderProgram Shader { get; set; } = new GizmoShader();

        /// <summary>
        /// Gets or sets the currently active axis for the gizmo operation.
        /// </summary>
        public GizmoActiveAxis ActiveAxis { get; set; } = GizmoActiveAxis.None;

        /// <summary>
        /// Gets or sets a value indicating whether the feature is enabled.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the type of gizmo to display or interact with.
        /// </summary>
        public GizmoType Type { get; set; } = GizmoType.Translation;

        /// <summary>
        /// Gets or sets the distance, in units, used for snapping operations.
        /// </summary>
        public float SnappingUnit { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the angle, in degrees, at which snapping occurs during rotation operations.
        /// </summary>
        /// <remarks>Use this property to control the increment at which objects snap when rotated. A
        /// smaller value allows for finer rotation steps, while a larger value restricts snapping to coarser angles.
        /// The default value is 45 degrees.</remarks>
        public float SnappingAngle { get; set; } = 45.0f;

        /// <summary>
        /// Gets or sets a value indicating whether snapping behavior is enabled.
        /// </summary>
        public bool EnableSnapping { get; set; } = false;

        /// <summary>
        /// Occurs when the gizmo has been moved to a new position.
        /// </summary>
        /// <remarks>Subscribers can use this event to respond to changes in the gizmo's position, such as
        /// updating UI elements or triggering related actions. The event provides information about the new position
        /// through the associated delegate.</remarks>
        public event GizmoPositionDelegate GizmoMoved;

        /// <summary>
        /// Occurs when the gizmo is scaled by the user or programmatically.
        /// </summary>
        /// <remarks>Subscribe to this event to be notified whenever the gizmo's scale changes. The event
        /// provides information about the scaling operation, such as the new scale value and the context in which the
        /// scaling occurred.</remarks>
        public event GizmoScaledDelegate GizmoScaled;

        /// <summary>
        /// Occurs when the gizmo has been rotated by the user.
        /// </summary>
        /// <remarks>Subscribe to this event to be notified whenever the gizmo's orientation changes as a
        /// result of user interaction. Handlers receive information about the rotation through the associated
        /// delegate.</remarks>
        public event GizmoRotatedDelegate GizmoRotated;

        // Indicates whether to swap the X and Z axes for certain operations.
        private bool _swapXZAxes = false;

        // Stores the unsnapped position of the gizmo before snapping operations.
        private Vector3 _unsnappedPosition;
        private float _rotationSnapRemainder = 0f;
        private float _scaleSnapRemainder = 0f;

        /// <summary>
        /// Constructor for the Gizmo class.
        /// </summary>
        /// <param name="file"></param>
        public Gizmo(String file)
        {
            this.Transform = new Transform();
            GizmoModel = new StaticMeshModel(file);
            // Change materials to GizmoMaterial
            foreach (var mesh in this.GizmoModel.Meshes.Values)
            {
                var sgMaterial = mesh.Material as SGMaterial;
                GizmoMaterial gizmoMaterial = new GizmoMaterial()
                {
                    Name = sgMaterial.Name,
                    VertexColor = sgMaterial.Color
                };
                mesh.Material = gizmoMaterial;
            }
        }

        /// <summary>
        /// Initializes the gizmo with the specified render device and viewport.
        /// </summary>
        /// <param name="renderDevice"></param>
        /// <param name="viewport"></param>
        public void Init(IRenderDevice renderDevice, Viewport viewport)
        {
            renderDevice.BuildShaderProgram(this.Shader);
            this.GizmoModel.Init(renderDevice);
        }
        
        /// <summary>
        /// Renders the gizmo using the specified render device, camera, and viewport.
        /// </summary>
        /// <param name="renderDevice"></param>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        public void RenderGizmo(IRenderDevice renderDevice, LibGFX.Graphics.Camera camera, Viewport viewport)
        {
            // Scale the gizmo based on the camera and viewport
            this.ScaleGizmo((PerspectiveCamera)camera, viewport, 25.0f);

            // Render the gizmo with the actual shader
            if (!this.Enabled)
                return;

            renderDevice.SetProjectionMatrix(camera.GetProjectionMatrix(viewport));
            renderDevice.BindShaderProgram(this.Shader);
            foreach (var mesh in this.GizmoModel.Meshes.Values)
            {
                renderDevice.DrawMesh(this.Transform, mesh);
            }
            renderDevice.UnbindShaderProgram();
        }

        /// <summary>
        /// Disposes of the gizmo resources.
        /// </summary>
        /// <param name="renderDevice"></param>
        public void Dispose(IRenderDevice renderDevice)
        {
            this.GizmoModel.Dispose(renderDevice);
            renderDevice.DisposeShaderProgram(this.Shader);
        }

        /// <summary>
        /// Highlights the gizmo based on mouse position.
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        public void HighlightGizmo(PerspectiveCamera camera, Viewport viewport, int mouseX, int mouseY)
        {
            if (this.Enabled == false)
                return;


            this.UnhoverMaterials();

            var ray = MeshRaycast.ScreenPointToWorldRay(camera, viewport, mouseX, mouseY);
            foreach (var mesh in this.GizmoModel.Meshes.Values)
            {
                var hit = MeshRaycast.IntersectsMesh(ray, this.Transform, mesh);
                if(hit.Hit)
                {
                    var material = (GizmoMaterial)mesh.Material;
                    material.Hovered = true;
                }
            }
        }

        /// <summary>
        /// Scales the gizmo based on the camera and viewport.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        /// <param name="desiredPixelHeight"></param>
        public void ScaleGizmo(PerspectiveCamera camera, Viewport viewport, float desiredPixelHeight = 100f)
        {
            if (this.Enabled == false)
                return;

            Vector3 cameraPos = camera.Transform.Position;
            float distance = (this.Transform.Position - cameraPos).Length;

            float fovRadians = MathHelper.DegreesToRadians(camera.Fov);
            float screenHeightAtDistance = 2.0f * distance * (float)System.Math.Tan(fovRadians / 2.0f);

            float pixelToWorld = screenHeightAtDistance / viewport.Height;
            float desiredWorldHeight = desiredPixelHeight * pixelToWorld;

            this.Transform.Scale = new Vector3(desiredWorldHeight);
        }

        /// <summary>
        /// Checks if the gizmo was picked by the mouse.
        /// </summary>
        /// <param name="mosueX"></param>
        /// <param name="mouseY"></param>
        /// <returns></returns>
        public bool PickGizmo(PerspectiveCamera camera, Viewport viewport, int mouseX, int mouseY)
        {
            var ray = MeshRaycast.ScreenPointToWorldRay(camera, viewport, mouseX, mouseY);
            foreach (var mesh in this.GizmoModel.Meshes.Values)
            {
                var hit = MeshRaycast.IntersectsMesh(ray, this.Transform, mesh);
                if(hit.Hit)
                {
                    if (mesh.Name.StartsWith("Gizmo_X", StringComparison.OrdinalIgnoreCase))
                    {
                        this.ActiveAxis = GizmoActiveAxis.X;
                        return true;
                    }
                    else if (mesh.Name.StartsWith("Gizmo_Y", StringComparison.OrdinalIgnoreCase))
                    {
                        this.ActiveAxis = GizmoActiveAxis.Y;
                        return true;
                    }
                    else if (mesh.Name.StartsWith("Gizmo_Z", StringComparison.OrdinalIgnoreCase))
                    {
                        this.ActiveAxis = GizmoActiveAxis.Z;
                        return true;
                    }
                }
            }
            this.ActiveAxis = GizmoActiveAxis.None;
            return false;
        }

        /// <summary>
        /// Prepares the current instance for performing an action.
        /// Such as moving, scaling, or rotating along the active axis.
        /// </summary>
        public void PrepareForAction()
        {
            _unsnappedPosition = this.Transform.Position;
            _rotationSnapRemainder = 0.0f;
            _scaleSnapRemainder = 0.0f;
        }

        /// <summary>
        /// Releases the gizmo, setting the active axis to none.
        /// </summary>
        public void ReleaseGizmo()
        {
            this.ActiveAxis = GizmoActiveAxis.None;
        }

        /// <summary>
        /// Moves the gizmo along the specified axis based on mouse movement.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        /// <param name="prevMouseX"></param>
        /// <param name="prevMouseY"></param>
        /// <param name="currMouseX"></param>
        /// <param name="currMouseY"></param>
        public void MoveAlongAxis2D(PerspectiveCamera camera, Viewport viewport, int prevMouseX, int prevMouseY, int currMouseX, int currMouseY)
        {
            if (this.ActiveAxis == GizmoActiveAxis.None || this.Enabled == false)
                return;

            Vector3 axisWorld = GetAxisDirection(this.ActiveAxis, _swapXZAxes);
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix(viewport);

            Vector4 gizmoWorld = new Vector4(Transform.Position, 1.0f);
            Vector4 gizmoClip = gizmoWorld * view * projection;
            gizmoClip /= gizmoClip.W;

            Vector3 gizmoScreen = new Vector3(
                (gizmoClip.X * 0.5f + 0.5f) * viewport.Width, 
                (1.0f - (gizmoClip.Y * 0.5f + 0.5f)) * viewport.Height, 
                0);

            Vector4 endWorld = new Vector4(this.Transform.Position + axisWorld, 1.0f);
            Vector4 endClip = endWorld * view * projection;
            endClip /= endClip.W;

            Vector3 endScreen = new Vector3(
                (endClip.X * 0.5f + 0.5f) * viewport.Width, 
                (1.0f - (endClip.Y * 0.5f + 0.5f)) * viewport.Height, 
                0);

            Vector2 axisScreenDir = (endScreen - gizmoScreen).Xy.Normalized();
            Vector2 mouseDelta = new Vector2(currMouseX - prevMouseX, currMouseY - prevMouseY);

            float movementOnAxis = Vector2.Dot(mouseDelta, axisScreenDir);
            float scaleFactor = 0.01f; // ggf. dynamisch skalieren

            _unsnappedPosition += axisWorld * movementOnAxis * scaleFactor;

            var finalPosition = _unsnappedPosition;

            if(this.EnableSnapping)
            {
                switch (ActiveAxis)
                {
                    case GizmoActiveAxis.None:
                        break;
                    case GizmoActiveAxis.X:
                        finalPosition.X = (float)Math.Round(finalPosition.X / SnappingUnit) * SnappingUnit;
                        break;
                    case GizmoActiveAxis.Y:
                        finalPosition.Y = (float)Math.Round(finalPosition.Y / SnappingUnit) * SnappingUnit;
                        break;
                    case GizmoActiveAxis.Z:
                        finalPosition.Z = (float)Math.Round(finalPosition.Z / SnappingUnit) * SnappingUnit;
                        break;
                    default:
                        break;
                }
            }

            this.Transform.Position = finalPosition;
            this.GizmoMoved?.Invoke(finalPosition);
        }

        /// <summary>
        /// Calculates the scale factor along the active axis based on mouse movement.
        /// Raises an event if the gizmo is scaled.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="viewport"></param>
        /// <param name="prevMouseX"></param>
        /// <param name="prevMouseY"></param>
        /// <param name="currMouseX"></param>
        /// <param name="currMouseY"></param>
        public void ScaleAlongAxis(PerspectiveCamera camera, Viewport viewport, int prevMouseX, int prevMouseY, int currMouseX, int currMouseY)
        {
            if (ActiveAxis == GizmoActiveAxis.None || !Enabled)
                return;

            // Get the world direction of the active axis
            Vector3 axisWorld = GetAxisDirection(ActiveAxis, _swapXZAxes);

            // Project axis to screen space
            Vector3 gizmoOrigin = Transform.Position;
            Vector3 gizmoAxisEnd = gizmoOrigin + axisWorld;

            // Screen space positions
            var screenOrigin = PerspectiveCamera.WorldToScreen(camera, gizmoOrigin, viewport);
            var screenAxisEnd = PerspectiveCamera.WorldToScreen(camera, gizmoAxisEnd, viewport);

            // Axis direction in screen space
            var axisScreenDir = (screenAxisEnd - screenOrigin).Xy;
            if (axisScreenDir.LengthSquared < 0.0001f)
                return;

            axisScreenDir.Normalize();

            // Mouse delta
            var mouseDelta = new Vector2(
                currMouseX - prevMouseX,
                currMouseY - prevMouseY
            );

            float delta = Vector2.Dot(mouseDelta, axisScreenDir) * 0.01f;

            // Return if snapping is disabled
            if (!EnableSnapping)
            {
                GizmoScaled?.Invoke(delta);
                return;
            }

            // Snap-accumulator
            _scaleSnapRemainder += delta;

            float snap = SnappingUnit;
            float snappedDelta = 0f;

            if (Math.Abs(_scaleSnapRemainder) >= snap)
            {
                snappedDelta = MathF.Floor(_scaleSnapRemainder / snap) * snap;
                _scaleSnapRemainder -= snappedDelta;
            }

            if (snappedDelta != 0f)
                GizmoScaled?.Invoke(snappedDelta);
        }

        public void RotateAlongAxis(PerspectiveCamera camera, Viewport viewport, int prevMouseX, int prevMouseY, int currMouseX, int currMouseY)
        {
            if (ActiveAxis == GizmoActiveAxis.None || !Enabled)
                return;

            Vector3 axisWorld = GetAxisDirection(ActiveAxis, _swapXZAxes);

            // Axis swap for rotation gizmo
            if (Type == GizmoType.Rotation)
            {
                if (ActiveAxis == GizmoActiveAxis.X || ActiveAxis == GizmoActiveAxis.Z)
                    axisWorld = Vector3.UnitY;
                else if (ActiveAxis == GizmoActiveAxis.Y)
                    axisWorld = Vector3.UnitX;
            }

            // Project axis to screen space
            Vector3 origin = Transform.Position;
            Vector3 axisEnd = origin + axisWorld;

            // Screen space positions
            var screenOrigin = PerspectiveCamera.WorldToScreen(camera, origin, viewport);
            var screenAxisEnd = PerspectiveCamera.WorldToScreen(camera, axisEnd, viewport);

            // Axis direction in screen space
            Vector2 axisScreenDir = (screenAxisEnd - screenOrigin).Xy;
            if (axisScreenDir.LengthSquared < 0.0001f)
                return;

            axisScreenDir.Normalize();

            // Mouse delta
            Vector2 mouseDelta = new Vector2(
                currMouseX - prevMouseX,
                currMouseY - prevMouseY
            );

            // Calculate rotation delta
            float deltaAngle = Vector2.Dot(mouseDelta, axisScreenDir) * 0.01f;

            // Return if snapping is disabled
            if (!EnableSnapping)
            {
                GizmoRotated?.Invoke(deltaAngle);
                return;
            }

            // Snap-accumulator
            _rotationSnapRemainder += deltaAngle;

            float snapAngle = MathHelper.DegreesToRadians(this.SnappingAngle);
            float snappedDelta = 0f;

            if (Math.Abs(_rotationSnapRemainder) >= snapAngle)
            {
                snappedDelta = MathF.Floor(_rotationSnapRemainder / snapAngle) * snapAngle;
                _rotationSnapRemainder -= snappedDelta;
            }

            if (snappedDelta != 0f)
                GizmoRotated?.Invoke(snappedDelta);
        }

        /// <summary>
        /// Unhovers all materials in the gizmo.
        /// </summary>
        private void UnhoverMaterials()
        {
            foreach (var mesh in this.GizmoModel.Meshes.Values)
            {
                var mat = (GizmoMaterial)mesh.Material;
                mat.Hovered = false;
            }
        }

        /// <summary>
        /// Sets the hovered state of a specific material based on its ID.
        /// </summary>
        /// <param name="id"></param>
        private void HoverMaterial(int id)
        {
            var mesh = this.GizmoModel.Meshes.Values.ToList()[id];
            var material = (GizmoMaterial)mesh.Material;
            material.Hovered = true;
        }

        /// <summary>
        /// Gets the direction of the specified axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private Vector3 GetAxisDirection(GizmoActiveAxis axis, bool swapXZ = false)
        {
            switch (axis)
            {
                case GizmoActiveAxis.X:
                    if(swapXZ)
                    {
                        return Vector3.UnitZ;
                    }

                    return Vector3.UnitX;
                case GizmoActiveAxis.Y:
                    return Vector3.UnitY;
                case GizmoActiveAxis.Z:
                    if (swapXZ)
                    {
                        return Vector3.UnitX;
                    }

                    return Vector3.UnitZ;
                default:
                    return Vector3.Zero;
            }
        }
    }
}
