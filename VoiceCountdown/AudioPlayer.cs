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
                waveOutEvent = new WaveOutEvent
                {
                    DeviceNumber = index
                };
                waveOutEvent.Init(audioReader);
                waveOutEvent.Volume = volume * 0.01f;
            }
        }
        internal AudioPlayer(Stream audioStream, int index, int volume = 100)
        {
            waveReader = new WaveFileReader(audioStream);
            if (waveReader != null)
            {
                waveOutEvent = new WaveOutEvent
                {
                    DeviceNumber = index
                };
                waveOutEvent.Init(waveReader);
                waveOutEvent.Volume = volume * 0.01f;
            }
        }
        private readonly WaveFileReader? waveReader = null;

        private readonly WaveOutEvent? waveOutEvent = null;
        private readonly AudioFileReader? audioReader = null;

        public void Play()
        {
            if (null == waveOutEvent)
            {
                return;
            }

            if (waveOutEvent is not null && waveOutEvent.PlaybackState != PlaybackState.Playing)
            {
                waveOutEvent.Play();
            }
        }
        public void Stop()
        {
            //if (null == audioReader)
            //{
            //    return;
            //}

            if (waveOutEvent is not null && waveOutEvent.PlaybackState != PlaybackState.Stopped)
            {
                waveOutEvent.Stop();
                audioReader?.Dispose();
                waveReader?.Dispose();
                waveOutEvent.Dispose();
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
