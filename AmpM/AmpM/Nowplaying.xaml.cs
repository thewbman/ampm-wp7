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
                    NavigationService.RemoveBackEntry();
                }
            }

            if ((null != BackgroundAudioPlayer.Instance.Track) && (App.ViewModel.AppSettings.NowplayingIndexSetting < App.ViewModel.Nowplaying.Count))
            {
                //DataContext = App.ViewModel.Nowplaying[App.ViewModel.AppSettings.NowplayingIndexSetting];
                artUrl.Source = new BitmapImage(BackgroundAudioPlayer.Instance.Track.AlbumArt);
                artUrl.Visibility = System.Windows.Visibility.Visible;
                songName.Text = BackgroundAudioPlayer.Instance.Track.Title;
                artistName.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                albumName.Text = BackgroundAudioPlayer.Instance.Track.Album;
                //App.ViewModel.AppSettings.NowplayingIndexSetting = int.Parse(BackgroundAudioPlayer.Instance.Track.Tag);

                int newIndex = App.ViewModel.AppSettings.NowplayingIndexSetting + 1;
                songCount.Text = newIndex + "/" + App.ViewModel.Nowplaying.Count;
                

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

            if ((null != BackgroundAudioPlayer.Instance.Track) && (App.ViewModel.AppSettings.NowplayingIndexSetting < App.ViewModel.Nowplaying.Count))
            {

                //DataContext = App.ViewModel.Nowplaying[App.ViewModel.AppSettings.NowplayingIndexSetting];
                artUrl.Source = new BitmapImage(BackgroundAudioPlayer.Instance.Track.AlbumArt);
                artUrl.Visibility = System.Windows.Visibility.Visible;
                songName.Text = BackgroundAudioPlayer.Instance.Track.Title;
                artistName.Text = BackgroundAudioPlayer.Instance.Track.Artist;
                albumName.Text = BackgroundAudioPlayer.Instance.Track.Album;
                //App.ViewModel.AppSettings.NowplayingIndexSetting = int.Parse(BackgroundAudioPlayer.Instance.Track.Tag);

                int newIndex = App.ViewModel.AppSettings.NowplayingIndexSetting + 1;
                songCount.Text = newIndex + "/" + App.ViewModel.Nowplaying.Count;
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
                App.ViewModel.AppSettings.NowplayingIndexSetting = nowplayingList.SelectedIndex;

                MyAudioPlaybackAgent.AudioPlayer.startPlaying(nowplayingList.SelectedIndex);
            }

            nowplayingList.SelectedItem = null;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            MyAudioPlaybackAgent.AudioPlayer.stopAll();
            MyAudioPlaybackAgent.AudioPlayer.resetList();
            BackgroundAudioPlayer.Instance.Track = null;

            App.ViewModel.Nowplaying.Clear();
            App.ViewModel.saveNowplaying();
            App.ViewModel.AppSettings.NowplayingIndexSetting = 0;

            nowplayingList.ItemsSource = App.ViewModel.Nowplaying;
        }

        private void songSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                BackgroundAudioPlayer.Instance.Position = System.TimeSpan.FromSeconds((songSlider.Value / 100) * (BackgroundAudioPlayer.Instance.Track.Duration.TotalSeconds));
            }
            catch (Exception ex)
            {
                //
            }
        }
    }
}