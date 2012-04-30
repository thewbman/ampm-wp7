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
    public partial class ArtistDetails : PhoneApplicationPage
    {
        public ArtistDetails()
        {
            InitializeComponent();

            _albums = new ObservableCollection<DataItemViewModel>();
            _songs = new ObservableCollection<DataItemViewModel>();

            //songList.ItemsSource = _songs;

            FinishedData = false;
        }

        public ObservableCollection<DataItemViewModel> _albums;
        public ObservableCollection<DataItemViewModel> _songs;

        private int viewsToRemove = 2;

        private bool FinishedData;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //panoramaTitle.Title = App.ViewModel.SelectedArtist.ArtistName;
            pivotTitle.Title = App.ViewModel.SelectedArtist.ArtistName;

            if (!FinishedData)
            {
                performanceProgressBarCustomized.IsIndeterminate = true;

                this.Perform(() => GetAlbums(), 50);
            }
            else
            {
                performanceProgressBarCustomized.IsIndeterminate = false;
            }

        }

        private void GetAlbums()
        {
            performanceProgressBarCustomized.IsIndeterminate = true;

            this._albums.Clear();

            albumList.ItemsSource = null;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("artist_albums", "&filter="+App.ViewModel.SelectedArtist.ArtistId)));
            webRequest.BeginGetResponse(new AsyncCallback(DataAlbumsCallback), webRequest);
        }
        private void DataAlbumsCallback(IAsyncResult asynchronousResult)
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
                    newItem.Auth = App.ViewModel.AppSettings.AuthSetting;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _albums.Add(newItem);

                    });

                }

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.GetSongs();
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

        private void GetSongs()
        {
            performanceProgressBarCustomized.IsIndeterminate = true;

            this._songs.Clear();

            songList.ItemsSource = null;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("artist_songs", "&filter=" + App.ViewModel.SelectedArtist.ArtistId)));
            webRequest.BeginGetResponse(new AsyncCallback(DataSongsCallback), webRequest);
        }
        private void DataSongsCallback(IAsyncResult asynchronousResult)
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
                    newItem.Auth = App.ViewModel.AppSettings.AuthSetting;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _songs.Add(newItem);
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
            albumList.ItemsSource = _albums;
            songList.ItemsSource = _songs;

            FinishedData = true;

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
                newSongs.Add(this._songs[inSelected]);
            }
            else if (!inShuffle)
            {
                foreach (DataItemViewModel s in _songs)
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

                foreach (DataItemViewModel s in _songs)
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


        private void albumList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (albumList.SelectedItem == null)
                return;

            var s = (DataItemViewModel)albumList.SelectedItem;

            //NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + s.AlbumId, UriKind.Relative));
            NavigationService.Navigate(new Uri("/SongsList.xaml?Album=" + s.AlbumId + "&AlbumName=" + s.AlbumName, UriKind.Relative));

            albumList.SelectedItem = null;
        }

        private void songList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (songList.SelectedItem == null)
                return;

            SelectAction(songList.SelectedIndex, App.ViewModel.AppSettings.DefaultPlayAllSetting, App.ViewModel.AppSettings.DefaultPlayShuffleSetting, App.ViewModel.AppSettings.DefaultPlayAddSetting);

            songList.SelectedItem = null;
        }


        private void playSingleSong_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int t = 0;

            for (int i = 0; i < _songs.Count; i++)
            {
                if (_songs[i].SongUrl == selectedItem.SongUrl)
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

            for (int i = 0; i < _songs.Count; i++)
            {
                if (_songs[i].SongUrl == selectedItem.SongUrl)
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

            for (int i = 0; i < _songs.Count; i++)
            {
                if (_songs[i].SongUrl == selectedItem.SongUrl)
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

            for (int i = 0; i < _songs.Count; i++)
            {
                if (_songs[i].SongUrl == selectedItem.SongUrl)
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

            for (int i = 0; i < _songs.Count; i++)
            {
                if (_songs[i].SongUrl == selectedItem.SongUrl)
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

            for (int i = 0; i < _songs.Count; i++)
            {
                if (_songs[i].SongUrl == selectedItem.SongUrl)
                    t = i;
            }

            SelectAction(t, true, true, true);
        }


        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            songList.IsEnabled = false;
        }
        private void ContextMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            songList.IsEnabled = true;
        }


    }
}