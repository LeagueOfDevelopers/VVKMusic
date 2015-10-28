using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;

namespace VKAPI
{
    interface IVKAPI
    {
        int Auth(string login, string password);
        Song[] GetSongs(string linkToSong);
    }
}
