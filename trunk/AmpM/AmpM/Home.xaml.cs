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
            SelectedHost = App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting];

            PageTitle.Text = SelectedHost.Name;
            PageSubtitle.Text = SelectedHost.Address;

            _items[0].Content = App.ViewModel.Nowplaying.Count.ToString();

            if (App.ViewModel.Connected == false)
            {
                //MessageBox.Show("AmpacheConnectUrl: " + App.ViewModel.Functions.GetAmpacheConnectUrl());
                
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheConnectUrl()));
                webRequest.BeginGetResponse(new AsyncCallback(ConnectCallback), webRequest);
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
                        App.ViewModel.Auth = xdoc.Element("root").Element("auth").Value;
                        App.ViewModel.AppSettings.AuthSetting = xdoc.Element("root").Element("auth").Value;
                        
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


                        App.ViewModel.AllSongs = int.Parse(xdoc.Element("root").Element("songs").Value);
                        App.ViewModel.AllAlbums = int.Parse(xdoc.Element("root").Element("albums").Value);
                        App.ViewModel.AllArtists = int.Parse(xdoc.Element("root").Element("artists").Value);
                        App.ViewModel.AllPlaylists = int.Parse(xdoc.Element("root").Element("playlists").Value);
                        App.ViewModel.AllVideos = int.Parse(xdoc.Element("root").Element("videos").Value);

                    });
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
                    MessageBox.Show("search");
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
    }
}