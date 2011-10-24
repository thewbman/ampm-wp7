using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using Microsoft.Phone.BackgroundAudio;
using MyAudioPlaybackAgent;
/*
using Wintellect.Sterling;
using Wintellect.Sterling.Database;
using Wintellect.Sterling.Events;
using Wintellect.Sterling.Exceptions;
using Wintellect.Sterling.Indexes;
using Wintellect.Sterling.Keys;
using Wintellect.Sterling.Serialization;
 **/


namespace AmpM
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Hosts = new ObservableCollection<HostViewModel>();

            //this.Nowplaying = new List<AudioTrack>();
            this.Nowplaying = new List<DataItemViewModel>();

            this.Albums = new ObservableCollection<DataItemViewModel>();
            this.Artists = new ObservableCollection<DataItemViewModel>();
            this.Playlists = new ObservableCollection<DataItemViewModel>();

            this.AppSettings = new AppSettingsModel();

            this.Functions = new FunctionsViewModel();

            this.prefs = IsolatedStorageSettings.ApplicationSettings;

            this.Connected = false;

            this.Auth = "";

        }

        public ObservableCollection<HostViewModel> Hosts { get; set; }

        //public List<AudioTrack> Nowplaying { get; set; }
        public List<DataItemViewModel> Nowplaying { get; set; }

        public ObservableCollection<DataItemViewModel> Albums { get; set; }
        public ObservableCollection<DataItemViewModel> Artists { get; set; }
        public ObservableCollection<DataItemViewModel> Playlists { get; set; }

        public DataItemViewModel SelectedArtist { get; set; }

        public int AllSongs { get; set; }
        public int AllAlbums { get; set; }
        public int AllArtists { get; set; }
        public int AllGenres { get; set; }
        public int AllPlaylists { get; set; }
        public int AllVideos { get; set; }


        public AppSettingsModel AppSettings;

        public FunctionsViewModel Functions;

        private IsolatedStorageSettings prefs;

        //public SterlingEngine engine;
        //public ISterlingDatabaseInstance databaseInstance;

        public bool Connected;

        public string Auth;

        public bool IsDataLoaded { get; private set; }
        public bool IsAlbumsLoaded { get; private set; }
        public bool IsArtistsLoaded { get; private set; }
        public bool IsTagsLoaded { get; private set; }
        public bool IsPlaylistsLoaded { get; private set; }

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

        /// <summary>
        /// Creates and adds a few objects into the Items collection.
        /// </summary>
        public void LoadData()
        {

            //load backends
            var savedHostsList = StorageLoad<List<HostViewModel>>("Hosts");

            if (savedHostsList.Count < 1)
            {
                this.Hosts.Add(new HostViewModel() { Name = "local", Address = "http://192.168.1.105/ampache/", Username = "ampm", Password = "ampm" });
            }
            else
            {
                foreach (var e in savedHostsList) this.Hosts.Add(e);
            }

            //save hosts
            //this.saveHosts();


            this.IsDataLoaded = true;
        }
        public void LoadAlbums()
        {

            var savedAlbums = StorageLoad<List<DataItemViewModel>>("Albums");
            if (savedAlbums.Count < 1)
            {
                //
            }
            else
            {
                foreach (var e in savedAlbums)
                {
                    DataItemViewModel s = new DataItemViewModel();

                    s = e;

                    s.ArtUrl.Replace(s.Auth, this.Auth);
                    s.Auth = this.Auth;
                    
                    this.Albums.Add(s);
                }
            }
            //this.saveAlbums();

            this.IsAlbumsLoaded = true;
        }
        public void LoadArtists()
        {
            
            var savedArtists = StorageLoad<List<DataItemViewModel>>("Artists");
            if (savedArtists.Count < 1)
            {
                //
            }
            else
            {
                foreach (var e in savedArtists)
                {
                    DataItemViewModel s = new DataItemViewModel();

                    s = e;

                    s.Auth = this.Auth;

                    this.Albums.Add(s);
                }
            }
            //this.saveArtists();

            this.IsArtistsLoaded = true;
        }


        public void savePrefs()
        {
            AppSettings.Save();
        }


        public void saveHosts()
        {
            List<HostViewModel> hostsList = new List<HostViewModel>(this.Hosts);
            StorageSave<List<HostViewModel>>("Hosts", hostsList);
        }

        public void saveAlbums()
        {
            List<DataItemViewModel> albumsList = new List<DataItemViewModel>(this.Albums);
            StorageSave<List<DataItemViewModel>>("Albums", albumsList);
        }
        public void saveArtists()
        {
            List<DataItemViewModel> artistsList = new List<DataItemViewModel>(this.Artists);
            StorageSave<List<DataItemViewModel>>("Artists", artistsList);
        }

        public void addSongs(List<AudioTrack> inTracks)
        {
            foreach (AudioTrack t in inTracks)
            {
                //this.Nowplaying.Add(t);
            }

            return;
        }

        public void saveNowplaying()
        {
            //List<AudioTrack> nowplayingList = encodeTracks(this.Nowplaying);
            //StorageSave<List<AudioTrack>>("Nowplaying", nowplayingList);
            List<DataItemViewModel> nowplayingList = encodeDataItems(this.Nowplaying);
            StorageSave<List<DataItemViewModel>>("Nowplaying", nowplayingList);
        }

        public List<DataItemViewModel> getNowplaying()
        {

            //var savedNowplaying = decodeTracks(StorageLoad<List<AudioTrack>>("Nowplaying"));
            var savedNowplaying = decodeDataItems(StorageLoad<List<DataItemViewModel>>("Nowplaying"));

            this.Nowplaying.Clear();
            this.Nowplaying = savedNowplaying;

            return savedNowplaying;
        }

        private List<AudioTrack> encodeTracks(List<AudioTrack> inTracks)
        {
            List<AudioTrack> outTracks = new List<AudioTrack>();

            UTF8Encoding encoder = new UTF8Encoding();
            AudioTrack s;

            foreach (AudioTrack t in inTracks)
            {
                byte[] src = encoder.GetBytes(t.Source.ToString());
                byte[] tit = encoder.GetBytes(t.Title);
                byte[] art = encoder.GetBytes(t.Artist);
                byte[] alb = encoder.GetBytes(t.Album);
                byte[] aar = encoder.GetBytes(t.AlbumArt.ToString());

                s = new AudioTrack(new Uri(Convert.ToBase64String(src)), Convert.ToBase64String(tit), Convert.ToBase64String(art), Convert.ToBase64String(alb), new Uri(Convert.ToBase64String(aar)));

                outTracks.Add(s);
            }

            return outTracks;
        }

        private List<AudioTrack> decodeTracks(List<AudioTrack> inTracks)
        {
            List<AudioTrack> outTracks = new List<AudioTrack>();

            //UTF8Encoding encoder = new UTF8Encoding();
            AudioTrack s;

            foreach (AudioTrack t in inTracks)
            {
                //byte[] src = encoder.GetBytes(t.Source.ToString());
                //byte[] tit = encoder.GetBytes(t.Title);
                //byte[] art = encoder.GetBytes(t.Artist);
                //byte[] alb = encoder.GetBytes(t.Album);
                //byte[] aar = encoder.GetBytes(t.AlbumArt.ToString());

                s = new AudioTrack(new Uri(Convert.FromBase64String(t.Source.ToString()).ToString()), Convert.FromBase64String(t.Title).ToString(), Convert.FromBase64String(t.Artist).ToString(), Convert.FromBase64String(t.Album).ToString(), new Uri(Convert.FromBase64String(t.AlbumArt.ToString()).ToString()));

                outTracks.Add(s);
            }

            return outTracks;
        }

        private List<DataItemViewModel> encodeDataItems(List<DataItemViewModel> inItems)
        {
            List<DataItemViewModel> outItems = new List<DataItemViewModel>();

            UTF8Encoding encoder = new UTF8Encoding();
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

                s.SongName = Convert.ToBase64String(encoder.GetBytes(t.SongName));
                s.AlbumName = Convert.ToBase64String(encoder.GetBytes(t.AlbumName));
                s.ArtistName = Convert.ToBase64String(encoder.GetBytes(t.ArtistName));
                s.PlaylistName = Convert.ToBase64String(encoder.GetBytes(t.PlaylistName));

                s.SongTrack = t.SongTrack;
                s.SongTime = t.SongTime;
                s.SongUrl = Convert.ToBase64String(encoder.GetBytes(t.SongUrl));

                s.AlbumTracks = t.AlbumTracks;
                s.ArtistAlbums = t.ArtistAlbums;
                s.ArtistTracks = t.ArtistTracks;
                s.PlaylistItems = t.PlaylistItems;

                s.ArtUrl = Convert.ToBase64String(encoder.GetBytes(t.ArtUrl));


                outItems.Add(s);
            }

            return outItems;
        }

        private List<DataItemViewModel> decodeDataItems(List<DataItemViewModel> inItems)
        {
            List<DataItemViewModel> outItems = new List<DataItemViewModel>();

            UTF8Encoding encoder = new UTF8Encoding();
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
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


    }
}