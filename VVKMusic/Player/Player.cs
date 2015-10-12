using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Player
{
    interface IPlayer
    {
        int Play(string LinkToSong);
        int AdjustSound(SoundSettings Settings);
    }
}
