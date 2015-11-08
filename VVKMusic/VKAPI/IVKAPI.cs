using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace VKAPI
{
    interface IVKAPI
    {
        string Auth();
        string GetResponse(string linkToSong);
        Song[] GetAudio();
    }
}
