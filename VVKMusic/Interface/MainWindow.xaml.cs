using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using Downloader;
using Infrastructure;
using Player;
using Playlist;
using UserManager;
using VKAPI;
using Status = Common.Common.Status;

namespace Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Playlist.Playlist Playlist1 = new Playlist.Playlist();
        public Player.Player Player1 = new Player.Player();
        public UserManager.UserManager UserManager1 = new UserManager.UserManager();
        public VKAPI.VKAPI VKAPI1 = new VKAPI.VKAPI();
        public string CurrentUser = null;
        public int CurrentSong = 0;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MenuClick(object sender, RoutedEventArgs e)
        {
            ListBox MenuList = (ListBox)FindName("MenuList");
            Image MenuButtonImage = (Image)FindName("MenuButtonImage");
            if (MenuList.Visibility == Visibility.Hidden)
            {
                MenuList.Visibility = Visibility.Visible;
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/close_menu.png", UriKind.Relative));
            }
            else
            {
                MenuList.Visibility = Visibility.Hidden;
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
            }
        }
        private void MenuListSelectionHandler(object sender, RoutedEventArgs e)
        {
            ListBox MenuList = (ListBox)FindName("MenuList");
            ListBoxItem item = (ListBoxItem)MenuList.SelectedValue;
            if(item != null)
            switch(item.Content.ToString())
            {
                case "Login":
                    //TODO Login part of logic
                    break;
            }
            MenuList.Visibility = Visibility.Hidden;
            MenuList.UnselectAll();
        }
        private void DownloadClick(object sender, RoutedEventArgs e)
        {
            //TODO
        }
        private void PrevClick(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
            Song[] SongList = Playlist1.GetList();
            if(SongList.Length > 0)
                if(CurrentSong > 0)
                {
                    CurrentSong--;
                }
                else
                {
                    CurrentSong = SongList.Length - 1;
                }
            Player1.SetSource(SongList[CurrentSong].url, SongList[CurrentSong].Downloaded);
            Player1.Play();
        }
        private void NextClick(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
            Song[] SongList = Playlist1.GetList();
            if (SongList.Length > 0)
                if (CurrentSong < SongList.Length - 1)
                {
                    CurrentSong++;
                }
                else
                {
                    CurrentSong = 0;
                }
            Player1.SetSource(SongList[CurrentSong].url, SongList[CurrentSong].Downloaded);
            Player1.Play();
        }
        private void PauseClick(object sender, RoutedEventArgs e)
        {
            Player1.Pause();
        }
        private void PlayClick(object sender, RoutedEventArgs e)
        {
            Player1.Play();
        }
        private void StopClick(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
        }
        private void SearchClick(object sender, RoutedEventArgs e)
        {
            TextBox MenuList = (TextBox)FindName("Search");
            Song[] result = Playlist1.SearchSong(MenuList.Text);
            //TODO Render
        }
        private void MixClick(object sender, RoutedEventArgs e)
        {
            Playlist1.MixPlaylist();
            //TODO Render playlist
        }
        private void SortClick(object sender, RoutedEventArgs e)
        {
            Playlist1.SortByDownloaded();
            //TODO Render Playlist
        }
        public void RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox Search = (TextBox)FindName("Search");
            Search.Text = "";
        }
        public void AddText(object sender, RoutedEventArgs e)
        {
            TextBox Search = (TextBox)FindName("Search");
            if (Search.Text == "")
                Search.Text = "Enter text here...";
        }
        //TODO User clicked on playlist and song was autoplayed
    }
}
