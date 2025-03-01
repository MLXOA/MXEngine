using MXEngine.Game;

namespace MXEngine.Core;

public static class ObjectInstance
{
    [ThreadStatic] private static List<GameObject>? _gameObjects;

    /// <summary>
    /// Initialize the ObjectInstance "registry."
    /// </summary>
    internal static void Initialize()
    {
        _gameObjects = [];
    }

    public static void Register(GameObject gameObject)
    {
        if (_gameObjects == null)
        {
            Initialize();
        }
    }
}