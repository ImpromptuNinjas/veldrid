﻿using System;
using System.IO;
using System.Reflection;

namespace Veldrid.SampleGallery
{
    public static class ShaderUtil
    {
        public static Shader[] LoadEmbeddedShaderSet(Assembly assembly, ResourceFactory factory, string name)
        {
            string extension;
            switch (factory.BackendType)
            {
                case GraphicsBackend.Direct3D11:
                    extension = "hlsl";
                    break;
                case GraphicsBackend.Vulkan:
                    extension = "spv";
                    break;
                case GraphicsBackend.OpenGL:
                    extension = "glsl";
                    break;
                case GraphicsBackend.Metal:
                    extension = "metal";
                    break;
                case GraphicsBackend.OpenGLES:
                case GraphicsBackend.WebGL:
                    extension = "essl";
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported GraphicsBackend: {factory.BackendType}");
            }

            byte[] vsBytes = ReadEmbeddedBytes(assembly, $"{name}_Vertex.{extension}");
            byte[] fsBytes = ReadEmbeddedBytes(assembly, $"{name}_Fragment.{extension}");

            return new[]
            {
                factory.CreateShader(new ShaderDescription(ShaderStages.Vertex, vsBytes, "main")),
                factory.CreateShader(new ShaderDescription(ShaderStages.Fragment, fsBytes, "main")),
            };
        }

        private static byte[] ReadEmbeddedBytes(Assembly assembly, string name)
        {
            using (Stream s = assembly.GetManifestResourceStream(name))
            {
                byte[] bytes = new byte[s.Length];
                s.Read(bytes, 0, (int)s.Length);
                return bytes;
            }
        }
    }
}
