using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AmpM
{
    public class DataItemViewModel : INotifyPropertyChanged
    {
        public string Type;

        public int SongId;
        public int AlbumId;
        public int ArtistId;
        public int PlaylistId;

        public string SongName;
        public string AlbumName;
        public string ArtistName;
        public string PlaylistName;

        public int AlbumTracks;
        public int ArtistAlbums;
        public int ArtistTracks;
        public int PlaylistItems;

        public string ArtUrl;


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
