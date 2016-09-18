using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xaml;
using Status = Common.Common.Status;

namespace Playlist
{
    public class Playlist : ListBox//, IPlaylist
    {
        private ObservableCollection<Song> _ListOfSongs = new ObservableCollection<Song>();
        private ObservableCollection<Song> _BaseListOfSongs = new ObservableCollection<Song>();
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
        public Status SetBaseList(ObservableCollection<Song> songList)
        {
            _BaseListOfSongs.Clear();
            _BaseListOfSongs = songList;
            return Status.Ok;
        }
        public Status UpdateList(ObservableCollection<Song> songList)
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
            //_ListOfSongs.InsertRange(index, songMas);
            songMas.Reverse();
            foreach (Song song in songMas)
            {
                _ListOfSongs.Insert(index, song);
            }
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
                _ListOfSongs = new ObservableCollection<Song>(_ListOfSongs.OrderBy((song => song.Downloaded)));
            }
            else
            {
                LostSort();
                _ListOfSongs = new ObservableCollection<Song>(_ListOfSongs.OrderByDescending((song => song.Downloaded)));
                _SortedByDownloaded = true;
            }
        }

        public void SortByDuration()
        {
            if (_SortedByDuration)
            {
                LostSort();
                _ListOfSongs = new ObservableCollection<Song>(_ListOfSongs.OrderBy((song => song.Duration)));
            }
            else
            {
                LostSort();
                _ListOfSongs = new ObservableCollection<Song>(_ListOfSongs.OrderByDescending((song => song.Duration)));
                _SortedByDuration = true;
            }
        }
        public void SortByArtist()
        {
            if (_SortedByArtist)
            {
                LostSort();
                _ListOfSongs = new ObservableCollection<Song>(_ListOfSongs.OrderByDescending((song => song.Artist)));

            }
            else
            {
                LostSort();
                _ListOfSongs = new ObservableCollection<Song>(_ListOfSongs.OrderBy((song => song.Artist)));
                _SortedByArtist = true;
            }
        }
        public void SortByTitle()
        {
            if (_SortedByTitle)
            {
                LostSort();
                _ListOfSongs = new ObservableCollection<Song>(_ListOfSongs.OrderByDescending((song => song.Title)));
            }
            else
            {
                LostSort();
                _ListOfSongs = new ObservableCollection<Song>(_ListOfSongs.OrderBy((song => song.Title)));
                _SortedByTitle = true;
            }
        }
        public void LostSort()
        {
            _SortedByArtist = _SortedByDuration = _SortedByTitle =_SortedByDownloaded= false;
        }
        public List<Song> SearchSong(string pattern)
        {
            return _ListOfSongs.Where(x => (x.Artist + " - " + x.Title).ToLower().Contains(pattern)).Select(x => x).ToList<Song>();
        }
        public ObservableCollection<Song> GetList()
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

