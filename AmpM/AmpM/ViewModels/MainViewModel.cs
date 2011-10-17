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


namespace AmpM
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Hosts = new ObservableCollection<HostViewModel>();

            this.Nowplaying = new List<AudioTrack>();

            this.Playlists = new ObservableCollection<DataItemViewModel>();

            this.AppSettings = new AppSettingsModel();

            this.Functions = new FunctionsViewModel();

            this.prefs = IsolatedStorageSettings.ApplicationSettings;

            this.Connected = false;

            this.Auth = "";

        }

        /// <summary>
        /// A collection of objects.
        /// </summary>
        public ObservableCollection<HostViewModel> Hosts { get; set; }

        public List<AudioTrack> Nowplaying { get; set; }

        public ObservableCollection<DataItemViewModel> Playlists { get; set; }

        public AppSettingsModel AppSettings;

        public FunctionsViewModel Functions;

        private IsolatedStorageSettings prefs;

        public bool Connected;

        public string Auth;

        public bool IsDataLoaded { get; private set; }

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
            this.saveHosts();

            //load prefs
            //appSettings.Save();

            this.IsDataLoaded = true;
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