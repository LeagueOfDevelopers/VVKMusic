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
        Status PlayAndStartTimer();
        Status StopAndStopTimer();
        Status PauseAndStopTimer();
        Status AdjustSound();
    }
}
