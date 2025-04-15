/*
 * LOCATION: Core/MXEngine/Component.cs
 * TITLE: Base Entity Class
 *
 * AUTHORS:
 *   Nathan
 */

namespace MXEngine;

public class Component : IDisposable
{
    public void Dispose()
    {
        OnDestroy();
    }

    ~Component()
    {
        Dispose();
    }

    #region Properties

    /// <summary>
    /// The parent entity for this component. If set to null, this component will be destroyed.
    /// </summary>
    public Entity? Parent
    {
        get => _parent;
        set
        {
            if (value == _parent) return;
            _parent?.Components.Remove(this);
            _parent = value;
            if (_parent != null)
            {
                _parent?.Components.Add(this);
            }
            else
            {
                _destroyed = false;
                OnDestroy();
            }
        }
    }

    /// <summary>
    /// Whether this component is enabled or not, this is used to determine if the component should be updated and/or rendered.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (value == _enabled) return;
            _enabled = value;
            if (_enabled)
            {
                OnEnable();
            }
            else
            {
                OnDisable();
            }
        }
    }

    #endregion

    #region Overridable Methods

    /// <summary>
    /// The optional method for initializing this component, this is run once when the component is created and will not run again.
    /// </summary>
    public virtual void Awake()
    {
    }

    /// <summary>
    /// The optional method for updating this component, this is run every frame.
    /// </summary>
    public virtual void Update()
    {
    }

    /// <summary>
    /// The optional method for rendering this component.
    /// </summary>
    public virtual void Render()
    {
    }

    #region Events

    /// <summary>
    /// The optional event for when this component is re-enabled.
    /// </summary>
    public virtual void OnEnable()
    {
    }

    /// <summary>
    /// The optional event for when this component is disabled.
    /// </summary>
    public virtual void OnDisable()
    {
    }

    /// <summary>
    /// The optional event for when this component (or the parent entity) is destroyed.
    /// </summary>
    public virtual void OnDestroy()
    {
    }

    #endregion

    #endregion

    #region Fields

    internal bool _destroyed = false;
    internal bool _enabled = false;
    internal Entity? _parent = null;

    #endregion
}