﻿#pragma checksum "C:\Users\wes\SVN\ampm\AmpM\AmpM\SongsList.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8372C10E675BF0FC4FABD3B63453DB1D"
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
    
    
    public partial class SongsList : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.PerformanceProgressBar performanceProgressBarCustomized;
        
        internal Microsoft.Phone.Controls.Pivot songsPivot;
        
        internal Microsoft.Phone.Controls.PivotItem songsAlphaPivot;
        
        internal Microsoft.Phone.Controls.LongListSelector SongsLL;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/AmpM;component/SongsList.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.performanceProgressBarCustomized = ((Microsoft.Phone.Controls.PerformanceProgressBar)(this.FindName("performanceProgressBarCustomized")));
            this.songsPivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("songsPivot")));
            this.songsAlphaPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("songsAlphaPivot")));
            this.SongsLL = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("SongsLL")));
        }
    }
}

