using System.Collections.Generic;
using System.Windows;
using Status = Common.Common.Status;
using Common;
using System.Windows.Controls;
using System.Net;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Downloader
{
    interface IDownloader
    {
        Status DownloadSong(List<Song> listToDownload, DownloadProgressChangedEventHandler ProgressChanged, AsyncCompletedEventHandler DownloadSongCallback);
        Task DownloadAudioAync(Song song, string path, DownloadProgressChangedEventHandler ProgressChanged, AsyncCompletedEventHandler DownloadSongCallback);
        void DownloadFileCallback(object sender, AsyncCompletedEventArgs e);
        void CheckIfDownloaded(Song song);
    }
}