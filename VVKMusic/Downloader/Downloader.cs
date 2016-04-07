using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Threading.Tasks;
using Status = Common.Common.Status;

namespace Downloader
{
    public class Downloader : IDownloader
    {
        int count = 0;
        public Status DownloadSong(List<Song> songList)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\";
            count = songList.Count;
            foreach (Song song in songList)
            {
                string fileName = song.Artist + "-" + song.Title + ".mp3";
                if (DownloadAudioAync(song.Uri, @folder + fileName).Exception != null)
                {
                    return Status.Error;
                }
                song.DownloadedUri = new Uri(@folder + fileName);
                song.Downloaded = true;
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

        public async Task DownloadAudioAync(Uri url, string path)
        {
            try
            {
                WebClient Client = new WebClient();
                Client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
                await Client.DownloadFileTaskAsync(url, path);
            }
            catch
            { }
        }
    }
}