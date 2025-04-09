﻿using LibGFX.Math;
using Newtonsoft.Json.Linq;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace LibGFX.Graphics
{
    /// <summary>
    /// Represents a cubemap texture
    /// </summary>
    public enum CubemapFlags
    {
        None,
        Loaded,
        Initialized,
        Disposed,
        Failed
    }

    /// <summary>
    /// Represents a cubemap texture
    /// </summary>
    public class Cubemap
    {
        /// <summary>
        /// The faces of the cubemap
        /// </summary>
        public List<byte[]> Faces { get; set; }

        /// <summary>
        /// The OpenGL texture ID of the cubemap
        /// </summary>
        public int TextureId { get; set; }

        /// <summary>
        /// The width of the cubemap
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the cubemap
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The flags of the cubemap
        /// </summary>
        public CubemapFlags Flags { get; set; }

        /// <summary>
        /// Creates a new cubemap
        /// </summary>
        public Cubemap()
        {
            this.Faces = new List<byte[]>();
        }

        /// <summary>
        /// Loads a cubemap from the specified paths
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="swapYAxisFaces"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static Cubemap LoadCubemap(String[] paths, bool swapYAxisFaces = true)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            
            Cubemap cubemap = new Cubemap();
            foreach (var path in paths)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Cubemap face not found: {path}");
                }

                ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
                cubemap.Faces.Add(image.Data);
                cubemap.Width = image.Width;
                cubemap.Height = image.Height;
            }

            if(swapYAxisFaces)
            {
                var temp = cubemap.Faces[2];
                cubemap.Faces[2] = cubemap.Faces[3];
                cubemap.Faces[3] = temp;
            }

            cubemap.Flags = CubemapFlags.Loaded;
            return cubemap;
        }

        /// <summary>
        /// Loads a cubemap from a JSON file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="swapYAxisFaces"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Cubemap LoadCubemap(string file, bool swapYAxisFaces = true)
        {
            // Check if the file exists
            if (!File.Exists(file))
            {
                throw new ArgumentException($"Cubemap file '{file}' does not exist.");
            }

            // Check if the file is a JSON file
            if (Path.GetExtension(file).ToLower() != ".json")
            {
                throw new ArgumentException($"Cubemap file '{file}' is not a JSON file.");
            }

            // Load the JSON file and parse it
            var basePath = Path.GetDirectoryName(file);
            var jsonString = File.ReadAllText(file);
            var jsonObject = JObject.Parse(jsonString);

            // Create the faces list and load the cubemap
            var faces = new List<string>();
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["px"].Value<string>()));
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["nx"].Value<string>()));
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["py"].Value<string>()));
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["ny"].Value<string>()));
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["pz"].Value<string>()));
            faces.Add(Path.Combine(basePath, jsonObject["cubemap"]["nz"].Value<string>()));
            var cubemap = LoadCubemap(faces.ToArray(), swapYAxisFaces);
            return cubemap;
        }
    }
}
