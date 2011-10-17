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
        private string _Type;
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if (value != _Type)
                {
                    _Type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }

        private int _SongId;
        public int SongId
        {
            get
            {
                return _SongId;
            }
            set
            {
                if (value != _SongId)
                {
                    _SongId = value;
                    NotifyPropertyChanged("SongId");
                }
            }
        }
        private int _AlbumId;
        public int AlbumId
        {
            get
            {
                return _AlbumId;
            }
            set
            {
                if (value != _AlbumId)
                {
                    _AlbumId = value;
                    NotifyPropertyChanged("AlbumId");
                }
            }
        }
        private int _ArtistId;
        public int ArtistId
        {
            get
            {
                return _ArtistId;
            }
            set
            {
                if (value != _ArtistId)
                {
                    _ArtistId = value;
                    NotifyPropertyChanged("ArtistId");
                }
            }
        }
        private int _PlaylistId;
        public int PlaylistId
        {
            get
            {
                return _PlaylistId;
            }
            set
            {
                if (value != _PlaylistId)
                {
                    _PlaylistId = value;
                    NotifyPropertyChanged("PlaylistId");
                }
            }
        }

        private string _SongName;
        public string SongName
        {
            get
            {
                return _SongName;
            }
            set
            {
                if (value != _SongName)
                {
                    _SongName = value;
                    NotifyPropertyChanged("SongName");
                }
            }
        }
        private string _AlbumName;
        public string AlbumName
        {
            get
            {
                return _AlbumName;
            }
            set
            {
                if (value != _AlbumName)
                {
                    _AlbumName = value;
                    NotifyPropertyChanged("AlbumName");
                }
            }
        }
        private string _ArtistName;
        public string ArtistName
        {
            get
            {
                return _ArtistName;
            }
            set
            {
                if (value != _ArtistName)
                {
                    _ArtistName = value;
                    NotifyPropertyChanged("ArtistName");
                }
            }
        }
        private string _PlaylistName;
        public string PlaylistName
        {
            get
            {
                return _PlaylistName;
            }
            set
            {
                if (value != _PlaylistName)
                {
                    _PlaylistName = value;
                    NotifyPropertyChanged("PlaylistName");
                }
            }
        }


        private int _SongTrack;
        public int SongTrack
        {
            get
            {
                return _SongTrack;
            }
            set
            {
                if (value != _SongTrack)
                {
                    _SongTrack = value;
                    NotifyPropertyChanged("SongTrack");
                }
            }
        }
        private int _SongTime;
        public int SongTime
        {
            get
            {
                return _SongTime;
            }
            set
            {
                if (value != _SongTime)
                {
                    _SongTime = value;
                    NotifyPropertyChanged("SongTime");
                }
            }
        }
        private string _SongUrl;
        public string SongUrl
        {
            get
            {
                return _SongUrl;
            }
            set
            {
                if (value != _SongUrl)
                {
                    _SongUrl = value;
                    NotifyPropertyChanged("SongUrl");
                }
            }
        }

        private int _AlbumTracks;
        public int AlbumTracks
        {
            get
            {
                return _AlbumTracks;
            }
            set
            {
                if (value != _AlbumTracks)
                {
                    _AlbumTracks = value;
                    NotifyPropertyChanged("AlbumTracks");
                }
            }
        }
        private int _ArtistAlbums;
        public int ArtistAlbums
        {
            get
            {
                return _ArtistAlbums;
            }
            set
            {
                if (value != _ArtistAlbums)
                {
                    _ArtistAlbums = value;
                    NotifyPropertyChanged("ArtistAlbums");
                }
            }
        }
        private int _ArtistTracks;
        public int ArtistTracks
        {
            get
            {
                return _ArtistTracks;
            }
            set
            {
                if (value != _ArtistTracks)
                {
                    _ArtistTracks = value;
                    NotifyPropertyChanged("ArtistTracks");
                }
            }
        }
        private int _PlaylistItems;
        public int PlaylistItems
        {
            get
            {
                return _PlaylistItems;
            }
            set
            {
                if (value != _PlaylistItems)
                {
                    _PlaylistItems = value;
                    NotifyPropertyChanged("PlaylistItems");
                }
            }
        }

        private string _ArtUrl;
        public string ArtUrl
        {
            get
            {
                return _ArtUrl;
            }
            set
            {
                if (value != _ArtUrl)
                {
                    _ArtUrl = value;
                    NotifyPropertyChanged("ArtUrl");
                }
            }
        }


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
