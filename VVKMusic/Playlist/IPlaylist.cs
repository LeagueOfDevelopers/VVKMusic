using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Status = Common.Common.Status;

namespace Playlist
{
    interface IPlaylist
    {
        Status MoveSong(Song song, int position);
        Status UpdateList(Song[] songMas);
        Status AddToList(Song[] songMas, int position);
        void MixPlaylist();
        void SortByDownloaded(bool pattern);
        Status SearchSong(string pattern);
        Song[] GetList();
    }
}
