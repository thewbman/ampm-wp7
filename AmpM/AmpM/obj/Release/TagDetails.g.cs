﻿#pragma checksum "C:\Users\wes\SVN\ampm\AmpM\AmpM\TagDetails.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5EC05FC209D86C75265995DA06D51CCA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Advertising.Mobile.UI;
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
    
    
    public partial class TagDetails : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.PerformanceProgressBar performanceProgressBarCustomized;
        
        internal Microsoft.Advertising.Mobile.UI.AdControl adControl1;
        
        internal Microsoft.Phone.Controls.Pivot pivotTitle;
        
        internal System.Windows.Controls.ListBox albumList;
        
        internal System.Windows.Controls.ListBox artistList;
        
        internal System.Windows.Controls.ListBox songList;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/AmpM;component/TagDetails.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.performanceProgressBarCustomized = ((Microsoft.Phone.Controls.PerformanceProgressBar)(this.FindName("performanceProgressBarCustomized")));
            this.adControl1 = ((Microsoft.Advertising.Mobile.UI.AdControl)(this.FindName("adControl1")));
            this.pivotTitle = ((Microsoft.Phone.Controls.Pivot)(this.FindName("pivotTitle")));
            this.albumList = ((System.Windows.Controls.ListBox)(this.FindName("albumList")));
            this.artistList = ((System.Windows.Controls.ListBox)(this.FindName("artistList")));
            this.songList = ((System.Windows.Controls.ListBox)(this.FindName("songList")));
        }
    }
}

