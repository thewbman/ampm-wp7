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
using System.Windows.Media.Imaging;
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
using MyAudioPlaybackAgent;
using Coding4Fun.Phone.Controls;

namespace AmpM
{
    public partial class Nowplaying : PhoneApplicationPage
    {
        public Nowplaying()
        {
            InitializeComponent();

            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);
        }

        private string lastfmImage = "";

        private bool CanRemoveBackStack;

        private string lastfmTitle;
        private string lastfmArtist;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            nowplayingList.ItemsSource = App.ViewModel.Nowplaying;

            CanRemoveBackStack = true;


            /*
            string inValue = "";
            if (NavigationContext.QueryString.TryGetValue("Remove", out inValue))
            {
                int toRemove = int.Parse(inValue);

                for (int i = 0; i < toRemove; i++)
                {
                    //dont get rid of Home
                    if (NavigationService.BackStack.Count() > 1) NavigationService.RemoveBackEntry();
                }
            }
             */

            this.Perform(() => RemoveBackStack(), 5000);

            if ((null != BackgroundAudioPlayer.Instance.Track) && (App.ViewModel.Nowplaying.Count > App.ViewModel.AppSettings.NowplayingIndexSetting))
            {
                //DataContext = App.ViewModel.Nowplaying[App.ViewModel.AppSettings.NowplayingIndexSetting];
                artUrl.Source = new BitmapImage(BackgroundAudioPlayer.Instance.Track.AlbumArt);
                artUrl.Visibility = System.Windows.Visibility.Visible;
                songName.Text = BackgroundAudioPlayer.Instance.Track.Title;
                artistName.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                albumName.Text = BackgroundAudioPlayer.Instance.Track.Album;
                //App.ViewModel.AppSettings.NowplayingIndexSetting = int.Parse(BackgroundAudioPlayer.Instance.Track.Tag);

                int newIndex = App.ViewModel.AppSettings.NowplayingIndexSetting + 1;
                /*
                //int newIndex = MyAudioPlaybackAgent.AudioPlayer.AppSettings.NowplayingIndexSetting + 1;
                int SongIndex;
                int newIndex;

                if (int.TryParse(BackgroundAudioPlayer.Instance.Track.Tag, out SongIndex))
                    newIndex = SongIndex + 1;
                else
                    newIndex = 1;
                */
                
                //songCount.Text = newIndex + "/" + App.ViewModel.Nowplaying.Count;
                songCount.Text = BackgroundAudioPlayer.Instance.Track.Tag + "/" + App.ViewModel.Nowplaying.Count;

            }
            else
            {
                songCount.Text = "";

                artUrl.Visibility = System.Windows.Visibility.Collapsed;
                songName.Text = "";
                artistName.Text = "";
                albumName.Text = "";
            }

            if (App.ViewModel.AppSettings.FirstNowplayingSetting)
            {
                MessageBox.Show("If you don't hear any music start shortly, you may need to reset the system audio control using the menu at the bottom of the screen.");

                App.ViewModel.AppSettings.FirstNowplayingSetting = false;
            }

            
            this.updateLastfmImage();

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CanRemoveBackStack = false;

            this.CloseBufferingPopup();

            base.OnNavigatedFrom(e);
        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            //BannerMessage("PlayerState: "+BackgroundAudioPlayer.Instance.PlayerState.ToString());

            switch (BackgroundAudioPlayer.Instance.PlayerState)
            {
                case PlayState.Playing:
                    this.bufferingPopup.IsOpen = false;
                    this.LastfmScrobble(BackgroundAudioPlayer.Instance.Track.Title, BackgroundAudioPlayer.Instance.Track.Artist);
                    break;

                case PlayState.Paused:
                case PlayState.Stopped:
                    this.bufferingPopup.IsOpen = false;
                    break;
                case PlayState.BufferingStarted:
                    this.bufferingPopup.IsOpen = true;
                    break;
                case PlayState.TrackEnded:
                    //this.bufferingPopup.IsOpen = true;
                    break;
                case PlayState.Unknown:
                    //this.bufferingPopup.IsOpen = true;
                    break;
            }

            //MyAudioPlaybackAgent.AudioPlayer.AppSettings.NowplayingIndexSetting = MyAudioPlaybackAgent.AudioPlayer.AppSettings.NowplayingIndexSetting + 0;

