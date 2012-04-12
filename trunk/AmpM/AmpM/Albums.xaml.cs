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
using System.Windows.Media.Imaging;

namespace AmpM
{
    public partial class Albums : PhoneApplicationPage
    {
        public Albums()
        {
            InitializeComponent();

            _items = new ObservableCollection<DataItemViewModel>();
            _searchItems = new ObservableCollection<DataItemViewModel>();

            _randomAlbum = new DataItemViewModel();

        }

        public ObservableCollection<DataItemViewModel> _items;
        public ObservableCollection<DataItemViewModel> _searchItems;

        private DataItemViewModel _randomAlbum;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            performanceProgressBarCustomized.IsIndeterminate = true;

            this.Perform(() => LoadAlbums(), 150);

        }
        private void LoadAlbums()
        {
            
            if (!App.ViewModel.IsAlbumsLoaded)
            {
                App.ViewModel.LoadAlbums();
            }

            if (App.ViewModel.Albums.Count == 0)
            {
                this.GetAlbums();
            }
            else
            {

                _items.Clear();

                foreach (DataItemViewModel s in App.ViewModel.Albums)
                {
                    _items.Add(s);
                }

                performanceProgressBarCustomized.IsIndeterminate = false;

                this.SortAndDisplay();
            }
        }

        private void GetAlbums()
        {
            performanceProgressBarCustomized.IsIndeterminate = true;

            App.ViewModel.Albums.Clear();
            this._items.Clear();

            AlbumsLL.ItemsSource = null;
            AlbumsArtistLL.ItemsSource = null;
            AlbumsYearLL.ItemsSource = null;
            AlbumsSearchLL.ItemsSource = null;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("albums", "")));
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
                        _items.Add(newItem);

                        //MessageBox.Show("adding newItem to list: "+newItem.PlaylistName+" _ items: "+newItem.PlaylistItems+" _ id: "+newItem.PlaylistId);

                        //albumsList.ItemsSource = _items;
                        //albumsJumpList.ItemsSource = _items;
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
                        _searchItems.Add(newItem);

                        //MessageBox.Show("adding newItem to list: "+newItem.PlaylistName+" _ items: "+newItem.PlaylistItems+" _ id: "+newItem.PlaylistId);

                        //albumsList.ItemsSource = _items;
                        //albumsJumpList.ItemsSource = _items;
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
            if (App.ViewModel.Albums.Count == 0)
            {
                //better logic here - asdf

                App.ViewModel.Albums.Clear();

                foreach (DataItemViewModel s in _items)
                {
                    App.ViewModel.Albums.Add(s);
                }

                App.ViewModel.saveAlbums();

            }


            var al = _items.OrderBy(x => x.AlbumName).ToArray();
            var bl = _items.OrderBy(x => x.ArtistName).ToArray();
            var cl = _items.OrderBy(x => x.Year).ToArray();
            var sl = _searchItems.OrderBy(x => x.AlbumName).ToArray();


            List<DataItemViewModel> b2 = new List<DataItemViewModel>();

            foreach (DataItemViewModel a in bl)
            {
                var a2 = App.ViewModel.Functions.CloneItem(a);
                
                a2.ItemChar = App.ViewModel.Functions.FirstChar(a2.ArtistName);

                b2.Add(a2);
            }


            var albumsByChar = (from t in al
                               group t by t.ItemChar into c
                               //orderby c.Key
                               select new Group<DataItemViewModel>(c.Key, c));

            var albumsByArtist = from t in b2
                                 group t by t.ItemChar into c
                               //orderby c.Key
                               select new Group<DataItemViewModel>(c.Key, c);

            var albumsByYear = from t in cl
                               group t by t.Year into c
                               //orderby c.Key
                               select new Group<DataItemViewModel>(c.Key, c);

            var albumsBySearch = from t in sl
                               group t by t.ItemChar into c
                               //orderby c.Key
                               select new Group<DataItemViewModel>(c.Key, c);
            
            //not sure how to have blank values for empty groups in LL group picker
            /*
            var emptyGroups = new List<Group<DataItemViewModel>>()
            {
                new Group<DataItemViewModel>("~", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("#", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("A", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("B", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("C", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("D", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("E", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("F", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("G", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("H", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("I", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("J", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("K", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("L", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("M", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("N", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("O", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("P", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("Q", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("R", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("S", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("T", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("U", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("V", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("W", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("X", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("Y", new List<DataItemViewModel>()),
                new Group<DataItemViewModel>("Z", new List<DataItemViewModel>())
            };

            AlbumsLL.ItemsSource = (from t in albumsByChar.Union(emptyGroups)
                                    orderby t.Title
                                    select t).ToList();

            */

            AlbumsLL.ItemsSource = albumsByChar;
            AlbumsArtistLL.ItemsSource = albumsByArtist;
            AlbumsYearLL.ItemsSource = albumsByYear;
            AlbumsSearchLL.ItemsSource = albumsBySearch;

            albumsPivot.Title = "ALBUMS (" + this._items.Count + ")";

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

            public bool HasItems
            {
                get
                {
                    return Items.Count > 0;
                }
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


        private void AlbumsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (AlbumsLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)AlbumsLL.SelectedItem;

            //NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + s.AlbumId, UriKind.Relative));
            NavigationService.Navigate(new Uri("/SongsList.xaml?Album=" + s.AlbumId + "&AlbumName=" + s.AlbumName, UriKind.Relative));

            AlbumsLL.SelectedItem = null;
        }

        private void AlbumsArtistLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlbumsArtistLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)AlbumsArtistLL.SelectedItem;

            //NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + s.AlbumId, UriKind.Relative));
            NavigationService.Navigate(new Uri("/SongsList.xaml?Album=" + s.AlbumId + "&AlbumName=" + s.AlbumName, UriKind.Relative));

            AlbumsArtistLL.SelectedItem = null;
        }

        private void AlbumsYearLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (AlbumsYearLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)AlbumsYearLL.SelectedItem;

            //NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + s.AlbumId, UriKind.Relative));
            NavigationService.Navigate(new Uri("/SongsList.xaml?Album=" + s.AlbumId + "&AlbumName=" + s.AlbumName, UriKind.Relative));

            AlbumsYearLL.SelectedItem = null;
        }

        private void AlbumsSearchLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (AlbumsSearchLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)AlbumsSearchLL.SelectedItem;

            //NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + s.AlbumId, UriKind.Relative));
            NavigationService.Navigate(new Uri("/SongsList.xaml?Album=" + s.AlbumId + "&AlbumName=" + s.AlbumName, UriKind.Relative));

            AlbumsSearchLL.SelectedItem = null;
        }

        private void searchBoxButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (searchBox.Text == "")
            {
                MessageBox.Show("You must enter some search text");
                return;
            }

            this.StartSearch();
        }

        private void StartSearch()
        {
            this.Focus();

            performanceProgressBarCustomized.IsIndeterminate = true;

            this._searchItems.Clear();

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("albums", "&filter="+searchBox.Text)));
            webRequest.BeginGetResponse(new AsyncCallback(DataSearchCallback), webRequest);
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            this.GetAlbums();
        }

        private void nextRandomButton_Click(object sender, RoutedEventArgs e)
        {
            updateRandom();
        }

        private void albumsPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateRandom();
        }

        private void updateRandom()
        {
            if (_items.Count > 1)
            {
                Random r = new Random();

                _randomAlbum = _items[r.Next(0, _items.Count - 1)];

                artistName.Text = "artist: "+_randomAlbum.ArtistName;
                albumName.Text = "album: "+_randomAlbum.AlbumName;
                albumTracks.Text = _randomAlbum.AlbumTracks + " tracks";
                albumYear.Text = _randomAlbum.Year;
                artUrl.Source = new BitmapImage(new Uri(_randomAlbum.ArtUrl));
            }
        }

        private void randomAlbum_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            //NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + _randomAlbum.AlbumId, UriKind.Relative));
            NavigationService.Navigate(new Uri("/SongsList.xaml?Album=" + _randomAlbum.AlbumId + "&AlbumName=" + _randomAlbum.AlbumName, UriKind.Relative));

        }

        private void randomAartistName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.ViewModel.SelectedArtist = _randomAlbum;
            NavigationService.Navigate(new Uri("/ArtistDetails.xaml?Artist=" + _randomAlbum.ArtistId, UriKind.Relative));
        }

        private void randomArtistButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void randomAlbumButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void searchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "Enter")
                this.StartSearch();
        }


    }
}