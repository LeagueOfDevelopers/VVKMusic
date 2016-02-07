using Common;
using System;
using System.Collections.Generic;
using Status = Common.Common.Status;

namespace Playlist
{
    public class Playlist : IPlaylist
    {
        private List<Song> _ListOfSongs = new List<Song>();

        public Status MoveSong(int oldIndex, int newIndex)
        {
            Song songToMove = _ListOfSongs[oldIndex];
            _ListOfSongs.RemoveAt(oldIndex);
            _ListOfSongs.Insert(newIndex, songToMove);
            return Status.OK;
        }
        public Status UpdateList(List<Song> songList)
        {
            _ListOfSongs.Clear();
            _ListOfSongs = songList;
            return Status.OK;
        }
        public Status AddToList(Song[] songMas, int index)
        {
            _ListOfSongs.InsertRange(index, songMas);
            return Status.OK;
        }
        public Status AddToList(Song song, int index)
        {
            _ListOfSongs.Insert(index, song);
            return Status.OK;
        }
        public Status RemoveFromList(int index)
        {
            _ListOfSongs.RemoveAt(index);
            return Status.OK;
        }
        public void MixPlaylist()
        {
            Random rng = new Random();
            int n = _ListOfSongs.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Song value = _ListOfSongs[k];
                _ListOfSongs[k] = _ListOfSongs[n];
                _ListOfSongs[n] = value;
            }
        }
        public void SortByDownloaded()
        {
            _ListOfSongs.Sort((song1,song2) => song2.Downloaded.CompareTo(song1.Downloaded));
        }
        public List<Song> SearchSong(string pattern)
        {
            return _ListOfSongs.FindAll(x => (x.Artist + " - " + x.Title).ToLower().Contains(pattern));
        }
        public List<Song> GetList()
        {
            return _ListOfSongs;
        }
    }
}
