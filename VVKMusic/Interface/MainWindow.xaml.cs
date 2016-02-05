using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
        protected User CurrentUser = null;
        protected int CurrentSong = 0;
        
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
            ListBox LoginAs = (ListBox)FindName("LoginAs");
            Image MenuButtonImage = (Image)FindName("MenuButtonImage");
            if (MenuList.Visibility == Visibility.Hidden)
            {
                MenuList.Visibility = Visibility.Visible;
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/close_menu.png", UriKind.Relative));
            }
            else
            {
                MenuList.Visibility = Visibility.Hidden;
                LoginAs.Visibility = Visibility.Hidden;
                LoginAs.UnselectAll();
                MenuList.UnselectAll();
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
            }
        }
        private void MenuList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ListBox MenuList = (ListBox)FindName("MenuList");
            ListBox LoginAs = (ListBox)FindName("LoginAs");
            ListBoxItem item = (ListBoxItem)MenuList.SelectedValue;
            Image MenuButtonImage = (Image)FindName("MenuButtonImage");
            if (item != null)
            {
                switch (item.Content.ToString())
                {
                    case "New Login":
                        MenuList.Visibility = Visibility.Hidden;
                        MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
                        WebLogin WebLogin1 = new WebLogin();
                        WebLogin1.RaiseCustomEvent += new EventHandler<CustomEventArgs>(WebLogin1_RaiseCustomEvent);
                        WebLogin1.Show();
                        MenuList.UnselectAll();
                        break;
                    case "Login as...":
                        if (UserManager1.GetListOfUsers().Count > 0)
                        {
                            LoginAs.Visibility = Visibility.Visible;
                            ObservableCollection<User> oUsers = new ObservableCollection<User>(UserManager1.GetListOfUsers());
                            LoginAs.DataContext = oUsers;
                            Binding binding = new Binding();
                            LoginAs.SetBinding(ListBox.ItemsSourceProperty, binding);
                        }
                        break;
                }
            }
        }
        private void WebLogin1_RaiseCustomEvent(object sender, CustomEventArgs e)
        {
            Player1.Stop();
            List<Song> SongList = new List<Song>(VKAPI1.GetAudioExternal(e.UserID.ToString(), e.AccessToken));
            UserManager1.AddUser(new User(e.Name, e.AccessToken, e.UserID.ToString(), SongList));
            Playlist1.UpdateList(SongList);
            Infrastructure1.SaveListOfUsers(UserManager1.GetListOfUsers());
            if (SongList.Count > 0)
            {
                CurrentSong = 0;
                Player1.SetSource(SongList[CurrentSong].url, false);
                RenderPlaylist(SongList);
                RenderNameAndSelectedSong();
                TextBox SongTime = (TextBox)FindName("SongTime");
                SongTime.Text = String.Format("0:00 / {0}", SongList[0].Duration);
                Player1.Play();
            }
            else
            {
                MessageBox.Show("Данный пользователь не имеет аудиозаписей.", "VVKMusic информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void LoginAs_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ListBox MenuList = (ListBox)FindName("MenuList");
            ListBox LoginAs = (ListBox)FindName("LoginAs");
            Image MenuButtonImage = (Image)FindName("MenuButtonImage");
            if (LoginAs.SelectedValue != null)
            {
                Player1.Stop();
                CurrentUser = (User)LoginAs.SelectedValue;
                List<Song> SongList1 = new List<Song>(VKAPI1.GetAudioExternal(CurrentUser.ID, CurrentUser.AccessToken));
                Playlist1.UpdateList(SongList1);
                if (SongList1.Count > 0)
                {
                    CurrentSong = 0;
                    Player1.SetSource(SongList1[CurrentSong].url, false);
                    RenderPlaylist(SongList1);
                    RenderNameAndSelectedSong();
                    TextBox SongTime = (TextBox)FindName("SongTime");
                    SongTime.Text = String.Format("0:00 / {0}", SongList1[0].Duration);
                    Player1.Play();
                }
                else
                {
                    MessageBox.Show("Данный пользователь не имеет аудиозаписей.", "VVKMusic информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
                MenuList.Visibility = Visibility.Hidden;
                MenuList.UnselectAll();
                LoginAs.Visibility = Visibility.Hidden;
                LoginAs.UnselectAll();
            }
        }
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }
        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
            List<Song> SongList = Playlist1.GetList();
            if(SongList.Count > 0)
                if(CurrentSong > 0)
                {
                    CurrentSong--;
                }
                else
                {
                    CurrentSong = SongList.Count - 1;
                }
            Player1.SetSource(SongList[CurrentSong].url, SongList[CurrentSong].Downloaded);
            Player1.Play();
            RenderNameAndSelectedSong();
        }
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
            List<Song> SongList = Playlist1.GetList();
            if (SongList.Count > 0)
                if (CurrentSong < SongList.Count - 1)
                {
                    CurrentSong++;
                }
                else
                {
                    CurrentSong = 0;
                }
            Player1.SetSource(SongList[CurrentSong].url, SongList[CurrentSong].Downloaded);
            Player1.Play();
            RenderNameAndSelectedSong();
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
            TextBox Search = (TextBox)FindName("Search");
            RenderPlaylist(Playlist1.SearchSong(Search.Text));
        }
        private void Mix_Click(object sender, RoutedEventArgs e)
        {
            Playlist1.MixPlaylist();
            RenderPlaylist(Playlist1.GetList());
        }
        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            Playlist1.SortByDownloaded();
            RenderPlaylist(Playlist1.GetList());
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
        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox Search = (TextBox)FindName("Search");
            Search.Text = "";
        }
        private void Search_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox Search = (TextBox)FindName("Search");
            if (Search.Text == "")
                Search.Text = "Search";
        }
        private void Draggable_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void RenderNameAndSelectedSong()
        {
            TextBox SongName = (TextBox)FindName("SongName");
            List<Song> SongList = Playlist1.GetList();
            SongName.Text = SongList[CurrentSong].ToString();
            ListBox PlaylistBox = (ListBox)FindName("Playlist");
            if(PlaylistBox.Items != null)
                PlaylistBox.SelectedItem = (PlaylistBox.Items[CurrentSong]);
        }
        private void RenderPlaylist(List<Song> SongList) 
        {
            ListBox PlaylistBox = (ListBox)FindName("Playlist");
            ObservableCollection<Song> oSong = new ObservableCollection<Song>(SongList);
            PlaylistBox.DataContext = oSong;
            Binding binding = new Binding();
            PlaylistBox.SetBinding(ListBox.ItemsSourceProperty, binding);
        }
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox Search = (TextBox)FindName("Search");
            if(Search.Text == "")
            {
                RenderPlaylist(Playlist1.GetList());
            }
        }
        private void Playlist_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ListBox PlaylistBox = (ListBox)FindName("Playlist");
            if (PlaylistBox.SelectedIndex != -1 && CurrentSong != PlaylistBox.SelectedIndex)
            {
                CurrentSong = PlaylistBox.SelectedIndex;
                RenderNameAndSelectedSong();
                Player1.Stop();
                List<Song> SongList = Playlist1.GetList();
                Player1.SetSource(SongList[CurrentSong].url, SongList[CurrentSong].Downloaded);
                Player1.Play();
            }
        }
        private void LoginAs_MouseLeave(object sender, MouseEventArgs e)
        {
            Image MenuButtonImage = (Image)FindName("MenuButtonImage");
            ListBox MenuList = (ListBox)FindName("MenuList");
            ListBox LoginAs = (ListBox)FindName("LoginAs");
            MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
            MenuList.Visibility = Visibility.Hidden;
            LoginAs.Visibility = Visibility.Hidden;
            MenuList.UnselectAll();
            LoginAs.UnselectAll();
        }
    }
}

