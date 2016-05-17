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
using DesignInControl;

namespace Downloader
{
    public class Downloader : IDownloader
    {
        CircularProgressBar progressBar;
        int count = 0;
        public Status DownloadSong(List<Song> songList, ListBox listboxPlaylist)
        {
            progressBar = (CircularProgressBar)listboxPlaylist.ItemTemplate.FindName("progressBar", (ListBoxItem)listboxPlaylist.Items[0]);
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

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                if (progressBar.Percentage != e.ProgressPercentage)
                    progressBar.Percentage = e.ProgressPercentage;
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