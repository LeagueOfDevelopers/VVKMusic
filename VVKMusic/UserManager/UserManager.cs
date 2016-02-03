using Common;
using System.Collections.Generic;
using Status = Common.Common.Status;

namespace UserManager
{
    public class UserManager : IUserManager 
    {
        private List<User> _ListOfUsers = new List<User>();

        public Status AddUser(User user)
        {
            if(_ListOfUsers.Exists(x => x.ID == user.ID))
            {
                _ListOfUsers[_ListOfUsers.FindIndex(x => x.ID == user.ID)] = user;
            }
            else
            {
                _ListOfUsers.Add(user);
            }
            return Status.OK;
        }
        public Status UpdateUserSettings(string userID, Settings settings)
        {
            if (_ListOfUsers.Exists(x => x.ID == userID))
            {
                _ListOfUsers[_ListOfUsers.FindIndex(x => x.ID == userID)].Settings = settings;
            }
            else
            {
                return Status.Error;
            }
            return Status.OK;
        }
        public Status UpdateUserSoundSettings(string userID, SoundSettings soundSettings)
        {
            if (_ListOfUsers.Exists(x => x.ID == userID))
            {
                _ListOfUsers[_ListOfUsers.FindIndex(x => x.ID == userID)].SoundSettings = soundSettings;
            }
            else
            {
                return Status.Error;
            }
            return Status.OK;
        }
        public Status UpdateUserListOfSongs(string userID, List<Song> songList)
        {
            if (_ListOfUsers.Exists(x => x.ID == userID))
            {
                _ListOfUsers[_ListOfUsers.FindIndex(x => x.ID == userID)].SongList = songList;
            }
            else
            {
                return Status.Error;
            }
            return Status.OK;
        }
        public Status RemoveUser(string userID)
        {
            if (_ListOfUsers.Exists(x => x.ID == userID))
            {
                _ListOfUsers.RemoveAt(_ListOfUsers.FindIndex(x => x.ID == userID));
            }
            else
            {
                return Status.Error;
            }
            return Status.OK;
        }
        public Status UpdateUserList(List<User> listOfUsers)
        {
            _ListOfUsers.Clear();
            _ListOfUsers = listOfUsers;
            return Status.OK;
        }
        public List<Song> GetUserListOfSongs(string userID)
        {
            if (_ListOfUsers.Exists(x => x.ID == userID))
            {
                return _ListOfUsers[_ListOfUsers.FindIndex(x => x.ID == userID)].SongList;
            }
            else
            {
                return null;
            }
            
        }
        public User GetUser(string name)
        {
            return _ListOfUsers.Find(x => x.Name == name);
        }
        public List<User> GetListOfUsers()
        {
            return _ListOfUsers;
        }
    }
}
