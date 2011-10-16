﻿using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Collections.Generic;

namespace AmpM
{
    public class AppSettingsModel
    {
        // Our isolated storage settings
        IsolatedStorageSettings settings;

        // The isolated storage key names of our settings
        const string FirstRunName = "FirstRun";

        const string HostIndexName = "HostIndex";





        // The default value of our settings
        const bool FirstRunDefault = true;


        const int HostIndexDefault = 0;





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


    }
}