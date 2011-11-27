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

namespace MyAudioPlaybackAgent
{
    public class DataItemViewModel : INotifyPropertyChanged
    {
        private string _Type = "";
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

        private string _ItemKey = "";
        public string ItemKey
        {
            get
            {
                return _ItemKey;
            }
            set
            {
                if (value != _ItemKey)
                {
                    _ItemKey = value;
                    NotifyPropertyChanged("ItemKey");
                }
            }
        }
        private string _ItemChar = "~";
        public string ItemChar
        {
            get
            {
                string a = "~";

                switch (Type)
                {
                    case "song":
                        a = SongName.Substring(0, 1);
                        break;
                    case "album":
                        a = AlbumName.Substring(0, 1);
                        break;
                    case "artist":
                        a = ArtistName.Substring(0, 1);
                        break;
                    case "tag":
                        //a = Tag.Substring(0, 1);
                        break;
                    case "playlist":
                        a = PlaylistName.Substring(0, 1);
                        break;
                }

                return _ItemChar;
                //return a;
            }
            set
            {
                if (value != _ItemChar)
                {
                    _ItemChar = value;
                    NotifyPropertyChanged("ItemChar");
                }
            }
        }

        private int _ItemId = -1;
        public int ItemId
        {
            get
            {
                return _ItemId;
            }
            set
            {
                if (value != _ItemId)
                {
                    _ItemId = value;
                    NotifyPropertyChanged("ItemId");
                }
            }
        }

        private int _SongId = -1;
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
        private int _AlbumId = -1;
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
        private int _ArtistId = -1;
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
        private int _PlaylistId = -1;
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
        private int _TagId = -1;
        public int TagId
        {
            get
            {
                return _TagId;
            }
            set
            {
                if (value != _TagId)
                {
                    _TagId = value;
                    NotifyPropertyChanged("TagId");
                }
            }
        }

        private string _SongName = "";
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
        private string _AlbumName = "";
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
        private string _ArtistName = "";
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
        private string _PlaylistName = "";
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
        private string _TagName = "";
        public string TagName
        {
            get
            {
                return _TagName;
            }
            set
            {
                if (value != _TagName)
                {
                    _TagName = value;
                    NotifyPropertyChanged("TagName");
                }
            }
        }

        private string _Year = "";
        public string Year
        {
            get
            {
                return _Year;
            }
            set
            {
                if (value != _Year)
                {
                    _Year = value;
                    NotifyPropertyChanged("Year");
                }
            }
        }

        private int _SongTrack = -1;
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
        private int _SongTime = -1;
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
        private string _SongUrl = "";
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

        private int _AlbumTracks = -1;
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
        private int _ArtistAlbums = -1;
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
        private int _ArtistTracks = -1;
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
        private int _PlaylistItems = -1;
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
        
        private int _TagAlbums = -1;
        public int TagAlbums
        {
            get
            {
                return _TagAlbums;
            }
            set
            {
                if (value != _TagAlbums)
                {
                    _TagAlbums = value;
                    NotifyPropertyChanged("TagAlbums");
                }
            }
        }
        private int _TagArtists = -1;
        public int TagArtists
        {
            get
            {
                return _TagArtists;
            }
            set
            {
                if (value != _TagArtists)
                {
                    _TagArtists = value;
                    NotifyPropertyChanged("TagArtists");
                }
            }
        }
        private int _TagSongs = -1;
        public int TagSongs
        {
            get
            {
                return _TagSongs;
            }
            set
            {
                if (value != _TagSongs)
                {
                    _TagSongs = value;
                    NotifyPropertyChanged("TagSongs");
                }
            }
        }
        public string TagDetails
        {
            get
            {
                string s = "";

                if (_TagAlbums == 1)
                {
                    s += "1 album, ";
                }
                else
                {
                    s += _TagAlbums + " albums, ";
                }

                if (_TagArtists == 1)
                {
                    s += "1 artist, ";
                }
                else
                {
                    s += _TagArtists + " artists, ";
                }

                if (_TagSongs == 1)
                {
                    s += "1 song";
                }
                else
                {
                    s += _TagSongs + " songs";
                }

                return s;
            }
            set
            {
                //
            }
        }

        private string _ArtUrl = "";
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

        private string _Auth = "";
        public string Auth
        {
            get
            {
                return _Auth;
            }
            set
            {
                if (value != _Auth)
                {
                    _Auth = value;
                    NotifyPropertyChanged("Auth");
                }
            }
        }

        private int _NowplayingIndex = 0;
        public int NowplayingIndex
        {
            get
            {
                return _NowplayingIndex;
            }
            set
            {
                if (value != _NowplayingIndex)
                {
                    _NowplayingIndex = value;
                    NotifyPropertyChanged("NowplayingIndex");
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
