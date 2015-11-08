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
        Status ChangeUser(User user);
        Status RemoveUser(User user);
        Status UpdateUserList(Song[] SongMas, User user);
        Status GetUserList(User user);
        Status UpdateSettings(User user, Settings settings);
        User[] GetListOfUsers();
    }
}
