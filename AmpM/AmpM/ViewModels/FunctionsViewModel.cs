using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Phone.BackgroundAudio;
using System.Xml.Linq;
using System.Security.Cryptography;
using MyAudioPlaybackAgent;

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

        public List<AudioTrack> SongsToTracks(ObservableCollection<DataItemViewModel> inSongs)
        {
            List<AudioTrack> outTracks = new List<AudioTrack>();

            AudioTrack t = new AudioTrack();

            foreach(DataItemViewModel s in inSongs)
            {
                t = new AudioTrack(new Uri(s.SongUrl, UriKind.Absolute), s.SongName, s.ArtistName, s.AlbumName, new Uri(s.ArtUrl, UriKind.Absolute));

                //t.Album = s.AlbumName;
                //t.AlbumArt = new Uri(s.ArtUrl);
                //t.Artist = s.ArtistName;
                //t.Source = new Uri(s.SongUrl);
                //t.Tag = s.SongId.ToString();
                //t.Title = s.SongName;

                outTracks.Add(t);
            }


            return outTracks;
        }

    }
}
