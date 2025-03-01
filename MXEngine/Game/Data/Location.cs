using System.Numerics;

namespace MXEngine.Game.Data;

/// <summary>
/// Coordinates, and more!
/// </summary>
public class Location
{
    public Vector3 Position = new(0, 0, 0);
    
    /// <summary>
    /// Whether the engine uses a Vector3 or Quaternion for rotation/orientation.
    /// </summary>
    public bool UseOrientation = true;
    public Vector3 Rotation = new(0, 0, 0);
    public Quaternion Orientation = new(0, 0, 0, 0);
}