using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Status = Common.Common.Status;

namespace UserManager
{
    class UserManager : IUserManager 
    {
        private List<User> _ListOfUsers;

        public Status AddUser(User user)
        {
            if(_ListOfUsers.Exists(x => x.UserID == user.UserID))
            {
                _ListOfUsers[_ListOfUsers.FindIndex(x => x.UserID == user.UserID)] = user;
            }
            else
            {
                _ListOfUsers.Add(user);
            }
            return Status.OK;
        }
        public Status UpdateUserSettings(string userID, Settings settings)
        {
            if (_ListOfUsers.Exists(x => x.UserID == userID))
            {
                _ListOfUsers[_ListOfUsers.FindIndex(x => x.UserID == userID)].Settings = settings;
            }
            else
            {
                return Status.Error;
            }
            return Status.OK;
        }
        public Status UpdateUserListOfSongs(string userID, List<Song> songList)
        {
            if (_ListOfUsers.Exists(x => x.UserID == userID))
            {
                _ListOfUsers[_ListOfUsers.FindIndex(x => x.UserID == userID)].SongList = songList;
            }
            else
            {
                return Status.Error;
            }
            return Status.OK;
        }
        public Status RemoveUser(string userID)
        {
            if (_ListOfUsers.Exists(x => x.UserID == userID))
            {
                _ListOfUsers.RemoveAt(_ListOfUsers.FindIndex(x => x.UserID == userID));
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
        public Song[] GetUserListOfSongs(string userID)
        {
            return _ListOfUsers[_ListOfUsers.FindIndex(x => x.UserID == userID)].SongList.ToArray();
        }
        public User[] GetListOfUsers()
        {
            return _ListOfUsers.ToArray();
        }
    }
}
