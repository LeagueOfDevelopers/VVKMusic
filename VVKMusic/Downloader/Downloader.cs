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
        double percentage;
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
            return Status.OK;
        }


        public void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            count--;
            if (count == 0)
            {
                MessageBox.Show("Скачивание завершено", "", MessageBoxButton.OK);
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                if (percentage != e.ProgressPercentage)
                    percentage = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public async Task DownloadAudioAync(Uri url, string path)
        {
            try
            {
                WebClient Client = new WebClient();
                Client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
                Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                await Client.DownloadFileTaskAsync(url, path);
            }
            catch
            { }
        }
    }
}