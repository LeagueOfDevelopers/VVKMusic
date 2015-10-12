using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playlist
{
    interface IPlaylist
    {
        int MoveSong(Song Song, int position);
        int UpdateList(Song[] SongMas);
        int AddToList(Song[] SongMas, int position);
        void MixPlaylist();
        int SearchSong(string pattern);
        Song[] GetList();
    }
}
