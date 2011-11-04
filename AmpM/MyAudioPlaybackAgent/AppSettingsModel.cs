using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Collections.Generic;

namespace MyAudioPlaybackAgent
{
    public class AppSettingsModel
    {
        // Our isolated storage settings
        IsolatedStorageSettings settings;

        // The isolated storage key names of our settings
        const string FirstRunName = "FirstRun";

        const string HostIndexName = "HostIndex";
        const string HostAddressName = "HostAddress";

        const string NowplayingIndexName = "NowplayingIndex";
        const string AuthName = "Auth";
        const string SessionExpireName = "SessionExpire";

        const string DefaultPlayAllName = "DefaultPlayAll";
        const string DefaultPlayShuffleName = "DefaultPlayShuffle";
        const string DefaultPlayAddName = "DefaultPlayAdd";





        // The default value of our settings
        const bool FirstRunDefault = true;

        const int HostIndexDefault = 0;
        const string HostAddressDefault = "http://google.com";

        const int NowplayingIndexDefault = 0;
        const string AuthDefault = "asdf";
        const string SessionExpireDefault = "1900-01-01T00:00:00";

        const bool DefaultPlayAllDefault = true;
        const bool DefaultPlayShuffleDefault = false;
        const bool DefaultPlayAddDefault = true;




        /// <summary>
        /// Constructor that gets the application settings.
        /// </summary>
        public AppSettingsModel()
        {
            // Get the settings for this application.
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            settings.Save();
        }



        public bool FirstRunSetting
        {
            get { return GetValueOrDefault<bool>(FirstRunName, FirstRunDefault); }
            set { if (AddOrUpdateValue(FirstRunName, value)) { Save(); } }
        }

        public int HostIndexSetting
        {
            get { return GetValueOrDefault<int>(HostIndexName, HostIndexDefault); }
            set { if (AddOrUpdateValue(HostIndexName, value)) { Save(); } }
        }
        public string HostAddressSetting
        {
            get { return GetValueOrDefault<string>(HostAddressName, HostAddressDefault); }
            set { if (AddOrUpdateValue(HostAddressName, value)) { Save(); } }
        }

        public int NowplayingIndexSetting
        {
            get { return GetValueOrDefault<int>(NowplayingIndexName, NowplayingIndexDefault); }
            set { if (AddOrUpdateValue(NowplayingIndexName, value)) { Save(); } }
        }
        public string AuthSetting
        {
            get { return GetValueOrDefault<string>(AuthName, AuthDefault); }
            set { if (AddOrUpdateValue(AuthName, value)) { Save(); } }
        }
        public string SessionExpireSetting
        {
            get { return GetValueOrDefault<string>(SessionExpireName, SessionExpireDefault); }
            set { if (AddOrUpdateValue(SessionExpireName, value)) { Save(); } }
        }

        public bool DefaultPlayAllSetting
        {
            get { return GetValueOrDefault<bool>(DefaultPlayAllName, DefaultPlayAllDefault); }
            set { if (AddOrUpdateValue(DefaultPlayAllName, value)) { Save(); } }
        }
        public bool DefaultPlayShuffleSetting
        {
            get { return GetValueOrDefault<bool>(DefaultPlayShuffleName, DefaultPlayShuffleDefault); }
            set { if (AddOrUpdateValue(DefaultPlayShuffleName, value)) { Save(); } }
        }
        public bool DefaultPlayAddSetting
        {
            get { return GetValueOrDefault<bool>(DefaultPlayAddName, DefaultPlayAddDefault); }
            set { if (AddOrUpdateValue(DefaultPlayAddName, value)) { Save(); } }
        }


    }
}