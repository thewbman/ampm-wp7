using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.BackgroundAudio;

namespace AmpM
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            hostsList.ItemsSource = App.ViewModel.Hosts;

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();

                if ((App.ViewModel.AppSettings.FirstRunSetting) || (false))
                {
                    MessageBox.Show("Welcome to AmpM.  This is an app for listening to music from an Ampache system.  If you do not know what that means this app is not for you.", "AmpM", MessageBoxButton.OK);

                    App.ViewModel.AppSettings.FirstRunSetting = false;

                }

            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.ViewModel.getNowplaying();
        }

        private void hostsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hostsList.SelectedItem == null)
                return;

            App.ViewModel.AppSettings.HostIndexSetting = hostsList.SelectedIndex;

            NavigationService.Navigate(new Uri("/Home.xaml", UriKind.Relative));
        }


        private void settingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Preferences.xaml", UriKind.Relative));
        }


        
    }
}