﻿using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Collections.Generic;

namespace MyAudioPlaybackAgent
{
    public class AppSettingsModel
    {
        // Our isolated storage settings
        IsolatedStorageSettings settings;


        public AppSettingsModel()
        {
            settings = IsolatedStorageSettings.ApplicationSettings;
        }

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

        public void Save()
        {
            try
            {
                settings.Save();
            }
            catch
            { }
        }




        // The isolated storage key names of our settings
        const string FirstRunName = "FirstRun";
        const string FirstNowplayingName = "FirstNowplaying";
        const string AppStartsName = "AppStarts";
        const string ReviewedName = "Reviewed";

        const string HostIndexName = "HostIndex";
        const string HostAddressName = "HostAddress";

        const string NowplayingIndexName = "NowplayingIndex";
        const string AuthName = "Auth";
        const string SessionExpireName = "SessionExpire";
        const string StreamSessionExpireName = "StreamSessionExpire";

        const string DefaultPlayAllName = "DefaultPlayAll";
        const string DefaultPlayShuffleName = "DefaultPlayShuffle";
        const string DefaultPlayAddName = "DefaultPlayAdd";

        const string KeepAliveName = "KeepAlive";

        const string AlbumsCountName = "AlbumsCount";
        const string ArtistsCountName = "ArtistsCount";
        const string PlaylistsCountName = "PlaylistsCount";
        const string SongsCountName = "SongsCount";
        const string VideosCountName = "VideosCount";

        const string LastfmName = "Lastfm";
        const string LastfmUsernameName = "LastfmUsername";
        const string LastfmPasswordName = "LastfmPassword";
        const string LastfmKeyName = "LastfmKey";
        const string LastfmImagesName = "LastfmImages";





        // The default value of our settings
        const bool FirstRunDefault = true;
        const bool FirstNowplayingDefault = true;
        const int AppStartsDefault = 0;
        const bool ReviewedDefault = false;
        
        const int HostIndexDefault = 0;
        const string HostAddressDefault = "http://google.com";

        const int NowplayingIndexDefault = 0;
        const string AuthDefault = "asdf";
        const string SessionExpireDefault = "1900-01-01T00:00:00";
        const string StreamSessionExpireDefault = "1900-01-01T00:00:00";

        const bool DefaultPlayAllDefault = true;
        const bool DefaultPlayShuffleDefault = false;
        const bool DefaultPlayAddDefault = true;

        const bool KeepAliveDefault = true;

        const int AlbumsCountDefault = 0;
        const int ArtistsCountDefault = 0;
        const int PlaylistsCountDefault = 0;
        const int SongsCountDefault = 0;
        const int VideosCountDefault = 0;

        const bool LastfmDefault = false;
        const string LastfmUsernameDefault = "";
        const string LastfmPasswordDefault = "";
        const string LastfmKeyDefault = "";
        const bool LastfmImagesDefault = true;




        public bool FirstRunSetting
        {
            get { return GetValueOrDefault<bool>(FirstRunName, FirstRunDefault); }
            set { if (AddOrUpdateValue(FirstRunName, value)) { Save(); } }
        }
        public bool FirstNowplayingSetting
        {
            get { return GetValueOrDefault<bool>(FirstNowplayingName, FirstNowplayingDefault); }
            set { if (AddOrUpdateValue(FirstNowplayingName, value)) { Save(); } }
        }
        public int AppStartsSetting
        {
            get { return GetValueOrDefault<int>(AppStartsName, AppStartsDefault); }
            set { if (AddOrUpdateValue(AppStartsName, value)) { Save(); } }
        }
        public bool ReviewedSetting
        {
            get { return GetValueOrDefault<bool>(ReviewedName, ReviewedDefault); }
            set { if (AddOrUpdateValue(ReviewedName, value)) { Save(); } }
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
        public string StreamSessionExpireSetting
        {
            get { return GetValueOrDefault<string>(StreamSessionExpireName, StreamSessionExpireDefault); }
            set { if (AddOrUpdateValue(StreamSessionExpireName, value)) { Save(); } }
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

        public bool KeepAliveSetting
        {
            get { return GetValueOrDefault<bool>(KeepAliveName, KeepAliveDefault); }
            set { if (AddOrUpdateValue(KeepAliveName, value)) { Save(); } }
        }

        public int AlbumsCountSetting
        {
            get { return GetValueOrDefault<int>(AlbumsCountName, AlbumsCountDefault); }
            set { if (AddOrUpdateValue(AlbumsCountName, value)) { Save(); } }
        }
        public int ArtistsCountSetting
        {
            get { return GetValueOrDefault<int>(ArtistsCountName, ArtistsCountDefault); }
            set { if (AddOrUpdateValue(ArtistsCountName, value)) { Save(); } }
        }
        public int PlaylistsCountSetting
        {
            get { return GetValueOrDefault<int>(PlaylistsCountName, PlaylistsCountDefault); }
            set { if (AddOrUpdateValue(PlaylistsCountName, value)) { Save(); } }
        }
        public int SongsCountSetting
        {
            get { return GetValueOrDefault<int>(SongsCountName, SongsCountDefault); }
            set { if (AddOrUpdateValue(SongsCountName, value)) { Save(); } }
        }
        public int VideosCountSetting
        {
            get { return GetValueOrDefault<int>(VideosCountName, VideosCountDefault); }
            set { if (AddOrUpdateValue(VideosCountName, value)) { Save(); } }
        }

        public bool LastfmSetting
        {
            get { return GetValueOrDefault<bool>(LastfmName, LastfmDefault); }
            set { if (AddOrUpdateValue(LastfmName, value)) { Save(); } }
        }
        public string LastfmUsernameSetting
        {
            get { return GetValueOrDefault<string>(LastfmUsernameName, LastfmUsernameDefault); }
            set { if (AddOrUpdateValue(LastfmUsernameName, value)) { Save(); } }
        }
        public string LastfmPasswordSetting
        {
            get { return GetValueOrDefault<string>(LastfmPasswordName, LastfmPasswordDefault); }
            set { if (AddOrUpdateValue(LastfmPasswordName, value)) { Save(); } }
        }
        public string LastfmKeySetting
        {
            get { return GetValueOrDefault<string>(LastfmKeyName, LastfmKeyDefault); }
            set { if (AddOrUpdateValue(LastfmKeyName, value)) { Save(); } }
        }
        public bool LastfmImagesSetting
        {
            get { return GetValueOrDefault<bool>(LastfmImagesName, LastfmImagesDefault); }
            set { if (AddOrUpdateValue(LastfmImagesName, value)) { Save(); } }
        }


    }
}