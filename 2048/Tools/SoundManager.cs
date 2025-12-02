using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _2048.Tools
{
    public static class SoundManager
    {
        public static void PlaySound(string filePath)
        {
            var audioFile = new AudioFileReader(filePath);
            var outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);
            outputDevice.Play();

            outputDevice.PlaybackStopped += (s, e) =>
            {
                outputDevice.Dispose();
                audioFile.Dispose();
            };
        }
    }

}
