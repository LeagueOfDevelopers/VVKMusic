using Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Status = Common.Common.Status;

namespace UserManager
{
    interface IUserManager
    {
        Status AddUser(User user);
        Status UpdateUserSettings(string userID, Settings settings);
        Status UpdateUserSoundSettings(string userID, SoundSettings soundSettings);
        Status UpdateUserListOfSongs(string userID, List<Song> songList);
        Status RemoveUser(string userID);
        ObservableCollection<Song> GetUserListOfSongs(string userID);
        List<User> GetListOfUsers();
    }
}
