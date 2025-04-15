/*
 * LOCATION: Core/MXEngine/Entity.cs
 * TITLE: Base Entity Class
 *
 * AUTHORS:
 *   Nathan Lee
 */

using System.Collections;
using MXEngine.Game.Data;

namespace MXEngine;

/// <summary>
/// The container for all components. This class can be enumerated to get all the child components.
/// </summary>
public sealed class Entity : IDisposable, IEnumerable<Component>
{
    [ThreadStatic] private static List<Entity>? _entities;

    #region Static Methods

    /// <summary>
    /// Initialize the list of Entities.
    /// </summary>
    internal static void InitializeRegistry()
    {
        _entities = new List<Entity>();
    }

    #endregion

    #region Methods

    public void Render()
    {
        foreach (var comp in Components)
        {
            comp.Render();
        }
    }

    // Update all the components in parallel.
    // This cannot be brought over to rendering, since you cannot access OpenGL from multiple threads.
    public void Update()
    {
        Parallel.ForEach(Components, (Component c, ParallelLoopState state, long index) => { c.Update(); });
    }

    public T AddComponent<T>() where T : Component, new()
    {
        T component = new();
        component.Parent = this;
        return component;
    }

    public T? GetFirstComponent<T>() where T : Component
    {
        foreach (var component in this)
        {
            if (component is T comp)
            {
                return comp;
            }
        }

        return null;
    }

    public List<T> GetComponentsOfType<T>() where T : Component
    {
        return this.Where(x => x is T).Cast<T>().ToList();
    }

    #endregion

    #region Base

    public Entity()
    {
        if (_entities == null)
        {
            InitializeRegistry();
        }

        // Add this Entity to the registry. If the registry doesn't exist, this will throw an error.
        _entities!.Add(this);
    }

    public IEnumerator<Component> GetEnumerator()
    {
        return Components.Where(x => x.Parent == this).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Dispose of the entity. This is called when GC collects the entity.
    /// </summary>
    ~Entity()
    {
        Dispose();
    }

    /// <summary>
    /// Remove this entity from the registry.
    /// </summary>
    public void Dispose()
    {
        _entities!.Remove(this);
    }

    #endregion

    #region Properties

    /// <summary>
    /// The name used for locating this object and showing it in the scene hierarchy.
    /// </summary>
    public string Name = "";

    /// <summary>
    /// The location of the entity.
    /// </summary>
    public Location Location = new Location();

    /// <summary>
    /// The parent of this entity, can be null.
    /// </summary>
    public Entity? Parent = null;

    /// <summary>
    /// The list of components that are attached to this entity, used internally. The game should enumerate the entity to get the components.
    /// </summary>
    internal List<Component> Components = new();

    #endregion
}