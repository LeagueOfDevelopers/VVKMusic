using System.Collections.Generic;
using Status = Common.Common.Status;
using Common;

namespace Downloader
{
    interface IDownloader
    {
        Status DownloadSong(List<Song> songList);
    }
}
