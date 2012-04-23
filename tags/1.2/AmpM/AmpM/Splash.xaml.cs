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
using Microsoft.Phone.Scheduler;
using System.Windows.Navigation;

namespace AmpM
{
    public partial class Splash : PhoneApplicationPage
    {
        public Splash()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            Microsoft.Xna.Framework.Media.MediaLibrary library = new Microsoft.Xna.Framework.Media.MediaLibrary();

        }

        private PeriodicTask periodicTask;
        private string periodicTaskName = "KeepAliveTask";

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            


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
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                DateTime d = DateTime.Parse(App.ViewModel.AppSettings.SessionExpireSetting);
                DateTime n = DateTime.Now;

                TimeSpan t = d - n;

                if (t.TotalSeconds > 0)
                {

                    if (!App.ViewModel.IsDataLoaded)
                    {
                        App.ViewModel.LoadData();

                    }

                    NavigationService.Navigate(new Uri("/Home.xaml?Remove=1&Ping=true", UriKind.Relative));

                    //App.ViewModel.AppSettings.AuthSetting = App.ViewModel.AppSettings.AuthSetting;
                }
                else
                {
                    App.ViewModel.AppSettings.SessionExpireSetting = "1900-01-01T00:00:00";

                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }
        }




    }
}