using System.Drawing;
using System.Numerics;
using MXEngine.Core;
using MXEngine.Interfacing;
using MXEngine.Game.Data;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace MXEngine.Graphics.Core;

public class Camera
{
    public Location Location = new Location();
    public CameraMode CameraMode = CameraMode.None;
    public bool Is3D = true;

    private Shader Shader;
    private Tex2D Texture;
    private Model Model;
    private GL Gl;
    IKeyboard? primaryKeyboard;

    internal Camera()
    {
        primaryKeyboard = Input.GetKeyboard();
        Gl = Engine.Instance.Gl;
        Shader = new Shader(Gl, Resources.GetStream("MXEngine.Graphics.DefaultResources.Shader.vert")!, Resources.GetStream("MXEngine.Graphics.DefaultResources.Shader.frag"));
        Texture = new Tex2D(Gl, Resources.GetStream("MXEngine.Graphics.DefaultResources.MissingTexture.png")!);
        Model = new Model(Gl, Resources.GetStream("MXEngine.Graphics.DefaultResources.Cube.obj")!);
    }

    public void Initialize()
    {
        Location.Position = new Vector3(0, 0, 5);
        Input.MouseMove += InputOnMouseMove;
    }
    
    Vector2D<float> LastMousePosition;
    private float yaw = 0;
    private float pitch = 0;
    
    private void InputOnMouseMove(Vector2D<float> position)
    {
        if (CameraMode != CameraMode.Freecam) return;
        
        var lookSensitivity = 0.25f;
        if (LastMousePosition == default)
        {
            LastMousePosition = position;
        }
        else
        {
            var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
            var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
            LastMousePosition = position;

            yaw -= xOffset;
            pitch += yOffset;

            Location.Orientation = Quaternion.CreateFromYawPitchRoll(MathHelper.DegreesToRadians(yaw), MathHelper.DegreesToRadians(pitch), 0);
        }
    }

    public void Render(double deltaTime)
    {
        Gl.Enable(EnableCap.DepthTest);
        //Gl.Enable(EnableCap.CullFace);
        //Gl.CullFace(TriangleFace.Back);
        Gl.ClearColor(Color.FromArgb(255, 157, 193, 227));
        Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Texture.Bind();
        Shader.Use();
        Shader.SetUniform("uTexture0", 0);

        var size = Engine.Instance.Size;

        var model = Matrix4x4.Identity;
        var view = Matrix4x4.CreateLookAt(Location.Position, Location.Position + Location.Forward, Location.Up);
        //Note that the aspect ratio calculation must be performed as a float, otherwise integer division will be performed (truncating the result).
        var projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(80), (float)size.X / (float)size.Y, 0.1f, 1000.0f);

        foreach (var mesh in Model.Meshes)
        {
            mesh.Bind();
            Shader.Use();
            Texture.Bind();
            Shader.SetUniform("uTexture0", 0);
            Shader.SetUniform("uModel", model);
            Shader.SetUniform("uView", view);
            Shader.SetUniform("uProjection", projection);

            Gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.Vertices.Length);
        }
    }

    public void Update(float deltaTime)
    {
        float moveSpeed = 10;
        if (CameraMode == CameraMode.Freecam)
        {
            Input.SetCursorMode(CursorMode.Raw);
            if (primaryKeyboard != null)
            {
                Vector3 delta = new(0, 0, 0);
                if (primaryKeyboard.IsKeyPressed(Key.W))
                {
                    //Move forwards
                    delta += Location.Forward * deltaTime * moveSpeed;
                }
                if (primaryKeyboard.IsKeyPressed(Key.S))
                {
                    //Move backwards
                    delta -= Location.Forward * deltaTime * moveSpeed;
                }
                if (primaryKeyboard.IsKeyPressed(Key.A))
                {
                    //Move left
                    delta += -Location.Right * deltaTime * moveSpeed;
                }
                if (primaryKeyboard.IsKeyPressed(Key.D))
                {
                    //Move right
                    delta += Location.Right * deltaTime * moveSpeed;
                }

                if (primaryKeyboard.IsKeyPressed(Key.E))
                {
                    delta += Location.Up * deltaTime * moveSpeed;
                }

                if (primaryKeyboard.IsKeyPressed(Key.Q))
                {
                    delta -= Location.Up * deltaTime * moveSpeed;
                }

                Location.Position += delta;
            }
        }
    }
}

public enum CameraMode
{
    None,
    Freecam
}