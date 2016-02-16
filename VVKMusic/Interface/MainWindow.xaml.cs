using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.Bass.Misc;


namespace Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Playlist.Playlist Playlist1 = new Playlist.Playlist();
        private Player.Player Player1 = new Player.Player();
        private UserManager.UserManager UserManager1 = new UserManager.UserManager();
        private VKAPI.VKAPI VKAPI1 = new VKAPI.VKAPI();
        private Infrastructure.Infrastructure Infrastructure1 = new Infrastructure.Infrastructure();
        private Downloader.Downloader Downloader1 = new Downloader.Downloader();

        private User _CurrentUser = null;
        private int _CurrentSong = 0;
        private int _updateInterval = 50;
        private int _tickCounter = 0;
        
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
            _CurrentUser = user;
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
                _CurrentSong = 0;
                Player1.SetSource(user.SongList[_CurrentSong]);
                RenderPlaylist(user.SongList);
                RenderNameAndSelectedSong();
                Player1.SetTimer(_updateInterval, timerUpdate_Tick);
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
            if (sender.GetHashCode() == btnDownload.GetHashCode())
                ListToDownload.Add(Playlist1.GetList()[_CurrentSong]);
            else
                ListToDownload = Playlist1.GetList();
            if (Downloader1.DownloadSong(ListToDownload) == Common.Common.Status.Error)
                MessageBox.Show("Ошибка скачивания", "", MessageBoxButton.OK);
        }
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
            List<Song> SongList = Playlist1.GetList();
            if(SongList.Count > 0)
                if(_CurrentSong > 0)
                {
                    _CurrentSong--;
                }
                else
                {
                    _CurrentSong = SongList.Count - 1;
                }
            Player1.SetSource(SongList[_CurrentSong]);
            Player1.Play();
            RenderNameAndSelectedSong();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Player1.Stop();
            List<Song> SongList = Playlist1.GetList();
            if (SongList.Count > 0)
                if (_CurrentSong < SongList.Count - 1)
                {
                    _CurrentSong++;
                }
                else
                {
                    _CurrentSong = 0;
                }
            Player1.SetSource(SongList[_CurrentSong]);
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
            Player1.Stop();
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
            txtSongName.Text = SongList[_CurrentSong].ToString();
            if(lstPlaylist.Items != null)
                lstPlaylist.SelectedItem = (lstPlaylist.Items[_CurrentSong]);
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
            if (lstPlaylist.SelectedIndex != -1 && _CurrentSong != lstPlaylist.SelectedIndex)
            {
                _CurrentSong = lstPlaylist.SelectedIndex;
                RenderNameAndSelectedSong();
                Player1.Stop();
                List<Song> SongList = Playlist1.GetList();
                Player1.SetSource(SongList[_CurrentSong]);
                Player1.Play();
            }
        }

        #region ProgressBar
        private void timerUpdate_Tick(object sender, System.EventArgs e)
        {
            int _stream = Player1.Stream;
            if (Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PLAYING)
            { }
            else
            {
                DrawPosition(-1, -1);
                return;
            }
            _tickCounter++;
            if (_tickCounter == 5)
            {
                long pos = Bass.BASS_ChannelGetPosition(_stream); // position in bytes
                long len = Bass.BASS_ChannelGetLength(_stream); // length in bytes
                // display the position every 250ms (since timer is 50ms)
                _tickCounter = 0;
                double totaltime = Bass.BASS_ChannelBytes2Seconds(_stream, len);
                double elapsedtime = Bass.BASS_ChannelBytes2Seconds(_stream, pos);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => 
                    txtSongTime.Text = String.Format("{0:#0.00} / {1:#0.00}", Utils.FixTimespan(elapsedtime, "MMSS"), Utils.FixTimespan(totaltime, "MMSS"))));
                DrawPosition(pos, len);
            }
        }
        private void DrawPosition(long pos, long len)
        {
            if (len == 0 || pos < 0)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    rectangleProgressBarElapsed.Width = 0;
                    circleProgressBar.Visibility = Visibility.Hidden;
                    circleProgressBar.Margin = new Thickness(0);
                }));
                return;
            }

            if (circleProgressBar.Visibility == Visibility.Hidden)
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    circleProgressBar.Visibility = Visibility.Visible));

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                double bpp = len / (double)rectangleProgressBarMain.Width;  // bytes per pixel
                int x = (int)Math.Round(pos / bpp);
                rectangleProgressBarElapsed.Width = x;
                circleProgressBar.Margin = new Thickness(x, 0, 0, 0);
            }));
        }
        private void rectangleProgressBarMain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            int _stream = Player1.Stream;
            if (_CurrentUser == null || _CurrentSong < 0)
                return;
            long len = Bass.BASS_ChannelGetLength(_stream);
            double multiplyer = 0;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                multiplyer = e.GetPosition(rectangleProgressBarMain).X / rectangleProgressBarMain.Width;
                long pos = (long)(multiplyer * len);
                Bass.BASS_ChannelSetPosition(_stream, pos);
            }));
        }
        #endregion

        
    }
}

