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
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace AmpM
{
    public class FunctionsViewModel
    {

        public string GetAmpacheConnectUrl()
        {
            string url = "";

            SHA256 mySha = new SHA256Managed();
            UTF8Encoding encoder = new UTF8Encoding();

            TimeSpan t = (DateTime.Now - new DateTime(1970, 1, 1));
            Int64 timestamp = (Int64)t.TotalSeconds - (Int64)TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalSeconds;

            byte[] passwordBuffer = encoder.GetBytes(App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Password);
            string passwordHash = BitConverter.ToString(mySha.ComputeHash(passwordBuffer)).Replace("-", "").ToLower();

            byte[] authBuffer = encoder.GetBytes(timestamp.ToString() + passwordHash);
            string authHash = BitConverter.ToString(mySha.ComputeHash(authBuffer)).Replace("-", "").ToLower();

            url += App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Address;
            url += "/server/xml.server.php?action=handshake";
            url += "&auth="+authHash;
            url += "&timestamp=" + timestamp.ToString();
            url += "&version=350001";
            url += "&user=" + App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Username;


            return url;
        }

        public string GetAmpacheDataUrl(string inAction, string inParams)
        {
            string url = "";

            url += App.ViewModel.Hosts[App.ViewModel.AppSettings.HostIndexSetting].Address;
            url += "/server/xml.server.php?";
            url += "auth=" + App.ViewModel.Auth;
            url += "&action="+inAction;
            url += inParams;
            url += "&rand=" + App.ViewModel.randText();

            return url;
        }

    }
}
