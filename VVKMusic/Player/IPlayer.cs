﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Status = Common.Common.Status;

namespace Player
{
    interface IPlayer
    {
        Status SetSource(Song playedSong);
        Status Play();
        Status Stop();
        Status Pause();
        Status AdjustSound();
    }
}
