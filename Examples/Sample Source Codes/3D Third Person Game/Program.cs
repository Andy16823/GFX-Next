using LibGFX;
using LibGFX.Audio;
using LibGFX.Core;
using LibGFX.Core.GameElements;
using LibGFX.Graphics;
using LibGFX.Graphics.Enviroment;
using LibGFX.Graphics.Lights;
using LibGFX.Graphics.Materials;
using LibGFX.Graphics.Primitives;
using LibGFX.Math;
using LibGFX.Pyhsics;
using LibGFX.Pyhsics.Behaviors3D;
using NewGFXTest;
using OpenTK.Mathematics;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace GFXNugetTest3D
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var viewport = new Viewport(800, 600);
            var window = GFX.Instance.CreateWindow("GFX", viewport, OpenTK.Windowing.Common.WindowState.Normal);
            var renderer = new GLRenderer();
            renderer.Init(window);
            renderer.UseVsync(true);

            // Load the assets
            var boxMaterial = GFX.Instance.AssetManager.Load<SGMaterial>("C:/Users/andy1/Documents/GFXMaterial/Box/material.json");
            var audioClip = GFX.Instance.AssetManager.Load<AudioClip>("C:/Users/andy1/Documents/Ultra Engine/Projects/Backrooms/Sound/Ambient/test4.wav");
            var noiseTexture = GFX.Instance.AssetManager.Load<Texture>("D:/3D Modele/Materials/NoiseTextures/Noise2/basecolor.png");
            var cubeMesh = GFX.Instance.AssetManager.AddAsset<Mesh>("QubeMesh", new Cube().GetMesh());

            // Create an OpenAL device and load the audio audioClip
            var audioDevice = new OpenALDevice();
            audioDevice.InitializeAudioDevice();
            audioDevice.MakeCurrent();
            audioDevice.LoadAudioClip(audioClip);
            GFX.Instance.Services.AddService<IAudioDevice>("audio", audioDevice);

            // Create an new 3D scene with an sun and an empty layer
            var scene = new Scene3D("BaseLayer");

            // Create an light manager for 3D lights for the scene and add an directional light and myny point lights
            var lightmanager = new Light3DManager();
            lightmanager.DirectionalLight = new DirectionalLight(new Vector3(-0.2f, 1.0f, -0.3f), new Vector4(0.3f, 0.3f, 0.3f, 0.3f), 0.3f);
            scene.LightManager = lightmanager;

            Random random = new Random();
            for (int x = -50; x < 50; x++)
            {
                for (int z = -50; z < 50; z++)
                {
                    var r = random.NextDouble();
                    var g = random.NextDouble();
                    var b = random.NextDouble();
                    var color = new Vector4((float)r, (float)g, (float)b, 1.0f);

                    var posX = x + (x * 5f);
                    var posZ = z + (z * 5f);
                    var posY = 0f;

                    lightmanager.AddPointLight(new PointLight3D(new Vector3(posX, posY, posZ), color, 2.5f, 70f));
                }
            }

            // Create an procedural sky for the scene
            var sky = new ProceduralSky();
            scene.Enviroment = sky;

            // Create an physics handler for the scene
            var physicsHander = new PhysicsHandler3D(new Vector3(0, -9.8f, 0));
            physicsHander.PhysicsWorld.DebugDrawer = new DebugDrawer(renderer);
            physicsHander.PhysicsWorld.DebugDrawer.DebugMode = BulletSharp.DebugDrawModes.DrawAabb;
            physicsHander.DebugPhysics = false;
            scene.PhysicsHandler = physicsHander;

            // Create an camera and set it as the current camera.
            var camera = new PerspectiveCamera(new Vector3(10, 5, 0), new Vector3(800, 600, 0));
            camera.SetAsCurrent();

            // Create an empty object for the audio source. The audio source is an behavior that will be attached to the empty object
            var audioSourceElement = new Empty("AudioSource", new Vector3(0, 5, 0));
            var audioSource = audioSourceElement.AddBehavior<AudioSource>(new AudioSource(audioDevice));
            audioSource.SetAudioClip(audioClip);
            audioSource.SetRange(new Vector3(1.0f, 10.0f, 1.0f));
            scene.AddGameElement("BaseLayer", audioSourceElement);

            // Create the player model and add an capsule rigidbody to it. The capsule rigidbody will be used for the character controller.
            var model = GFX.Instance.AssetManager.Load<Model>("C:/Users/andy1/source/repos/3DNetGame/3DNetGame/Resources/Models/Girly/girly.fbx");
            model.OverrideMeshScale(1.0f);
            model.AnimationSpeed = 1.5f;
            model.Transform.Position = new Vector3(0, 0, 0);
            model.PlayAnimation("Run");

            var modelRigidBody = model.AddBehavior<CapsuleRigidBody>(new CapsuleRigidBody(scene.PhysicsHandler));
            modelRigidBody.Offset = new Vector3(0, 1, 0);
            modelRigidBody.CreateRigidBody(10f);

            var thirdPersonController = model.AddBehavior<ThirdPersonController>(new ThirdPersonController());
            scene.AddGameElement("BaseLayer", model);

            // Add an ground cube to the scene. We can use the instancer as well for this.
            var cube = new Primitive("Qube", boxMaterial, new Cube());
            cube.Transform.Position = new Vector3(0, -2, 0);
            cube.Transform.Scale = new Vector3(50.5f, 0.5f, 50.5f);
            var collider = cube.AddBehavior<BoxCollider>(new BoxCollider(scene.PhysicsHandler));
            collider.CreateCollider(0f);
            scene.AddGameElement("BaseLayer", cube);

            // Add an static model without animations
            var car = GFX.Instance.AssetManager.Load<Model>("C:/Users/andy1/Documents/Ultra Engine/NuclearFrontiers/Models/Cars/PoliceCar/PoliceCar.gltf");
            car.Transform.Position = new Vector3(10, 0, 10);
            scene.AddGameElement("BaseLayer", car);

            var carCollider = car.AddBehavior<MeshCollider>(new MeshCollider(scene.PhysicsHandler));
            carCollider.CreateCollider(10, "C:/Users/andy1/Documents/Ultra Engine/NuclearFrontiers/Models/Cars/PoliceCar/PoliceCar.gltf");

            // Create an debug cube to show the hit location of the raycast
            var debugCube = new Primitive("DebugCube", boxMaterial, new Cube());
            debugCube.Transform.Position = new Vector3(0, 0, 0);
            debugCube.Transform.Scale = new Vector3(2f, 2f, 2f);
            scene.AddGameElement("BaseLayer", debugCube);

            // Load an font
            var font = renderer.LoadFont("C:/Users/andy1/Downloads/ARIAL.ttf", 64);

            // Initialize all textures from the asset manager
            GFX.Instance.AssetManager.ForeachAsset<Texture>(texture =>
            {
                renderer.LoadTexture(texture);
            });

            // Initialize all meshes from the asset manager
            GFX.Instance.AssetManager.ForeachAsset<Mesh>(mesh =>
            {
                renderer.LoadMesh(mesh);
            });

            // Intialize all materials from the asset manager
            GFX.Instance.AssetManager.ForeachAsset<SGMaterial>(material =>
            {
                material.Init(renderer);
            });

            // Initialize the scene
            scene.Init(viewport, renderer);

            // Enable alpha blending for the renderer and play the audio clip
            renderer.EnableAlphaBlend();
            audioSource.Play();

            // Game Loop
            while (!window.RequestClose())
            {
                // End game if escape is pressed
                if (window.IsKeyPressed(Keys.Escape))
                {
                    window.ShowCursor();
                    window.Close();
                }

                // Set the camera to the window size
                viewport = window.GetViewport();
                camera.Transform.Scale = new Vector3(viewport.Width, viewport.Height, 0);

                // Process the input events
                window.ProcessEvents();

                // Update the audio listener position and orientation
                audioDevice.SetAudioListenerPosition(camera.Transform.Position);
                audioDevice.SetAudioListenerOrientation(camera.Transform.GetFront(), camera.Transform.GetUp());

                // Update the scene
                scene.UpdatePhysics();
                scene.Update();

                // Render the scene
                renderer.MakeCurrent();
                renderer.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
                renderer.Clear(RenderFlags.ClearFlags.Color | RenderFlags.ClearFlags.Depth);

                scene.Render(viewport, renderer, Camera.Current);
                renderer.Flush();
                renderer.SwapBuffers();
            }

            // Free the texture resources
            GFX.Instance.AssetManager.ForeachAsset<Texture>(texture =>
            {
                renderer.DisposeTexture(texture);
            });

            // Free the material resources
            GFX.Instance.AssetManager.ForeachAsset<SGMaterial>(material =>
            {
                material.Dispose(renderer);
            });

            // Free the mesh resources
            GFX.Instance.AssetManager.ForeachAsset<Mesh>(mesh =>
            {
                renderer.DisposeMesh(mesh);
            });

            // Free other resources
            audioDevice.DisposeAudioClip(audioClip);
            scene.DisposeScene(renderer);
            renderer.DisposeFont(font);
            renderer.Dispose();
        }
    }
}