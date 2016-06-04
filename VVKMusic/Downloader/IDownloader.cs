using System.Collections.Generic;
using System.Windows;
using Status = Common.Common.Status;
using Common;
using System.Windows.Controls;
using System.Net;
using System.ComponentModel;

namespace Downloader
{
    interface IDownloader
    {
        Status DownloadSong(List<Song> listToDownload, DownloadProgressChangedEventHandler ProgressChanged, AsyncCompletedEventHandler DownloadSongCallback);
    }
}