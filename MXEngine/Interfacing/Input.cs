using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using MXEngine.Core;

namespace MXEngine.Interfacing;

/// <summary>
/// Input events and methods.
/// </summary>
public class Input
{
    private static IWindow? _window;
    private static IInputContext? _inputContext;
    internal static Input? Instance;
    internal Input(IWindow window)
    {
        Engine._logger.Info("Input system created.");
        Instance = this;
        _window = window;
        Engine._logger.Info("Creating input context.");
        CreateInputContext();
    }

    internal static void Create(IWindow window)
    {
        Instance = new Input(window);
    }

    /// <summary>
    /// Create the IInputContext and register events.
    /// </summary>
    void CreateInputContext()
    {
        _inputContext = _window!.CreateInput();
        _inputContext.ConnectionChanged += InputContextOnConnectionChanged;
        
        // register mouse-related events, only one mouse is supported (since most operating systems only support one mouse)
        var mouse = _inputContext.Mice.FirstOrDefault();
        if (mouse != null)
        {
            mouse.MouseMove += (_, moveDelta) =>
            {
                MouseMove?.Invoke(new Vector2D<float>(moveDelta.X, moveDelta.Y));
            };
            mouse.Scroll += (_, wheel) =>
            {
                MouseWheel?.Invoke(new Vector2D<float>(wheel.X, wheel.Y));
            };
        }
        
        // register keyboard-related events.
        foreach (IKeyboard keyboard in _inputContext.Keyboards)
        {
            if (keyboard.IsConnected)
            {
                keyboard.KeyChar += (_, c) =>
                {
                    KeyTyped?.Invoke(c);
                };
                keyboard.KeyDown += (_, key, _) =>
                {
                    KeyDown?.Invoke(key);
                };
                keyboard.KeyUp += (_, key, _) =>
                {
                    KeyUp?.Invoke(key);
                };
            }
        }
        
        // register gamepad-related events
        foreach (IGamepad gamepad in _inputContext.Gamepads)
        {
            if (gamepad.IsConnected)
            {
                gamepad.ButtonDown += (_, button) =>
                {
                    GamepadButtonDown?.Invoke(gamepad.Index, button);
                };
                gamepad.ButtonUp += (_, button) =>
                {
                    GamepadButtonUp?.Invoke(gamepad.Index, button);
                };
            }
        }
    }

    private void InputContextOnConnectionChanged(IInputDevice arg1, bool arg2)
    {
        Engine._logger.Info("Device list changed, recreating input context.");
        // recreate the input context, the device list has updated. sadly this will cause some input loss and overall a minor delay.
        _inputContext!.Dispose();
        CreateInputContext();
    }

    public static void SetCursorMode(CursorMode mode)
    {
        if (_window == null) return;
        var mice = GetMice();
        
        if (mice == null) return;
        bool flag = false;
        foreach (IMouse mouse in mice)
        {
            if (mouse.Cursor.CursorMode != mode) flag = true;
            mouse.Cursor.CursorMode = mode;
        }
    }

    public static IMouse[]? GetMice()
    {
        return _inputContext?.Mice.ToArray();
    }

    public static IKeyboard? GetKeyboard()
    {
        return _inputContext?.Keyboards.FirstOrDefault();
    }

    /// <summary>
    /// Event for when the user's pointer moves on the game window.
    /// </summary>
    public static event Action<Vector2D<float>>? MouseMove;
    /// <summary>
    /// Event for when the user scrolls on the game window.
    /// </summary>
    public static event Action<Vector2D<float>>? MouseWheel;
    /// <summary>
    /// Event for when a key is pressed.
    /// </summary>
    public static event Action<Key>? KeyDown;
    /// <summary>
    /// Event for when a key is released.
    /// </summary>
    public static event Action<Key>? KeyUp;
    /// <summary>
    /// Event for when a character is received for a key.
    /// </summary>
    public static event Action<char>? KeyTyped;
    /// <summary>
    /// Event for when a gamepad button is pressed.
    /// The gamepad index is the first argument.
    /// </summary>
    public static event Action<int, Button>? GamepadButtonDown;
    /// <summary>
    /// Event for when a gamepad button is released.
    /// The gamepad index is the first argument.
    /// </summary>
    public static event Action<int, Button>? GamepadButtonUp;
}