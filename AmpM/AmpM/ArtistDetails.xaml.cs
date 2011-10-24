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

            songList.ItemsSource = _songs;
        }

        public ObservableCollection<DataItemViewModel> _albums;
        public ObservableCollection<DataItemViewModel> _songs;

        private int viewsToRemove = 2;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            panoramaTitle.Title = App.ViewModel.SelectedArtist.ArtistName;

            performanceProgressBarCustomized.IsIndeterminate = true;

            this.Perform(() => GetAlbums(), 50);

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
                    newItem.Auth = App.ViewModel.Auth;

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
                    newItem.Auth = App.ViewModel.Auth;

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



        private void albumList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (albumList.SelectedItem == null)
                return;

            var s = (DataItemViewModel)albumList.SelectedItem;

            NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + s.AlbumId, UriKind.Relative));

            albumList.SelectedItem = null;
        }

        private void songList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (songList.SelectedItem == null)
                return;

            int currentPlayingCount = App.ViewModel.Nowplaying.Count;

            foreach (DataItemViewModel s in _songs)
            {
                App.ViewModel.Nowplaying.Add(s);
            }

            if (App.ViewModel.Nowplaying.Count > 0)
            {
                App.ViewModel.saveNowplaying();

                App.ViewModel.AppSettings.NowplayingIndexSetting = songList.SelectedIndex + currentPlayingCount;
                MyAudioPlaybackAgent.AudioPlayer.startPlaying(songList.SelectedIndex + currentPlayingCount);


                NavigationService.Navigate(new Uri("/Nowplaying.xaml?Remove=" + viewsToRemove, UriKind.Relative));
            }

            songList.SelectedItem = null;
        }

    }
}