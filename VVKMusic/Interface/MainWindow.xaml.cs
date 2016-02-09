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
        public Downloader.Downloader Downloader1 = new Downloader.Downloader();
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
        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            if (lstMenu.Visibility == Visibility.Hidden)
            {
                lstMenu.Visibility = Visibility.Visible;
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/close_menu.png", UriKind.Relative));
            }
            else
            {
                lstMenu.Visibility = Visibility.Hidden;
                lstLoginAs.Visibility = Visibility.Hidden;
                lstLoginAs.UnselectAll();
                lstMenu.UnselectAll();
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
            }
        }
        private void lstMenu_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)lstMenu.SelectedValue;
            if (item != null)
            {
                switch (item.Content.ToString())
                {
                    case "New Login":
                        lstMenu.Visibility = Visibility.Hidden;
                        MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
                        WebLogin WebLogin1 = new WebLogin();
                        WebLogin1.RaiseCustomEvent += new EventHandler<CustomEventArgs>(WebLogin1_RaiseCustomEvent);
                        WebLogin1.Show();
                        lstMenu.UnselectAll();
                        break;
                    case "Login as...":
                        if (UserManager1.GetListOfUsers().Count > 0)
                        {
                            lstLoginAs.Visibility = Visibility.Visible;
                            ObservableCollection<User> oUsers = new ObservableCollection<User>(UserManager1.GetListOfUsers());
                            lstLoginAs.DataContext = oUsers;
                            Binding binding = new Binding();
                            lstLoginAs.SetBinding(ListBox.ItemsSourceProperty, binding);
                        }
                        break;
                }
            }
        }
        private void WebLogin1_RaiseCustomEvent(object sender, CustomEventArgs e)
        {
            List<Song> SongList = new List<Song>(VKAPI1.GetAudioExternal(e.UserID.ToString(), e.AccessToken));
            SetUser(new User(e.Name, e.AccessToken, e.UserID.ToString(), SongList), false);
            
        }
        private void lstLoginAs_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            SetUser((User)lstLoginAs.SelectedValue, true);
        }
        private void lstLoginAs_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
            lstMenu.Visibility = Visibility.Hidden;
            lstLoginAs.Visibility = Visibility.Hidden;
            lstMenu.UnselectAll();
            lstLoginAs.UnselectAll();
        }
        private void SetUser(User user, bool wasAlreadyEnteringThroughThisApp)
        {
            Player1.Stop();
            CurrentUser = user;
            if (!wasAlreadyEnteringThroughThisApp)
            {
                UserManager1.AddUser(user);
                Playlist1.UpdateList(user.SongList);
                Infrastructure1.SaveListOfUsers(UserManager1.GetListOfUsers());
            }
            else
            {
                List<Song> SongList = new List<Song>(VKAPI1.GetAudioExternal(user.ID, user.AccessToken));
                UserManager1.UpdateUserListOfSongs(user.ID, SongList);
                Playlist1.UpdateList(SongList);
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
                lstMenu.Visibility = Visibility.Hidden;
                lstMenu.UnselectAll();
                lstLoginAs.Visibility = Visibility.Hidden;
                lstLoginAs.UnselectAll();

            }
            if (user.SongList.Count > 0)
            {
                CurrentSong = 0;
                Player1.SetSource(user.SongList[CurrentSong]);
                RenderPlaylist(user.SongList);
                RenderNameAndSelectedSong();
                txtSongTime.Text = String.Format("0:00 / {0}", user.SongList[0].Duration);
                Player1.Play();
            }
            else
            {
                if (!wasAlreadyEnteringThroughThisApp)
                {
                    MessageBox.Show("У данного пользователя нет аудиозаписей.", "VVKMusic информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Вконтакте сообщает, что аудиозаписей нет. Если на данном аккаунте есть аудиозаписи, возможно Вам нужно залогиниться заново.", "VVKMusic информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            List<Song> ListToDownload = new List<Song>();
            if (e.GetHashCode() == btnDownload.GetHashCode())
                ListToDownload.Add(Playlist1.GetList()[CurrentSong]);
            else
                ListToDownload = Playlist1.GetList();
            if (Downloader1.DownloadSong(ListToDownload) == Common.Common.Status.Error)
                MessageBox.Show("Ошибка скачивания", "", MessageBoxButton.OK);
            else
                MessageBox.Show("Скачивание завершено", "", MessageBoxButton.OK);
        }
        private void btnPrev_Click(object sender, RoutedEventArgs e)
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
            Player1.SetSource(SongList[CurrentSong]);
            Player1.Play();
            RenderNameAndSelectedSong();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
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
            Player1.SetSource(SongList[CurrentSong]);
            Player1.Play();
            RenderNameAndSelectedSong();
        }
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            Player1.Pause();
        }
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Player1.Play();
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            RenderPlaylist(Playlist1.SearchSong(txtSearch.Text.ToLower()));
        }
        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
        }
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "")
                txtSearch.Text = "Search";
        }
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RenderPlaylist(Playlist1.SearchSong(txtSearch.Text.ToLower()));
            }
        }
        private void btnMix_Click(object sender, RoutedEventArgs e)
        {
            Playlist1.MixPlaylist();
            RenderPlaylist(Playlist1.GetList());
        }
        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            Playlist1.SortByDownloaded();
            RenderPlaylist(Playlist1.GetList());
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnExpand_Click(object sender, RoutedEventArgs e)
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
        private void btnCollapse_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void rectangleDraggable_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void RenderNameAndSelectedSong()
        {
            List<Song> SongList = Playlist1.GetList();
            txtSongName.Text = SongList[CurrentSong].ToString();
            if(lstPlaylist.Items != null)
                lstPlaylist.SelectedItem = (lstPlaylist.Items[CurrentSong]);
        }
        private void RenderPlaylist(List<Song> SongList) 
        {
            ObservableCollection<Song> oSong = new ObservableCollection<Song>(SongList);
            lstPlaylist.DataContext = oSong;
            Binding binding = new Binding();
            lstPlaylist.SetBinding(ListBox.ItemsSourceProperty, binding);
        }
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txtSearch.Text == "")
            {
                RenderPlaylist(Playlist1.GetList());
            }
        }
        private void lstPlaylist_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lstPlaylist.SelectedIndex != -1 && CurrentSong != lstPlaylist.SelectedIndex)
            {
                CurrentSong = lstPlaylist.SelectedIndex;
                RenderNameAndSelectedSong();
                Player1.Stop();
                List<Song> SongList = Playlist1.GetList();
                Player1.SetSource(SongList[CurrentSong]);
                Player1.Play();
            }
        }
    }
}

