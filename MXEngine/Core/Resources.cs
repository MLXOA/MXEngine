using System.Reflection;

namespace MXEngine.Core;

public static class Resources
{
    public static Stream? GetStream(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        return assembly.GetManifestResourceStream(resourceName);
    }
}