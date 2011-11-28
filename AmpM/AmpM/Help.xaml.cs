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

            this.Questions.Add(new NameContentViewModel() { Name = "What is Ampache?", Content = "Ampache is a web based audio/video streaming application and file manager allowing you to access your music & videos from anywhere, using almost any internet enabled device." });
            this.Questions.Add(new NameContentViewModel() { Name = "How is Ampache different than the 'cloud' music players from Google, Amazon and others?", Content = "With Ampache all of your music is streamed directly from your server to your device.  The music is not copied to any other server in the cloud.  There are no restrictions on the number or length of songs you can have and there is never any cost for the service itself.  And it is really easy to share your music collection with others." });
            this.Questions.Add(new NameContentViewModel() { Name = "What does this app do?", Content = "AmpM is a WP7 app that allows you to play back music from a Ampache server." });
            this.Questions.Add(new NameContentViewModel() { Name = "How do I get started?", Content = "You need to have Ampache up and running on your computer for this app to work.  Once you have Ampache setup you will need to setup the ACL to allow the app access to your library.  Setting up an Ampache system is not hard process, but it is not a trivial task either.  You should make sure everything is working fine on your computer before trying to use this app." });
            this.Questions.Add(new NameContentViewModel() { Name = "Do I have to keep the app open to play music?", Content = "No.  You should be able to close the app fully and have your music continue to play through the full 'now playing' list." });
            this.Questions.Add(new NameContentViewModel() { Name = "What I have trouble getting this app to work?", Content = "Try emailing the developer.  The contact information is available to right." });

            QuestionListBox.ItemsSource = this.Questions;

            this.Support.Clear();

            this.Support.Add(new NameContentViewModel() { Name = "email", Content = "ampm.wp7@gmail.com" });
            this.Support.Add(new NameContentViewModel() { Name = "twitter", Content = "@webmyth_dev" });
            this.Support.Add(new NameContentViewModel() { Name = "app homepage", Content = "http://code.google.com/p/ampm-wp7" });
            this.Support.Add(new NameContentViewModel() { Name = "leave review", Content = "" });

            SupportListBox.ItemsSource = this.Support;

        }

        private void email_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            EmailComposeTask emailcomposer = new EmailComposeTask();
            emailcomposer.To = "ampm.wp7@gmail.com";
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