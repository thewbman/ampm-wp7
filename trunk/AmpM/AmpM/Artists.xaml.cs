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
    public partial class Artists : PhoneApplicationPage
    {
        public Artists()
        {
            InitializeComponent();

            _items = new ObservableCollection<DataItemViewModel>();
            _searchItems = new ObservableCollection<DataItemViewModel>();

        }

        public ObservableCollection<DataItemViewModel> _items;
        public ObservableCollection<DataItemViewModel> _searchItems;

        private DataItemViewModel _randomArtist;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            performanceProgressBarCustomized.IsIndeterminate = true;

            this.Perform(() => LoadArtists(), 100);

        }
        private void LoadArtists()
        {

            if (!App.ViewModel.IsArtistsLoaded)
            {
                App.ViewModel.LoadArtists();
            }

            if (App.ViewModel.Artists.Count == 0)
            {
                this.GetArtists();
            }
            else
            {

                _items.Clear();

                foreach (DataItemViewModel s in App.ViewModel.Artists)
                {
                    _items.Add(s);
                }

                performanceProgressBarCustomized.IsIndeterminate = false;

                this.SortAndDisplay();
            }
        }

        private void GetArtists()
        {
            performanceProgressBarCustomized.IsIndeterminate = true;

            App.ViewModel.Artists.Clear();
            this._items.Clear();

            ArtistsLL.ItemsSource = null;
            ArtistsSearchLL.ItemsSource = null;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("artists", "")));
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
                    newItem.Auth = App.ViewModel.AppSettings.AuthSetting;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _items.Add(newItem);

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
        private void DataSearchCallback(IAsyncResult asynchronousResult)
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
                    newItem.Auth = App.ViewModel.AppSettings.AuthSetting;

                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        _searchItems.Add(newItem);

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
            if (App.ViewModel.Artists.Count == 0)
            {
                //better logic here - asdf

                App.ViewModel.Artists.Clear();

                foreach (DataItemViewModel s in _items)
                {
                    App.ViewModel.Artists.Add(s);
                }

                App.ViewModel.saveArtists();

            }


            var al = _items.OrderBy(x => x.ArtistName).ToArray();
            var sl = _searchItems.OrderBy(x => x.ArtistName).ToArray();


            var artistsByChar = from t in al
                               group t by t.ItemChar into c
                               //orderby c.Key
                               select new Group<DataItemViewModel>(c.Key, c);

            var artistsBySearch = from t in sl
                                 group t by t.ItemChar into c
                                 //orderby c.Key
                                 select new Group<DataItemViewModel>(c.Key, c);



            ArtistsLL.ItemsSource = artistsByChar;
            ArtistsSearchLL.ItemsSource = artistsBySearch;

            artistsPivot.Title = "ARTISTS (" + this._items.Count + ")";

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









        private void ArtistsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ArtistsLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)ArtistsLL.SelectedItem;

            App.ViewModel.SelectedArtist = s;

            NavigationService.Navigate(new Uri("/ArtistDetails.xaml?Artist=" + s.ArtistId, UriKind.Relative));
            //MessageBox.Show("Artist details for " + s.ArtistId + " " + s.ArtistName);

            ArtistsLL.SelectedItem = null;
        }

        private void ArtistsSearchLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArtistsSearchLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)ArtistsSearchLL.SelectedItem;

            App.ViewModel.SelectedArtist = s;

            NavigationService.Navigate(new Uri("/ArtistDetails.xaml?Artist=" + s.ArtistId, UriKind.Relative));
            //MessageBox.Show("Artist details for " + s.ArtistId + " " + s.ArtistName);

            ArtistsSearchLL.SelectedItem = null;

        }


        private void searchBoxButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            performanceProgressBarCustomized.IsIndeterminate = true;

            this._searchItems.Clear();

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("artists", "&filter=" + searchBox.Text)));
            webRequest.BeginGetResponse(new AsyncCallback(DataSearchCallback), webRequest);
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.GetArtists();
        }

        private void artistsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateRandom();

        }

        private void nextRandomButton_Click(object sender, RoutedEventArgs e)
        {
            updateRandom();

        }



        private void updateRandom()
        {
            if (_items.Count > 1)
            {
                Random r = new Random();

                _randomArtist = _items[r.Next(0, _items.Count - 1)];

                artistName.Text = _randomArtist.ArtistName;
                artistAlbums .Text = _randomArtist.ArtistAlbums + " albums";
                artistTracks.Text = _randomArtist.ArtistTracks + " tracks";
            }
        }

        private void randomArtist_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.ViewModel.SelectedArtist = _randomArtist;

            NavigationService.Navigate(new Uri("/ArtistDetails.xaml?Artist=" + _randomArtist.ArtistId, UriKind.Relative));

        }

    }
}