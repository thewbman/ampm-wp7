﻿#pragma checksum "C:\Users\wes\SVN\ampm\AmpM\AmpM\AddServer.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "98E18536FF964E2DF72192458C156ECE"
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
    
    
    public partial class AddServer : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.StackPanel ContentPanel;
        
        internal System.Windows.Controls.TextBox namebox;
        
        internal System.Windows.Controls.TextBox serverbox;
        
        internal System.Windows.Controls.TextBox usernamebox;
        
        internal System.Windows.Controls.TextBox passwordbox;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton saveButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton cancelButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/AmpM;component/AddServer.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.ContentPanel = ((System.Windows.Controls.StackPanel)(this.FindName("ContentPanel")));
            this.namebox = ((System.Windows.Controls.TextBox)(this.FindName("namebox")));
            this.serverbox = ((System.Windows.Controls.TextBox)(this.FindName("serverbox")));
            this.usernamebox = ((System.Windows.Controls.TextBox)(this.FindName("usernamebox")));
            this.passwordbox = ((System.Windows.Controls.TextBox)(this.FindName("passwordbox")));
            this.saveButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("saveButton")));
            this.cancelButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("cancelButton")));
        }
    }
}

