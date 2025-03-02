using System.Numerics;

namespace MXEngine.Game.Data;

/// <summary>
/// Coordinates, and more!
/// </summary>
public class Location
{
    public Vector3 Position = new(0, 0, 0);
    public Vector3 Scale = new(1, 1, 1);
    public Quaternion Orientation = Quaternion.Identity;

    public Vector3 Forward = -Vector3.UnitZ;
    public Vector3 Up = Vector3.UnitY;
    public Vector3 Direction = Vector3.Zero;
    
    public Matrix4x4 ViewMatrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Orientation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);
}