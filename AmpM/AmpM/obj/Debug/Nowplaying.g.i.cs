﻿#pragma checksum "C:\Users\wes\SVN\ampm\AmpM\AmpM\Nowplaying.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F389AAC68EEC4E88BD1923385C8AD42A"
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
        
        internal Microsoft.Phone.Controls.PanoramaItem songHeader;
        
        internal System.Windows.Controls.TextBlock songCount;
        
        internal System.Windows.Controls.Image artUrl;
        
        internal System.Windows.Controls.TextBlock songName;
        
        internal System.Windows.Controls.TextBlock artistName;
        
        internal System.Windows.Controls.TextBlock albumName;
        
        internal System.Windows.Controls.ListBox nowplayingList;
        
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
            this.songHeader = ((Microsoft.Phone.Controls.PanoramaItem)(this.FindName("songHeader")));
            this.songCount = ((System.Windows.Controls.TextBlock)(this.FindName("songCount")));
            this.artUrl = ((System.Windows.Controls.Image)(this.FindName("artUrl")));
            this.songName = ((System.Windows.Controls.TextBlock)(this.FindName("songName")));
            this.artistName = ((System.Windows.Controls.TextBlock)(this.FindName("artistName")));
            this.albumName = ((System.Windows.Controls.TextBlock)(this.FindName("albumName")));
            this.nowplayingList = ((System.Windows.Controls.ListBox)(this.FindName("nowplayingList")));
        }
    }
}
