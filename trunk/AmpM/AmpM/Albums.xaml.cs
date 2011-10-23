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
    public partial class Albums : PhoneApplicationPage
    {
        public Albums()
        {
            InitializeComponent();

            _items = new ObservableCollection<DataItemViewModel>();
            _searchItems = new ObservableCollection<DataItemViewModel>();

            //_items.Add(new DataItemViewModel() { PlaylistId = -1, PlaylistName = "playlist name", PlaylistItems = 44, AlbumId = -1, AlbumName = "album name", AlbumTracks = 34, ArtistAlbums = 2, ArtistId = 32, ArtistName = "artist name", ArtistTracks = 23, ArtUrl = "http://www.google.com/", SongId = 343, SongName = "song name", Type = "playlist" });

            //albumsList.ItemsSource = _items;
            //albumsJumpList.ItemsSource = _items;
        }

        public ObservableCollection<DataItemViewModel> _items;
        public ObservableCollection<DataItemViewModel> _searchItems;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //ApplicationTitle.Text = "AmpM - " + App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Name;

            if (App.ViewModel.Albums.Count == 0)
            {
                this.GetAlbums();

                //this.SortAndDisplay();
            }
            else
            {
                //this.GetAlbums();

                _items.Clear();

                //_items = App.ViewModel.Albums;

                foreach (DataItemViewModel s in App.ViewModel.Albums)
                {
                    _items.Add(s);
                }

                //albumsList.ItemsSource = _items;
                //albumsJumpList.ItemsSource = _items;

                performanceProgressBarCustomized.IsIndeterminate = false;

                this.SortAndDisplay();
            }
        }

        private void GetAlbums()
        {
            performanceProgressBarCustomized.IsIndeterminate = true;

            this._items.Clear();

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

            }

            //albumsList.ItemsSource = _items;
            //albumsJumpList.ItemsSource = _items;


            var al = _items.OrderBy(x => x.AlbumName).ToArray();
            var bl = _items.OrderBy(x => x.Year).ToArray();
            var sl = _searchItems.OrderBy(x => x.AlbumName).ToArray();

            var albumsByChar = from t in al
                                  group t by t.ItemChar into c
                                  //orderby c.Key
                                  select new Group<DataItemViewModel>(c.Key, c);

            var albumsByYear = from t in bl
                               group t by t.Year into c
                               //orderby c.Key
                               select new Group<DataItemViewModel>(c.Key, c);

            var albumsBySearch = from t in sl
                               group t by t.ItemChar into c
                               //orderby c.Key
                               select new Group<DataItemViewModel>(c.Key, c);



            AlbumsLL.ItemsSource = albumsByChar;
            AlbumsYearLL.ItemsSource = albumsByYear;
            AlbumsSearchLL.ItemsSource = albumsBySearch;


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



        private void AlbumsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (AlbumsLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)AlbumsLL.SelectedItem;

            NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + s.AlbumId, UriKind.Relative));

            AlbumsLL.SelectedItem = null;
        }

        private void AlbumsYearLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (AlbumsYearLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)AlbumsYearLL.SelectedItem;

            NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + s.AlbumId, UriKind.Relative));

            AlbumsYearLL.SelectedItem = null;
        }

        private void AlbumsSeachLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (AlbumsSearchLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)AlbumsSearchLL.SelectedItem;

            NavigationService.Navigate(new Uri("/Songs.xaml?Album=" + s.AlbumId, UriKind.Relative));

            AlbumsSearchLL.SelectedItem = null;
        }

        private void searchBoxButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            performanceProgressBarCustomized.IsIndeterminate = true;

            this._searchItems.Clear();

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("albums", "&filter="+searchBox.Text)));
            webRequest.BeginGetResponse(new AsyncCallback(DataSearchCallback), webRequest);
        }

    }
}