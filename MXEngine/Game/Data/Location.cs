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

    private Quaternion _orientation = Quaternion.Identity;
    public Quaternion Orientation
    {
        get
        {
            return _orientation;
        }
        set
        {
            _orientation = value;
        }
    }

    public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Orientation);
    public Vector3 Right => Vector3.Normalize(Vector3.Cross(Forward, Up));
    public Vector3 Direction => Position + Forward;

    public Vector3 Up
    {
        get => Vector3.Transform(Vector3.UnitY, Orientation);
        set
        {
            // 1. Get the current forward direction
            Vector3 currentForward = Vector3.Transform(Vector3.UnitZ, Orientation);
            currentForward = Vector3.Normalize(currentForward); // Ensure it's normalized

            // 2. Normalize the new up vector
            Vector3 normalizedNewUp = Vector3.Normalize(value);

            // Handle the case where the new up is parallel or anti-parallel to the current forward
            // In such cases, a stable cross product is not guaranteed.
            if (Math.Abs(Vector3.Dot(currentForward, normalizedNewUp)) > 0.999f)
            {
                // Find an arbitrary vector not aligned with the forward vector
                Vector3 fallbackAxis = Math.Abs(Vector3.Dot(currentForward, Vector3.UnitX)) < 0.9f ? Vector3.UnitX : Vector3.UnitY;
                Vector3 newRight = Vector3.Normalize(Vector3.Cross(fallbackAxis, currentForward));
                Vector3 newUpComp = Vector3.Normalize(Vector3.Cross(currentForward, newRight));

                // If the desired newUp is in the opposite direction, rotate 180 degrees around the forward
                if (Vector3.Dot(currentForward, normalizedNewUp) < 0)
                {
                    Orientation = Quaternion.CreateFromAxisAngle(currentForward, MathF.PI) * Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateWorld(Vector3.Zero, currentForward, newUpComp));
                }
                else
                {
                    Orientation = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateWorld(Vector3.Zero, currentForward, newUpComp));
                }
            }
            else
            {
                // 3. Calculate the new right vector (orthogonal to forward and new up)
                Vector3 newRight = Vector3.Normalize(Vector3.Cross(normalizedNewUp, currentForward));
    
                // 4. Recalculate the forward vector to ensure orthogonality (optional but recommended)
                Vector3 newForward = Vector3.Normalize(Vector3.Cross(newRight, normalizedNewUp));
    
                // 5. Create a rotation matrix from the new basis vectors
                Matrix4x4 rotationMatrix = new Matrix4x4(
                    newRight.X, newRight.Y, newRight.Z, 0,
                    normalizedNewUp.X, normalizedNewUp.Y, normalizedNewUp.Z, 0,
                    newForward.X, newForward.Y, newForward.Z, 0,
                    0, 0, 0, 1
                );

                // 6. Convert the rotation matrix to a quaternion
                Quaternion newRotation = Quaternion.CreateFromRotationMatrix(rotationMatrix);
                Orientation = newRotation;
            }
        }
    }
    
    
    
    /// <summary>
    /// Convert the orientation to euler angles. The returned Vector3 are radian values of Roll (X), Pitch (Y), and Yaw (Z).
    /// </summary>
    /// <remarks>This is a C# version of the <a href="https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles#Source_code_2">code found on Wikipedia</a>.</remarks>
    /// <returns></returns>
    public Vector3 ToEulerAngles(bool degrees = false)
    {
        Quaternion q = _orientation;
        Vector3 eulerAngles = Vector3.Zero;
        
        double sinrCosp = 2 * (q.W * q.X + q.Y * q.Z);
        double cosrCosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
        eulerAngles.Z = (float)Math.Atan2(sinrCosp, cosrCosp);

        double sinp = Math.Sqrt(1 + 2 * (q.W * q.Y - q.X * q.Z));
        double cosp = Math.Sqrt(1 - 2 * (q.W * q.Y - q.X * q.Z));
        eulerAngles.Y = (float)(2 * Math.Atan2(sinp, cosp) - Math.PI / 2);

        double sinyCosp = 2 * (q.W * q.Z + q.X * q.Y);
        double cosyCosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
        eulerAngles.X = (float)Math.Atan2(sinyCosp, cosyCosp);

        if (degrees)
        {
            eulerAngles *= 180 / MathF.PI;
        }

        return eulerAngles;
    }

    public static Quaternion FromEulerAngles(Vector3 eulerAngles, bool degrees = false)
    {
        if (degrees)
        {
            eulerAngles *= MathF.PI/180;
        }
        
        // Roll (X)
        float cr = MathF.Cos(eulerAngles.X * 0.5f);
        float sr = MathF.Sin(eulerAngles.X * 0.5f);

        // Pitch (Y)
        float cp = MathF.Cos(eulerAngles.Y * 0.5f);
        float sp = MathF.Sin(eulerAngles.Y * 0.5f);

        // Yaw (Z)
        float cy = MathF.Cos(eulerAngles.Z * 0.5f);
        float sy = MathF.Sin(eulerAngles.Z * 0.5f);
        
        // Calculate
        Quaternion q = new();
        q.W = cr * cp * cy + sr * sp * sy;
        q.X = sr * cp * cy - cr * sp * sy;
        q.Y = cr * sp * cy + sr * cp * sy;
        q.Z = cr * cp * sy - sr * sp * cy;

        return q;
    }
}