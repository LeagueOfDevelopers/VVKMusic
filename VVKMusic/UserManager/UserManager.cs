using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManager
{
    interface IUserManager
    {
        int AddUser(string Name);
        int ChangeUser(string Name);
        int RemoveUser(string Name);
        //int UpdateList(Song[] SongMas, string Name);
        int GetList(string Name);
        int UpdateSettings(string Name);
        String[] GetListOfUsers();
    }
}
