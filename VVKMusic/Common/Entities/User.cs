using System;
using System.Collections.Generic;

namespace Common
{
    [Serializable()]
    public class User
    {
        public User(string accessToken, string userID, List<Song> songList)
        {
            this.Name = "UnnamedUser";
            this.ID = userID;
            this.AccessToken = accessToken;
            this.Settings = new global::Common.Settings();
            this.SoundSettings = new global::Common.SoundSettings();
            this.SongList = songList;
        }
        public User(string userID)
        {
            this.Name = "UnnamedUser";
            this.ID = userID;
            this.Settings = new global::Common.Settings();
            this.SoundSettings = new global::Common.SoundSettings();
            this.SongList = new List<Song>();
        }
        public User ()
        {
            this.Settings = new global::Common.Settings();
            this.SoundSettings = new global::Common.SoundSettings();
            this.SongList = new List<Song>();
        }
        public string Name { get; set; }
        public string ID { get; set; }
        public string AccessToken { get; set; }
        public Settings Settings { get; set; }
        public SoundSettings SoundSettings { get; set; }
        public List<Song> SongList { get; set; }
    }
}
