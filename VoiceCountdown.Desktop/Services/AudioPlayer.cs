using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using NAudio.Wave;

namespace VoiceCountdown.Desktop.Services;

/// <summary>
/// 埋め込みの WAV 音声を 1 つずつ再生する。新しい音声を再生すると、再生中の音声は止める。
/// </summary>
public sealed class AudioPlayer : IDisposable
{
    /// <summary>既定の出力デバイスを表す番号。</summary>
    public const int DefaultDevice = -1;

    private WaveOutEvent? waveOut;
    private WaveStream? reader;
    private Stream? source;

    /// <summary>
    /// 出力デバイス名の一覧。インデックスがデバイス番号（WaveOutEvent.DeviceNumber）になる。
    /// NAudio 2.x の WaveOut クラスは NAudio.WinForms にあるため、winmm.dll を直接呼ぶ。
    /// </summary>
    public static IReadOnlyList<string> GetDeviceNames()
    {
        int count = NativeMethods.waveOutGetNumDevs();
        var list = new List<string>(count);

        for (int i = 0; i < count; i++)
        {
            var caps = new NativeMethods.WaveOutCaps();
            int result = NativeMethods.waveOutGetDevCaps((nuint)i, ref caps, Marshal.SizeOf<NativeMethods.WaveOutCaps>());
            list.Add(result == 0 ? caps.ProductName : $"デバイス {i + 1}");
        }

        return list;
    }

    /// <summary>
    /// "Sounds.{soundKey}.wav" として埋め込んだ音声を再生する。
    /// </summary>
    public void Play(string soundKey, int deviceNumber)
    {
        Stop();

        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Sounds.{soundKey}.wav")
            ?? throw new InvalidOperationException($"音声 {soundKey} が見つかりません。");

        source = stream;
        reader = new WaveFileReader(stream);
        waveOut = new WaveOutEvent { DeviceNumber = deviceNumber };
        waveOut.Init(reader);
        waveOut.Play();
    }

    public void Stop()
    {
        waveOut?.Dispose();   // Dispose で再生も止まる
        reader?.Dispose();
        source?.Dispose();
        waveOut = null;
        reader = null;
        source = null;
    }

    public void Dispose() => Stop();

    private static class NativeMethods
    {
        [DllImport("winmm.dll")]
        public static extern int waveOutGetNumDevs();

        [DllImport("winmm.dll", EntryPoint = "waveOutGetDevCapsW", CharSet = CharSet.Unicode)]
        public static extern int waveOutGetDevCaps(nuint deviceId, ref WaveOutCaps caps, int size);

        /// <summary>WAVEOUTCAPSW 構造体。</summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WaveOutCaps
        {
            public ushort ManufacturerId;
            public ushort ProductId;
            public uint DriverVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string ProductName;

            public uint Formats;
            public ushort Channels;
            public ushort Reserved;
            public uint Support;
        }
    }
}
