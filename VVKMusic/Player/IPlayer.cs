using System;
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
        Status Play(string linkToSong);
        Status AdjustSound(SoundSettings settings);
    }
}
