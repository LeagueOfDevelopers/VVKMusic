using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Download
{
    interface IDownload
    {
        int DownloadSong(Song[] SongMas);
    }
}
