using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace AmpM
{
    public partial class AddServer : PhoneApplicationPage
    {
        public AddServer()
        {
            InitializeComponent();

            AllowSave = true;
        }

        private bool AllowSave;

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            Save();
            
            base.OnNavigatedFrom(e);
        }

        private void Save()
        {
            if (AllowSave)
            {
                App.ViewModel.Hosts.Add(new HostViewModel()
                {
                    Name = namebox.Text,
                    Address = serverbox.Text,
                    Username = usernamebox.Text,
                    Password = passwordbox.Text
                });
            }

            App.ViewModel.saveHosts();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            AllowSave = true;

            NavigationService.GoBack();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            AllowSave = false;

            NavigationService.GoBack();
        }
    }
}