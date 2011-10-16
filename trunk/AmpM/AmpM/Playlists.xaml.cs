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
    public partial class Playlists : PhoneApplicationPage
    {
        public Playlists()
        {
            InitializeComponent();

            //DataContext = App.ViewModel;

            this._Items = new ObservableCollection<DataItemViewModel>();

            this._Items.Add(new DataItemViewModel() { PlaylistId = -1, PlaylistName = "test data name", PlaylistItems = 44 });

            itemsList.ItemsSource = this._Items;
        }

        private ObservableCollection<DataItemViewModel> _Items;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ApplicationTitle.Text = "AmpM - " + App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Name;

            if (App.ViewModel.Playlists.Count == 0)
            {
                //this.GetPlaylists();
            }
            else
            {
                //this.GetPlaylists();

                //this._Items.Clear();

                //this._Items = App.ViewModel.Playlists;

                //this.SortAndDisplay();
            }
        }

        private void GetPlaylists()
        {

            this._Items.Clear();

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("playlists", "")));
            webRequest.BeginGetResponse(new AsyncCallback(DataCallback), webRequest);
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

                foreach (XElement singlePlaylistElement in xdoc.Element("root").Descendants("playlist"))
                {
                    DataItemViewModel newItem = new DataItemViewModel();

                    newItem.PlaylistId = int.Parse(singlePlaylistElement.Attribute("id").Value);

                    newItem.PlaylistName = singlePlaylistElement.Element("name").FirstNode.ToString();
                    newItem.PlaylistItems = int.Parse(singlePlaylistElement.Element("items").FirstNode.ToString());
                    
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        this._Items.Add(newItem);

                        MessageBox.Show("adding newItem to list: "+newItem.PlaylistName+" _ items: "+newItem.PlaylistItems+" _ id: "+newItem.PlaylistId);

                        itemsList.ItemsSource = this._Items;
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
            //App.ViewModel.Playlists.Clear();

            foreach (DataItemViewModel s in this._Items)
            {
                //App.ViewModel.Playlists.Add(s);
            }

            itemsList.ItemsSource = this._Items;

        }




        private void itemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}