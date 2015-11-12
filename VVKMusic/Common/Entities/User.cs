using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class User
    {
        public User(string name, string userID, List<Song> songList)
        {
            Name = name;
            UserID = userID;
            Settings = new global::Common.Settings();
            SoundSettings = new global::Common.SoundSettings();
            SongList = songList;
        }
        public string Name { get; set; }
        public string UserID { get; set; }
        public Settings Settings { get; set; }
        public SoundSettings SoundSettings { get; set; }
        public List<Song> SongList { get; set; }
    }
}
