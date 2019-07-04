using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playzer
{
    public class Song
    {
        public Song(string title, string artist,string id,string image,int selected)
        {
            this.Title = title;
            this.Artist = artist;
            this.Id = id;
            this.Url = image;
            this.Selected = selected;
            //this.Album = album;
            //this.ByDate = addedbydate;
        }

        public Song(string id, string title, string artist)
        {
            this.Title = title;
            this.Artist = artist;
            this.Id = id;
        }
        private string url;


        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        private string title;
        private string artist;
        private string album;
        private string id;
        private int selected;

        public int Selected
        {
            get { return selected; }
            set { selected = value; }
        }


        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        private DateTime byDate;

        public DateTime ByDate
        {
            get { return byDate; }
            set { byDate = value; }
        }


        public string Album
        {
            get { return album; }
            set { album = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

     

        public string Artist
        {
            get { return artist; }
            set { artist = value; }
        }

    }
}



