using System.IO.Compression;
using System.Reflection;

namespace MXEngine.Core;

public static class Resources
{
    public static Stream? GetStream(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        return assembly.GetManifestResourceStream(resourceName);
    }

    public static Stream? GetDecompressedStream(string resourceName)
    {
        Stream? resourceStream = GetStream(resourceName);
        if (resourceStream == null) return null;
        using (var compressedMemoryStream = new MemoryStream())
        {
            resourceStream.CopyTo(compressedMemoryStream);
            compressedMemoryStream.Position = 0; // Reset position

            var decompressedMemoryStream = new MemoryStream();
            using (var brotliStream = new BrotliStream(compressedMemoryStream, CompressionMode.Decompress))
            {
                brotliStream.CopyTo(decompressedMemoryStream);
                decompressedMemoryStream.Position = 0; // Reset position
                return decompressedMemoryStream;
            }
        }
    }
}