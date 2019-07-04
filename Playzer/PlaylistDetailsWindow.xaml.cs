using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Playzer
{
    /// <summary>
    /// Interaction logic for PlaylistDetailsWindow.xaml
    /// </summary>
    public partial class PlaylistDetailsWindow : Window
    {

        public static List<MyPlaylist> all_playlists = new List<MyPlaylist>();
        public PlaylistDetailsWindow()
        {
            InitializeComponent();

            Loaded += async (s, e) =>
            {
                Cursor = Cursors.Wait;
              
                foreach(MyPlaylist playlist in MainWindow.allPL)
                {
                    if(_source.SelectedId == playlist.SelectedId)
                    {
                        Playlist = playlist;
                        break;

                    }
                }

                Cursor = Cursors.Arrow;
            };
        }

        private List<Song> selectedTracks = new List<Song>();


        private MyPlaylist _source = null;
        public MyPlaylist PlaylistSource
        {
            get { return _source; }
            set
            {
                if (value == null)
                    return;

                _source = value;
                Title = "Loading " + _source.Title;
               
            }
        }

        private MyPlaylist _playlist = null;
        public MyPlaylist Playlist
        {
            get { return _playlist; }
            set
            {
                if (value == null)
                    return;

                _playlist = value;
                Title = _playlist.Title;
                addSongs(_playlist.Songs);
            }
        }

        public void addSongs(List<Song> songs)
        {
            foreach (var song in songs)
            {
                ListItem mc = new ListItem();
                mc.Track = song;
                mc.addBtn.Visibility = Visibility.Hidden;
                mc.OnTrackSelected += (s, e) =>
                {
                    selectedTracks.Add(mc.Track);
                
                };
                mc.OnTrackDiselected += (s, e) =>
                {
                    selectedTracks.Remove(mc.Track);
                 
                };
                mc.HorizontalAlignment = HorizontalAlignment.Stretch;
                container.Children.Add(mc);
            }
        }

        private void exportBtn_Click(object sender, RoutedEventArgs e)
        {
          
            
                Cursor = Cursors.Wait;
                    using (StreamWriter theWriter = new StreamWriter(_source.Title+".csv"))
                {
                    theWriter.WriteLine("Track ID" + ',' + "Track Name"+','+ "Artist Name"+','+"Album Name");
                    
                    foreach (Song s in _source.Songs)
                    {
                        theWriter.Write(s.Id + ',');
                        theWriter.Write(s.Title + ',');
                        theWriter.Write(s.Artist + ',');
                        theWriter.WriteLine("");

                    }
                    MessageBox.Show(_source.Title + " playlist are exported to file.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                Cursor = Cursors.Arrow;
               
            
        }


      
    }
}
