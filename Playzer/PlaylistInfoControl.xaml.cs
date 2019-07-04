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
    /// Interaction logic for PlaylistInfoControl.xaml
    /// </summary>
    public partial class PlaylistInfoControl : UserControl
    {
        public PlaylistInfoControl()
        {
            InitializeComponent();
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
                titleLbl.Text = _playlist.Title;
                songsCountLbl.Text = _playlist.NumOfSongs + " song" + (_playlist.Songs.Count > 1 ? "s" : string.Empty);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_playlist.Url, UriKind.Absolute);
                bitmap.EndInit();
                imageCover.Source = bitmap;
            }
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
        }

        private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PlaylistDetailsWindow pdw = new PlaylistDetailsWindow();
            pdw.PlaylistSource = Playlist;
            pdw.ShowDialog();
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

    }
}
