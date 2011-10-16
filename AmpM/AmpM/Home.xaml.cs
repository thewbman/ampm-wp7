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

            this.Items = new ObservableCollection<NameContentViewModel>();

            Items.Add(new NameContentViewModel() { Name = "now playing" });
            Items.Add(new NameContentViewModel() { Name = "search" });
            Items.Add(new NameContentViewModel() { Name = "songs" });
            Items.Add(new NameContentViewModel() { Name = "albums" });
            Items.Add(new NameContentViewModel() { Name = "artists" });
            Items.Add(new NameContentViewModel() { Name = "genres" });
            Items.Add(new NameContentViewModel() { Name = "playlists" });
            Items.Add(new NameContentViewModel() { Name = "videos" });

            itemsList.ItemsSource = Items;

        }

        private ObservableCollection<NameContentViewModel> Items;
        private HostViewModel SelectedHost;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SelectedHost = App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting];

            PageTitle.Text = SelectedHost.Name;
            PageSubtitle.Text = SelectedHost.Address;

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
                        Items[2].Content = xdoc.Element("root").Element("songs").Value;
                        Items[3].Content = xdoc.Element("root").Element("albums").Value;
                        Items[4].Content = xdoc.Element("root").Element("artists").Value;
                        //
                        Items[6].Content = xdoc.Element("root").Element("playlists").Value;
                        Items[7].Content = xdoc.Element("root").Element("videos").Value;
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
                    MessageBox.Show("now playing");
                    break;
                case "search":
                    MessageBox.Show("search");
                    break;
                case "songs":
                    MessageBox.Show("songs");
                    break;
                case "albums":
                    MessageBox.Show("albums");
                    break;
                case "artists":
                    MessageBox.Show("artist");
                    break;
                case "genres":
                    MessageBox.Show("genres");
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
        }
    }
}