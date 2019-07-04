using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playzer
{
    public class MyPlaylist
    {
        private List<Song> songs;
        private string title;
        private int numOfSongs;
        private int id;
        private string url;


        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        public int SelectedId
        {
            get { return id; }
            set { id = value; }
        }


        public int NumOfSongs
        {
            get { return numOfSongs; }
            set { numOfSongs = value; }
        }


        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public List<Song> Songs
        {
            get { return songs; }
            set { songs = value; }
        }

        public MyPlaylist()
        {
            songs = new List<Song>();
        }

    }
}
