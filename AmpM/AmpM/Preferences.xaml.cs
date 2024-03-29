﻿using System;
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

namespace AmpM
{
    public partial class Preferences : PhoneApplicationPage
    {
        public Preferences()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            //PlayAdd.IsChecked = App.ViewModel.AppSettings.DefaultPlayAddSetting;
            //PlayShuffle.IsChecked = App.ViewModel.AppSettings.DefaultPlayShuffleSetting;
            //PlayAll.IsChecked = App.ViewModel.AppSettings.DefaultPlayAllSetting;

            TogglePlayAdd.IsChecked = App.ViewModel.AppSettings.DefaultPlayAddSetting;
            TogglePlayShuffle.IsChecked = App.ViewModel.AppSettings.DefaultPlayShuffleSetting;
            TogglePlayAll.IsChecked = App.ViewModel.AppSettings.DefaultPlayAllSetting;

            ToggleLastfm.IsChecked = App.ViewModel.AppSettings.LastfmSetting;
            LastfmUsername.Text = App.ViewModel.AppSettings.LastfmUsernameSetting;
            LastfmPassword.Password = App.ViewModel.AppSettings.LastfmPasswordSetting;
            ToggleLastfmImages.IsChecked = App.ViewModel.AppSettings.LastfmImagesSetting;

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            App.ViewModel.AppSettings.DefaultPlayAddSetting = (bool)TogglePlayAdd.IsChecked;
            App.ViewModel.AppSettings.DefaultPlayShuffleSetting = (bool)TogglePlayShuffle.IsChecked;
            App.ViewModel.AppSettings.DefaultPlayAllSetting = (bool)TogglePlayAll.IsChecked;

            App.ViewModel.AppSettings.LastfmSetting = (bool)ToggleLastfm.IsChecked;
            App.ViewModel.AppSettings.LastfmUsernameSetting = LastfmUsername.Text;
            App.ViewModel.AppSettings.LastfmPasswordSetting = LastfmPassword.Password;
            App.ViewModel.AppSettings.LastfmKeySetting = "";
            App.ViewModel.AppSettings.LastfmImagesSetting = (bool)ToggleLastfmImages.IsChecked;

            base.OnNavigatedFrom(e);
        }

        private void appbarSave_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}