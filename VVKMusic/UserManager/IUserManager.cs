using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Status = Common.Common.Status;

namespace UserManager
{
    interface IUserManager
    {
        Status AddUser(User user);
        Status UpdateUserSettings(string userID, Settings settings);
        Status UpdateUserListOfSongs(string userID, List<Song> songList);
        Status RemoveUser(string userID);
        Song[] GetUserListOfSongs(string userID);
        User[] GetListOfUsers();
    }
}
