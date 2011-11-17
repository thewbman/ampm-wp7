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

namespace AmpM
{
    public partial class Home : PhoneApplicationPage
    {
        public Home()
        {
            InitializeComponent();

            this._items = new ObservableCollection<NameContentViewModel>();

            _items.Add(new NameContentViewModel() { Name = "now playing" });
            _items.Add(new NameContentViewModel() { Name = "search" });
            _items.Add(new NameContentViewModel() { Name = "songs" });
            _items.Add(new NameContentViewModel() { Name = "albums" });
            _items.Add(new NameContentViewModel() { Name = "artists" });
            _items.Add(new NameContentViewModel() { Name = "genres" });
            _items.Add(new NameContentViewModel() { Name = "playlists" });
            //_items.Add(new NameContentViewModel() { Name = "videos" });

            itemsList.ItemsSource = _items;

        }

        private ObservableCollection<NameContentViewModel> _items;
        private HostViewModel SelectedHost;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string inValue = "";
            if (NavigationContext.QueryString.TryGetValue("Remove", out inValue))
            {
                int toRemove = int.Parse(inValue);

                for (int i = 0; i < toRemove; i++)
                {
                    NavigationService.RemoveBackEntry();
                }
            }
            
            SelectedHost = App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting];

            PageTitle.Text = SelectedHost.Name;
            PageSubtitle.Text = SelectedHost.Address;

            App.ViewModel.AppSettings.HostAddressSetting = SelectedHost.Address;

            _items[0].Content = App.ViewModel.Nowplaying.Count.ToString();



