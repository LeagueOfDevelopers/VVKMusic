using Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using System.Globalization;
using System.Net;
using System.ComponentModel;
using System.Windows.Media;
using System.Threading.Tasks;

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
        private List<Song> ListToDownload1 = new List<Song>();

        private User _CurrentUser = null;
        private int _CurrentSong = 0;
        private int _updateInterval = 50;
        private int _tickCounter = 0;
        private bool _repeat = false;

        public MainWindow()
        {
            InitializeComponent();
            if (Infrastructure1.LoadListOfUsers() != null)
            {
                UserManager1.UpdateUserList(Infrastructure1.LoadListOfUsers());
            }
        }
        private void buttonMenu_Click(object sender, RoutedEventArgs e)
        {
            if (listboxMenu.Visibility == Visibility.Hidden)
            {
                listboxMenu.Visibility = Visibility.Visible;
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/close_menu.png", UriKind.Relative));
            }
            else
            {
                listboxMenu.Visibility = Visibility.Hidden;
                listboxLoginAs.Visibility = Visibility.Hidden;
                listboxLoginAs.UnselectAll();
                listboxMenu.UnselectAll();
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
            }
        }
        private void listboxMenu_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)listboxMenu.SelectedValue;
            if (item != null)
            {
                switch (item.Content.ToString())
                {
                    case "New Login":
                        listboxMenu.Visibility = Visibility.Hidden;
                        listboxLoginAs.Visibility = Visibility.Hidden;
                        MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
                        WebLogin WebLogin1 = new WebLogin();
                        WebLogin1.RaiseCustomEvent += new EventHandler<CustomEventArgs>(WebLogin1_RaiseCustomEvent);
                        WebLogin1.Show();
                        listboxMenu.UnselectAll();
                        break;
                    case "Login as...":
                        if (UserManager1.GetListOfUsers().Count > 0)
                        {
                            listboxLoginAs.Visibility = Visibility.Visible;
                            ObservableCollection<User> oUsers = new ObservableCollection<User>(UserManager1.GetListOfUsers());
                            listboxLoginAs.DataContext = oUsers;
                            Binding binding = new Binding();
                            listboxLoginAs.SetBinding(ListBox.ItemsSourceProperty, binding);
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
        private void listboxLoginAs_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            SetUser((User)listboxLoginAs.SelectedValue, true);
        }
        private void listboxLoginAs_MouseLeave(object sender, MouseEventArgs e)
        {
            MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
            listboxMenu.Visibility = Visibility.Hidden;
            listboxLoginAs.Visibility = Visibility.Hidden;
            listboxMenu.UnselectAll();
            listboxLoginAs.UnselectAll();
        }
        private void SetUser(User user, bool wasAlreadyEnteringThroughThisApp)
        {
            Player1.StopAndStopTimer();
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
                listboxMenu.Visibility = Visibility.Hidden;
                listboxMenu.UnselectAll();
                listboxLoginAs.Visibility = Visibility.Hidden;
                listboxLoginAs.UnselectAll();

            }
            if (user.SongList.Count > 0)
            {
                _CurrentSong = 0;
                Player1.SetSource(user.SongList[_CurrentSong]);
                RenderPlaylist(user.SongList);
                RenderNameAndSelectedSong();
                Player1.SetTimer(_updateInterval, timerUpdate_Tick);
                Player1.PlayAndStartTimer();
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

        private void buttonDownload_Click(object sender, RoutedEventArgs e)
        {           
            List<Song> listToDownload = new List<Song>();
            if (sender.GetHashCode() == buttonDownload.GetHashCode())
            {
                HoverEffect(imageDownload, @"Resources/Pictures/download.png");
                listToDownload = this.ListToDownload1;
            }
            else
            {
                HoverEffect(imageDownloadOne, @"Resources/Pictures/download.png");
                listToDownload.Add(Playlist1.GetList()[_CurrentSong]);
            }
            foreach (Song song in ListToDownload1)
            {
                if (!song.Downloaded)
                    song.Image = @"Resources/Pictures/ok_lightgrey.png";
                else
                    song.Image = @"Resources/Pictures/ok_small.png";
            }
            if (Downloader1.DownloadSong(listToDownload, ProgressChanged, DownloadSongCallback) == Common.Common.Status.Error)
                MessageBox.Show("Ошибка скачивания", "", MessageBoxButton.OK);
            ListToDownload1.Clear();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Song song = (Song)(sender as FrameworkElement).Tag;
            if (song.Image == @"Resources/Pictures/ok_lightgrey.png")
            {
                song.Image = @"Resources/Pictures/ok_orangefill.png";
                ListToDownload1.Add(song);
            }
            else
            {
                if (song.Downloaded)
                    song.Image = @"Resources/Pictures/ok_small.png";
                else
                {
                    song.Image = @"Resources/Pictures/ok_lightgrey.png";
                    ListToDownload1.Remove(song);
                }
            }
            listboxPlaylist.Items.Refresh();
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                foreach (Song song in Playlist1.GetList())
                {
                    if (song.GetHashCode() == Downloader1.hashCodeConnection[sender.GetHashCode()])
                    {
                        if (song.Percentage != e.ProgressPercentage)
                        {
                            song.Percentage = e.ProgressPercentage;
                            listboxPlaylist.Items.Refresh();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DownloadSongCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                foreach (Song song in Playlist1.GetList())
                {
                    if (song.GetHashCode() == Downloader1.hashCodeConnection[sender.GetHashCode()])
                    {
                        song.Image = @"Resources/Pictures/ok_small.png";
                        song.Percentage = 0;
                        song.Downloaded = true;
                        listboxPlaylist.Items.Refresh();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imagePrev,@"Resources/Pictures/prev.png");
            Player1.StopAndStopTimer();
            List<Song> SongList = Playlist1.GetList();
            if (SongList.Count > 0)
                if (_CurrentSong > 0)
                {
                    _CurrentSong--;
                }
                else
                {
                    _CurrentSong = SongList.Count - 1;
                }
            Player1.SetSource(SongList[_CurrentSong]);
            Player1.PlayAndStartTimer();
            RenderNameAndSelectedSong();
        }
        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imageNext, @"Resources/Pictures/next.png");
            Player1.StopAndStopTimer();
            List<Song> SongList = Playlist1.GetList();
            if (SongList.Count > 0)
                if (_CurrentSong < SongList.Count - 1)
                {
                    if (!_repeat)
                        _CurrentSong++;
                }
                else
                {
                    _CurrentSong = 0;
                }
            Player1.SetSource(SongList[_CurrentSong]);
            Player1.PlayAndStartTimer();
            if (!_repeat)
                RenderNameAndSelectedSong();
        }
        private void buttonPause_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imagePause, @"Resources/Pictures/pause.png");
            Player1.PauseAndStopTimer();
        }
        private void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imagePlay, @"Resources/Pictures/play.png");
            Player1.PlayAndStartTimer();
        }
        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imageStop, @"Resources/Pictures/stop.png");
            Player1.StopAndStopTimer();        
        }
        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imageSearch, @"Resources/Pictures/search.png");
            //RenderPlaylist(Playlist1.SearchSong(textboxSearch.Text.ToLower()));
        }
        private void textboxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            textboxSearch.Text = "";
        }
        private void textboxSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textboxSearch.Text == "")
                textboxSearch.Text = "Search";
        }
        private void textboxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //RenderPlaylist(Playlist1.SearchSong(textboxSearch.Text.ToLower()));
            }
        }
        private void buttonMix_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imageMix, @"Resources/Pictures/mix.png");
            Playlist1.MixPlaylist();
            RenderPlaylist(Playlist1.GetList());
        }

        private void buttonRepeat_Click(object sender, RoutedEventArgs e)
        {
            if (_repeat)
            {
                _repeat = false;
                imageRepeat.Source = AddImage("Resources/Pictures/repeat_grey.png"); 
            }
            else
            {
                _repeat = true;
                imageRepeat.Source = AddImage("Resources/Pictures/repeat.png");
            }
        }

        private void buttonSettings_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imageSettings,"Resources/Pictures/settings.png");
        }

        private void buttonSort_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imageSort,"Resources/Pictures/sort.png");
            Playlist1.SortByDownloaded();
            RenderPlaylist(Playlist1.GetList());
        }
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Player1.StopAndStopTimer();
            this.Close();
        }
        private void buttonCollapse_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void rectangleDraggable_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SetInformation(string text)
        {
            Action action = new Action(() => {textboxSongName.Text = text; });
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(action);
            else
                action();
        }
        private void SetInformation(Song song)
        {
            Action action = new Action(() => { listboxPlaylist.SelectedItem = song; });
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(action);
            else
                action();
        }

        private void RenderNameAndSelectedSong()
        {
                List<Song> SongList = Playlist1.GetList();
                SetInformation(SongList[_CurrentSong].ToString());
                if (listboxPlaylist.Items != null)
                    SetInformation((Song)listboxPlaylist.Items[_CurrentSong]);
        }

        private void RenderPlaylist(List<Song> SongList)
        {
            foreach (Song song in SongList)
                if (song.Downloaded)
                    song.Image = @"Resources/Pictures/ok_.png";
            listboxPlaylist.ItemsSource = SongList;
            listboxPlaylist.AlternationCount = SongList.Count;
            Binding binding = new Binding();
            binding.Source = SongList;
            listboxPlaylist.SetBinding(ListBox.ItemsSourceProperty, binding);

            listboxPlaylist.Items.Refresh();
        }
        private void textboxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textboxSearch.Text == "")
            {
                RenderPlaylist(Playlist1.GetList());
            }
        }
        private void listboxPlaylist_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (listboxPlaylist.SelectedIndex != -1 && _CurrentSong != listboxPlaylist.SelectedIndex)
            {
                _CurrentSong = listboxPlaylist.SelectedIndex;
                RenderNameAndSelectedSong();
                Player1.StopAndStopTimer();
                List<Song> SongList = Playlist1.GetList();
                Player1.SetSource(SongList[_CurrentSong]);
                Player1.PlayAndStartTimer();
            }
        }

        private BitmapImage AddImage(string adress)
        {
            BitmapImage bmi = new BitmapImage(new Uri(adress, UriKind.RelativeOrAbsolute))
            { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
            return bmi;
        }

        private async void HoverEffect(Image image, String imageString)
        {
            image.Source = AddImage("Resources/Pictures/ok_orangefill.png");
            await Task.Delay(100);
            image.Source = AddImage(imageString);
        }

        #region ProgressBar
        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            int _stream = Player1.Stream;
            if (Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PLAYING)
            { }
            else
            {
                DrawPosition(-1, -1);
                buttonNext_Click(new object(), new RoutedEventArgs());
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
                    textboxSongTime.Text = String.Format("{0:#0.00} / {1:#0.00}", Utils.FixTimespan(elapsedtime, "MMSS"), Utils.FixTimespan(totaltime, "MMSS"))));
                DrawPosition(pos, len);
                //if (Math.Round(totaltime) == Math.Round(elapsedtime))
                //    buttonNext_Click(new object(), new RoutedEventArgs());
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
                if (x >= 0) circleProgressBar.Margin = new Thickness(x, 0, 0, 0);
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

    public class SongNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int number = (int)value;
            return ++number;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DurationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int time = (int)value;
            int minutes = time / 60;
            time -= minutes * 60;
            if (time < 10)
                return String.Format("{0}:0{1}", minutes, time);
            else
                return String.Format("{0}:{1}", minutes, time);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}