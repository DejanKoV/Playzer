using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playzer
{
     public class TableItem
    {
        private string trackID;
        private string trackName;
        private string trackArtist;
        private string albumName;

       


        public string Track_ID
        {
            get { return trackID; }
            set { trackID = value; }
        }

        public string Track_Name
        {
            get { return trackName; }
            set { trackName = value; }
        }


        public string Track_Artist
        {
            get { return trackArtist; }
            set { trackArtist = value; }
        }
        public string Album_Name
        {
            get { return albumName; }
            set { albumName = value; }
        }




        public TableItem(string id, string title, string artist, string albumN)
        {
            this.Track_ID = id;
            this.Track_Name = title;
            this.Track_Artist = artist;
            this.Album_Name = albumN;
        }
    }
}
