using MXEngine.Game;

namespace MXEngine.Core;

public static class ObjectInstance
{
    [ThreadStatic] private static List<Entity>? _gameObjects;

    /// <summary>
    /// Initialize the ObjectInstance "registry."
    /// </summary>
    internal static void Initialize()
    {
        _gameObjects = [];
    }

    public static void Register(Entity entity)
    {
        if (_gameObjects == null)
        {
            Initialize();
        }
    }
}