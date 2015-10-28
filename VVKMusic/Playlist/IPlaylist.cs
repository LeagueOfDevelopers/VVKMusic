using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;

namespace Playlist
{
    interface IPlaylist
    {
        int MoveSong(Song song, int position);
        int UpdateList(Song[] songMas);
        int AddToList(Song[] songMas, int position);
        void MixPlaylist();
        void SortByDownloaded(bool pattern);
        int SearchSong(string pattern);
        Song[] GetList();
    }
}
