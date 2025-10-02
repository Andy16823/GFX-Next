using Assimp;
using Assimp.Configs;
using LibGFX.Graphics;
using LibGFX.Graphics.Animation3D;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Assets.Loaders
{
    /// <summary>
    /// Defines the modes for loading a skeleton.
    /// </summary>
    public enum SkeletonLoadingMode
    {
        Full,
        MeshBones,
        AnimationBones,
        MeshAndAnimationBones
    }

    /// <summary>
    /// Arguments for loading a skeleton.
    /// </summary>
    public struct SkeletonLoadingArgs
    {
        public SkeletonLoadingMode loadingMode;

        public SkeletonLoadingArgs(SkeletonLoadingMode mode = SkeletonLoadingMode.Full)
        {
            this.loadingMode = mode;
        }

        public static readonly SkeletonLoadingArgs Default = new SkeletonLoadingArgs(SkeletonLoadingMode.Full);
        public static readonly SkeletonLoadingArgs Full = new SkeletonLoadingArgs(SkeletonLoadingMode.Full);
        public static readonly SkeletonLoadingArgs MeshBonesOnly = new SkeletonLoadingArgs(SkeletonLoadingMode.MeshBones);
        public static readonly SkeletonLoadingArgs AnimationBonesOnly = new SkeletonLoadingArgs(SkeletonLoadingMode.AnimationBones);
        public static readonly SkeletonLoadingArgs MeshAndAnimationBones = new SkeletonLoadingArgs(SkeletonLoadingMode.MeshAndAnimationBones);
    }

    /// <summary>
    /// Loader for Skeleton assets.
    /// </summary>
    public class SkeletonLoader : IAssetLoader
    {
        public bool ShouldCache => true;

        public bool CanCreate => false;

        public T Create<T>(string id, Action<T>? initializer = null, object? creationArgs = null) where T : class
        {
            throw new NotImplementedException();
        }

        public T Load<T>(string path, object? loadingArgs = null) where T : class
        {
            if (typeof(T) != typeof(Skeleton))
            {
                throw new NotSupportedException($"Asset type '{typeof(T)}' is not supported.");
            }

            SkeletonLoadingArgs args;
            if (loadingArgs is SkeletonLoadingArgs validArgs)
            {
                args = validArgs;
            }
            else
            {
                args = SkeletonLoadingArgs.Default;
            }

            // Load the model using Assimp
            var importer = new Assimp.AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
            var assimpScene = importer.ImportFile(path, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.CalculateTangentSpace | Assimp.PostProcessSteps.JoinIdenticalVertices);


            var skeleton = new Skeleton();

            switch (args.loadingMode)
            {
                case SkeletonLoadingMode.Full:
                    AddBonesFromNode(assimpScene.RootNode, skeleton);
                    break;
                case SkeletonLoadingMode.MeshBones:
                    AddMeshBones(assimpScene, skeleton);
                    break;
                case SkeletonLoadingMode.AnimationBones:
                    AddAnimationBones(assimpScene, skeleton);
                    break;
                case SkeletonLoadingMode.MeshAndAnimationBones:
                    AddMeshBones(assimpScene, skeleton);
                    AddAnimationBones(assimpScene, skeleton);
                    break;
                default:
                    AddBonesFromNode(assimpScene.RootNode, skeleton);
                    break;
            }

            return skeleton as T;
        }

        public void AddBonesFromNode(Node node, Skeleton skeleton)
        {
            String nodeName = node.Name;
            if (!skeleton.BoneInfoMap.ContainsKey(nodeName))
            {
                var boneInfo = new BoneInfo();
                boneInfo.id = skeleton.BoneCounter;
                boneInfo.offset = Matrix4.Identity;
                skeleton.BoneInfoMap[nodeName] = boneInfo;
            }
            foreach (var child in node.Children)
            {
                AddBonesFromNode(child, skeleton);
            }
        }

        public void AddMeshBones(Assimp.Scene assimpScene, Skeleton skeleton)
        {
            foreach (var mesh in assimpScene.Meshes)
            {
                for (int boneIndex = 0; boneIndex < mesh.BoneCount; boneIndex++)
                {
                    var boneName = mesh.Bones[boneIndex].Name;
                    if (!skeleton.BoneInfoMap.ContainsKey(boneName))
                    {
                        var boneInfo = new BoneInfo();
                        boneInfo.id = skeleton.BoneCounter;
                        boneInfo.offset = Math.Math.ToTKMatrix(mesh.Bones[boneIndex].OffsetMatrix);
                        skeleton.BoneInfoMap.Add(boneName, boneInfo);
                        skeleton.BoneCounter++;
                    }
                }
            }
        }

        public void AddAnimationBones(Assimp.Scene assimpScene, Skeleton skeleton)
        {
            foreach (var animation in assimpScene.Animations)
            {
                foreach (var channel in animation.NodeAnimationChannels)
                {
                    var boneName = channel.NodeName;
                    if (!skeleton.BoneInfoMap.ContainsKey(boneName))
                    {
                        var boneInfo = new BoneInfo();
                        boneInfo.id = skeleton.BoneCounter;
                        boneInfo.offset = Matrix4.Identity;
                        skeleton.BoneInfoMap.Add(boneName, boneInfo);
                        skeleton.BoneCounter++;
                    }
                }
            }
        }
    }
}
