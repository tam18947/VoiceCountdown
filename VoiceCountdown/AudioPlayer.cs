using NAudio.Wave;

namespace VoiceCountdown;

internal class AudioPlayer
{
    internal AudioPlayer(Stream audioStream, int deviceNumber, int volume = 100)
    {
        if (audioStream.Position > 0)
        {
            return;
        }
        waveReader = new WaveFileReader(audioStream);
        if (waveReader != null)
        {
            waveOutEvent = new WaveOutEvent
            {
                DeviceNumber = deviceNumber
            };
            waveOutEvent.Init(waveReader);
            waveOutEvent.Volume = volume > 100 ? 1 : volume * 0.01f;
        }
    }

    private readonly WaveOutEvent? waveOutEvent = null;
    private readonly WaveFileReader? waveReader = null;

    public void Play()
    {
        if (waveOutEvent is null)
        {
            return;
        }

        if (waveOutEvent.PlaybackState != PlaybackState.Playing)
        {
            waveOutEvent.Play();
        }
    }
    public void Stop()
    {
        if (waveOutEvent is not null &&
            waveOutEvent.PlaybackState != PlaybackState.Stopped)
        {
            waveOutEvent.Stop();
        }
        waveReader?.Dispose();
        waveOutEvent?.Dispose();
    }

    public static List<string> GetDevices()
    {
        List<string> deviceList = [];
        for (int i = 0; i < WaveOut.DeviceCount; i++)
        {
            var capabilities = WaveOut.GetCapabilities(i);
            deviceList.Add(capabilities.ProductName);
        }
        return deviceList;
    }
}
