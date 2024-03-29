﻿using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Phone.BackgroundAudio;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Phone.Tasks;
//using Coding4Fun.Phone.Controls;

namespace MyAudioPlaybackAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        private static volatile bool _classInitialized;

        private static UTF8Encoding encoder = new UTF8Encoding();

        //public static int AppSettings.NowplayingIndexSetting;
        static AppSettingsModel AppSettings = new AppSettingsModel();

        public static List<AudioTrack> _playList = new List<AudioTrack>();

        
        public string LastfmUrl = "http://ws.audioscrobbler.com/2.0/";
        public string LastfmSecret = "cfaaaa9417b5ded38e6ed30434ca8be7";
        public string LastfmApikey = "ee337ff6dfdd301251d3e1c234d2ccba";

        private string NowplayingArtist = "";
        private string NowplayingTrack = "";
        private string ScrobbleArtist = "";
        private string ScrobbleTrack = "";

        /// <remarks>
        /// AudioPlayer instances can share the same process. 
        /// Static fields can be used to share state between AudioPlayer instances
        /// or to communicate with the Audio Streaming agent.
        /// </remarks>
        public AudioPlayer()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;

                //_playList = new List<AudioTrack>();

                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += AudioPlayer_UnhandledException;
                });
            }

           
        }

        public static void resetList()
        {

            //BackgroundAudioPlayer.Instance.Stop();

            //_playList.Clear();
            _playList = new List<AudioTrack>();

            //if (_playList == null) _playList = new List<AudioTrack>();
            /*
            foreach (AudioTrack t in _playList)
            {
                _playList.Remove(t);
            }
             */

            AppSettings.NowplayingIndexSetting = 0;


            saveCurrentIndex();

            return;
        }

        public static void setNowplaying(List<AudioTrack> inTracks)
        {
            StorageSave<List<AudioTrack>>("Nowplaying", inTracks);

            return;
        }

        public static List<AudioTrack> getCurrentList()
        {
            //var currentplayList = SongsToTracks(decodeDataItems(StorageLoad<List<DataItemViewModel>>("Nowplaying")));
            var currentplayList = SongsToTracks((StorageLoad<List<DataItemViewModel>>("Nowplaying")));
            return currentplayList;
        }

        private static List<DataItemViewModel> decodeDataItems(List<DataItemViewModel> inItems)
        {
            List<DataItemViewModel> outItems = new List<DataItemViewModel>();

            //UTF8Encoding encoder = new UTF8Encoding();
            DataItemViewModel s;

            foreach (DataItemViewModel t in inItems)
            {
                s = new DataItemViewModel();

                s.Type = t.Type;
                s.ItemKey = t.ItemKey;
                s.ItemId = t.ItemId;

                s.SongId = t.SongId;
                s.AlbumId = t.AlbumId;
                s.ArtistId = t.ArtistId;
                s.PlaylistId = t.PlaylistId;

                //s.SongName = encoder.GetString(Convert.FromBase64String(t.SongName));
                //s.AlbumName = encoder.GetString(Convert.FromBase64String(t.AlbumName));
                //s.ArtistName = encoder.GetString(Convert.FromBase64String(t.ArtistName));
                //s.PlaylistName = encoder.GetString(Convert.FromBase64String(t.PlaylistName)); 
                s.SongName = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(t.SongName), 0, Convert.FromBase64String(t.SongName).Length);
                s.AlbumName = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(t.AlbumName), 0, Convert.FromBase64String(t.AlbumName).Length);
                s.ArtistName = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(t.ArtistName), 0, Convert.FromBase64String(t.ArtistName).Length);
                s.PlaylistName = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(t.PlaylistName), 0, Convert.FromBase64String(t.PlaylistName).Length);
                
                s.SongTrack = t.SongTrack;
                s.SongTime = t.SongTime;
                //s.SongUrl = encoder.GetString(Convert.FromBase64String(t.SongUrl));
                s.SongUrl = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(t.SongUrl), 0, Convert.FromBase64String(t.SongUrl).Length);
                
                s.AlbumTracks = t.AlbumTracks;
                s.ArtistAlbums = t.ArtistAlbums;
                s.ArtistTracks = t.ArtistTracks;
                s.PlaylistItems = t.PlaylistItems;

                //s.ArtUrl = encoder.GetString(Convert.FromBase64String(t.ArtUrl));
                s.ArtUrl = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(t.ArtUrl), 0, Convert.FromBase64String(t.ArtUrl).Length);

                s.NowplayingIndex = t.NowplayingIndex;

                outItems.Add(s);
            }

            return outItems;
        }

        private static List<AudioTrack> SongsToTracks(List<DataItemViewModel> inSongs)
        {
            List<AudioTrack> outTracks = new List<AudioTrack>();

            AudioTrack t = new AudioTrack();

            int i = 1;

            foreach (DataItemViewModel s in inSongs)
            {
                t = new AudioTrack(new Uri(s.SongUrl, UriKind.Absolute), s.SongName, s.ArtistName, s.AlbumName, new Uri(s.ArtUrl, UriKind.Absolute), i.ToString(), EnabledPlayerControls.All);

                outTracks.Add(t);

                i++;
            }


            return outTracks;
        }


        public static void addSongs(List<AudioTrack> inTracks)
        {
            foreach (AudioTrack t in inTracks)
            {
                _playList.Add(t);
            }

            return;
        }

        public static void startPlaying(int inIndex)
        {
            try
            {
                if (inIndex == -1)
                {
                    BackgroundAudioPlayer.Instance.Play();
                }
                else
                {
                    _playList = getCurrentList();

                    AppSettings.NowplayingIndexSetting = inIndex + 0;
                    saveCurrentIndex(inIndex + 0);

                    //DataItemViewModel s = new DataItemViewModel();
                    AudioTrack t = new AudioTrack();
                    t = _playList[AppSettings.NowplayingIndexSetting];

                    int i = AppSettings.NowplayingIndexSetting;

                    //t.Tag = i.ToString();

                    //t = new AudioTrack(new Uri(s.SongUrl, UriKind.Absolute), s.SongName, s.ArtistName, s.AlbumName, new Uri(s.ArtUrl, UriKind.Absolute));

                    //BackgroundAudioPlayer.Instance.Close();
                    //BackgroundAudioPlayer.Instance.Track = null;

                    BackgroundAudioPlayer.Instance.Track = t;
                    //BackgroundAudioPlayer.Instance.Play();
                }

                saveCurrentIndex();
            }
            catch
            { }

            return;
        }

        public static void startPlayingTrack(AudioTrack inTrack)
        {
            if (inTrack == null)
            {
                //
            }
            else
            {

                inTrack.Tag = ((float)AppSettings.NowplayingIndexSetting).ToString();

                BackgroundAudioPlayer.Instance.Track = inTrack;
                //BackgroundAudioPlayer.Instance.Play();
            }

            saveCurrentIndex();

            return;
        }

        public static void stopAll()
        {
            //
        }

        /// Code to execute on Unhandled Exceptions
        private void AudioPlayer_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Called when the playstate changes, except for the Error state (see OnError)
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time the playstate changed</param>
        /// <param name="playState">The new playstate of the player</param>
        /// <remarks>
        /// Play State changes cannot be cancelled. They are raised even if the application
        /// caused the state change itself, assuming the application has opted-in to the callback.
        /// 
        /// Notable playstate events: 
        /// (a) TrackEnded: invoked when the player has no current track. The agent can set the next track.
        /// (b) TrackReady: an audio track has been set and it is now ready for playack.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnPlayStateChanged(BackgroundAudioPlayer player, AudioTrack track, PlayState playState)
        {
            try
            {
                switch (playState)
                {
                    case PlayState.TrackEnded:
                        //player.Track = GetPreviousTrack();
                        this.LastfmScrobble(track.Title, track.Artist);
                        PlayNextTrack(player);
                        break;
                    case PlayState.TrackReady:
                        this.LastfmNowplaying(track.Title, track.Artist);
                        player.Play();
                        //CheckPingAmpache();
                        AppSettings.StreamSessionExpireSetting = DateTime.Now.AddMinutes(90).ToString("s");
                        break;
                    case PlayState.Shutdown:
                        // TODO: Handle the shutdown state here (e.g. save state)
                        break;
                    case PlayState.Unknown:
                        break;
                    case PlayState.Stopped:
                        break;
                    case PlayState.Paused:
                        break;
                    case PlayState.Playing:
                        break;
                    case PlayState.BufferingStarted:
                        break;
                    case PlayState.BufferingStopped:
                        break;
                    case PlayState.Rewinding:
                        break;
                    case PlayState.FastForwarding:
                        break;
                }

                saveCurrentIndex();
            }
            catch
            { }

            this.Perform(() => NotifyComplete(), 1000);
        }


        /// <summary>
        /// Called when the user requests an action using application/system provided UI
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track playing at the time of the user action</param>
        /// <param name="action">The action the user has requested</param>
        /// <param name="param">The data associated with the requested action.
        /// In the current version this parameter is only for use with the Seek action,
        /// to indicate the requested position of an audio track</param>
        /// <remarks>
        /// User actions do not automatically make any changes in system state; the agent is responsible
        /// for carrying out the user actions if they are supported.
        /// 
        /// Call NotifyComplete() only once, after the agent request has been completed, including async callbacks.
        /// </remarks>
        protected override void OnUserAction(BackgroundAudioPlayer player, AudioTrack track, UserAction action, object param)
        {
            try
            {
                switch (action)
                {
                    case UserAction.Play:
                        if (player.PlayerState != PlayState.Playing)
                        {
                            player.Play();
                        }
                        break;
                    case UserAction.Stop:
                        player.Stop();
                        break;
                    case UserAction.Pause:
                        player.Pause();
                        break;
                    case UserAction.FastForward:
                        player.FastForward();
                        break;
                    case UserAction.Rewind:
                        player.Rewind();
                        break;
                    case UserAction.Seek:
                        player.Position = (TimeSpan)param;
                        break;
                    case UserAction.SkipNext:
                        player.Track = GetNextTrack();
                        break;
                    case UserAction.SkipPrevious:
                        AudioTrack previousTrack = GetPreviousTrack();
                        if (previousTrack != null)
                        {
                            player.Track = previousTrack;
                        }
                        break;
                }

                saveCurrentIndex();

            }
            catch
            { }

            this.Perform(() => NotifyComplete(), 1000);
        }


        /// <summary>
        /// Implements the logic to get the next AudioTrack instance.
        /// In a playlist, the source can be from a file, a web request, etc.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if the playback is completed</returns>
        private static AudioTrack GetNextTrack()
        {
            // TODO: add logic to get the next audio track

            AudioTrack track = null;

            _playList = getCurrentList();
            int i;

            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                if (int.TryParse(BackgroundAudioPlayer.Instance.Track.Tag, out i))
                {
                    AppSettings.NowplayingIndexSetting = i;
                }
                else
                {
                    AppSettings.NowplayingIndexSetting = AppSettings.NowplayingIndexSetting + 1;
                }
            }
            else
            {
                AppSettings.NowplayingIndexSetting = AppSettings.NowplayingIndexSetting + 1;
            }


            if (_playList.Count <= AppSettings.NowplayingIndexSetting)
            {
                //AppSettings.NowplayingIndexSetting = 0;
            }
            else
            {
                track = _playList[AppSettings.NowplayingIndexSetting];
                //track.Tag = ((float)AppSettings.NowplayingIndexSetting).ToString();
            }

            saveCurrentIndex();

            return track;
        }

        /// <summary>
        /// Implements the logic to get the previous AudioTrack instance.
        /// </summary>
        /// <remarks>
        /// The AudioTrack URI determines the source, which can be:
        /// (a) Isolated-storage file (Relative URI, represents path in the isolated storage)
        /// (b) HTTP URL (absolute URI)
        /// (c) MediaStreamSource (null)
        /// </remarks>
        /// <returns>an instance of AudioTrack, or null if previous track is not allowed</returns>
        private static AudioTrack GetPreviousTrack()
        {
            // TODO: add logic to get the previous audio track

            AudioTrack track = null;

            _playList = getCurrentList();
            int i;

            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                if (int.TryParse(BackgroundAudioPlayer.Instance.Track.Tag, out i))
                {
                    AppSettings.NowplayingIndexSetting = i-2;
                }
                else
                {
                    AppSettings.NowplayingIndexSetting = AppSettings.NowplayingIndexSetting - 1;
                }
            }
            else
            {
                AppSettings.NowplayingIndexSetting = AppSettings.NowplayingIndexSetting - 1;
            }


            if (0 > AppSettings.NowplayingIndexSetting)
            {
                AppSettings.NowplayingIndexSetting = _playList.Count - 1;
            }

            track = _playList[AppSettings.NowplayingIndexSetting];
            //track.Tag = ((float)AppSettings.NowplayingIndexSetting).ToString();

            saveCurrentIndex();

            return track;
        }

        /// <summary>
        /// Called whenever there is an error with playback, such as an AudioTrack not downloading correctly
        /// </summary>
        /// <param name="player">The BackgroundAudioPlayer</param>
        /// <param name="track">The track that had the error</param>
        /// <param name="error">The error that occured</param>
        /// <param name="isFatal">If true, playback cannot continue and playback of the track will stop</param>
        /// <remarks>
        /// This method is not guaranteed to be called in all cases. For example, if the background agent 
        /// itself has an unhandled exception, it won't get called back to handle its own errors.
        /// </remarks>
        protected override void OnError(BackgroundAudioPlayer player, AudioTrack track, Exception error, bool isFatal)
        {
            if (isFatal)
            {
                Abort();
            }
            else
            {
                this.Perform(() => NotifyComplete(), 1000);
            }

        }

        /// <summary>
        /// Called when the agent request is getting cancelled
        /// </summary>
        /// <remarks>
        /// Once the request is Cancelled, the agent gets 5 seconds to finish its work,
        /// by calling NotifyComplete()/Abort().
        /// </remarks>
        protected override void OnCancel()
        {

        }


        private static void PlayNextTrack(BackgroundAudioPlayer player)
        {
            _playList = getCurrentList();
            int i;

            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                if (int.TryParse(BackgroundAudioPlayer.Instance.Track.Tag, out i))
                {
                    AppSettings.NowplayingIndexSetting = i;
                }
                else
                {
                    AppSettings.NowplayingIndexSetting = AppSettings.NowplayingIndexSetting + 1;
                }
            }
            else
            {
                AppSettings.NowplayingIndexSetting = AppSettings.NowplayingIndexSetting + 1;
            }

            if (_playList.Count <= AppSettings.NowplayingIndexSetting)
            {
                //AppSettings.NowplayingIndexSetting = 0;
                player.Track = null;
            }
            else
            {
                PlayTrack(player);
            }
        }

        private static void PlayPreviousTrack(BackgroundAudioPlayer player)
        {
            _playList = getCurrentList();
            int i;


            if (BackgroundAudioPlayer.Instance.Track != null)
            {
                if (int.TryParse(BackgroundAudioPlayer.Instance.Track.Tag, out i))
                {
                    AppSettings.NowplayingIndexSetting = i-2;
                }
                else
                {
                    AppSettings.NowplayingIndexSetting = AppSettings.NowplayingIndexSetting - 1;
                }
            }
            else
            {
                AppSettings.NowplayingIndexSetting = AppSettings.NowplayingIndexSetting - 1;
            }


            if (0 > AppSettings.NowplayingIndexSetting)
            {
                AppSettings.NowplayingIndexSetting = _playList.Count - 1;
            }

            PlayTrack(player);
        }

        private static void PlayTrack(BackgroundAudioPlayer player)
        {
            _playList = getCurrentList();

            // Sets the track to play. When the TrackReady state is received, 
            // playback begins from the OnPlayStateChanged handler.
            player.Track = _playList[AppSettings.NowplayingIndexSetting];

            saveCurrentIndex();
        }


        private void CheckPingAmpache()
        {
            if(AppSettings.SessionExpireSetting == DateTime.Parse("1900-01-01").ToString("s"))
            {
                PingAmpache(getRandom());
            }
            else 
            {
                TimeSpan t = new TimeSpan();

                t = (DateTime.Parse(AppSettings.SessionExpireSetting) - DateTime.Now);

                //15 minutes
                if (t.TotalSeconds < (15 * 60))
                {
                    PingAmpache(getRandom());
                }
            }
        }

        private static void PingAmpache(string inRandom)
        {
            //HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(AppSettings.HostAddressSetting + "/server/xml.server.php?action=ping&auth="+AppSettings.AuthSetting+"&SessionExpireSetting="+AppSettings.SessionExpireSetting+"&random="+inRandom, UriKind.Absolute));
            //webRequest.BeginGetResponse(new AsyncCallback(PingCallback), webRequest);
        }
        private static void PingCallback(IAsyncResult asynchronousResult)
        {

            string resultString;

            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

            HttpWebResponse response;

            //try
            //{
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            /*
                //}
            //catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //MessageBox.Show("Failed to get ping response: " + ex.ToString(), "Error", MessageBoxButton.OK);
                    BannerMessage("Failed to get ping response: " + ex.ToString());
                });

                return;
            }
             */

            using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
            {
                resultString = streamReader1.ReadToEnd();
            }

            response.GetResponseStream().Close();
            response.Close();

            try
            {

                XDocument xdoc = XDocument.Parse(resultString, LoadOptions.None);

                string s = xdoc.Element("root").Element("session_expire").FirstNode.ToString().Replace("<![CDATA[", "").Replace("]]>", "").Trim();

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    AppSettings.SessionExpireSetting = DateTime.Parse(s).ToString("s");
                });

            }
            catch (Exception ex)
            {
                
                //Deployment.Current.Dispatcher.BeginInvoke(() =>
                //{
                    AppSettings.SessionExpireSetting = DateTime.Parse("1900-01-01").ToString("s");

                    //MessageBox.Show("Failed to parse ping response: " + ex.ToString(), "Error", MessageBoxButton.OK);
                    //BannerMessage("Failed to parse ping response: " + ex.ToString());
                //});

            }

        }


        public static T StorageLoad<T>(string name) where T : class, new()
        {
            T loadedObject = null;
            using (IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForApplication())
            using (IsolatedStorageFileStream storageFileStream = new IsolatedStorageFileStream(name, System.IO.FileMode.OpenOrCreate, storageFile))
            {
                if (storageFileStream.Length > 0)
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    loadedObject = serializer.ReadObject(storageFileStream) as T;
                }
                if (loadedObject == null)
                {
                    loadedObject = new T();
                }
            }

            return loadedObject;
        }
        public static void StorageSave<T>(string name, T objectToSave)
        {
            using (IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForApplication())
            using (IsolatedStorageFileStream storageFileStream = new IsolatedStorageFileStream(name, System.IO.FileMode.Create, storageFile))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(storageFileStream, objectToSave);
            }
        }
        public static void StorageDelete(string name)
        {
            using (IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                storageFile.Remove();
            }
        }

        private static void saveCurrentIndex()
        {
            //AppSettingsModel AppSettings = new AppSettingsModel();
            AppSettings.NowplayingIndexSetting = AppSettings.NowplayingIndexSetting + 0;

            return;
        }

        private static void saveCurrentIndex(int inIndex)
        {
            //AppSettings.NowplayingIndexSetting = inIndex;
            
            //AppSettingsModel AppSettings = new AppSettingsModel();
            AppSettings.NowplayingIndexSetting = AppSettings.NowplayingIndexSetting+0;

            return;
        }

        private static string myRandom()
        {
            Random random = new Random();

            return random.Next().ToString();
        }
        private string getRandom()
        {
            return myRandom();
        }

        private void LastfmNowplaying(string inTitle, string inArtist)
        {
            this.NowplayingArtist = inArtist;
            this.NowplayingTrack = inTitle;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(this.LastfmUrl));
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            // Start the request
            webRequest.BeginGetRequestStream(new AsyncCallback(LastfmNowplayingStreamCallback), webRequest);

        }
        private void LastfmNowplayingStreamCallback(IAsyncResult asynchronousResult)
        {

            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            // End the stream request operation
            System.IO.Stream postStream = webRequest.EndGetRequestStream(asynchronousResult);

            
            string method = "track.updateNowPlaying";

            string api_sig = MD5Core.GetHashString("api_key" + this.LastfmApikey + "artist" + this.NowplayingArtist + "method" + method + "sk" + AppSettings.LastfmKeySetting + "track" + this.NowplayingTrack + this.LastfmSecret).ToLower();


            string postData = "";
            postData += "api_key=" + this.LastfmApikey;
            postData += "&artist=" + this.NowplayingArtist;
            postData += "&method=" + method;
            postData += "&track=" + this.NowplayingTrack;
            postData += "&sk=" + AppSettings.LastfmKeySetting;
            postData += "&api_sig=" + api_sig.ToLower();

            //int i = int.Parse("asdf");

            if (AppSettings.LastfmKeySetting.Length > 0)
            {
                /*
                HttpWebRequest webRequest = WebRequest.CreateHttp(new Uri(fullUrl));
                webRequest.Method = "POST";
                webRequest.BeginGetResponse(new AsyncCallback(LastfmNowplayingCallback), webRequest);
                */

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                // Add the post data to the web request
                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Close();

                // Start the web request
                webRequest.BeginGetResponse(new AsyncCallback(LastfmNowplayingCallback), webRequest);

            }
        }
        private void LastfmNowplayingCallback(IAsyncResult asynchronousResult)
        {

            //int i = int.Parse("asdf");

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse response;

                // End the get response operation
                response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                System.IO.Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse);
                var Response = streamReader.ReadToEnd();
                streamResponse.Close();
                streamReader.Close();
                response.Close();


                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //MessageBox.Show("Success: " + Response, "SUCCESS", MessageBoxButton.OK);

                });

            }
            catch (WebException e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //MessageBox.Show("Error: " + e.ToString(), "ERROR", MessageBoxButton.OK);
                });
            }
        }
        private void LastfmNowplayingStreamCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result);
        }

        private void LastfmScrobble(string inTitle, string inArtist)
        {

            this.ScrobbleArtist = inArtist;
            this.ScrobbleTrack = inTitle;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(this.LastfmUrl));
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            // Start the request
            webRequest.BeginGetRequestStream(new AsyncCallback(LastfmScrobbleStreamCallback), webRequest);

        }
        private void LastfmScrobbleStreamCallback(IAsyncResult asynchronousResult)
        {

            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            // End the stream request operation
            System.IO.Stream postStream = webRequest.EndGetRequestStream(asynchronousResult);

            DateTime date = DateTime.Now.ToUniversalTime();
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan span = (date - epoch);
            string timestamp = span.TotalSeconds.ToString().Substring(0,10);        //lazy trim to just integer

            string method = "track.scrobble";

            string api_sig = MD5Core.GetHashString("api_key" + this.LastfmApikey + "artist" + this.ScrobbleArtist + "method" + method + "sk" + AppSettings.LastfmKeySetting + "timestamp" + timestamp + "track" + this.ScrobbleTrack + this.LastfmSecret).ToLower();


            string postData = "";
            postData += "api_key=" + this.LastfmApikey;
            postData += "&artist=" + this.ScrobbleArtist;
            postData += "&method=" + method;
            postData += "&timestamp=" + timestamp;
            postData += "&track=" + this.ScrobbleTrack;
            postData += "&sk=" + AppSettings.LastfmKeySetting;
            postData += "&api_sig=" + api_sig.ToLower();



            //int i = int.Parse("asdf");

            if (AppSettings.LastfmKeySetting.Length > 0)
            {
                /*
                HttpWebRequest webRequest = WebRequest.CreateHttp(new Uri(fullUrl));
                webRequest.Method = "POST";
                webRequest.BeginGetResponse(new AsyncCallback(LastfmScrobbleCallback), webRequest);
                */

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                // Add the post data to the web request
                postStream.Write(byteArray, 0, byteArray.Length);
                postStream.Close();

                // Start the web request
                webRequest.BeginGetResponse(new AsyncCallback(LastfmScrobbleCallback), webRequest);
            }
        }
        private void LastfmScrobbleCallback(IAsyncResult asynchronousResult)
        {

            string a = this.ScrobbleTrack;
            string b = this.ScrobbleArtist;


                HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse response;

                // End the get response operation
                response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
                System.IO.Stream streamResponse = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(streamResponse);
                var Response = streamReader.ReadToEnd();
                streamResponse.Close();
                streamReader.Close();
                response.Close();


                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //MessageBox.Show("Success: " + Response, "SUCCESS", MessageBoxButton.OK);

                });

                try
                {
            }
            catch (WebException e)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //MessageBox.Show("Error: " + e.ToString(), "ERROR", MessageBoxButton.OK);
                    //AppSettings.LastfmKeySetting = "";
                });
            }
        }
        private void LastfmScrobbleStreamCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            //MessageBox.Show(e.Result);
        } 


        private static void BannerMessage(string inMessage)
        {
            /*
             * ToastPrompt toast = new ToastPrompt();

            toast.Title = inMessage;
            toast.TextOrientation = System.Windows.Controls.Orientation.Horizontal;

            toast.Show();
             * */
        }


        private void Perform(Action myMethod, int delayInMilliseconds)
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (s, e) => Thread.Sleep(delayInMilliseconds);

            worker.RunWorkerCompleted += (s, e) => myMethod.Invoke();

            worker.RunWorkerAsync();
        }
    }
}
