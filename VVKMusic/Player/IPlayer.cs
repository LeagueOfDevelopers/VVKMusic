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
