using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Phone.BackgroundAudio;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

namespace MyAudioPlaybackAgent
{
    public class AudioPlayer : AudioPlayerAgent
    {
        private static volatile bool _classInitialized;

        private static UTF8Encoding encoder = new UTF8Encoding();

        static int currentTrackNumber;

        public static List<AudioTrack> _playList = new List<AudioTrack>();

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

            //currentTrackNumber = 0;

            return;
        }

        public static void setNowplaying(List<AudioTrack> inTracks)
        {
            StorageSave<List<AudioTrack>>("Nowplaying", inTracks);

            return;
        }

        public static List<AudioTrack> getCurrentList()
        {
            var currentplayList = SongsToTracks(decodeDataItems(StorageLoad<List<DataItemViewModel>>("Nowplaying")));
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
                


                outItems.Add(s);
            }

            return outItems;
        }

        private static List<AudioTrack> SongsToTracks(List<DataItemViewModel> inSongs)
        {
            List<AudioTrack> outTracks = new List<AudioTrack>();

            AudioTrack t = new AudioTrack();

            foreach (DataItemViewModel s in inSongs)
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
            if (inIndex == -1)
            {
                BackgroundAudioPlayer.Instance.Play();
            }
            else
            {
                _playList = getCurrentList();

                currentTrackNumber = inIndex+0;
                saveCurrentIndex(inIndex + 0);

                //DataItemViewModel s = new DataItemViewModel();
                AudioTrack t = new AudioTrack();
                t = _playList[currentTrackNumber];

                //t = new AudioTrack(new Uri(s.SongUrl, UriKind.Absolute), s.SongName, s.ArtistName, s.AlbumName, new Uri(s.ArtUrl, UriKind.Absolute));

                BackgroundAudioPlayer.Instance.Track = t;
                //BackgroundAudioPlayer.Instance.Play();
            }

            //saveCurrentIndex();

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
                BackgroundAudioPlayer.Instance.Track = inTrack;
                //BackgroundAudioPlayer.Instance.Play();
            }

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
            switch (playState)
            {
                case PlayState.TrackEnded:
                    //player.Track = GetPreviousTrack();
                    PlayNextTrack(player);
                    break;
                case PlayState.TrackReady:
                    player.Play();
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

            NotifyComplete();
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

            NotifyComplete();
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
        private AudioTrack GetNextTrack()
        {
            // TODO: add logic to get the next audio track

            AudioTrack track = null;

            _playList = getCurrentList();

            if (++currentTrackNumber >= _playList.Count)
            {
                currentTrackNumber = 0;
            }

            track = _playList[currentTrackNumber];

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
        private AudioTrack GetPreviousTrack()
        {
            // TODO: add logic to get the previous audio track

            AudioTrack track = null;

            _playList = getCurrentList();

            if (--currentTrackNumber < 0)
            {
                currentTrackNumber = _playList.Count - 1;
            }
            
            track = _playList[currentTrackNumber];

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
                NotifyComplete();
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


        private void PlayNextTrack(BackgroundAudioPlayer player)
        {
            _playList = getCurrentList();

            if (++currentTrackNumber >= _playList.Count)
            {
                currentTrackNumber = 0;
            }

            PlayTrack(player);
        }

        private void PlayPreviousTrack(BackgroundAudioPlayer player)
        {
            _playList = getCurrentList();

            if (--currentTrackNumber < 0)
            {
                currentTrackNumber = _playList.Count - 1;
            }

            PlayTrack(player);
        }

        private void PlayTrack(BackgroundAudioPlayer player)
        {
            _playList = getCurrentList();

            // Sets the track to play. When the TrackReady state is received, 
            // playback begins from the OnPlayStateChanged handler.
            player.Track = _playList[currentTrackNumber];
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
            AppSettingsModel AppSettings = new AppSettingsModel();
            AppSettings.NowplayingIndexSetting = currentTrackNumber;

            return;
        }

        private static void saveCurrentIndex(int inIndex)
        {
            currentTrackNumber = inIndex;
            
            AppSettingsModel AppSettings = new AppSettingsModel();
            AppSettings.NowplayingIndexSetting = currentTrackNumber;

            return;
        }
    }
}
