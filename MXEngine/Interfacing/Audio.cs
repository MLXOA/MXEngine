using MeltySynth;
using MXEngine.Audio;
using MXEngine.Core;
using Silk.NET.OpenAL;

namespace MXEngine.Interfacing;

public unsafe class Audio
{
    public static ALContext AlContext = ALContext.GetApi(true);
    public static AL Al = AL.GetApi(true);

    internal static Device* AlDevice;
    internal static Context* AlDeviceContext;
    
    internal static List<MidiPlayer?> MidiPlayers = new();
    
    private static SoundFont? _defaultSoundFont;

    public static SoundFont DefaultSoundfont
    {
        get
        {
            if (_defaultSoundFont == null)
            {
                _defaultSoundFont = new(Resources.GetDecompressedStream("MXEngine.Audio.DefaultResources.SC-55.sf2.br")!);
            }

            return _defaultSoundFont;
        }
    }

    public static MidiPlayer CreateMidiPlayer(SoundFont soundFont)
    {
        MidiPlayer midi = new(Al, soundFont);
        MidiPlayers.Add(midi);
        return midi;
    }

    internal static void Initialize()
    {
        AlDevice = AlContext.OpenDevice("");
        AlDeviceContext = AlContext.CreateContext(AlDevice, null);
        AlContext.MakeContextCurrent(AlDeviceContext);
        Al.GetError();
    }

    internal static void Dispose()
    {
        foreach (MidiPlayer? midi in MidiPlayers)
        {
            if (midi != null)
            {
                midi.Stop();
                midi.Dispose();
            }
        }
        AlContext.Dispose();
        Al.Dispose();
        _defaultSoundFont = null;
    }
}