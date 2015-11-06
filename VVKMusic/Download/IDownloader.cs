using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Status = Common.Common.Status;
using Common;

namespace Downloader
{
    interface IDownloader
    {
         Status DownloadSong(Song[] songMas);
    }
}
