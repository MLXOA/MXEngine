using MXEngine.Core;
using MXEngine.Game.Data;

namespace MXEngine;

/// <summary>
/// The container for all components.
/// </summary>
public sealed class Entity : IDisposable
{
    [ThreadStatic] private static List<Entity>? _entities;

    /// <summary>
    /// Initialize the list of Entities.
    /// </summary>
    internal static void InitializeRegistry()
    {
        _entities = new List<Entity>();
    }

    public Entity()
    {
        if (_entities == null)
        {
            InitializeRegistry();
        }
        // Add this Entity to the registry. If the registry doesn't exist, this will throw an error.
        _entities!.Add(this);
    }

    /// <summary>
    /// Dispose of the entity. This is called when GC collects the entity.
    /// </summary>
    ~Entity()
    {
        Dispose();
    }

    /// <summary>
    /// Remove this Entity from the registry.
    /// </summary>
    public void Dispose()
    {
        _entities!.Remove(this);
    }
    
    #region Properties
    
    /// <summary>
    /// The name used for locating this object and showing it in the scene hierarchy.
    /// </summary>
    public string Name = "";
    
    /// <summary>
    /// The location of the GameObject.
    /// </summary>
    public Location Location = new Location();
    
    /// <summary>
    /// The parent of this entity, can be null.
    /// </summary>
    public Entity? Parent = null;
    
    #endregion
}