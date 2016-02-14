using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Windows;
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
                try
                {
                    WebClient Client = new WebClient();
                    foreach (Song song in songList)
                    {
                        Client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
                        string fileName = song.Artist + "-" + song.Title + ".mp3";
                        Client.DownloadFileAsync(song.Uri, @folder + fileName);
                        song.DownloadedUri = new Uri(@folder + fileName);
                        song.Downloaded = true;
                    }
                }
                catch (WebException)
                {
                    return Status.Error;
                }
                catch (ArgumentNullException)
                {
                    return Status.Error;
                }
                catch (NotSupportedException) { }
                return Status.OK;
            }
            public void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
            {
                count--;
                MessageBox.Show("Скачивание завершено полностью " + count.ToString(), "", MessageBoxButton.OK);
            }

        }
    }