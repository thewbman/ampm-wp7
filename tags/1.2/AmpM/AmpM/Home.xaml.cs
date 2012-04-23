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
using System.Windows.Media.Imaging;

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

            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);

        }

        private ObservableCollection<NameContentViewModel> _items;
        private HostViewModel SelectedHost;

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string inValue = "", inValue2 = "";
            if (NavigationContext.QueryString.TryGetValue("Remove", out inValue))
            {
                int toRemove = int.Parse(inValue);

                for (int i = 0; i < toRemove; i++)
                {
                    NavigationService.RemoveBackEntry();
                }

                
            }

            /*
            if (NavigationContext.QueryString.TryGetValue("NewSession", out inValue3))
            {
                this.NewSession = true;
             
                List<MyAudioPlaybackAgent.DataItemViewModel> NewNP = new List<MyAudioPlaybackAgent.DataItemViewModel>();

                foreach (var s in App.ViewModel.Nowplaying)
                {
                    NewNP.Add(App.ViewModel.Functions.CloneItem(s, App.ViewModel.aut);
                }

            }
             */

            SelectedHost = App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting];

            PageTitle.Text = SelectedHost.Name;
            PageSubtitle.Text = SelectedHost.Address;

            App.ViewModel.AppSettings.HostAddressSetting = SelectedHost.Address;

            _items[0].Content = App.ViewModel.Nowplaying.Count.ToString();

            

            //if ((App.ViewModel.AppSettings.SessionExpireSetting == "1900-01-01T00:00:00") || (App.ViewModel.Connected == false))
            if ((App.ViewModel.AppSettings.SessionExpireSetting == "1900-01-01T00:00:00"))
            {
                //MessageBox.Show("AmpacheConnectUrl: " + App.ViewModel.Functions.GetAmpacheConnectUrl());

                try
                {

                    performanceProgressBarCustomized.IsIndeterminate = true;

                    _items[1].Name = "";
                    _items[2].Name = "";
                    _items[3].Name = "";
                    _items[4].Name = "";
                    _items[5].Name = "";
                    _items[6].Name = "";

                    _items[1].Content = "";
                    _items[2].Content = "";
                    _items[3].Content = "";
                    _items[4].Content = "";
                    _items[5].Content = "";
                    _items[6].Content = "";

                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheConnectUrl()));
                    webRequest.BeginGetResponse(new AsyncCallback(ConnectCallback), webRequest);
                }
                catch 
                {
                    MessageBox.Show("Invalid server URL.  Make sure it starts with 'http://' and is a valid address or hostname.");

                    App.ViewModel.AppSettings.SessionExpireSetting = "1900-01-01T00:00:00";
                    App.ViewModel.AppSettings.StreamSessionExpireSetting = "1900-01-01T00:00:00";

                    NavigationService.Navigate(new Uri("/MainPage.xaml?Remove=1", UriKind.Relative));
                }
            }
            else
            {
                performanceProgressBarCustomized.IsIndeterminate = false;

                _items[2].Content = App.ViewModel.AppSettings.SongsCountSetting.ToString();
                _items[3].Content = App.ViewModel.AppSettings.AlbumsCountSetting.ToString();
                _items[4].Content = App.ViewModel.AppSettings.ArtistsCountSetting.ToString();
                //
                _items[6].Content = App.ViewModel.AppSettings.PlaylistsCountSetting.ToString();
                //_items[7].Content = App.ViewModel.AppSettings.VideosCountSetting.ToString();

                _items[1].Name = "search";
                _items[2].Name = "songs";
                _items[3].Name = "albums";
                _items[4].Name = "artists";
                _items[5].Name = "genres";
                _items[6].Name = "playlists";
            }

            if (NavigationContext.QueryString.TryGetValue("Ping", out inValue2))
            {
                if (inValue2 == "true") this.Ping();

                if (App.ViewModel.AppSettings.LastfmSetting)
                    this.LastfmConnect();
            }

            this.updateLastfmImage();

            NavigationContext.QueryString.Clear();
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

                    App.ViewModel.AppSettings.SessionExpireSetting = "1900-01-01T00:00:00";
                    //App.ViewModel.AppSettings.StreamSessionExpireSetting = "1900-01-01T00:00:00";

                    NavigationService.Navigate(new Uri("/MainPage.xaml?Remove=1", UriKind.Relative));
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

                        App.ViewModel.AppSettings.SessionExpireSetting = "1900-01-01T00:00:00";
                        //App.ViewModel.AppSettings.StreamSessionExpireSetting = "1900-01-01T00:00:00";

                        NavigationService.Navigate(new Uri("/MainPage.xaml?Remove=1", UriKind.Relative));
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

                        _items[1].Name = "search";
                        _items[2].Name = "songs";
                        _items[3].Name = "albums";
                        _items[4].Name = "artists";
                        _items[5].Name = "genres";
                        _items[6].Name = "playlists";

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


                        if (App.ViewModel.Nowplaying.Count > 0)
                        {
                            List<MyAudioPlaybackAgent.DataItemViewModel> NewNP = new List<MyAudioPlaybackAgent.DataItemViewModel>();

                            foreach (var s in App.ViewModel.Nowplaying)
                            {
                                NewNP.Add(App.ViewModel.Functions.CloneItem(s, App.ViewModel.AppSettings.AuthSetting));
                            }

                            App.ViewModel.Nowplaying.Clear();

                            foreach (var t in NewNP)
                            {
                                App.ViewModel.Nowplaying.Add(t);
                            }

                            App.ViewModel.saveNowplaying();
                        }

                        if (App.ViewModel.AppSettings.LastfmSetting)
                            this.LastfmConnect();


                        performanceProgressBarCustomized.IsIndeterminate = false;

                        this.Ping();
                    });

                }

            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Error parsing handshake request: " + ex.ToString());

                    App.ViewModel.AppSettings.SessionExpireSetting = "1900-01-01T00:00:00";
                    App.ViewModel.AppSettings.StreamSessionExpireSetting = "1900-01-01T00:00:00";

                    NavigationService.Navigate(new Uri("/MainPage.xaml?Remove=1", UriKind.Relative));
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
                //MessageBox.Show("ping xml error: " + ex.ToString());

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

        private void emptyButton_Click(object sender, EventArgs e)
        {
            MyAudioPlaybackAgent.AudioPlayer.stopAll();
            MyAudioPlaybackAgent.AudioPlayer.resetList();
            BackgroundAudioPlayer.Instance.Track = null;

            App.ViewModel.Nowplaying.Clear();
            App.ViewModel.saveNowplaying();
            App.ViewModel.AppSettings.NowplayingIndexSetting = 0;

            _items[0].Content = App.ViewModel.Nowplaying.Count.ToString();
        }


        private void LastfmConnect()
        {

            //MessageBox.Show("Starting to connect to last.fm");

            //string url = "http://ws.audioscrobbler.com/2.0/";
            //string secret = "cfaaaa9417b5ded38e6ed30434ca8be7";
            //string key = "ee337ff6dfdd301251d3e1c234d2ccba";
            string method = "auth.getMobileSession";

            string authToken = MD5Core.GetHashString(App.ViewModel.AppSettings.LastfmUsernameSetting + MD5Core.GetHashString(App.ViewModel.AppSettings.LastfmPasswordSetting).ToLower()).ToLower();

            string api_sig = MD5Core.GetHashString("api_key" + App.ViewModel.LastfmApikey + "authToken" + authToken + "method" + method + "username" + App.ViewModel.AppSettings.LastfmUsernameSetting + App.ViewModel.LastfmSecret).ToLower();

            string fullUrl = App.ViewModel.LastfmUrl;
            fullUrl += "?api_key=" + App.ViewModel.LastfmApikey;
            fullUrl += "&method=" + method;
            fullUrl += "&username=" + App.ViewModel.AppSettings.LastfmUsernameSetting;
            fullUrl += "&authToken=" + authToken.ToLower();
            fullUrl += "&api_sig=" + api_sig.ToLower();

            //fullUrl += "&password=" + App.ViewModel.AppSettings.LastfmPasswordSetting;
            //fullUrl += "&passhash=" + MD5Core.GetHashString(App.ViewModel.AppSettings.LastfmPasswordSetting);


            //MessageBox.Show("fullUrl: " + fullUrl);

            /*
             
            EmailComposeTask emailcomposer = new EmailComposeTask();
            emailcomposer.To = "ampm.wp7@gmail.com";
            emailcomposer.Subject = "AmpM Help";
            emailcomposer.Body = fullUrl;
            emailcomposer.Show();
             */

            if (App.ViewModel.AppSettings.LastfmKeySetting.Length == 0)
            {
                HttpWebRequest webRequest = WebRequest.CreateHttp(new Uri(fullUrl));
                webRequest.BeginGetResponse(new AsyncCallback(LastfmConnectCallback), webRequest);
            }


        }
        private void LastfmConnectCallback(IAsyncResult asynchronousResult)
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
                    //MessageBox.Show("Failed to get last.fm response: " + ex.ToString(), "Error", MessageBoxButton.OK);
                    MessageBox.Show("Error logging into last.fm - check your username and password");
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

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //MessageBox.Show("Got last.fm login response: " + resultString);

                    XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);

                    App.ViewModel.AppSettings.LastfmKeySetting = xdoc.Element("lfm").Element("session").Element("key").FirstNode.ToString();

                    //MessageBox.Show("Last.fm key: " + App.ViewModel.AppSettings.LastfmKeySetting);

                });
            
            }
            catch (Exception ex)
            {
                
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Error getting last.fm login response: " + ex.ToString());

                });
            }
             

        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            this.updateLastfmImage();
        }

        private void updateLastfmImage()
        {

            if ((BackgroundAudioPlayer.Instance.Track != null) && (App.ViewModel.AppSettings.LastfmImagesSetting))
            {

                string lastfmTitle = BackgroundAudioPlayer.Instance.Track.Title;
                string lastfmArtist = BackgroundAudioPlayer.Instance.Track.Artist;


                string method = "artist.getimages";

                string fullUrl = App.ViewModel.LastfmUrl;
                fullUrl += "?api_key=" + App.ViewModel.LastfmApikey;
                fullUrl += "&method=" + method;
                fullUrl += "&artist=" + lastfmArtist;
                fullUrl += "&limit=1";

                //MessageBox.Show("fullUrl: " + fullUrl);

                Uri scrobbleuri = new Uri(fullUrl);

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(fullUrl));
                webRequest.BeginGetResponse(new AsyncCallback(LastfmImagesCallback), webRequest);
            }
        }
        private void LastfmImagesCallback(IAsyncResult asynchronousResult)
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
                    //MessageBox.Show("Failed to get data response: " + ex.ToString(), "Error", MessageBoxButton.OK);
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

                string imageType = "";
                string sizeName = "";
                string sizeWidth = "";
                string sizeHeight = "";
                string sizeUrl = "";

                foreach (XElement singleImage in xdoc.Element("lfm").Element("images").Descendants("image"))
                {
                    imageType = singleImage.Element("format").ToString();

                    sizeName = singleImage.Element("sizes").Element("size").Attribute("name").ToString();
                    sizeWidth = singleImage.Element("sizes").Element("size").Attribute("width").ToString();
                    sizeHeight = singleImage.Element("sizes").Element("size").Attribute("height").ToString();
                    sizeUrl = singleImage.Element("sizes").Element("size").FirstNode.ToString();
                }

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {

                    var app = Application.Current as App;
                    if (app == null)
                    {
                        MessageBox.Show("null app");
                    }


                    if (sizeUrl == "")
                    {
                        LayoutRoot.Background = new SolidColorBrush(Color.FromArgb(255, 19, 00, 49));
                    }
                    else
                    {
                        System.Windows.Media.Imaging.BitmapImage bmp = new BitmapImage(new Uri(sizeUrl));


                        var imageBrush = new ImageBrush
                        {
                            ImageSource = bmp,
                            Opacity = 0.5d
                        };
                        //app.RootFrame.Background = imageBrush;
                        LayoutRoot.Background = imageBrush;

                        //MessageBox.Show("Found image '" + sizeName + "' that is " + sizeWidth + " by " + sizeHeight + ": " + sizeUrl + ".", "Last.fm", MessageBoxButton.OK);
                    }

                });
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //MessageBox.Show("Error when getting images data: " + ex.ToString(), "Error", MessageBoxButton.OK);
                });
            }
        }
    }
}