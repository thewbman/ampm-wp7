﻿#pragma checksum "C:\Users\wes\SVN\ampm\AmpM\AmpM\Nowplaying.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "387A9E889C122A3A60D2EA30C7150E4B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Advertising.Mobile.UI;
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
    
    
    public partial class Nowplaying : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Advertising.Mobile.UI.AdControl adControl1;
        
        internal Microsoft.Phone.Controls.Pivot nowplayingPivot;
        
        internal Microsoft.Phone.Controls.PivotItem songPivot;
        
        internal System.Windows.Controls.Primitives.Popup bufferingPopup;
        
        internal System.Windows.Controls.TextBlock songCount;
        
        internal System.Windows.Controls.Image artUrl;
        
        internal System.Windows.Controls.TextBlock artistName;
        
        internal System.Windows.Controls.TextBlock songName;
        
        internal System.Windows.Controls.TextBlock albumName;
        
        internal Microsoft.Phone.Controls.PivotItem playlistPivot;
        
        internal System.Windows.Controls.ListBox nowplayingList;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton prevButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton pauseButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton nextButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem takeAudioControl;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/AmpM;component/Nowplaying.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.adControl1 = ((Microsoft.Advertising.Mobile.UI.AdControl)(this.FindName("adControl1")));
            this.nowplayingPivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("nowplayingPivot")));
            this.songPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("songPivot")));
            this.bufferingPopup = ((System.Windows.Controls.Primitives.Popup)(this.FindName("bufferingPopup")));
            this.songCount = ((System.Windows.Controls.TextBlock)(this.FindName("songCount")));
            this.artUrl = ((System.Windows.Controls.Image)(this.FindName("artUrl")));
            this.artistName = ((System.Windows.Controls.TextBlock)(this.FindName("artistName")));
            this.songName = ((System.Windows.Controls.TextBlock)(this.FindName("songName")));
            this.albumName = ((System.Windows.Controls.TextBlock)(this.FindName("albumName")));
            this.playlistPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("playlistPivot")));
            this.nowplayingList = ((System.Windows.Controls.ListBox)(this.FindName("nowplayingList")));
            this.prevButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("prevButton")));
            this.pauseButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("pauseButton")));
            this.nextButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("nextButton")));
            this.takeAudioControl = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("takeAudioControl")));
        }
    }
}

