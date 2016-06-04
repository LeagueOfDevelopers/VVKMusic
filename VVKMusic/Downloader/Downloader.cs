using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Status = Common.Common.Status;
using System.Windows.Controls;

namespace Downloader
{
    public class Downloader : IDownloader
    {
        public Dictionary<int, int> hashCodeConnection = new Dictionary<int, int>();
        int count = 0;

        public Status DownloadSong(List<Song> listToDownload, DownloadProgressChangedEventHandler ProgressChanged, AsyncCompletedEventHandler DownloadSongCallback)
        {
            string folderName = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string pathString = Path.Combine(folderName, "VVKMusic");
            if (!File.Exists(pathString)) Directory.CreateDirectory(pathString);
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\VVKMusic\";
            count = listToDownload.Count;
            foreach (Song song in listToDownload)
            {
                string fileName = song.Artist + "-" + song.Title + ".mp3";
                if (DownloadAudioAync(song, @folder + fileName, ProgressChanged, DownloadSongCallback).Exception != null)
                {
                    return Status.Error;
                }
                song.DownloadedUri = new Uri(@folder + fileName);
            }
            return Status.Ok;
        }


        public void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            count--;
            if (count == 0)
            {
                MessageBox.Show("Скачивание завершено", "", MessageBoxButton.OK);
            }
        }

        public async Task DownloadAudioAync(Song song, string path, DownloadProgressChangedEventHandler ProgressChanged, AsyncCompletedEventHandler DownloadSongCallback)
        {
            try
            {
                WebClient Client = new WebClient();
                hashCodeConnection.Add(Client.GetHashCode(), song.GetHashCode());
                Client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadSongCallback);
                Client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
                Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                await Client.DownloadFileTaskAsync(song.Uri, path);
            }
            catch
            { }
        }
    }
}