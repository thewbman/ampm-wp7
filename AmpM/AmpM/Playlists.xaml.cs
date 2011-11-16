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
using MyAudioPlaybackAgent;

namespace AmpM
{
    public partial class Playlists : PhoneApplicationPage
    {
        public Playlists()
        {
            InitializeComponent();

            //DataContext = App.ViewModel;

            _items = new ObservableCollection<DataItemViewModel>();
            //_items = new List<DataItemViewModel>();

            //_items.Add(new DataItemViewModel() { PlaylistId = -1, PlaylistName = "playlist name", PlaylistItems = 44, AlbumId = -1, AlbumName = "album name", AlbumTracks = 34, ArtistAlbums = 2, ArtistId = 32, ArtistName = "artist name", ArtistTracks = 23, ArtUrl = "http://www.google.com/", SongId = 343, SongName = "song name", Type = "playlist" });

            playlistList.ItemsSource = _items;
            //playlistsJumpList.ItemsSource = _items;
            //playlistsJumpList.ItemsSource = null;
        }

        public ObservableCollection<DataItemViewModel> _items;
        //public List<DataItemViewModel> _items;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                ApplicationTitle.Text = "AmpM - " + App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Name;

                if (App.ViewModel.Playlists.Count == 0)
                {
                    this.GetPlaylists();

                    //this.SortAndDisplay();
                }
                else
                {
                    //this.GetPlaylists();

                    _items.Clear();

                    //_items = App.ViewModel.Playlists;

                    foreach (DataItemViewModel s in App.ViewModel.Playlists)
                    {
                        _items.Add(s);
                    }

                    playlistList.ItemsSource = _items;
                    //playlistsJumpList.ItemsSource = _items;

                    //this.SortAndDisplay();

                    performanceProgressBarCustomized.IsIndeterminate = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "to error", MessageBoxButton.OK);
            }
        }

        private void GetPlaylists()
        {
            try
            {
                performanceProgressBarCustomized.IsIndeterminate = true;

                this._items.Clear();

                playlistList.ItemsSource = null;

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("playlists", "")));
                webRequest.BeginGetResponse(new AsyncCallback(DataCallback), webRequest);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "to error", MessageBoxButton.OK);
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

                foreach (XElement singleDataElement in xdoc.Element("root").Descendants("playlist"))
                {
                    DataItemViewModel newItem = new DataItemViewModel();

                    newItem.Type = "playlist";

                    newItem.PlaylistId = int.Parse(singleDataElement.Attribute("id").Value);

                    newItem.PlaylistName = singleDataElement.Element("name").FirstNode.ToString().Replace("<![CDATA[","").Replace("]]>","").Trim();
                    newItem.PlaylistItems = int.Parse(singleDataElement.Element("items").FirstNode.ToString());

                    newItem.ItemKey = "playlist" + newItem.PlaylistId;
                    newItem.ItemId = newItem.PlaylistId;
                    newItem.ItemChar = App.ViewModel.Functions.FirstChar(newItem.PlaylistName);
                    newItem.Auth = App.ViewModel.AppSettings.AuthSetting;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _items.Add(newItem);

                        //MessageBox.Show("adding newItem to list: "+newItem.PlaylistName+" _ items: "+newItem.PlaylistItems+" _ id: "+newItem.PlaylistId);

                        //playlistList.ItemsSource = _items;
                        //playlistsJumpList.ItemsSource = _items;
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
            App.ViewModel.Playlists.Clear();

            foreach (DataItemViewModel s in _items)
            {
                App.ViewModel.Playlists.Add(s);
            }

            try
            {
                playlistList.ItemsSource = _items;
                //playlistsJumpList.ItemsSource = _items;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "list error", MessageBoxButton.OK);
            }

            performanceProgressBarCustomized.IsIndeterminate = false;

        }





        private void playlistList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (playlistList.SelectedItem == null)
                return;

            var s = (DataItemViewModel)playlistList.SelectedItem;

            NavigationService.Navigate(new Uri("/Songs.xaml?Playlist="+s.PlaylistId, UriKind.Relative));

            playlistList.SelectedItem = null;
             
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {

            App.ViewModel.Playlists.Clear();

            this.GetPlaylists();
        }
    }
}