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

    public static BrotliStream? GetBrotliStream(string resourceName)
    {
        Stream? resourceStream = GetStream(resourceName);
        if (resourceStream == null) return null;
        return new BrotliStream(resourceStream, CompressionMode.Decompress);
    }
}