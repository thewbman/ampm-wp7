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
    public partial class Tags : PhoneApplicationPage
    {
        public Tags()
        {
            InitializeComponent();

            //DataContext = App.ViewModel;

            _items = new ObservableCollection<DataItemViewModel>();

        }

        public ObservableCollection<DataItemViewModel> _items;
        //public List<DataItemViewModel> _items;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                ApplicationTitle.Text = "AmpM - " + App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Name;

                if (App.ViewModel.Tags.Count == 0)
                {
                    this.GetTags();

                    //this.SortAndDisplay();
                }
                else
                {

                    _items.Clear();

                    foreach (DataItemViewModel s in App.ViewModel.Tags)
                    {
                        _items.Add(s);
                    }

                    this.SortAndDisplay();

                    performanceProgressBarCustomized.IsIndeterminate = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "to error", MessageBoxButton.OK);
            }
        }

        private void GetTags()
        {
            try
            {
                performanceProgressBarCustomized.IsIndeterminate = true;

                this._items.Clear();

                TagsLL.ItemsSource = null;

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(App.ViewModel.Functions.GetAmpacheDataUrl("tags", "")));
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

                foreach (XElement singleDataElement in xdoc.Element("root").Descendants("tag"))
                {
                    DataItemViewModel newItem = new DataItemViewModel();

                    newItem.Type = "tag";

                    newItem.TagId = int.Parse(singleDataElement.Attribute("id").Value);

                    newItem.TagName = singleDataElement.Element("name").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();
                    newItem.TagAlbums = int.Parse(singleDataElement.Element("albums").FirstNode.ToString());
                    newItem.TagArtists = int.Parse(singleDataElement.Element("artists").FirstNode.ToString());
                    newItem.TagSongs = int.Parse(singleDataElement.Element("songs").FirstNode.ToString());

                    newItem.ItemKey = "tag" + newItem.PlaylistId;
                    newItem.ItemId = newItem.TagId;
                    newItem.ItemChar = App.ViewModel.Functions.FirstChar(newItem.TagName);
                    newItem.Auth = App.ViewModel.Auth;

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

            if (App.ViewModel.Tags.Count == 0)
            {
                App.ViewModel.Tags.Clear();

                foreach (DataItemViewModel s in _items)
                {
                    App.ViewModel.Tags.Add(s);
                }
            }

            var al = _items.OrderBy(x => x.TagName).ToArray();


            var tagsByChar = from t in al
                               group t by t.ItemChar into c
                               //orderby c.Key
                               select new Group<DataItemViewModel>(c.Key, c);



            TagsLL.ItemsSource = tagsByChar;

            //tagsPivot.Title = "albums (" + this._items.Count + ")";

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



        private void TagsLL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (TagsLL.SelectedItem == null)
                return;

            var s = (DataItemViewModel)TagsLL.SelectedItem;

            App.ViewModel.SelectedTag = s;

            NavigationService.Navigate(new Uri("/TagDetails.xaml?Tag=" + s.TagId, UriKind.Relative));

            TagsLL.SelectedItem = null;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            App.ViewModel.Tags.Clear();

            this.GetTags();
        }
    }
}