﻿#pragma checksum "C:\Users\wes\SVN\ampm\AmpM\AmpM\Preferences.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DEF4DEBC1EFFC5C2CDED1F7922904DA3"
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
        
        internal System.Windows.Controls.CheckBox PlayAll;
        
        internal System.Windows.Controls.CheckBox PlayShuffle;
        
        internal System.Windows.Controls.CheckBox PlayAdd;
        
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
            this.PlayAll = ((System.Windows.Controls.CheckBox)(this.FindName("PlayAll")));
            this.PlayShuffle = ((System.Windows.Controls.CheckBox)(this.FindName("PlayShuffle")));
            this.PlayAdd = ((System.Windows.Controls.CheckBox)(this.FindName("PlayAdd")));
            this.appbarSave = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("appbarSave")));
        }
    }
}

