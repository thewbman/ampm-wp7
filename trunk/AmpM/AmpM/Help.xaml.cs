using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Threading;
using Microsoft.Phone;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Xml.Linq;
using System.Security.Cryptography;

namespace AmpM
{
    public partial class Help : PhoneApplicationPage
    {
        public Help()
        {
            InitializeComponent();

            this.Questions = new ObservableCollection<NameContentViewModel>();
            this.Support = new ObservableCollection<NameContentViewModel>();
        }

        public ObservableCollection<NameContentViewModel> Questions { get; private set; }
        public ObservableCollection<NameContentViewModel> Support { get; private set; }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Questions.Clear();


            this.Questions.Add(new NameContentViewModel() { Name = "What I have trouble getting this app to work?", Content = "Try emailing the developer.  The contact information is available to right." });

            QuestionListBox.ItemsSource = this.Questions;

            this.Support.Clear();

            this.Support.Add(new NameContentViewModel() { Name = "email", Content = "ampachexl.help@gmail.com" });
            this.Support.Add(new NameContentViewModel() { Name = "twitter", Content = "@webmyth_dev" });
            this.Support.Add(new NameContentViewModel() { Name = "app homepage", Content = "http://code.google.com/p/ampm-wp7" });
            this.Support.Add(new NameContentViewModel() { Name = "leave review", Content = "" });

            SupportListBox.ItemsSource = this.Support;

        }

        private void email_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailcomposer = new EmailComposeTask();
            emailcomposer.To = "ampachexl.help@gmail.com";
            emailcomposer.Subject = "AmpM Help";
            emailcomposer.Show();

        }

        private void twitter_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webopen = new WebBrowserTask();

            webopen.Uri = new Uri("http://twitter.com/webmyth_dev");
            webopen.Show();
        }

        private void homepage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webopen = new WebBrowserTask();

            webopen.Uri = new Uri("http://code.google.com/p/ampm-wp7/");
            webopen.Show();

        }

        private void reviewTitle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MarketplaceReviewTask marketReview = new MarketplaceReviewTask();
            marketReview.Show();

            /*
            MarketplaceDetailTask marketDetail = new MarketplaceDetailTask();
            marketDetail.ContentIdentifier = "455f5645-0b06-429b-9cac-9097b10ae6d2";
            marketDetail.Show();
            */
        }

        private void SupportListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SupportListBox.SelectedItem == null)
                return;

            var myItem = (NameContentViewModel)SupportListBox.SelectedItem;

            switch (myItem.Name)
            {
                case "email":
                    email_Tap(sender, new System.Windows.Input.GestureEventArgs());
                    break;
                case "twitter":
                    twitter_Tap(sender, new System.Windows.Input.GestureEventArgs());
                    break;
                case "app homepage":
                    homepage_Tap(sender, new System.Windows.Input.GestureEventArgs());
                    break;
                case "leave review":
                    reviewTitle_Tap(sender, new System.Windows.Input.GestureEventArgs());
                    break;
            }
        }
    }
}