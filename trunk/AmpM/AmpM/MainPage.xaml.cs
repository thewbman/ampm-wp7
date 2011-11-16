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

        private PeriodicTask periodicTask;
        private string periodicTaskName = "KeepAliveTask";

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

            this.StartPeriodicAgent();

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
                ScheduledActionService.Remove(periodicTaskName);
            }

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
                    MessageBox.Show("background agent error: "+exception.ToString());
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

                    NavigationService.Navigate(new Uri("/Home.xaml?Remove=1", UriKind.Relative));

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

            NavigationService.Navigate(new Uri("/Home.xaml?Remove=1", UriKind.Relative));
        }


        private void settingsButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Preferences.xaml", UriKind.Relative));

        }


        
    }
}