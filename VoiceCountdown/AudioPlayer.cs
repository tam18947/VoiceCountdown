using NAudio.Wave;

namespace VoiceCountdown;

/// <summary>
/// WAV 音声を 1 つずつ再生する。新しい音声を再生すると、再生中の音声は止めて破棄する。
/// </summary>
internal sealed class AudioPlayer : IDisposable
{
    /// <summary>既定の出力デバイスを表す番号。</summary>
    public const int DefaultDevice = -1;

    private WaveOutEvent? waveOut;
    private WaveStream? reader;
    private Stream? source;

    /// <summary>
    /// 音声を再生する。<paramref name="wavStream"/> の破棄はこのクラスが引き受ける。
    /// </summary>
    public void Play(Stream wavStream, int deviceNumber)
    {
        Stop();

        source = wavStream;
        reader = new WaveFileReader(wavStream);
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

    /// <summary>出力デバイス名の一覧。インデックスがデバイス番号になる。</summary>
    public static IReadOnlyList<string> GetDeviceNames()
    {
        int count = WaveOut.DeviceCount;
        var list = new string[count];

        for (int i = 0; i < count; i++)
            list[i] = WaveOut.GetCapabilities(i).ProductName;

        return list;
    }

    public void Dispose() => Stop();
}
