using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using Playlist;

namespace Tests
{
    [TestClass]
    public class PlaylistTests
    {
        [TestMethod]
        public void Playlist_GetList()
        {
            Playlist.Playlist playlist = new Playlist.Playlist();
            List<Song> songList = new List<Song>();
            songList.AddRange(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4) });
            playlist.UpdateList(songList.ToArray());
            Assert.AreEqual(1, playlist.GetList()[0].ID);
            Assert.AreEqual(4, playlist.GetList()[3].ID);
        }
        [TestMethod]
        public void Playlist_UpdateList()
        {
            Playlist.Playlist playlist = new Playlist.Playlist();
            List<Song> songList = new List<Song>();
            songList.AddRange(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4) });
            playlist.UpdateList(songList.ToArray());
            Assert.AreEqual(songList[0].ID, playlist.GetList()[0].ID);
        }
        [TestMethod]
        public void Playlist_MoveSong()
        {
            Playlist.Playlist playlist = new Playlist.Playlist();
            List<Song> songList = new List<Song>();
            songList.AddRange(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4) });
            playlist.UpdateList(songList.ToArray());
            playlist.MoveSong(0, 2);
            Assert.AreEqual(1, playlist.GetList()[2].ID);
        }
        [TestMethod]
        public void Playlist_SingleAddToList()
        {
            Playlist.Playlist playlist = new Playlist.Playlist();
            List<Song> songList = new List<Song>();
            songList.AddRange(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4) });
            playlist.UpdateList(songList.ToArray());
            Song insertedSong = new Song(8);
            playlist.AddToList(insertedSong, 0);
            Assert.AreEqual(insertedSong.ID, playlist.GetList()[0].ID);
            playlist.AddToList(insertedSong, 5);
            Assert.AreEqual(insertedSong.ID, playlist.GetList()[5].ID);
            playlist.AddToList(insertedSong, 3);
            Assert.AreEqual(insertedSong.ID, playlist.GetList()[3].ID);
        }
        [TestMethod]
        public void Playlist_MultipleAddToList()
        {
            Playlist.Playlist playlist = new Playlist.Playlist();
            List<Song> songList = new List<Song>();
            songList.AddRange(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4) });
            playlist.UpdateList(songList.ToArray());
            Song[] insertedArrayOfSongs = new Song[] { new Song(9), new Song(8), new Song(7) };
            playlist.AddToList(insertedArrayOfSongs, 4);
            Assert.AreEqual(insertedArrayOfSongs[0].ID, playlist.GetList()[4].ID);
        }
        [TestMethod]
        public void Playlist_RemoveFromList()
        {
            Playlist.Playlist playlist = new Playlist.Playlist();
            List<Song> songList = new List<Song>();
            songList.AddRange(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4) });
            playlist.UpdateList(songList.ToArray());
            playlist.RemoveFromList(2);
            Assert.AreEqual(4, playlist.GetList()[2].ID);
            playlist.RemoveFromList(0);
            Assert.AreEqual(2, playlist.GetList()[0].ID);
        }
        [TestMethod]
        public void Playlist_SortByDownloaded()
        {
            Playlist.Playlist playlist = new Playlist.Playlist();
            List<Song> songList = new List<Song>();
            songList.AddRange(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4) });
            songList[1].Downloaded = true;
            songList[3].Downloaded = true;
            playlist.UpdateList(songList.ToArray());
            playlist.SortByDownloaded();
            Assert.AreEqual(true, playlist.GetList()[0].Downloaded);
            Assert.AreEqual(true, playlist.GetList()[1].Downloaded);
        }
        [TestMethod]
        public void Playlist_SearchSong()
        {
            Playlist.Playlist playlist = new Playlist.Playlist();
            List<Song> songList = new List<Song>();
            songList.AddRange(new Song[] { new Song(1), new Song(2), new Song(3), new Song(4) });
            songList[0].Title = "bar";
            songList[1].Title = "foo";
            songList[2].Title = "foobar";
            songList[3].Title = "baz";
            playlist.UpdateList(songList.ToArray());
            Assert.AreEqual(playlist.SearchSong("foo")[0].ID, 2);
        }
    }
}
