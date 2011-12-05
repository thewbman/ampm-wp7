using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Xml.Linq;
using System.Security.Cryptography;
using Microsoft.Phone.BackgroundAudio;
using MyAudioPlaybackAgent;

namespace AmpM
{
    public partial class Songs : PhoneApplicationPage
    {
        public Songs()
        {
            InitializeComponent();

            //DataContext = App.ViewModel;

            _items = new ObservableCollection<DataItemViewModel>();
            _searchItems = new ObservableCollection<DataItemViewModel>();

            //_items.Add(new DataItemViewModel() { PlaylistId = -1, PlaylistName = "playlist name", PlaylistItems = 44, AlbumId = -1, AlbumName = "album name", AlbumTracks = 34, ArtistAlbums = 2, ArtistId = 32, ArtistName = "artist name", ArtistTracks = 23, ArtUrl = "http://www.google.com/", SongId = 343, SongName = "song name", Type = "playlist" });

            //songList.ItemsSource = _items;
        }

        public ObservableCollection<DataItemViewModel> _items;
        public ObservableCollection<DataItemViewModel> _searchItems;

        private int viewsToRemove = 1;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //ApplicationTitle.Text = "AmpM - " + App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Name;

            string inValue = "";
            if (NavigationContext.QueryString.TryGetValue("Playlist", out inValue))
            {
                viewsToRemove = 2;

                performanceProgressBarCustomized.IsIndeterminate = true;

                this._items.Clear();

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("playlist_songs", "&filter=" + inValue)));
                webRequest.BeginGetResponse(new AsyncCallback(DataCallback), webRequest);
            }
            else if (NavigationContext.QueryString.TryGetValue("Album", out inValue))
            {
                viewsToRemove = 2;

                performanceProgressBarCustomized.IsIndeterminate = true;

                this._items.Clear();

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("album_songs", "&filter=" + inValue)));
                webRequest.BeginGetResponse(new AsyncCallback(DataCallback), webRequest);
            }
            else
            {
                this._items.Clear();
                //songList.ItemsSource = null;

                performanceProgressBarCustomized.IsIndeterminate = false;
            }
        }
        private void DataCallback(IAsyncResult asynchronousResult)
        {

            string resultString;

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Failed to get data response: " + ex.ToString(), "Error", MessageBoxButton.OK);
                });

                return;
            }

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            response.GetResponseStream().Close();
            response.Close();

            try
            {

                XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //MessageBox.Show("Got data response: " + resultString, "Error", MessageBoxButton.OK);
                });

                foreach (XElement singleDataElement in xdoc.Element("root").Descendants("song"))
                {
                    DataItemViewModel newItem = new DataItemViewModel();

                    newItem.SongId = int.Parse(singleDataElement.Attribute("id").Value);

                    newItem.SongName = singleDataElement.Element("title").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.ArtistId = int.Parse(singleDataElement.Element("artist").Attribute("id").Value);
                    newItem.ArtistName = singleDataElement.Element("artist").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.AlbumId = int.Parse(singleDataElement.Element("album").Attribute("id").Value);
                    newItem.AlbumName = singleDataElement.Element("album").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.SongTime = int.Parse(singleDataElement.Element("time").FirstNode.ToString());
                    newItem.SongTrack = int.Parse(singleDataElement.Element("track").FirstNode.ToString());

                    newItem.ArtUrl = singleDataElement.Element("art").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.SongUrl = singleDataElement.Element("url").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.ItemKey = "song" + newItem.SongId;
                    newItem.ItemId = newItem.SongId;
                    newItem.ItemChar = App.ViewModel.Functions.FirstChar(newItem.SongName);
                    newItem.Auth = App.ViewModel.AppSettings.AuthSetting;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _items.Add(newItem);

                        //MessageBox.Show("adding newItem to list: " + newItem.PlaylistName + " _ items: " + newItem.PlaylistItems + " _ id: " + newItem.PlaylistId);

                        //songList.ItemsSource = _items;
                    });

                }

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.SortAndDisplay();
                });

            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Error parsing data: " + ex.ToString());
                });
            }
        }
        private void DataSearchCallback(IAsyncResult asynchronousResult)
        {

            string resultString;

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Failed to get data response: " + ex.ToString(), "Error", MessageBoxButton.OK);
                });

                return;
            }

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            response.GetResponseStream().Close();
            response.Close();

            try
            {

                XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //MessageBox.Show("Got data response: " + resultString, "Error", MessageBoxButton.OK);
                });

                foreach (XElement singleDataElement in xdoc.Element("root").Descendants("song"))
                {
                    DataItemViewModel newItem = new DataItemViewModel();

                    newItem.SongId = int.Parse(singleDataElement.Attribute("id").Value);

                    newItem.SongName = singleDataElement.Element("title").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.ArtistId = int.Parse(singleDataElement.Element("artist").Attribute("id").Value);
                    newItem.ArtistName = singleDataElement.Element("artist").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.AlbumId = int.Parse(singleDataElement.Element("album").Attribute("id").Value);
                    newItem.AlbumName = singleDataElement.Element("album").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.SongTime = int.Parse(singleDataElement.Element("time").FirstNode.ToString());
                    newItem.SongTrack = int.Parse(singleDataElement.Element("track").FirstNode.ToString());

                    newItem.ArtUrl = singleDataElement.Element("art").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.SongUrl = singleDataElement.Element("url").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.ItemKey = "song" + newItem.SongId;
                    newItem.ItemId = newItem.SongId;
                    newItem.ItemChar = App.ViewModel.Functions.FirstChar(newItem.SongName);
                    newItem.Auth = App.ViewModel.AppSettings.AuthSetting;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _searchItems.Add(newItem);

                        //MessageBox.Show("adding newItem to list: " + newItem.PlaylistName + " _ items: " + newItem.PlaylistItems + " _ id: " + newItem.PlaylistId);

                        //songList.ItemsSource = _items;
                    });

                }

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.SortAndDisplay();
                });

            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Error parsing data: " + ex.ToString());
                });
            }
        }

        private void SortAndDisplay()
        {

            //songList.ItemsSource = _items;

            //var al = _items.OrderBy(x => x.AlbumName).ToArray();
            //var sl = _searchItems.OrderBy(x => x.SongName).ToArray();
            var al = _items.ToArray();
            var sl = _searchItems.ToArray();


            var songsByAlbum = from t in al
                                group t by t.AlbumName into c
                                //orderby c.Key
                                select new Group<DataItemViewModel>(c.Key, c);

            var songsBySearch = from t in sl
                                  group t by t.ItemChar into c
                                  //orderby c.Key
                                  select new Group<DataItemViewModel>(c.Key, c);



            SongsLL.ItemsSource = songsByAlbum;
            SongsSearchLL.ItemsSource = songsBySearch;

            songsPivot.Title = "SONGS (" + this._items.Count + ")";

            performanceProgressBarCustomized.IsIndeterminate = false;

            this.Perform(() => this.stopProgressBar(), 500);

        }
        private void stopProgressBar()
        {
            performanceProgressBarCustomized.IsIndeterminate = false;
        }



        public class Group<T> : IEnumerable<T>
        {
            public Group(string name, IEnumerable<T> items)
            {
                this.Title = name;
                this.Items = new List<T>(items);
            }

            public override bool Equals(object obj)
            {
                Group<T> that = obj as Group<T>;

                return (that != null) && (this.Title.Equals(that.Title));
            }

            public string Title
            {
                get;
                set;
            }

            public IList<T> Items
            {
                get;
                set;
            }

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            #endregion
        }

        private void Perform(Action myMethod, int delayInMilliseconds)
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (s, e) => Thread.Sleep(delayInMilliseconds);

            worker.RunWorkerCompleted += (s, e) => myMethod.Invoke();

            worker.RunWorkerAsync();
        }





        private void SelectAction(int inSelected, bool inAll, bool inShuffle, bool inAdd)
        {

            int currentPlayingCount = App.ViewModel.Nowplaying.Count;
            int trackIndex = 0;

            List<DataItemViewModel> newSongs = new List<DataItemViewModel>();

            if (!inAll)
            {
                newSongs.Add(this._items[inSelected]);
            }
            else if(!inShuffle)
            {
                foreach (DataItemViewModel s in _items)
                {
                    newSongs.Add(App.ViewModel.Functions.CloneItem(s));
                }

                trackIndex = inSelected;
            }
            else
            {
                Random r = new Random();
                List<DataItemViewModel> newSongs2 = new List<DataItemViewModel>();
                int i = 0;

                foreach (DataItemViewModel s in _items)
                {
                    newSongs2.Add(App.ViewModel.Functions.CloneItem(s));
                }

                newSongs.Add(newSongs2[inSelected]);
                newSongs2.RemoveAt(inSelected);

                while (newSongs2.Count > 0)
                {
                    i = r.Next(0, (newSongs2.Count-1));

                    newSongs.Add(newSongs2[i]);
                    newSongs2.RemoveAt(i);

                }

                trackIndex = 0;

            }

            if (!inAdd)
            {
                MyAudioPlaybackAgent.AudioPlayer.resetList();
                App.ViewModel.Nowplaying.Clear();

                foreach (DataItemViewModel s in newSongs)
                {
                    App.ViewModel.Nowplaying.Add(App.ViewModel.Functions.CloneItem(s));
                }

                App.ViewModel.saveNowplaying();

                App.ViewModel.AppSettings.NowplayingIndexSetting = 0;

                //MyAudioPlaybackAgent.AudioPlayer.startPlaying(0);
                MyAudioPlaybackAgent.AudioPlayer.startPlaying(trackIndex);
            }
            else
            {
                foreach (DataItemViewModel s in newSongs)
                {
                    App.ViewModel.Nowplaying.Add(App.ViewModel.Functions.CloneItem(s));
                }

                App.ViewModel.saveNowplaying();

                if (BackgroundAudioPlayer.Instance.PlayerState != PlayState.Playing)
                {
                    MyAudioPlaybackAgent.AudioPlayer.startPlaying(trackIndex + currentPlayingCount);
                }
            }

            NavigationService.Navigate(new Uri("/Nowplaying.xaml?Remove=" + viewsToRemove, UriKind.Relative));
            
        }

        private void SelectSearchAction(int inSelected, bool inAll, bool inShuffle, bool inAdd)
        {

            int currentPlayingCount = App.ViewModel.Nowplaying.Count;
            int trackIndex = 0;

            List<DataItemViewModel> newSongs = new List<DataItemViewModel>();

            if (!inAll)
            {
                newSongs.Add(this._searchItems[inSelected]);
            }
            else if (!inShuffle)
            {
                foreach (DataItemViewModel s in _searchItems)
                {
                    newSongs.Add(App.ViewModel.Functions.CloneItem(s));
                }

                trackIndex = inSelected;
            }
            else
            {
                Random r = new Random();
                List<DataItemViewModel> newSongs2 = new List<DataItemViewModel>();
                int i = 0;

                foreach (DataItemViewModel s in _searchItems)
                {
                    newSongs2.Add(App.ViewModel.Functions.CloneItem(s));
                }

                newSongs.Add(newSongs2[inSelected]);
                newSongs2.RemoveAt(inSelected);

                while (newSongs2.Count > 0)
                {
                    i = r.Next(0, (newSongs2.Count - 1));

                    newSongs.Add(newSongs2[i]);
                    newSongs2.RemoveAt(i);

                }

                trackIndex = 0;

            }

            if (!inAdd)
            {
                MyAudioPlaybackAgent.AudioPlayer.resetList();
                App.ViewModel.Nowplaying.Clear();

                foreach (DataItemViewModel s in newSongs)
                {
                    App.ViewModel.Nowplaying.Add(App.ViewModel.Functions.CloneItem(s));
                }

                App.ViewModel.saveNowplaying();

                //MyAudioPlaybackAgent.AudioPlayer.startPlaying(0);
                MyAudioPlaybackAgent.AudioPlayer.startPlaying(trackIndex);
            }
            else
            {
                foreach (DataItemViewModel s in newSongs)
                {
                    App.ViewModel.Nowplaying.Add(App.ViewModel.Functions.CloneItem(s));
                }

                App.ViewModel.saveNowplaying();

                if (BackgroundAudioPlayer.Instance.PlayerState != PlayState.Playing)
                {
                    MyAudioPlaybackAgent.AudioPlayer.startPlaying(trackIndex + currentPlayingCount);
                }
            }

            NavigationService.Navigate(new Uri("/Nowplaying.xaml?Remove=" + viewsToRemove, UriKind.Relative));

        }



        private void SongsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongsLL.SelectedItem == null)
                return;

            var myItem = (DataItemViewModel)SongsLL.SelectedItem;
            int t = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].SongUrl == myItem.SongUrl)
                    t = i;
            }

            SelectAction(t, App.ViewModel.AppSettings.DefaultPlayAllSetting, App.ViewModel.AppSettings.DefaultPlayShuffleSetting, App.ViewModel.AppSettings.DefaultPlayAddSetting);

            SongsLL.SelectedItem = null;
        }

        private void SongsSearchLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongsSearchLL.SelectedItem == null)
                return;

            var myItem = (DataItemViewModel)SongsSearchLL.SelectedItem;
            int t = 0;

            for (int i = 0; i < _searchItems.Count; i++)
            {
                if (_searchItems[i].SongUrl == myItem.SongUrl)
                    t = i;
            }

            SelectSearchAction(t, App.ViewModel.AppSettings.DefaultPlayAllSetting, App.ViewModel.AppSettings.DefaultPlayShuffleSetting, App.ViewModel.AppSettings.DefaultPlayAddSetting);

            SongsSearchLL.SelectedItem = null;

        }

        private void searchBoxButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            performanceProgressBarCustomized.IsIndeterminate = true;

            this._searchItems.Clear();

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("songs", "&filter=" + searchBox.Text)));
            webRequest.BeginGetResponse(new AsyncCallback(DataSearchCallback), webRequest);
        }


        private void playSingleSong_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectAction(t, false, false, false);

        }
        private void playAllStraight_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectAction(t, true, false, false);

        }
        private void playAllShuffled_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectAction(t, true, true, false);
        }
        private void queueSingleSong_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectAction(t, false, false, true);

        }
        private void queueAllStraight_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectAction(t, true, false, true);
        }
        private void queueAllShuffled_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectAction(t, true, true, true);
        }

        private void playSingleSongSearch_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _searchItems.Count; i++)
            {
                if (_searchItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectSearchAction(t, false, false, false);

        }
        private void playAllStraightSearch_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _searchItems.Count; i++)
            {
                if (_searchItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectSearchAction(t, true, false, false);

        }
        private void playAllShuffledSearch_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _searchItems.Count; i++)
            {
                if (_searchItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectSearchAction(t, true, true, false);
        }
        private void queueSingleSongSearch_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _searchItems.Count; i++)
            {
                if (_searchItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectSearchAction(t, false, false, true);

        }
        private void queueAllStraightSearch_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _searchItems.Count; i++)
            {
                if (_searchItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectSearchAction(t, true, false, true);
        }
        private void queueAllShuffledSearch_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _searchItems.Count; i++)
            {
                if (_searchItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectSearchAction(t, true, true, true);
        }

        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            SongsLL.IsEnabled = false;
        }
        private void ContextMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            SongsLL.IsEnabled = true;
        }

        private void SearchContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            SongsSearchLL.IsEnabled = false;
        }
        private void SearchContextMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            SongsSearchLL.IsEnabled = true;
        }
    }
}