            if ((null != BackgroundAudioPlayer.Instance.Track) && (App.ViewModel.Nowplaying.Count > App.ViewModel.AppSettings.NowplayingIndexSetting))
            {

                //DataContext = App.ViewModel.Nowplaying[App.ViewModel.AppSettings.NowplayingIndexSetting];
                artUrl.Source = new BitmapImage(BackgroundAudioPlayer.Instance.Track.AlbumArt);
                artUrl.Visibility = System.Windows.Visibility.Visible;
                songName.Text = BackgroundAudioPlayer.Instance.Track.Title;
                artistName.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                albumName.Text = BackgroundAudioPlayer.Instance.Track.Album;
                //App.ViewModel.AppSettings.NowplayingIndexSetting = int.Parse(BackgroundAudioPlayer.Instance.Track.Tag);

                int newIndex = App.ViewModel.AppSettings.NowplayingIndexSetting + 1;
                /*
                //int newIndex = MyAudioPlaybackAgent.AudioPlayer.AppSettings.NowplayingIndexSetting + 1;
                int SongIndex;
                int newIndex;

                if (int.TryParse(BackgroundAudioPlayer.Instance.Track.Tag, out SongIndex))
                    newIndex = SongIndex + 1;
                else
                    newIndex = 1;
                */

                //songCount.Text = newIndex + "/" + App.ViewModel.Nowplaying.Count;
                songCount.Text = BackgroundAudioPlayer.Instance.Track.Tag + "/" + App.ViewModel.Nowplaying.Count;
            }
            else
            {
                songCount.Text = "";

                artUrl.Visibility = System.Windows.Visibility.Collapsed;
                songName.Text = "";
                artistName.Text = "";
                albumName.Text = "";
            }

