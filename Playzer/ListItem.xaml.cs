using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
    /// Interaction logic for ListItem.xaml
    /// </summary>
    public partial class ListItem : UserControl
    {
        public ListItem()
        {
            InitializeComponent();
        }

        public event EventHandler OnTrackSelected, OnTrackDiselected;

        private bool _isTrackSelected = false;
        public bool IsTrackSelected
        {
            get { return _isTrackSelected; }
            set
            {
                _isTrackSelected = value;
                if (value)
                {
                    addBtn.Content = "Selected";
                    addBtn.IsChecked = true;
                }
                else
                {
                    addBtn.Content = "Select";
                    addBtn.IsChecked = false;
                }
            }
        }

        private Song _track;

        public Song Track
        {
            get { return _track; }
            set
            {
                if (value == null)
                    return;
                _track = value;

                songNameLbl.Content = _track.Title;
                songAuthorLbl.Content = _track.Artist;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_track.Url, UriKind.Absolute);
                bitmap.EndInit();
                trackImage.Source = bitmap;
            }
        }

        private void AddBtn_Checked(object sender, RoutedEventArgs e)
        {

            MyPlaylists window = new MyPlaylists();
            window.Track = Track;
            window.ShowDialog();


        }

       

    
    }
}
