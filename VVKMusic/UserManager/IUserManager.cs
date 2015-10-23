using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;

namespace UserManager
{
    interface IUserManager
    {
        int AddUser(User user);
        int ChangeUser(User user);
        int RemoveUser(User user);
        int UpdateUserList(Song[] SongMas, User user);
        int GetUserList(User user);
        int UpdateSettings(User user, Settings settings);
        User[] GetListOfUsers();
    }
}
