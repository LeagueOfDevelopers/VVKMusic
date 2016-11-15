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
        private float volume;

        public Player()
        {
            BassNet.Registration("xxxddr3@gmail.com", "2X441017152222");
            if (!(Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero)))
            {
                throw new Exception("Error while initializing Player instance");
            }
            Equalize();
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
            if (Bass.BASS_ChannelPlay(_stream, false))
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
        public Status SetVolume(long vol)
        {
            volume = (float)vol / 80;
            Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, volume);
            return Status.Ok;
        }
        public Status IncreaseVolume()
        {
            volume += (float)1 / 8;
            Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, volume);
            return Status.Ok;
        }
        public Status DecreaseVolume()
        {
            volume -= (float)1 / 8;
            Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, volume);
            return Status.Ok;
        }

        int[] _fxEQ = { 0, 0, 0 };
        public void Equalize()
        {
            // 3-band EQ
            BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();
            _fxEQ[0] = Bass.BASS_ChannelSetFX(_stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[1] = Bass.BASS_ChannelSetFX(_stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            _fxEQ[2] = Bass.BASS_ChannelSetFX(_stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 0);
            eq.fBandwidth = 18f;
            eq.fCenter = 100f;
            eq.fGain = 0f;
            Bass.BASS_FXSetParameters(_fxEQ[0], eq);
            eq.fCenter = 1000f;
            Bass.BASS_FXSetParameters(_fxEQ[1], eq);
            eq.fCenter = 8000f;
            Bass.BASS_FXSetParameters(_fxEQ[2], eq);
        }
        public void UpdateEQ(int band, float gain)
        {
            BASS_DX8_PARAMEQ eq = new BASS_DX8_PARAMEQ();
            //if (Bass.BASS_FXGetParameters(_fxEQ[band], eq))
            //{
                eq.fGain = gain*100;
                Bass.BASS_FXSetParameters(_fxEQ[band], eq);
            //}
        }
        ~Player()
        {
            Bass.BASS_StreamFree(_stream);
            Bass.BASS_Free();
        }
    }
}
