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
        Dictionary<int, int> hashCodeConnection = new Dictionary<int, int>();
        public List<Song> SongList;
        int count = 0;

        public Status DownloadSong(List<Song> listToDownload, List<Song> songs)
        {
            SongList = songs;
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\";
            count = listToDownload.Count;
            foreach (Song song in listToDownload)
            {
                string fileName = song.Artist + "-" + song.Title + ".mp3";
                if (DownloadAudioAync(song, @folder + fileName).Exception != null)
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
                foreach (Song song in SongList)
                {
                    if (song.GetHashCode() == hashCodeConnection[sender.GetHashCode()])
                    {
                        if (song.Percentage != e.ProgressPercentage)
                            song.Percentage = e.ProgressPercentage;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public async Task DownloadAudioAync(Song song, string path)
        {
            try
            {
                WebClient Client = new WebClient();
                hashCodeConnection.Add(Client.GetHashCode(), song.GetHashCode());
                Client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
                Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                await Client.DownloadFileTaskAsync(song.Uri, path);
            }
            catch
            { }
        }
    }
}