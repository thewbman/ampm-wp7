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

            if ((null != BackgroundAudioPlayer.Instance.Track) && (App.ViewModel.AppSettings.NowplayingIndexSetting < App.ViewModel.Nowplaying.Count))
            {
                int newIndex = App.ViewModel.AppSettings.NowplayingIndexSetting + 1;
                songCount.Text = newIndex + "/" + App.ViewModel.Nowplaying.Count;
                DataContext = App.ViewModel.Nowplaying[App.ViewModel.AppSettings.NowplayingIndexSetting];
            }
            else
            {
                songCount.Text = "";
                DataContext = null;
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
                int newIndex = App.ViewModel.AppSettings.NowplayingIndexSetting + 1;
                songCount.Text = newIndex + "/" + App.ViewModel.Nowplaying.Count;
                DataContext = App.ViewModel.Nowplaying[App.ViewModel.AppSettings.NowplayingIndexSetting];
            }
            else
            {
                songCount.Text = "";
                DataContext = null;
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
    }
}