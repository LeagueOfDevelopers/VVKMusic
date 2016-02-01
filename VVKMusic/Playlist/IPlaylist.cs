using Common;
using System.Collections.Generic;
using Status = Common.Common.Status;

namespace Playlist
{
    interface IPlaylist
    {
        Status MoveSong(int oldIndex, int newIndex);
        Status UpdateList(List<Song> songList);
        Status AddToList(Song[] songMas, int index);
        Status AddToList(Song song, int index);
        Status RemoveFromList(int index);
        void MixPlaylist();
        void SortByDownloaded();
        Song[] SearchSong(string pattern);
        List<Song> GetList();
    }
}
