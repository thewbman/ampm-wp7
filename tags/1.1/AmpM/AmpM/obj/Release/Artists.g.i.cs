﻿#pragma checksum "C:\Users\wes\SVN\ampm\AmpM\AmpM\Artists.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6E2D8974308F6536E1ACF14074F3116F"
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
    
    
    public partial class Artists : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.PerformanceProgressBar performanceProgressBarCustomized;
        
        internal Microsoft.Advertising.Mobile.UI.AdControl adControl1;
        
        internal Microsoft.Phone.Controls.Pivot artistsPivot;
        
        internal Microsoft.Phone.Controls.PivotItem artsitsAlphaPivot;
        
        internal Microsoft.Phone.Controls.LongListSelector ArtistsLL;
        
        internal Microsoft.Phone.Controls.PivotItem artistsRandomPivot;
        
        internal System.Windows.Controls.TextBlock artistName;
        
        internal System.Windows.Controls.TextBlock artistAlbums;
        
        internal System.Windows.Controls.TextBlock artistTracks;
        
        internal System.Windows.Controls.Button nextRandomButton;
        
        internal Microsoft.Phone.Controls.PivotItem artistsSearchPivot;
        
        internal System.Windows.Controls.TextBox searchBox;
        
        internal System.Windows.Controls.Button searchBoxButton;
        
        internal Microsoft.Phone.Controls.LongListSelector ArtistsSearchLL;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/AmpM;component/Artists.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.performanceProgressBarCustomized = ((Microsoft.Phone.Controls.PerformanceProgressBar)(this.FindName("performanceProgressBarCustomized")));
            this.adControl1 = ((Microsoft.Advertising.Mobile.UI.AdControl)(this.FindName("adControl1")));
            this.artistsPivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("artistsPivot")));
            this.artsitsAlphaPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("artsitsAlphaPivot")));
            this.ArtistsLL = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("ArtistsLL")));
            this.artistsRandomPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("artistsRandomPivot")));
            this.artistName = ((System.Windows.Controls.TextBlock)(this.FindName("artistName")));
            this.artistAlbums = ((System.Windows.Controls.TextBlock)(this.FindName("artistAlbums")));
            this.artistTracks = ((System.Windows.Controls.TextBlock)(this.FindName("artistTracks")));
            this.nextRandomButton = ((System.Windows.Controls.Button)(this.FindName("nextRandomButton")));
            this.artistsSearchPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("artistsSearchPivot")));
            this.searchBox = ((System.Windows.Controls.TextBox)(this.FindName("searchBox")));
            this.searchBoxButton = ((System.Windows.Controls.Button)(this.FindName("searchBoxButton")));
            this.ArtistsSearchLL = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("ArtistsSearchLL")));
        }
    }
}

