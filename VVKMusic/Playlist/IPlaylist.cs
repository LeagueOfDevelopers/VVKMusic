using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playlist
{
    interface IPlaylist
    {
        // int MoveSong(Song song, int position);
        // int UpdateList(Song[] songMas);
        // int AddToList(Song[] songMas, int position);
        void MixPlaylist();
        int SearchSong(string pattern);
        // Song[] GetList();
    }
}
