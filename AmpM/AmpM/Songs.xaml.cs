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

            _items.Add(new DataItemViewModel() { PlaylistId = -1, PlaylistName = "playlist name", PlaylistItems = 44, AlbumId = -1, AlbumName = "album name", AlbumTracks = 34, ArtistAlbums = 2, ArtistId = 32, ArtistName = "artist name", ArtistTracks = 23, ArtUrl = "http://www.google.com/", SongId = 343, SongName = "song name", Type = "playlist" });

            songList.ItemsSource = _items;
        }

        public ObservableCollection<DataItemViewModel> _items;

        private int viewsToRemove = 1;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ApplicationTitle.Text = "AmpM - " + App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Name;

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

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _items.Add(newItem);

                        //MessageBox.Show("adding newItem to list: " + newItem.PlaylistName + " _ items: " + newItem.PlaylistItems + " _ id: " + newItem.PlaylistId);

                        songList.ItemsSource = _items;
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

            songList.ItemsSource = _items;

            performanceProgressBarCustomized.IsIndeterminate = false;
        }




        private void songList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (songList.SelectedItem == null)
                return;

            int currentPLayingCount = App.ViewModel.Nowplaying.Count;

            //List<AudioTrack> newSongs = new List<AudioTrack>();
            
            //AudioTrack t = new AudioTrack();
            //DataItemViewModel t = new DataItemViewModel();

            //App.ViewModel.Nowplaying.Clear();

            foreach(DataItemViewModel s in _items)
            {
                /*
                t = new AudioTrack();

                t.Album = s.AlbumName;
                t.AlbumArt = new Uri(s.ArtUrl);
                t.Artist = s.ArtistName;
                t.Source = new Uri(s.SongUrl);
                t.Tag = s.SongId.ToString();
                t.Title = s.SongName;

                newSongs.Add(t);
                */
                App.ViewModel.Nowplaying.Add(s);

            }
             
            //newSongs = App.ViewModel.Functions.SongsToTracks(this._items);

            //App.ViewModel.Nowplaying = newSongs;

            if (App.ViewModel.Nowplaying.Count > 0)
            {
                //App.ViewModel.Nowplaying.Clear();
                //App.ViewModel.addSongs(newSongs);
                App.ViewModel.saveNowplaying();

                //var newlist = App.ViewModel.getNowplaying();

                //songList.ItemsSource = newlist;

                //MyAudioPlaybackAgent.AudioPlayer.setNowplaying(newSongs);
                //MyAudioPlaybackAgent.AudioPlayer.resetList();
                //MyAudioPlaybackAgent.AudioPlayer.addSongs(newSongs);
                //MyAudioPlaybackAgent.AudioPlayer.startPlaying(0);
                App.ViewModel.AppSettings.NowplayingIndexSetting = songList.SelectedIndex + currentPLayingCount;
                MyAudioPlaybackAgent.AudioPlayer.startPlaying(songList.SelectedIndex + currentPLayingCount);
                //MyAudioPlaybackAgent.AudioPlayer.startPlayingTrack(newSongs[songList.SelectedIndex]);


                NavigationService.Navigate(new Uri("/Nowplaying.xaml?Remove="+viewsToRemove, UriKind.Relative));
                //NavigationService.RemoveBackEntry();
                //NavigationService.RemoveBackEntry();
            }

            //MyAudioPlaybackAgent.AudioPlayer._playList = App.ViewModel.Nowplaying;

            songList.SelectedItem = null;

        }
    }
}