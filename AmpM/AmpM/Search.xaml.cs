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
    public partial class Search : PhoneApplicationPage
    {
        public Search()
        {
            InitializeComponent();
        
            _allItems = new ObservableCollection<DataItemViewModel>();
            _albumItems = new ObservableCollection<DataItemViewModel>();
            _artistItems = new ObservableCollection<DataItemViewModel>();
            _tagItems = new ObservableCollection<DataItemViewModel>();
            _playlistItems = new ObservableCollection<DataItemViewModel>();
            _songItems = new ObservableCollection<DataItemViewModel>();

        }

        public ObservableCollection<DataItemViewModel> _allItems;
        public ObservableCollection<DataItemViewModel> _albumItems;
        public ObservableCollection<DataItemViewModel> _artistItems;
        public ObservableCollection<DataItemViewModel> _tagItems;
        public ObservableCollection<DataItemViewModel> _playlistItems;
        public ObservableCollection<DataItemViewModel> _songItems;

        int viewsToRemove = 1;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            AllLL.ItemsSource = null;
            AlbumsLL.ItemsSource = null;
            ArtistsLL.ItemsSource = null;
            TagsLL.ItemsSource = null;
            PlaylistsLL.ItemsSource = null;
            SongsLL.ItemsSource = null;

            _allItems = new ObservableCollection<DataItemViewModel>();
            _albumItems = new ObservableCollection<DataItemViewModel>();
            _artistItems = new ObservableCollection<DataItemViewModel>();
            _tagItems = new ObservableCollection<DataItemViewModel>();
            _playlistItems = new ObservableCollection<DataItemViewModel>();
            _songItems = new ObservableCollection<DataItemViewModel>();

            searchBox.Text = "";
        }






        private void searchBoxButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (searchBox.Text == "")
            {
                MessageBox.Show("You must enter somme search text");
                return;
            }


            performanceProgressBarCustomized.IsIndeterminate = true;


            switch (searchPivot.SelectedIndex)
            {
                case 0:
                    //all
                    AllLL.ItemsSource = null;
                    _allItems = new ObservableCollection<DataItemViewModel>();
                    HttpWebRequest webRequest0 = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("search_songs", "&filter=" + searchBox.Text)));
                    webRequest0.BeginGetResponse(new AsyncCallback(AllCallback), webRequest0);
                    break;
                case 1:
                    //albums
                    AlbumsLL.ItemsSource = null;
                    _albumItems = new ObservableCollection<DataItemViewModel>();
                    HttpWebRequest webRequest1 = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("albums", "&filter=" + searchBox.Text)));
                    webRequest1.BeginGetResponse(new AsyncCallback(AlbumsCallback), webRequest1);
                    break;
                case 2:
                    //artists
                    ArtistsLL.ItemsSource = null;
                    _artistItems = new ObservableCollection<DataItemViewModel>();
                    HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("artists", "&filter=" + searchBox.Text)));
                    webRequest2.BeginGetResponse(new AsyncCallback(ArtistsCallback), webRequest2);
                    break;
                case 3:
                    //tags
                    TagsLL.ItemsSource = null;
                    _tagItems = new ObservableCollection<DataItemViewModel>();
                    HttpWebRequest webRequest3 = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("tags", "&filter=" + searchBox.Text)));
                    webRequest3.BeginGetResponse(new AsyncCallback(TagsCallback), webRequest3);
                    break;
                case 4:
                    //playlists
                    PlaylistsLL.ItemsSource = null;
                    _playlistItems = new ObservableCollection<DataItemViewModel>();
                    HttpWebRequest webRequest4 = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("playlists", "&filter=" + searchBox.Text)));
                    webRequest4.BeginGetResponse(new AsyncCallback(PlaylistsCallback), webRequest4);
                    break;
                case 5:
                    //songs
                    SongsLL.ItemsSource = null;
                    _songItems = new ObservableCollection<DataItemViewModel>();
                    HttpWebRequest webRequest5 = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("songs", "&filter=" + searchBox.Text)));
                    webRequest5.BeginGetResponse(new AsyncCallback(SongsCallback), webRequest5);
                    break;
            }

        }



        private void AllCallback(IAsyncResult asynchronousResult)
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
                    newItem.Auth = App.ViewModel.Auth;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _allItems.Add(newItem);

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
        private void AlbumsCallback(IAsyncResult asynchronousResult)
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

                foreach (XElement singleDataElement in xdoc.Element("root").Descendants("album"))
                {
                    DataItemViewModel newItem = new DataItemViewModel();

                    newItem.Type = "album";

                    newItem.AlbumId = int.Parse(singleDataElement.Attribute("id").Value);

                    newItem.AlbumName = singleDataElement.Element("name").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();
                    newItem.ArtistName = singleDataElement.Element("artist").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();
                    newItem.ArtistId = int.Parse(singleDataElement.Element("artist").Attribute("id").Value);
                    newItem.AlbumTracks = int.Parse(singleDataElement.Element("tracks").FirstNode.ToString());
                    newItem.Year = singleDataElement.Element("year").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.ArtUrl = singleDataElement.Element("art").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                    newItem.ItemKey = "album" + newItem.AlbumId;
                    newItem.ItemId = newItem.AlbumId;
                    newItem.ItemChar = App.ViewModel.Functions.FirstChar(newItem.AlbumName);
                    newItem.Auth = App.ViewModel.Auth;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _albumItems.Add(newItem);

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
        private void ArtistsCallback(IAsyncResult asynchronousResult)
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

                foreach (XElement singleDataElement in xdoc.Element("root").Descendants("artist"))
                {
                    DataItemViewModel newItem = new DataItemViewModel();

                    newItem.Type = "artist";

                    newItem.ArtistId = int.Parse(singleDataElement.Attribute("id").Value);

                    newItem.ArtistName = singleDataElement.Element("name").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();
                    newItem.ArtistAlbums = int.Parse(singleDataElement.Element("albums").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim());
                    newItem.ArtistTracks = int.Parse(singleDataElement.Element("songs").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim());

                    newItem.ItemKey = "artist" + newItem.ArtistId;
                    newItem.ItemId = newItem.ArtistId;
                    newItem.ItemChar = App.ViewModel.Functions.FirstChar(newItem.ArtistName);
                    newItem.Auth = App.ViewModel.Auth;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _artistItems.Add(newItem);
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
        private void TagsCallback(IAsyncResult asynchronousResult)
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

                foreach (XElement singleDataElement in xdoc.Element("root").Descendants("tag"))
                {
                    DataItemViewModel newItem = new DataItemViewModel();

                    newItem.Type = "tag";

                    newItem.TagId = int.Parse(singleDataElement.Attribute("id").Value);

                    newItem.TagName = singleDataElement.Element("name").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();
                    newItem.TagAlbums = int.Parse(singleDataElement.Element("albums").FirstNode.ToString());
                    newItem.TagArtists = int.Parse(singleDataElement.Element("artists").FirstNode.ToString());
                    newItem.TagSongs = int.Parse(singleDataElement.Element("songs").FirstNode.ToString());

                    newItem.ItemKey = "tag" + newItem.PlaylistId;
                    newItem.ItemId = newItem.TagId;
                    newItem.ItemChar = App.ViewModel.Functions.FirstChar(newItem.TagName);
                    newItem.Auth = App.ViewModel.Auth;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _tagItems.Add(newItem);
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
        private void PlaylistsCallback(IAsyncResult asynchronousResult)
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

                foreach (XElement singleDataElement in xdoc.Element("root").Descendants("playlist"))
                {
                    DataItemViewModel newItem = new DataItemViewModel();

                    newItem.Type = "playlist";

                    newItem.PlaylistId = int.Parse(singleDataElement.Attribute("id").Value);

                    newItem.PlaylistName = singleDataElement.Element("name").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();
                    newItem.PlaylistItems = int.Parse(singleDataElement.Element("items").FirstNode.ToString());

                    newItem.ItemKey = "playlist" + newItem.PlaylistId;
                    newItem.ItemId = newItem.PlaylistId;
                    newItem.ItemChar = App.ViewModel.Functions.FirstChar(newItem.PlaylistName);
                    newItem.Auth = App.ViewModel.Auth;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _playlistItems.Add(newItem);
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
        private void SongsCallback(IAsyncResult asynchronousResult)
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
                    newItem.Auth = App.ViewModel.Auth;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _songItems.Add(newItem);

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

            var all = from t in _allItems
                         group t by t.ItemChar into c
                         //orderby c.Key
                         select new Group<DataItemViewModel>(c.Key, c);
            var albums = from t in _albumItems
                          group t by t.ItemChar into c
                          //orderby c.Key
                          select new Group<DataItemViewModel>(c.Key, c);
            var artists = from t in _artistItems
                          group t by t.ItemChar into c
                          //orderby c.Key
                          select new Group<DataItemViewModel>(c.Key, c);
            var tags = from t in _tagItems
                          group t by t.ItemChar into c
                          //orderby c.Key
                          select new Group<DataItemViewModel>(c.Key, c);
            var playlists = from t in _playlistItems
                          group t by t.ItemChar into c
                          //orderby c.Key
                          select new Group<DataItemViewModel>(c.Key, c);
            var songs = from t in _songItems
                          group t by t.ItemChar into c
                          //orderby c.Key
                          select new Group<DataItemViewModel>(c.Key, c);

            AllLL.ItemsSource = all;
            AlbumsLL.ItemsSource = albums;
            ArtistsLL.ItemsSource = artists;
            TagsLL.ItemsSource = tags;
            PlaylistsLL.ItemsSource = playlists;
            SongsLL.ItemsSource = songs;

            //albumsPivot.Title = "albums (" + this._items.Count + ")";

            performanceProgressBarCustomized.IsIndeterminate = false;

            this.Perform(() => this.stopProgressBar(), 500);

        }
        private void stopProgressBar()
        {
            performanceProgressBarCustomized.IsIndeterminate = false;
        }



        private void AllSelectAction(int inSelected, bool inAll, bool inShuffle, bool inAdd)
        {

            int currentPlayingCount = App.ViewModel.Nowplaying.Count;
            int trackIndex = 0;

            List<DataItemViewModel> newSongs = new List<DataItemViewModel>();

            if (!inAll)
            {
                newSongs.Add(this._allItems[inSelected]);
            }
            else if (!inShuffle)
            {
                foreach (DataItemViewModel s in _allItems)
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

                foreach (DataItemViewModel s in _allItems)
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
        private void SongSelectAction(int inSelected, bool inAll, bool inShuffle, bool inAdd)
        {

            int currentPlayingCount = App.ViewModel.Nowplaying.Count;
            int trackIndex = 0;

            List<DataItemViewModel> newSongs = new List<DataItemViewModel>();

            if (!inAll)
            {
                newSongs.Add(this._songItems[inSelected]);
            }
            else if (!inShuffle)
            {
                foreach (DataItemViewModel s in _songItems)
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

                foreach (DataItemViewModel s in _songItems)
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



        private void allPlaySingleSong_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _allItems.Count; i++)
            {
                if (_allItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            AllSelectAction(t, false, false, false);

        }
        private void allPlayAllStraight_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _allItems.Count; i++)
            {
                if (_allItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            AllSelectAction(t, true, false, false);

        }
        private void allPlayAllShuffled_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _allItems.Count; i++)
            {
                if (_allItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            AllSelectAction(t, true, true, false);
        }
        private void allQueueSingleSong_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _allItems.Count; i++)
            {
                if (_allItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            AllSelectAction(t, false, false, true);

        }
        private void allQueueAllStraight_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _allItems.Count; i++)
            {
                if (_allItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            AllSelectAction(t, true, false, true);
        }
        private void allQueueAllShuffled_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _allItems.Count; i++)
            {
                if (_allItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            AllSelectAction(t, true, true, true);
        }



        private void playSingleSong_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _songItems.Count; i++)
            {
                if (_songItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SongSelectAction(t, false, false, false);

        }
        private void playAllStraight_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _songItems.Count; i++)
            {
                if (_songItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SongSelectAction(t, true, false, false);

        }
        private void playAllShuffled_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _songItems.Count; i++)
            {
                if (_songItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SongSelectAction(t, true, true, false);
        }
        private void queueSingleSong_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _songItems.Count; i++)
            {
                if (_songItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SongSelectAction(t, false, false, true);

        }
        private void queueAllStraight_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _songItems.Count; i++)
            {
                if (_songItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SongSelectAction(t, true, false, true);
        }
        private void queueAllShuffled_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _songItems.Count; i++)
            {
                if (_songItems[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SongSelectAction(t, true, true, true);
        }




        private void AllLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllLL.SelectedItem == null)
                return;

            var myItem = (DataItemViewModel)AllLL.SelectedItem;
            int t = 0;

            for (int i = 0; i < _allItems.Count; i++)
            {
                if (_allItems[i].SongUrl == myItem.SongUrl)
                    t = i;
            }

            AllSelectAction(t, App.ViewModel.AppSettings.DefaultPlayAllSetting, App.ViewModel.AppSettings.DefaultPlayShuffleSetting, App.ViewModel.AppSettings.DefaultPlayAddSetting);

            AllLL.SelectedItem = null;
        }
        private void AlbumsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlbumsLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)AlbumsLL.SelectedItem;

            NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + s.AlbumId, UriKind.Relative));

            AlbumsLL.SelectedItem = null;
        }
        private void ArtistsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ArtistsLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)ArtistsLL.SelectedItem;

            App.ViewModel.SelectedArtist = s;

            NavigationService.Navigate(new Uri("/ArtistDetails.xaml?Artist=" + s.ArtistId, UriKind.Relative));

            ArtistsLL.SelectedItem = null;
        }
        private void TagsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TagsLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)TagsLL.SelectedItem;

            App.ViewModel.SelectedTag = s;

            NavigationService.Navigate(new Uri("/TagDetails.xaml?Tag=" + s.TagId, UriKind.Relative));

            TagsLL.SelectedItem = null;
        }
        private void PlaylistsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (PlaylistsLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)PlaylistsLL.SelectedItem;

            NavigationService.Navigate(new Uri("/Songs.xaml?Playlist=" + s.PlaylistId, UriKind.Relative));

            PlaylistsLL.SelectedItem = null;
        }
        private void SongsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SongsLL.SelectedItem == null)
                return;

            var myItem = (DataItemViewModel)SongsLL.SelectedItem;
            int t = 0;

            for (int i = 0; i < _songItems.Count; i++)
            {
                if (_songItems[i].SongUrl == myItem.SongUrl)
                    t = i;
            }

            SongSelectAction(t, App.ViewModel.AppSettings.DefaultPlayAllSetting, App.ViewModel.AppSettings.DefaultPlayShuffleSetting, App.ViewModel.AppSettings.DefaultPlayAddSetting);

            SongsLL.SelectedItem = null;
        }




        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ContextMenu_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void ContextMenu_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void ContextMenu_Unloaded_1(object sender, RoutedEventArgs e)
        {

        }

    }
}