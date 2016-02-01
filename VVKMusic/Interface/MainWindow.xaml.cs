using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Infrastructure.Infrastructure Infrastructure1 = new Infrastructure.Infrastructure();
        public string CurrentUser = null;
        public int CurrentSong = 0;
        public MainWindow()
        {
            InitializeComponent();
            if(Infrastructure1.LoadListOfUsers() != null)
            {
                UserManager1.UpdateUserList(Infrastructure1.LoadListOfUsers());
            }
        }
        private void Menu_Click(object sender, RoutedEventArgs e)
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
        private void MenuList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ListBox MenuList = (ListBox)FindName("MenuList");
            ListBoxItem item = (ListBoxItem)MenuList.SelectedValue;
            Image MenuButtonImage = (Image)FindName("MenuButtonImage");
            if(item != null)
            switch(item.Content.ToString())
            {
                case "Login":
                    MenuList.Visibility = Visibility.Hidden;
                    MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
                    WebLogin WebLogin1 = new WebLogin();
                    WebLogin1.RaiseCustomEvent += new EventHandler<CustomEventArgs>(WebLogin1_RaiseCustomEvent);
                    WebLogin1.Show();
                    break;
            }
            MenuList.Visibility = Visibility.Hidden;
            MenuList.UnselectAll();
        }
        private void WebLogin1_RaiseCustomEvent(object sender, CustomEventArgs e)
        {
            List<Song> SongList = new List<Song>(VKAPI1.GetAudioExternal(e.UserID.ToString(), e.AccessToken));
            UserManager1.AddUser(new User(e.AccessToken, e.UserID.ToString(), SongList));
            Playlist1.UpdateList(SongList);
            Player1.SetSource(SongList[0].url, false);

            ListBox PlaylistBox = (ListBox)FindName("Playlist");
            ObservableCollection<Song> oSong = new ObservableCollection<Song>(SongList);
            PlaylistBox.DataContext = oSong;
            Binding binding = new Binding();
            PlaylistBox.SetBinding(ListBox.ItemsSourceProperty, binding);
            TextBox SongName = (TextBox)FindName("SongName");
            SongName.Text = SongList[0].ToString();
            TextBox SongTime = (TextBox)FindName("SongTime");
            SongTime.Text = String.Format("0:00 / {0}",SongList[0].Duration);
        }
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }
        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
            Song[] SongList = Playlist1.GetList().ToArray();
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
            //TODO View in Name, Time and Playlist
        }
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
            Song[] SongList = Playlist1.GetList().ToArray();
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
            //TODO View in Name, Time and Playlist
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Player1.Pause();
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            Player1.Play();
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            TextBox MenuList = (TextBox)FindName("Search");
            Song[] result = Playlist1.SearchSong(MenuList.Text);
            //TODO Render in playlist
        }
        private void Mix_Click(object sender, RoutedEventArgs e)
        {
            Playlist1.MixPlaylist();
            //TODO Render playlist
        }
        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            Playlist1.SortByDownloaded();
            //TODO Render Playlist
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Expand_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
        private void Collapse_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        public void Search_RemoveText(object sender, RoutedEventArgs e)
        {
            TextBox Search = (TextBox)FindName("Search");
            Search.Text = "";
        }
        public void Search_AddText(object sender, RoutedEventArgs e)
        {
            TextBox Search = (TextBox)FindName("Search");
            if (Search.Text == "")
                Search.Text = "Search";
        }
        private void Draggable_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        //TODO User clicked on playlist and song was autoplayed
    }
}
