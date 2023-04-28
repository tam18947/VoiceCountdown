using NAudio.Wave;

namespace VoiceCountdown
{
    internal class AudioPlayer
    {
        internal AudioPlayer(string audioPath, int index, int volume = 100)
        {
            if (Path.GetExtension(audioPath) != ".mp3" &&
                Path.GetExtension(audioPath) != ".wav")
            {
                return;
            }

            audioReader = new AudioFileReader(audioPath);
            if (audioReader != null)
            {
                waveOut = new WaveOut
                {
                    DeviceNumber = index
                };
                waveOut.Init(audioReader);
                audioReader.Volume = volume * 0.01f;
            }
        }

        private readonly WaveOut? waveOut = null;
        private readonly AudioFileReader? audioReader = null;

        public void Play()
        {
            if (null == audioReader)
            {
                return;
            }

            if (waveOut is not null && waveOut.PlaybackState != PlaybackState.Playing)
            {
                waveOut.Play();
            }
        }
        public void Stop()
        {
            if (null == audioReader)
            {
                return;
            }

            if (waveOut is not null && waveOut.PlaybackState != PlaybackState.Stopped)
            {
                waveOut.Stop();
                audioReader.Dispose();
                waveOut.Dispose();
            }
        }

        public static List<string> GetDevices()
        {
            List<string> deviceList = new();
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var capabilities = WaveOut.GetCapabilities(i);
                deviceList.Add(capabilities.ProductName);
            }
            return deviceList;
        }
    }
}
