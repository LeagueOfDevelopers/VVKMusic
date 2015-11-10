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
        Status AddUser(User user)
        {
            return Status.OK;
        }
        Status UpdateUserSettings(string userID, Settings settings)
        {
            return Status.OK;
        }
        Status UpdateUserListOfSongs(Song[] SongMas, User user)
        {
            return Status.OK;
        }
        Status RemoveUser(string userID)
        {
            return Status.OK;
        }
        Song[] GetUserListOfSongs(string userID)
        {
            return null;
        }
        User[] GetListOfUsers()
        {
            return null;
        }
    }
}
