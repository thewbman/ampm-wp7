﻿#pragma checksum "C:\Users\wes\SVN\ampm\AmpM\AmpM\Albums.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "27969BFFEEFCA61C94D7FE781EFE100C"
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
    
    
    public partial class Albums : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Microsoft.Phone.Controls.PerformanceProgressBar performanceProgressBarCustomized;
        
        internal Microsoft.Phone.Controls.Pivot albumsPivot;
        
        internal Microsoft.Phone.Controls.PivotItem albumsAlphaPivot;
        
        internal Microsoft.Phone.Controls.LongListSelector AlbumsLL;
        
        internal Microsoft.Phone.Controls.PivotItem albumsArtistPivot;
        
        internal Microsoft.Phone.Controls.LongListSelector AlbumsArtistLL;
        
        internal Microsoft.Phone.Controls.PivotItem albumsYearPivot;
        
        internal Microsoft.Phone.Controls.LongListSelector AlbumsYearLL;
        
        internal Microsoft.Phone.Controls.PivotItem albumsRandomPivot;
        
        internal System.Windows.Controls.Image artUrl;
        
        internal System.Windows.Controls.TextBlock artistName;
        
        internal System.Windows.Controls.TextBlock albumName;
        
        internal System.Windows.Controls.TextBlock albumTracks;
        
        internal System.Windows.Controls.TextBlock albumYear;
        
        internal System.Windows.Controls.Button nextRandomButton;
        
        internal Microsoft.Phone.Controls.PivotItem albumsSearchPivot;
        
        internal System.Windows.Controls.TextBox searchBox;
        
        internal System.Windows.Controls.Button searchBoxButton;
        
        internal Microsoft.Phone.Controls.LongListSelector AlbumsSearchLL;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/AmpM;component/Albums.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.performanceProgressBarCustomized = ((Microsoft.Phone.Controls.PerformanceProgressBar)(this.FindName("performanceProgressBarCustomized")));
            this.albumsPivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("albumsPivot")));
            this.albumsAlphaPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("albumsAlphaPivot")));
            this.AlbumsLL = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("AlbumsLL")));
            this.albumsArtistPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("albumsArtistPivot")));
            this.AlbumsArtistLL = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("AlbumsArtistLL")));
            this.albumsYearPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("albumsYearPivot")));
            this.AlbumsYearLL = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("AlbumsYearLL")));
            this.albumsRandomPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("albumsRandomPivot")));
            this.artUrl = ((System.Windows.Controls.Image)(this.FindName("artUrl")));
            this.artistName = ((System.Windows.Controls.TextBlock)(this.FindName("artistName")));
            this.albumName = ((System.Windows.Controls.TextBlock)(this.FindName("albumName")));
            this.albumTracks = ((System.Windows.Controls.TextBlock)(this.FindName("albumTracks")));
            this.albumYear = ((System.Windows.Controls.TextBlock)(this.FindName("albumYear")));
            this.nextRandomButton = ((System.Windows.Controls.Button)(this.FindName("nextRandomButton")));
            this.albumsSearchPivot = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("albumsSearchPivot")));
            this.searchBox = ((System.Windows.Controls.TextBox)(this.FindName("searchBox")));
            this.searchBoxButton = ((System.Windows.Controls.Button)(this.FindName("searchBoxButton")));
            this.AlbumsSearchLL = ((Microsoft.Phone.Controls.LongListSelector)(this.FindName("AlbumsSearchLL")));
        }
    }
}

