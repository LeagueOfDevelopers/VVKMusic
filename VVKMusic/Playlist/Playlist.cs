using Common;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xaml;
using Status = Common.Common.Status;

namespace Playlist
{
    public class Playlist : ListBox//, IPlaylist
    {
        private List<Song> _ListOfSongs = new List<Song>();
        private List<Song> _BaseListOfSongs = new List<Song>();
        private Boolean _SortedByTitle = false;
        private Boolean _SortedByArtist = false;
        private Boolean _SortedByDuration = false;
        private Boolean _SortedByDownloaded = false;

        public Status MoveSong(int oldIndex, int newIndex)
        {
            Song songToMove = _ListOfSongs[oldIndex];
            _ListOfSongs.RemoveAt(oldIndex);
            _ListOfSongs.Insert(newIndex, songToMove);
            return Status.Ok;
        }
        public Status SetBaseList(List<Song> songList)
        {
            _BaseListOfSongs.Clear();
            _BaseListOfSongs = songList;
            return Status.Ok;
        }
        public Status UpdateList(List<Song> songList)
        {
            _ListOfSongs = songList;
            return Status.Ok;
        }
        public Status UpdateListToBase()
        {
            return UpdateList(_BaseListOfSongs);
        }
        public Status AddToList(Song[] songMas, int index)
        {
            _ListOfSongs.InsertRange(index, songMas);
            return Status.Ok;
        }
        public Status AddToList(Song song, int index)
        {
            _ListOfSongs.Insert(index, song);
            return Status.Ok;
        }
        public Status RemoveFromList(int index)
        {
            _ListOfSongs.RemoveAt(index);
            return Status.Ok;
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
            if (_SortedByDownloaded)
            {
                LostSort();
                _ListOfSongs.Sort((song1, song2) => song1.Downloaded.CompareTo(song2.Downloaded));
            }
            else
            {
                LostSort();
                _ListOfSongs.Sort((song1, song2) => song2.Downloaded.CompareTo(song1.Downloaded));
                _SortedByDownloaded = true;
            }
        }

        public void SortByDuration()
        {
            if (_SortedByDuration)
            {
                LostSort();
                _ListOfSongs.Sort((song1, song2) => song2.Duration.CompareTo(song1.Duration));
            }
            else
            {
                LostSort();
                _ListOfSongs.Sort((song1, song2) => song1.Duration.CompareTo(song2.Duration));
                _SortedByDuration = true;
            }
        }
        public void SortByArtist()
        {
            if (_SortedByArtist)
            {
                LostSort();
                _ListOfSongs.Sort((song1, song2) => song2.Artist.CompareTo(song1.Artist));
            }
            else
            {
                LostSort();
                _ListOfSongs.Sort((song1, song2) => song1.Artist.CompareTo(song2.Artist));
                _SortedByArtist = true;
            }
        }
        public void SortByTitle()
        {
            if (_SortedByTitle)
            {
                LostSort();
                _ListOfSongs.Sort((song1, song2) => song2.Title.CompareTo(song1.Title));
            }
            else
            {
                LostSort();
                _ListOfSongs.Sort((song1, song2) => song1.Title.CompareTo(song2.Title));
                _SortedByTitle = true;
            }
        }
        public void LostSort()
        {
            _SortedByArtist = _SortedByDuration = _SortedByTitle =_SortedByDownloaded= false;
        }
        public List<Song> SearchSong(string pattern)
        {
            return _ListOfSongs.FindAll(x => (x.Artist + " - " + x.Title).ToLower().Contains(pattern));
        }
        public List<Song> GetList()
        {
            return _ListOfSongs;
        }

        public Status NextSong()
        {
            throw new NotImplementedException();
        }

        public Status PreviousSong()
        {
            throw new NotImplementedException();
        }
    }
}

