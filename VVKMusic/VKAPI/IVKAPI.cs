using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKAPI
{
    interface IVKAPI
    {
        int Auth(string login, string password);
        // Song[] GetSong(string linkToSong);
        int CheckConnection();
    }
}
