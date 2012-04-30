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
using Microsoft.Phone.BackgroundAgentSharedConsts;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Tasks;

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

            Microsoft.Xna.Framework.Media.MediaLibrary library = new Microsoft.Xna.Framework.Media.MediaLibrary();
            
        }

        private PeriodicTask periodicTask;
        private string periodicTaskName = "KeepAliveTask";

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            string inValue = "";
            if (NavigationContext.QueryString.TryGetValue("Remove", out inValue))
            {
                int toRemove = int.Parse(inValue);

                for (int i = 0; i < toRemove; i++)
                {
                    NavigationService.RemoveBackEntry();
                }
            }
             */
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();

                if ((App.ViewModel.AppSettings.FirstRunSetting) || (false))
                {
                    MessageBox.Show("Welcome to AmpM.  This is an app for listening to music from an Ampache system.  If you do not know what that means this app is not for you.", "AmpM", MessageBoxButton.OK);

                    App.ViewModel.AppSettings.FirstRunSetting = false;

                }

                if ((App.ViewModel.AppSettings.AppStartsSetting > 10) && (!App.ViewModel.AppSettings.ReviewedSetting))
                {
                    App.ViewModel.AppSettings.ReviewedSetting = true;

                    if (MessageBox.Show("Would you mind taking a minute to review this app in the marketplace?", "App Review", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        MarketplaceDetailTask marketDetail = new MarketplaceDetailTask();
                        marketDetail.ContentIdentifier = "c4c72638-8a65-4a57-89cb-6c186555b702";
                        marketDetail.Show();
                    }
                }

            }

            //this.StartPeriodicAgent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.ViewModel.getNowplaying();

        }


        private void StartPeriodicAgent()
        {
            
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                //ScheduledActionService.Remove(periodicTaskName);
            }
            else
            {

                periodicTask = new PeriodicTask(periodicTaskName);

                periodicTask.Description = "Pings the Ampache server to keep you connected.";

                try
                {
                    ScheduledActionService.Add(periodicTask);

                    //ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(5));

                }
                catch (InvalidOperationException exception)
                {
                    if (exception.Message.Contains("BNS Error: The action is disabled"))
                    {
                        MessageBox.Show("Background agents for this application have been disabled by the user.");
                    }
                    else
                    {
                        MessageBox.Show("background agent error: " + exception.ToString());
                    }
                }
            }


            if (App.ViewModel.AppSettings.SessionExpireSetting == "1900-01-01T00:00:00")
            {
                //
            }
            else
            {
                DateTime d = DateTime.Parse(App.ViewModel.AppSettings.SessionExpireSetting);
                DateTime n = DateTime.Now;

                TimeSpan t = d - n;

                if (t.TotalSeconds > 0)
                {

                    NavigationService.Navigate(new Uri("/Home.xaml?Remove=1&Ping=true", UriKind.Relative));

                    //App.ViewModel.AppSettings.AuthSetting = App.ViewModel.AppSettings.AuthSetting;
                }
                else
                {
                    App.ViewModel.AppSettings.SessionExpireSetting = "1900-01-01T00:00:00";
                }
            }
        }


        private void hostsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (hostsList.SelectedItem == null)
                return;

            App.ViewModel.AppSettings.HostIndexSetting = hostsList.SelectedIndex;
            App.ViewModel.AppSettings.HostAddressSetting = App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Address;

            NavigationService.Navigate(new Uri("/Home.xaml?Remove=1&NewSession=true", UriKind.Relative));
        }


        private void settingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Preferences.xaml", UriKind.Relative));

        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddServer.xaml", UriKind.Relative));
        }

        private void deleteHost_Click(object sender, RoutedEventArgs e)
        {

            MenuItem menu = sender as MenuItem;
            HostViewModel selectedItem = menu.DataContext as HostViewModel;

            if (selectedItem == null)
                return;

            
            for (int i = 0; i < App.ViewModel.Hosts.Count; i++)
            {
                if ((App.ViewModel.Hosts[i].Address == selectedItem.Address) && (App.ViewModel.Hosts[i].Username == selectedItem.Username) && (App.ViewModel.Hosts[i].Password == selectedItem.Password))
                    App.ViewModel.Hosts.RemoveAt(i);
            }


            App.ViewModel.saveHosts();
        }



        private void emptyButton_Click(object sender, EventArgs e)
        {
            MyAudioPlaybackAgent.AudioPlayer.stopAll();
            MyAudioPlaybackAgent.AudioPlayer.resetList();
            BackgroundAudioPlayer.Instance.Track = null;

            App.ViewModel.Nowplaying.Clear();
            App.ViewModel.saveNowplaying();
            App.ViewModel.AppSettings.NowplayingIndexSetting = 0;
        }

        
    }
}