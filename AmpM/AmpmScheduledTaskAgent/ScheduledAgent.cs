using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using Microsoft.Phone.Shell;

namespace AmpmScheduledTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;


        private static AppSettingsModel2 AppSettings = new AppSettingsModel2();

        private PeriodicTask periodicTask;
        private string periodicTaskName = "KeepAliveTask";

        public string randText()
        {
            //return random.Next().ToString();
            return myRandom();
        }
        private static string myRandom()
        {
            Random random = new Random();

            return random.Next().ToString();
        }

        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            /*
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                ScheduledActionService.Remove(periodicTaskName);
            }
             */

            /*
            ShellToast s = new ShellToast();

            s.Title = "AmpM periodic task";
            s.Content = AppSettings.SessionExpireSetting;

            s.Show();
            */

            if (!AppSettings.KeepAliveSetting)
            {
                //user has disabled keep alive

                NotifyComplete();
            }
            else if (AppSettings.SessionExpireSetting == "1900-01-01T00:00:00")
            {
                //last response failed, so we lost session info

                NotifyComplete();
            }
            else
            {
                DateTime d = DateTime.Parse(AppSettings.SessionExpireSetting);
                DateTime n = DateTime.Now;

                TimeSpan t = d - n;

                if (t.TotalSeconds < (45 * 60))
                {

                    string url = "";

                    url += AppSettings.HostAddressSetting;
                    url += "/server/xml.server.php?";
                    url += "auth=" + AppSettings.AuthSetting ;
                    url += "&action=" + "ping";
                    url += "&rand=" + randText();


                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    webRequest.BeginGetResponse(new AsyncCallback(PingCallback), webRequest);
                }

            }

        }
        private void PingCallback(IAsyncResult asynchronousResult)
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

                DateTime d = DateTime.Parse(xdoc.Element("root").Element("session_expire").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim()) ;

                AppSettings.SessionExpireSetting = d.ToString("s");
            
            }
            catch (Exception ex)
            {
                //MessageBox.Show("ping xml error: " + ex.ToString());

                AppSettings.SessionExpireSetting = "1900-01-01T00:00:00";
            }
             


            NotifyComplete();
        }

    }
}