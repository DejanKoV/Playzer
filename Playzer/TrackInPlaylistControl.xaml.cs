using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Playzer
{
    /// <summary>
    /// Interaction logic for TrackInPlaylistControl.xaml
    /// </summary>
    public partial class TrackInPlaylistControl : UserControl
    {
        public TrackInPlaylistControl()
        {
            InitializeComponent();
        }
        public event EventHandler OnPlaylistSelected;

        public bool Added
        {
            get { return addedLbl.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                    addedLbl.Visibility = Visibility.Visible;
                else addedLbl.Visibility = Visibility.Hidden;
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
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_playlist.Url, UriKind.Absolute);
                bitmap.EndInit();
                playlistImage.Source = bitmap;
                playlistNameLbl.Content = _playlist.Title;
                tracksCountLbl.Content = _playlist.NumOfSongs + " song" + (_playlist.NumOfSongs > 1 ? "s" : string.Empty);
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnPlaylistSelected?.Invoke(this, EventArgs.Empty);
        }
    }
}
