using Common;
using System;
using System.Collections.Generic;
using System.Net;
using Status = Common.Common.Status;

namespace Downloader
{
    public class Downloader : IDownloader
    {
        public Status DownloadSong(List<Song> songList)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\";
            try
            {
                using (WebClient Client = new WebClient())
                {
                    foreach (Song song in songList)
                    {
                        if (!song.Downloaded)
                        {
                            string fileName = @folder + song.Artist + "-" + song.Title + ".mp3"; 
                            Client.DownloadFileAsync(song.Uri, @fileName);
                            song.DownloadedUri = new Uri(@fileName);
                            song.Downloaded = true;
                        }
                    }
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
            catch (NotSupportedException)
            {
                return Status.OK;
            }
            return Status.OK;
        }
    }
}