            //if ((App.ViewModel.AppSettings.SessionExpireSetting == "1900-01-01T00:00:00") || (App.ViewModel.Connected == false))
            if ((App.ViewModel.AppSettings.SessionExpireSetting == "1900-01-01T00:00:00"))
            {
                //MessageBox.Show("AmpacheConnectUrl: " + App.ViewModel.Functions.GetAmpacheConnectUrl());

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheConnectUrl()));
                webRequest.BeginGetResponse(new AsyncCallback(ConnectCallback), webRequest);
            }
            else
            {
                _items[2].Content = App.ViewModel.AppSettings.SongsCountSetting.ToString();
                _items[3].Content = App.ViewModel.AppSettings.AlbumsCountSetting.ToString();
                _items[4].Content = App.ViewModel.AppSettings.ArtistsCountSetting.ToString();
                //
                _items[6].Content = App.ViewModel.AppSettings.PlaylistsCountSetting.ToString();
                //_items[7].Content = App.ViewModel.AppSettings.VideosCountSetting.ToString();
            }
        }

        private void ConnectCallback(IAsyncResult asynchronousResult)
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
                    MessageBox.Show("Failed to get handshake response: " + ex.ToString(), "Error", MessageBoxButton.OK);
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
                    //MessageBox.Show("Got handshake response: " + resultString, "Error", MessageBoxButton.OK);
                    App.ViewModel.Connected = true;
                });

                if (xdoc.Element("root").Element("auth") == null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show("Did not get successfull response: " + resultString, "Error", MessageBoxButton.OK);
                    });
                }
                else
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        //App.ViewModel.AppSettings.AuthSetting = xdoc.Element("root").Element("auth").Value;
                        App.ViewModel.AppSettings.AuthSetting = xdoc.Element("root").Element("auth").Value;
                        App.ViewModel.AppSettings.SessionExpireSetting = DateTime.Now.ToString("s");
                        
                        //now playing
                        //search
                        //songs
                        //albums
                        //artists
                        //genres
                        //playlists
                        //videos

                        //
                        //
                        _items[2].Content = xdoc.Element("root").Element("songs").Value;
                        _items[3].Content = xdoc.Element("root").Element("albums").Value;
                        _items[4].Content = xdoc.Element("root").Element("artists").Value;
                        //
                        _items[6].Content = xdoc.Element("root").Element("playlists").Value;
                        //_items[7].Content = xdoc.Element("root").Element("videos").Value;

                        /*
                        App.ViewModel.AllSongs = int.Parse(xdoc.Element("root").Element("songs").Value);
                        App.ViewModel.AllAlbums = int.Parse(xdoc.Element("root").Element("albums").Value);
                        App.ViewModel.AllArtists = int.Parse(xdoc.Element("root").Element("artists").Value);
                        App.ViewModel.AllPlaylists = int.Parse(xdoc.Element("root").Element("playlists").Value);
                        App.ViewModel.AllVideos = int.Parse(xdoc.Element("root").Element("videos").Value);
                        */

                        App.ViewModel.AppSettings.SongsCountSetting = int.Parse(xdoc.Element("root").Element("songs").Value);
                        App.ViewModel.AppSettings.AlbumsCountSetting = int.Parse(xdoc.Element("root").Element("albums").Value);
                        App.ViewModel.AppSettings.ArtistsCountSetting = int.Parse(xdoc.Element("root").Element("artists").Value);
                        App.ViewModel.AppSettings.PlaylistsCountSetting = int.Parse(xdoc.Element("root").Element("playlists").Value);
                        App.ViewModel.AppSettings.VideosCountSetting = int.Parse(xdoc.Element("root").Element("videos").Value);

                    });

                    this.Ping();
                }

            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Error parsing handshake request: " + ex.ToString());
                });
            }

        }

        private void Ping()
        {
             HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("ping", "")));
             webRequest.BeginGetResponse(new AsyncCallback(PingCallback), webRequest);

        }
        private void PingCallback(IAsyncResult asynchronousResult)
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

                DateTime d = DateTime.Parse(xdoc.Element("root").Element("session_expire").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim()) ;

                App.ViewModel.AppSettings.SessionExpireSetting = d.ToString("s");
            
            }
            catch (Exception ex)
            {
                MessageBox.Show("ping xml error: " + ex.ToString());

                App.ViewModel.AppSettings.SessionExpireSetting = "1900-01-01T00:00:00";
            }
             

        }

        private void itemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemsList.SelectedItem == null)
                return;

            var s = (NameContentViewModel)itemsList.SelectedItem;

            switch (s.Name)
            {
                case "now playing":
                    //MessageBox.Show("now playing");
                    NavigationService.Navigate(new Uri("/Nowplaying.xaml", UriKind.Relative));
                    break;
                case "search":
                    //MessageBox.Show("search");
                    NavigationService.Navigate(new Uri("/Search.xaml", UriKind.Relative));
                    break;
                case "songs":
                    //MessageBox.Show("songs");
                    NavigationService.Navigate(new Uri("/Songs.xaml", UriKind.Relative));
                    break;
                case "albums":
                    //MessageBox.Show("albums");
                    NavigationService.Navigate(new Uri("/Albums.xaml", UriKind.Relative));
                    break;
                case "artists":
                    //MessageBox.Show("artist");
                    NavigationService.Navigate(new Uri("/Artists.xaml", UriKind.Relative));
                    break;
                case "genres":
                    //MessageBox.Show("genres");
                    NavigationService.Navigate(new Uri("/Tags.xaml", UriKind.Relative));
                    break;
                case "playlists":
                    //MessageBox.Show("playlists");
                    NavigationService.Navigate(new Uri("/Playlists.xaml", UriKind.Relative));
                    break;
                case "videos":
                    MessageBox.Show("videos");
                    break;
                default:
                    MessageBox.Show("unknown choice: " + s.Name);
                    break;
            }

            itemsList.SelectedItem = null;
        }

        private void logoffButton_Click(object sender, EventArgs e)
        {
            App.ViewModel.AppSettings.SessionExpireSetting = "1900-01-01T00:00:00";
            App.ViewModel.AppSettings.StreamSessionExpireSetting = "1900-01-01T00:00:00";

            NavigationService.Navigate(new Uri("/MainPage.xaml?Remove=1", UriKind.Relative));
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Preferences.xaml", UriKind.Relative));
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }
    }
}