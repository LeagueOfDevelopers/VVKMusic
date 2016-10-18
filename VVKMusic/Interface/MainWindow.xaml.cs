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
using System.Collections;
using System.Windows.Media.Effects;
using System.Linq;
using WPF.JoshSmith.ServiceProviders.UI;

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
        private ListViewDragDropManager<Song> dragMgr;

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
                this.Loaded += MainWindow_Loaded;
            }
        }
        #region MainWindow_Loaded

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.dragMgr = new ListViewDragDropManager<Song>(this.listboxPlaylist);
            this.dragMgr.ListView = this.listboxPlaylist;
            this.dragMgr.ShowDragAdorner = true;
            this.dragMgr.DragAdornerOpacity = 0.7;
            this.dragMgr.ProcessDrop += dragMgr_ProcessDrop;
            this.listboxPlaylist.DragEnter += OnListViewDragEnter;
            this.listboxPlaylist.Drop += OnListViewDrop;
        }

        #endregion // MainWindow_Loaded
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
                Playlist1.SetBaseList(new ObservableCollection<Song>(user.SongList));
                Playlist1.UpdateListToBase();
                Infrastructure1.SaveListOfUsers(UserManager1.GetListOfUsers());
            }
            else
            {
                List<Song> SongList = new List<Song>(VKAPI1.GetAudioExternal(user.ID, user.AccessToken));
                UserManager1.UpdateUserListOfSongs(user.ID, SongList);
                Playlist1.SetBaseList(new ObservableCollection<Song>(SongList));
                Playlist1.UpdateListToBase();
                MenuButtonImage.Source = new BitmapImage(new Uri("/Resources/Pictures/menu.png", UriKind.Relative));
                listboxMenu.Visibility = Visibility.Hidden;
                listboxMenu.UnselectAll();
                listboxLoginAs.Visibility = Visibility.Hidden;
                listboxLoginAs.UnselectAll();

            }
            if (user.SongList.Count > 0)
            {
                _CurrentSong = -1;
                RenderPlaylist(new ObservableCollection<Song> (user.SongList));
                Player1.SetTimer(_updateInterval, timerUpdate_Tick);
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
                    song.DownloadImage = @"Resources/Pictures/ok_lightgrey.png";
                else
                    Downloader1.CheckIfDownloaded(song);
            }
            if (Downloader1.DownloadSong(listToDownload, ProgressChanged, DownloadSongCallback) == Common.Common.Status.Error)
                MessageBox.Show("Ошибка скачивания", "", MessageBoxButton.OK);
            ListToDownload1.Clear();
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            Song song = (Song)(sender as FrameworkElement).Tag;
            if (song.DownloadImage == @"Resources/Pictures/ok_lightgrey.png")
            {
                song.DownloadImage = @"Resources/Pictures/ok_orangefill.png";
                ListToDownload1.Add(song);
            }
            else
            {
                if (song.Downloaded)
                    song.DownloadImage = @"Resources/Pictures/ok_small.png";
                else
                {
                    song.DownloadImage = @"Resources/Pictures/ok_lightgrey.png";
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
                        song.DownloadImage = @"Resources/Pictures/ok_small.png";
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
            HoverEffect(imagePrev, @"Resources/Pictures/prev.png");
            Player1.StopAndStopTimer();
            ObservableCollection<Song> SongList = Playlist1.GetList();
            if (SongList.Count > 0)
                if (_CurrentSong > 0)
                {
                    _CurrentSong--;
                }
                else
                {
                    _CurrentSong = SongList.Count - 1;
                }
            Downloader1.CheckIfDownloaded(SongList[_CurrentSong]);
            Player1.SetSource(SongList[_CurrentSong]);
            Player1.PlayAndStartTimer();
            RenderNameAndSelectedSong();
            ListboxPullOver(_CurrentSong);
        }
        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imageNext, @"Resources/Pictures/next.png");
            NextSongMethod();         
        }

        private void NextSongMethod()
        {
            Player1.StopAndStopTimer();
            ObservableCollection<Song> SongList = Playlist1.GetList();
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
            Downloader1.CheckIfDownloaded(SongList[_CurrentSong]);
            Player1.SetSource(SongList[_CurrentSong]);
            ListboxPullOver(_CurrentSong);
            Player1.PlayAndStartTimer();
            if (!_repeat)
                RenderNameAndSelectedSong();
        }

        private void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (Player1.IsTimerStarted())
            {
                HoverEffect(imagePlayOrPause, @"Resources/Pictures/play.png");
                Player1.PauseAndStopTimer();
            }
            else
            {
                Play();
            }
        }
        private void Play()
        {
            HoverEffect(imagePlayOrPause, @"Resources/Pictures/pause.png");
            if (_CurrentSong == -1)
            {
                ObservableCollection<Song> SongList = Playlist1.GetList();
                if (SongList.Count > 0)
                    _CurrentSong = 0;
                Player1.SetSource(SongList[_CurrentSong]);
                RenderNameAndSelectedSong();
            }
            Player1.PlayAndStartTimer();
        }
        //private void buttonStop_Click(object sender, RoutedEventArgs e)
        //{
        //    HoverEffect(imageStop, @"Resources/Pictures/stop.png");
        //    Player1.StopAndStopTimer();
        //    ObservableCollection<Song> SongList = Playlist1.GetList();
        //    _CurrentSong = -1;
        //    DrawPosition(0, 0);
        //    Player1.SetTimer(_updateInterval, timerUpdate_Tick);
        //    RenderNameAndSelectedSong();               
        //}
        private void textboxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            textboxSearch.Text = "";
        }
        private void textboxSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textboxSearch.Text == "")
                textboxSearch.Text = "Search";
        }
        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imageSearch, @"Resources/Pictures/search.png");
            Search();
        }
        private void textboxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Search();
            }
        }
        private void Search()
        {
            Playlist1.UpdateListToBase();
            _CurrentSong = -1;
            if (textboxSearch.Text != "")
            {
                List<Song> SongList = new List<Song>((List<Song>)Playlist1.SearchSong(textboxSearch.Text.ToLower()));
                SongList.AddRange(VKAPI1.SearchAudio(textboxSearch.Text.ToLower(),_CurrentUser.AccessToken));
                SongList = SongList.Distinct(new SongComparer()).ToList<Song>();
                // if (Playlist1.SearchSong(textboxSearch.Text.ToLower()).Count > 0) SongList[Playlist1.SearchSong(textboxSearch.Text.ToLower()).Count-1].BorderBrush = (Brush)new BrushConverter().ConvertFrom("#F59184");
                Playlist1.UpdateList(new ObservableCollection<Song>(SongList));
            }
            RenderPlaylist(Playlist1.GetList());
            BorderPaintOff();
            Playlist1.LostSort();
        }
        private void buttonMix_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imageMix, @"Resources/Pictures/mix.png");
            Playlist1.MixPlaylist();
            RenderPlaylist(Playlist1.GetList());
            _CurrentSong = -1;
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
            HoverEffect(imageSettings, "Resources/Pictures/settings.png");
        }

        private void buttonSort_Click(object sender, RoutedEventArgs e)
        {
            HoverEffect(imageSort, "Resources/Pictures/sort.png");
            if (listboxSort.Visibility == Visibility.Hidden)
            {
                listboxSort.Visibility = Visibility.Visible;
            }
            else
            {
                listboxSort.Visibility = Visibility.Hidden;
                listboxSort.UnselectAll();             
            }
        }
        private void listboxSort_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)listboxSort.SelectedValue;
            BorderPaintOff();
            _CurrentSong = -1;
            if (item != null)
            {
                switch (item.Content.ToString())
                {
                    case "By downloaded":
                        Playlist1.SortByDownloaded();
                        BorderPaintDownloaded();
                        break;
                    case "By duration":
                        Playlist1.SortByDuration();
                        break;
                    case "By artist":
                        Playlist1.SortByArtist();
                        BorderPaintArtist();
                        break;
                    case "By song title":
                        Playlist1.SortByTitle();
                        break;
                }
                RenderPlaylist(Playlist1.GetList());
                listboxSort.Visibility = Visibility.Hidden;
                listboxSort.UnselectAll();
            }
        }
        private void BorderPaintDownloaded()
        {
            ObservableCollection<Song> playlist = Playlist1.GetList();
            if (playlist.Count > 1)
            {
                for (int i = 0; i < playlist.Count - 1; i++)
                {
                    if (playlist[i].Downloaded != playlist[i + 1].Downloaded)
                    {
                        playlist[i].BorderBrush = (Brush)new BrushConverter().ConvertFrom("#F59184");
                    }
                }
            }
            listboxPlaylist.Items.Refresh();
        }
        private void BorderPaintArtist()
        {
            ObservableCollection<Song> playlist = Playlist1.GetList();
            if (playlist.Count > 1)
            {
                for (int i = 0; i < playlist.Count - 1; i++)
                {
                    if (playlist[i].Artist != playlist[i + 1].Artist)
                    {
                        playlist[i].BorderBrush = (Brush)new BrushConverter().ConvertFrom("#F59184");
                    }
                }
            }
            listboxPlaylist.Items.Refresh();
        }
        private void BorderPaintOff()
        {
            ObservableCollection<Song> playlist = Playlist1.GetList();
            foreach (Song song in playlist)
                song.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFD1D3DA");
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                      listboxPlaylist.Items.Refresh()));
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

        private void RenderNameAndSelectedSong()
        {
            ObservableCollection<Song> SongList = Playlist1.GetList();
            if (_CurrentSong == -1)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                   textboxSongName.Text = ""));
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                textboxSongTime.Text = ""));
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                       textboxSongName.Text = SongList[_CurrentSong].ToString()));
                if (listboxPlaylist.Items != null)
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                       listboxPlaylist.SelectedItem = (Song)listboxPlaylist.Items[_CurrentSong]));
            }
        }

        private void RenderPlaylist(ObservableCollection<Song> oSongs)
        {
            foreach (Song song in oSongs)
            {
                Downloader1.CheckIfDownloaded(song);
                Playlist1.CheckIfBaseListOfSongsContainsSong(song);
            }
            listboxPlaylist.DataContext = oSongs;
            listboxPlaylist.AlternationCount = oSongs.Count;
            Binding binding = new Binding();
            binding.Source = oSongs;
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
        private void listboxPlaylist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RenderPlaylist(Playlist1.GetList());
            if (listboxPlaylist.SelectedIndex != -1 && _CurrentSong != listboxPlaylist.SelectedIndex)
            {
                ListboxPullOver(listboxPlaylist.SelectedIndex);
                RenderNameAndSelectedSong();
                Player1.StopAndStopTimer();
                ObservableCollection<Song> SongList = Playlist1.GetList();
                Downloader1.CheckIfDownloaded(SongList[0]);
                Player1.SetSource(SongList[0]);
                Play();
            }
        }

        private void ListboxPullOver(Int32 current)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                if (listboxPlaylist.SelectedIndex != 0)
                {
                    BorderPaintOff();
                    Playlist1.LostSort();
                }
            }));
            for (int i = current; i < Playlist1.GetList().Count; i++)
            {
                for (int j = i; j > i - current; j--)
                {
                    Song song = Playlist1.GetList()[j - 1];
                    Playlist1.GetList()[j - 1] = Playlist1.GetList()[j];
                    Playlist1.GetList()[j] = song;
                }
            }
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                   listboxPlaylist.SelectedIndex = 0));
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                  listboxPlaylist.Items.Refresh()));

            _CurrentSong = 0;
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
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    DrawPosition(-1, -1);
                    NextSongMethod();
                }));
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

        #region dragMgr_ProcessDrop

        // Performs custom drop logic for the top ListView.
        void dragMgr_ProcessDrop(object sender, ProcessDropEventArgs<Song> e)
        {
            // This shows how to customize the behavior of a drop.
            // Here we perform a swap, instead of just moving the dropped item.

            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            if (lowerIdx < 0)
            {
                // The item came from the lower ListView
                // so just insert it.
                e.ItemsSource.Insert(higherIdx, e.DataItem);
            }
            else
            {
                // null values will cause an error when calling Move.
                // It looks like a bug in ObservableCollection to me.
                if (e.ItemsSource[lowerIdx] == null ||
                    e.ItemsSource[higherIdx] == null)
                    return;

                // The item came from the ListView into which
                // it was dropped, so swap it with the item
                // at the target index.
                e.ItemsSource.Move(lowerIdx, higherIdx);
                e.ItemsSource.Move(higherIdx - 1, lowerIdx);
            }

            // Set this to 'Move' so that the OnListViewDrop knows to 
            // remove the item from the other ListView.
            e.Effects = DragDropEffects.Move;
        }

        #endregion // dragMgr_ProcessDrop

        #region OnListViewDragEnter

        // Handles the DragEnter event for both ListViews.
        void OnListViewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        #endregion // OnListViewDragEnter

        #region OnListViewDrop

        // Handles the Drop event for both ListViews.
        void OnListViewDrop(object sender, DragEventArgs e)
        {
            if (e.Effects == DragDropEffects.None)
                return;

            Task task = e.Data.GetData(typeof(Task)) as Task;
            if (sender == this.listboxPlaylist)
            {
                if (this.dragMgr.IsDragInProgress)
                    return;

                // An item was dragged from the bottom ListView into the top ListView
                // so remove that item from the bottom ListView.
                (this.listboxPlaylist.ItemsSource as ObservableCollection<Task>).Remove(task);
            }
        }

        #endregion // OnListViewDrop

        //#region DragAndDrop
        //private Point startPoint;

        //private void listboxPlaylist_Drag(object sender, MouseButtonEventArgs e)
        //{
        //    ListBox parent = (ListBox)sender;
        //    startPoint = e.GetPosition(parent);
        //    object data = GetDataFromListBox(parent, e.GetPosition(parent));
        //    if (data != null)
        //    {
        //        DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
        //    }
        //}

        //private void listboxPlaylist_Drop(object sender, DragEventArgs e)
        //{
        //    BorderPaintOff();
        //    Playlist1.LostSort();
        //    try
        //    {
        //        ListBox parent = (ListBox)sender;
        //        Song data = (Song)e.Data.GetData(typeof(Song));
        //        Song data2 = (Song)GetDataFromListBox(parent, e.GetPosition(parent));
        //        Int32 index = parent.Items.IndexOf(data2);
        //        if (data2 != null)
        //        {
        //            ((IList)parent.ItemsSource).Remove(data);
        //            ((IList)parent.ItemsSource).Insert(index, data);
        //        }
        //        else
        //        {
        //            ((IList)parent.ItemsSource).Remove(data);
        //            ((IList)parent.ItemsSource).Add(data);
        //        }
        //        listboxPlaylist.Items.Refresh();
        //    }
        //    catch { }
        //}

        //#region GetDataFromListBox(ListBox,Point)
        //private static object GetDataFromListBox(ListBox source, Point point)
        //{
        //    UIElement element = source.InputHitTest(point) as UIElement;
        //    if (element != null)
        //    {
        //        object data = DependencyProperty.UnsetValue;
        //        while (data == DependencyProperty.UnsetValue)
        //        {
        //            data = source.ItemContainerGenerator.ItemFromContainer(element);
        //            if (data == DependencyProperty.UnsetValue)
        //            {
        //                element = VisualTreeHelper.GetParent(element) as UIElement;
        //            }
        //            if (element == source)
        //            {
        //                return null;
        //            }
        //        }
        //        if (data != DependencyProperty.UnsetValue)
        //        {
        //            return data;
        //        }
        //    }
        //    return null;
        //}
        //#endregion

        //#endregion

        #region MouseEnter MouseLeave
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            DropShadowEffect dse = new DropShadowEffect()
            {
                Opacity = 1,
                ShadowDepth = 0,
                Color = Colors.Orange
            };
            button.Effect = dse;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            DropShadowEffect dse = new DropShadowEffect() { Opacity = 0 };
            button.Effect = dse;
        }
        private void ButtonAddRemove_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            Image image = (Image)button.FindName("imageAddRemove");
            image.Visibility = Visibility.Visible;
            TextBlock textblock = (TextBlock)button.FindName("textBlockDuration");
            textblock.Visibility = Visibility.Hidden;
        }

        private void ButtonAddRemove_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            Image image = (Image)button.FindName("imageAddRemove");
            image.Visibility = Visibility.Hidden;
            TextBlock textblock = (TextBlock)button.FindName("textBlockDuration");
            textblock.Visibility = Visibility.Visible;
        }

        #endregion //MouseEnter MouseLeave
        private void buttonAddRemove_Click(object sender, RoutedEventArgs e)
        {
            Song song = (Song)(sender as FrameworkElement).Tag;
            if (song.AddRemoveImage == @"Resources/Pictures/ok_lightgrey.png")
            {
                song.AddRemoveImage = @"Resources/Pictures/ok_small.png"; ;
                VKAPI1.AddAudio(song.ID, _CurrentUser.ID, _CurrentUser.AccessToken);
                Playlist1.AddToBaseList(song);
            }
            else
            {
                song.AddRemoveImage = @"Resources/Pictures/ok_lightgrey.png";
                //VKAPI1.DeleteAudio(song.ID);
                Playlist1.RemoveFromBaseList(song);
            }
            listboxPlaylist.Items.Refresh();
        }

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