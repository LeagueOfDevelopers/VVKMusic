using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKAPI
{
    interface IVKAPI
    {
        int Auth(string Login, string Password);
        Song[] GetSong(string Link);
        int CheckConnection();
    }
}
