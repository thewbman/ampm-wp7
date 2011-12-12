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
using System.Text.RegularExpressions;
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
            url += "auth=" + App.ViewModel.AppSettings.AuthSetting;
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

        public string FirstChar(string inString)
        {
            string a = inString.ToUpper().Substring(0, 1);
            
            if (Regex.IsMatch(a, "[0-9]"))
            {
                a = "#";
            }
            else if (Regex.IsMatch(a, "[A-Z]"))
            {
                //
            }
            else
            {
                a = "~";
            }

            return a;
        }

        public DataItemViewModel CloneItem(DataItemViewModel e)
        {
            DataItemViewModel s = new DataItemViewModel();

            s.AlbumId = e.AlbumId;
            s.AlbumName = e.AlbumName;
            s.AlbumTracks = e.AlbumTracks;
            s.ArtistAlbums = e.ArtistAlbums;
            s.ArtistId = e.ArtistId;
            s.ArtistName = e.ArtistName;
            s.ArtistTracks = e.ArtistTracks;
            s.ArtUrl = e.ArtUrl;
            s.Auth = e.Auth;
            s.ItemChar = e.ItemChar;
            s.ItemId = e.ItemId;
            s.ItemKey = e.ItemKey;
            s.PlaylistId = e.PlaylistId;
            s.PlaylistItems = e.PlaylistItems;
            s.PlaylistName = e.PlaylistName;
            s.SongId = e.SongId;
            s.SongName = e.SongName;
            s.SongTime = e.SongTime;
            s.SongTrack = e.SongTrack;
            s.SongUrl = e.SongUrl;
            s.Type = e.Type;
            s.Year = e.Year;

            return s;
        }
    }
}
