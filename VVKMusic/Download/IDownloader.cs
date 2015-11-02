using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;

namespace Downloader
{
    interface IDownloader
    {
        int DownloadSong(Song[] songMas);
    }
}
