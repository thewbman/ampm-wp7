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

namespace AmpM
{
    public partial class Nowplaying : PhoneApplicationPage
    {
        public Nowplaying()
        {
            InitializeComponent();

            BackgroundAudioPlayer.Instance.PlayStateChanged += new EventHandler(Instance_PlayStateChanged);
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            nowplayingList.ItemsSource = App.ViewModel.Nowplaying;

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
        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            switch (BackgroundAudioPlayer.Instance.PlayerState)
            {
                case PlayState.Playing:
                    //
                    break;

                case PlayState.Paused:
                case PlayState.Stopped:
                    //
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
        }

        private void nowplayingList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (nowplayingList.SelectedItem == null)
                return;

            if (App.ViewModel.Nowplaying.Count > 0)
            {
                App.ViewModel.AppSettings.NowplayingIndexSetting = nowplayingList.SelectedIndex+0;

                MyAudioPlaybackAgent.AudioPlayer.startPlaying(App.ViewModel.AppSettings.NowplayingIndexSetting);
            }

            nowplayingList.SelectedItem = null;
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
            
            int CurrentSongIndex = int.Parse(BackgroundAudioPlayer.Instance.Track.Tag) - 1;
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

            AudioTrack track = BackgroundAudioPlayer.Instance.Track;
            track.BeginEdit();
            track.Tag = i.ToString();
            track.EndEdit();

            App.ViewModel.AppSettings.NowplayingIndexSetting = inNewIndex;
            App.ViewModel.saveNowplaying();

            songCount.Text = BackgroundAudioPlayer.Instance.Track.Tag + "/" + App.ViewModel.Nowplaying.Count;
            nowplayingList.ItemsSource = App.ViewModel.Nowplaying;
        }

    }
}