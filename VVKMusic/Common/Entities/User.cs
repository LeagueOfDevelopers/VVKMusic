using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable()]
    public class User
    {
        public User(string name, string userID, List<Song> songList)
        {
            this.Name = name;
            this.UserID = userID;
            this.Settings = new global::Common.Settings();
            this.SoundSettings = new global::Common.SoundSettings();
            this.SongList = songList;
        }
        public User ()
        {
            this.Settings = new global::Common.Settings();
            this.SoundSettings = new global::Common.SoundSettings();
            this.SongList = new List<Song>();
        }
        public string Name { get; set; }
        public string UserID { get; set; }
        public Settings Settings { get; set; }
        public SoundSettings SoundSettings { get; set; }
        public List<Song> SongList { get; set; }
    }
}
