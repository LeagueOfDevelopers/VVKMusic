using Status = Common.Common.Status;
using Common;

namespace Downloader
{
    interface IDownloader
    {
         Status DownloadSong(Song[] songMas);
    }
}
