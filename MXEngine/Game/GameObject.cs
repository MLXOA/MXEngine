using MXEngine.Core;
using MXEngine.Game.Data;

namespace MXEngine.Game;

public abstract class GameObject
{
    /// <summary>
    /// The location of the GameObject.
    /// </summary>
    public Location Location = new Location();
    
    public static T Create<T>() where T : GameObject, new()
    {
        T gameObject = new();
        ObjectInstance.Register(gameObject);
        return gameObject;
    }
}