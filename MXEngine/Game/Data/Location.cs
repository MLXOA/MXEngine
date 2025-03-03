using MXEngine.Core;
using System.Numerics;

namespace MXEngine.Game.Data;

/// <summary>
/// Coordinates, and more!
/// </summary>
public class Location
{
    public Vector3 Position = new(0, 0, 0);
    public Vector3 Scale = new(1, 1, 1);

    public Vector3 Rotation = new(0, 0, 0);
    public Quaternion Orientation => CreateFromEulerAngles(Rotation);

    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Orientation);
    public Vector3 Right => Vector3.Normalize(Vector3.Cross(Forward, Up));
    public Vector3 Direction => Position + Forward;
    public Vector3 Up = Vector3.UnitY;
    public Vector3 Down => -Up;
    public Vector3 Left => -Right;
    public Vector3 Back => -Forward;

    private static Quaternion CreateFromEulerAngles(Vector3 eulerAngles)
    {
        float pitch = MathHelper.DegreesToRadians(eulerAngles.X);
        float yaw = MathHelper.DegreesToRadians(eulerAngles.Y);
        float roll = MathHelper.DegreesToRadians(eulerAngles.Z);

        return Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
    }
}