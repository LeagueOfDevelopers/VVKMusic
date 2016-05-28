using System.Collections.Generic;
using System.Windows;
using Status = Common.Common.Status;
using Common;
using System.Windows.Controls;

namespace Downloader
{
    interface IDownloader
    {
        Status DownloadSong(List<Song> listToDownload, List<Song> songList);
    }
}