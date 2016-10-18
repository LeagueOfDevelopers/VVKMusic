using System;
using System.Windows.Media;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Wma;
using Un4seen.Bass.Misc;
using Common;
using Status = Common.Common.Status;

namespace Player
{
    public class Player : IPlayer
    {
        private int _stream;
        private BASSTimer _updateTimer = new BASSTimer(50);

        public Player()
        {
            BassNet.Registration("xxxddr3@gmail.com", "2X441017152222");
            if(!(Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero)))
            {
                throw new Exception("Error while initializing Player instance");
            }
        }
        public int Stream
        {
            get { return _stream; }
        }
        public Status SetSource(Song playedSong)
        {
            if (playedSong.Downloaded)
            {
                _stream = Bass.BASS_StreamCreateFile(playedSong.DownloadedUri.LocalPath, 0, 0, BASSFlag.BASS_DEFAULT);
            }
            else
            {
                _stream = Bass.BASS_StreamCreateURL(playedSong.Uri.ToString(), 0, 0, null, new IntPtr(0));
            }
            if (_stream != 0)
            {
                return Status.Ok;
            }
            else
            {
                if (playedSong.Downloaded)
                {
                    playedSong.Downloaded = false;
                    _stream = Bass.BASS_StreamCreateURL(playedSong.Uri.ToString(), 0, 0, null, new IntPtr(0));
                }
                if (_stream != 0)
                {
                    return Status.Ok;
                }
                else
                {
                    return Status.Error;
                }
            }
        }
        public Status PlayAndStartTimer()
        {
            _updateTimer.Start();
            if(Bass.BASS_ChannelPlay(_stream, false))
            {
                return Status.Ok;
            }
            else
            {
                return Status.Error;
            }
        }
        public Boolean IsTimerStarted()
        {
            if (_updateTimer.Enabled)
                return true;
            else
                return false;
        }
        public Status StopAndStopTimer()
        {
            _updateTimer.Stop();
            if (Bass.BASS_ChannelStop(_stream))
            {
                return Status.Ok;
            }
            else
            {
                return Status.Error;
            }
        }
        public Status PauseAndStopTimer()
        {
            _updateTimer.Stop();
            if (Bass.BASS_ChannelPause(_stream))
            {
                return Status.Ok;
            }
            else
            {
                return Status.Error;
            }
        }
        public Status AdjustSound()
        {
            return Status.Ok;
        }
        public Status SetTimer(int updateInterval, EventHandler e)
        {
            _updateTimer = new BASSTimer(updateInterval);
            _updateTimer.Tick += e;
            return Status.Ok;
        }
        ~Player()
        {
            Bass.BASS_StreamFree(_stream);
            Bass.BASS_Free();
        }
    }
}
