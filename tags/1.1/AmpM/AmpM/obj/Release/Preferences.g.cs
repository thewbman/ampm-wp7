﻿#pragma checksum "C:\Users\wes\SVN\ampm\AmpM\AmpM\Preferences.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "129A4D2AB24042EF397C93B8816F10AA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace AmpM {
    
    
    public partial class Preferences : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.ToggleSwitch TogglePlayAll;
        
        internal Microsoft.Phone.Controls.ToggleSwitch TogglePlayShuffle;
        
        internal Microsoft.Phone.Controls.ToggleSwitch TogglePlayAdd;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton appbarSave;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/AmpM;component/Preferences.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TogglePlayAll = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("TogglePlayAll")));
            this.TogglePlayShuffle = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("TogglePlayShuffle")));
            this.TogglePlayAdd = ((Microsoft.Phone.Controls.ToggleSwitch)(this.FindName("TogglePlayAdd")));
            this.appbarSave = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("appbarSave")));
        }
    }
}