            this.updateLastfmImage();
        }

        private void nowplayingList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (nowplayingList.SelectedItem == null)
                return;

            this.bufferingPopup.IsOpen = true;

            if (App.ViewModel.Nowplaying.Count > 0)
            {
                App.ViewModel.AppSettings.NowplayingIndexSetting = nowplayingList.SelectedIndex+0;

                MyAudioPlaybackAgent.AudioPlayer.startPlaying(App.ViewModel.AppSettings.NowplayingIndexSetting);
            }

            nowplayingList.SelectedItem = null;
        }

        private void RemoveBackStack()
        {
            if (CanRemoveBackStack)
            {
                for (int i = 0; i < 10; i++)
                {
                    //dont get rid of Home
                    if (NavigationService.BackStack.Count() > 1) NavigationService.RemoveBackEntry();
                }
            }
        }



        private void Perform(Action myMethod, int delayInMilliseconds)
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (s, e) => Thread.Sleep(delayInMilliseconds);

            worker.RunWorkerCompleted += (s, e) => myMethod.Invoke();

            worker.RunWorkerAsync();
        }


        private void emptyButton_Click(object sender, EventArgs e)
        {
            MyAudioPlaybackAgent.AudioPlayer.stopAll();
            MyAudioPlaybackAgent.AudioPlayer.resetList();
            BackgroundAudioPlayer.Instance.Track = null;

            App.ViewModel.Nowplaying.Clear();
            App.ViewModel.saveNowplaying();
            App.ViewModel.AppSettings.NowplayingIndexSetting = 0;

            nowplayingList.ItemsSource = null;

            this.Perform(() => CloseBufferingPopup(), 1000);
        }

        private void takeAudioControl_Click(object sender, EventArgs e)
        {
            MediaPlayerLauncher m = new MediaPlayerLauncher();
            m.Media = new Uri("empty.mp3", UriKind.Relative);
            //m.Orientation = MediaPlayerOrientation.Landscape;
            m.Location = MediaLocationType.Data;
            m.Show();

        }

        private void songSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                //BackgroundAudioPlayer.Instance.Position = System.TimeSpan.FromSeconds((songSlider.Value / 100) * (BackgroundAudioPlayer.Instance.Track.Duration.TotalSeconds));
            }
            catch
            {
                //
            }
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            BackgroundAudioPlayer.Instance.SkipPrevious();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (BackgroundAudioPlayer.Instance.PlayerState == PlayState.Playing)
            {
                BackgroundAudioPlayer.Instance.Pause();
            }
            else
            {
                BackgroundAudioPlayer.Instance.Play();
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            BackgroundAudioPlayer.Instance.SkipNext();
        }

        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            nowplayingList.IsEnabled = false;
        }

        private void ContextMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            nowplayingList.IsEnabled = true;
        }

        private void removeSingleSong_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            //string s = Uri.EscapeUriString(BackgroundAudioPlayer.Instance.Track.Source.ToString());
            
            int CurrentSongIndex = -1;

            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                if (int.TryParse(BackgroundAudioPlayer.Instance.Track.Tag, out CurrentSongIndex))
                {
                    CurrentSongIndex--;
                }
            }

            int SelectedIndex = -1;
            int i = 0;

            foreach (DataItemViewModel d in App.ViewModel.Nowplaying)
            {
                if (selectedItem == d)
                    SelectedIndex = i;

                i++;
            }

            if (CurrentSongIndex == SelectedIndex)
            {
                MessageBox.Show("You cannot remove the current song");
            }
            else if(CurrentSongIndex < SelectedIndex)
            {
                //MessageBox.Show("selectedItem.SongUrl: " + selectedItem.SongUrl);
                //MessageBox.Show("BackgroundAudioPlayer.Instance.Track.Source.ToString(): " + s);
                
                App.ViewModel.Nowplaying.Remove(selectedItem);

                updateNowplaying(CurrentSongIndex);
            }
            else if (CurrentSongIndex > SelectedIndex)
            {
                //MessageBox.Show("selectedItem.SongUrl: " + selectedItem.SongUrl);
                //MessageBox.Show("BackgroundAudioPlayer.Instance.Track.Source.ToString(): " + s);

                App.ViewModel.Nowplaying.Remove(selectedItem);

                updateNowplaying(CurrentSongIndex-1);
            }

        }

        private void removeAbove_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;

            int CurrentSongIndex = int.Parse(BackgroundAudioPlayer.Instance.Track.Tag) - 1;
            int SelectedIndex = -1;
            int i = 0;

            foreach (DataItemViewModel d in App.ViewModel.Nowplaying)
            {
                if (selectedItem == d)
                    SelectedIndex = i;

                i++;
            }

            if (CurrentSongIndex >= SelectedIndex)
            {
                List<DataItemViewModel> newSongs = new List<DataItemViewModel>();

                for (int n = SelectedIndex; n < App.ViewModel.Nowplaying.Count; n++)
                {
                    newSongs.Add(App.ViewModel.Functions.CloneItem(App.ViewModel.Nowplaying[n]));
                }

                App.ViewModel.Nowplaying.Clear();

                foreach(DataItemViewModel s in newSongs)
                {
                    App.ViewModel.Nowplaying.Add(App.ViewModel.Functions.CloneItem(s));
                }
            }
            else
            {
                MessageBox.Show("You cannot remove the current song");
            }

            updateNowplaying(CurrentSongIndex-SelectedIndex);
        }

        private void removeBelow_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            DataItemViewModel selectedItem = menu.DataContext as DataItemViewModel;

            if (selectedItem == null)
                return;
            
            int CurrentSongIndex = int.Parse(BackgroundAudioPlayer.Instance.Track.Tag) - 1;
            int SelectedIndex = -1;
            int i = 0;

            foreach (DataItemViewModel d in App.ViewModel.Nowplaying)
            {
                if (selectedItem == d)
                    SelectedIndex = i;

                i++;
            }

            if (CurrentSongIndex <= SelectedIndex)
            {
                List<DataItemViewModel> newSongs = new List<DataItemViewModel>();

                for (int n = 0; n <= SelectedIndex; n++)
                {
                    newSongs.Add(App.ViewModel.Functions.CloneItem(App.ViewModel.Nowplaying[n]));
                }

                App.ViewModel.Nowplaying.Clear();

                foreach (DataItemViewModel s in newSongs)
                {
                    App.ViewModel.Nowplaying.Add(App.ViewModel.Functions.CloneItem(s));
                }
            }
            else
            {
                MessageBox.Show("You cannot remove the current song");
            }

            updateNowplaying(CurrentSongIndex);
        }

        private void updateNowplaying(int inNewIndex)
        {
            nowplayingList.ItemsSource = null;

            int i = inNewIndex + 1;

            App.ViewModel.AppSettings.NowplayingIndexSetting = inNewIndex;
            App.ViewModel.saveNowplaying();

            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                AudioTrack track = BackgroundAudioPlayer.Instance.Track;
                track.BeginEdit();
                track.Tag = i.ToString();
                track.EndEdit();

                songCount.Text = BackgroundAudioPlayer.Instance.Track.Tag + "/" + App.ViewModel.Nowplaying.Count;
            }

            nowplayingList.ItemsSource = App.ViewModel.Nowplaying;
        }

        private void CloseBufferingPopup()
        {
            this.bufferingPopup.IsOpen = false;
        }

        private void LastfmScrobble(string inTitle, string inArtist)
        {
            lastfmTitle = inTitle;
            lastfmArtist = inArtist;

            DateTime date = DateTime.Now;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan span = (date - epoch);
            string timestamp = span.TotalSeconds.ToString();

            string method = "track.updateNowPlaying";

            string api_sig = MD5Core.GetHashString("api_key" + App.ViewModel.LastfmApikey + "artist" + inArtist + "method" + method + "sk" + App.ViewModel.AppSettings.LastfmKeySetting + "timestamp" + timestamp + "track" + inTitle + App.ViewModel.LastfmSecret).ToLower();

            string fullUrl = App.ViewModel.LastfmUrl;
            fullUrl += "?api_key=" + App.ViewModel.LastfmApikey;
            fullUrl += "&method=" + method;
            fullUrl += "&timestamp=" + timestamp;
            fullUrl += "&track=" + inTitle;
            fullUrl += "&artist=" + inArtist;
            fullUrl += "&sk=" + App.ViewModel.AppSettings.LastfmKeySetting;
            fullUrl += "&api_sig=" + api_sig.ToLower();

            //MessageBox.Show("fullUrl: " + fullUrl);

            Uri scrobbleuri = new Uri(fullUrl);

            string postData = "";
            postData += "api_key=" + App.ViewModel.LastfmApikey;
            postData += "&method=" + method;
            postData += "&timestamp=" + timestamp;
            postData += "&track=" + lastfmTitle;
            postData += "&artist=" + lastfmArtist;
            postData += "&sk=" + App.ViewModel.AppSettings.LastfmKeySetting;
            postData += "&api_sig=" + api_sig.ToLower();

            UTF8Encoding encoding=new UTF8Encoding();

            byte[] postDataBytes = encoding.GetBytes(postData);

            /*
            EmailComposeTask emailcomposer = new EmailComposeTask();
            emailcomposer.To = "ampm.wp7@gmail.com";
            emailcomposer.Subject = "Lastfm scrobble";
            emailcomposer.Body = scrobbleuri.ToString();
            emailcomposer.Show();
             */

            if (App.ViewModel.AppSettings.LastfmKeySetting.Length > 0)
            {
                /*
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.CreateHttp(new Uri(App.ViewModel.LastfmUrl));
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.BeginGetRequestStream(new AsyncCallback(LastfmScrobbleStreamCallback), webRequest);
                //webRequest.BeginGetResponse(new AsyncCallback(LastfmScrobbleCallback), webRequest);
                


                WebClient client = new WebClient(); client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_UploadStringCompleted);
                client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                client.Encoding = Encoding.UTF8;
                client.UploadStringAsync(new Uri(App.ViewModel.LastfmUrl), "POST", postData);
                
                 */

            }
        }
        private void LastfmScrobbleStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            // End the stream request operation
            Stream postStream = webRequest.EndGetRequestStream(asynchronousResult);


            DateTime date = DateTime.Now;
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan span = (date - epoch);
            string timestamp = span.TotalSeconds.ToString();

            string method = "track.updateNowPlaying";

            string api_sig = MD5Core.GetHashString("api_key" + App.ViewModel.LastfmApikey + "artist" + lastfmArtist + "method" + method + "sk" + App.ViewModel.AppSettings.LastfmKeySetting + "timestamp" + timestamp + "track" + lastfmTitle + App.ViewModel.LastfmSecret).ToLower();

            string postData = "";
            postData += "api_key=" + App.ViewModel.LastfmApikey;
            postData += "&method=" + method;
            postData += "&timestamp=" + timestamp;
            postData += "&track=" + lastfmTitle;
            postData += "&artist=" + lastfmArtist;
            postData += "&sk=" + App.ViewModel.AppSettings.LastfmKeySetting;
            postData += "&api_sig=" + api_sig.ToLower();


            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Add the post data to the web request
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();


            webRequest.BeginGetResponse(new AsyncCallback(LastfmScrobbleCallback), webRequest);
        }
        private void LastfmScrobbleCallback(IAsyncResult asynchronousResult)
        {
            //

        }

        private void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            MessageBox.Show(e.Result);
        }


        private void updateLastfmImage()
        {

            if ((BackgroundAudioPlayer.Instance.Track != null)&&(App.ViewModel.AppSettings.LastfmImagesSetting))
            {

                lastfmTitle = BackgroundAudioPlayer.Instance.Track.Title;
                lastfmArtist = BackgroundAudioPlayer.Instance.Track.Artist;


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


        private void BannerMessage(string inMessage)
        {
            ToastPrompt toast = new ToastPrompt();

            toast.Title = inMessage;
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;

            toast.Show();
        }

    }
}