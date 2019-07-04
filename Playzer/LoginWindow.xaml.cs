using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        public static ICookieJar cookie = null;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            blockUI();
            Cursor = Cursors.Wait;

            try
            {
                if (File.Exists(emailTextbox.Text + ".txt"))
                {

                    string bin = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    string pass = File.ReadAllText(System.IO.Path.Combine(bin, emailTextbox.Text + ".txt"));


                    if(pass!=passwordBox.Password)
                    {
                        MessageBox.Show("Wrong password", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {

                        if(!File.Exists(emailTextbox.Text + "Playlists.txt"))
                        {
                            Populate populate = new Populate();
                            bool loged = populate.Login(emailTextbox.Text, passwordBox.Password);
                            if (loged == false)
                            {
                                MessageBox.Show("You are not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            else
                            {

                                MainWindow.Pass = passwordBox.Password;
                                MainWindow.Email = emailTextbox.Text;
                                MainWindow mw1 = new MainWindow();
                                mw1.Show();
                                Close();
                            }
                        }
                        else
                        {
                            List<MyPlaylist> plejliste = new List<MyPlaylist>();
                            string path1 = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                            string text = File.ReadAllText(System.IO.Path.Combine(path1, emailTextbox.Text + "Playlists.txt"));

                            string[] plist = text.Split(new string[] { "PLAYLIST " }, StringSplitOptions.None);

                            foreach (string lists in plist)
                            {
                                if (lists != "")
                                {
                                    MyPlaylist mpl = new MyPlaylist();
                                    int selected;
                                    string[] items = lists.Split(new string[] { "*#" }, StringSplitOptions.None);
                                    Int32.TryParse(items[0], out selected);
                                    mpl.SelectedId = selected;
                                    mpl.Title = items[1];
                                    mpl.Url = items[2].Split('\n')[0].Split('\r')[0];

                                    string[] tracks = items[2].Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                                    string[] tr = tracks[1].Split(new string[] { "\r\n" }, StringSplitOptions.None);

                                    foreach (string track in tr)
                                    {
                                        if (track != "")
                                        {
                                            string[] song_stuff = track.Split(new string[] { "|?" }, StringSplitOptions.None);
                                            int selectedtr;
                                            Int32.TryParse(song_stuff[0].Split(' ')[1], out selectedtr);
                                            Song song = new Song(song_stuff[2], song_stuff[3], song_stuff[1], song_stuff[4], selectedtr);
                                            mpl.NumOfSongs = tr.Count() - 1;
                                            mpl.Songs.Add(song);
                                        }



                                    }
                                    plejliste.Add(mpl);
                                }
                            }

                            MainWindow.allPL = plejliste;
                            MainWindow.Pass = passwordBox.Password;
                            MainWindow.Email = emailTextbox.Text;
                            MainWindow mw = new MainWindow();
                            mw.Show();
                            Close();
                        }

                       
                    }

                }
                else
                {
                    Populate populate = new Populate();
                    bool loged = populate.Login(emailTextbox.Text, passwordBox.Password);
                    if (loged == false)
                    {
                        MessageBox.Show("You are not logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                      
                        MainWindow.Pass = passwordBox.Password;
                        MainWindow.Email = emailTextbox.Text;
                        MainWindow mw = new MainWindow();
                        mw.Show();
                        Close();
                    }
                }
              
               
            }
            finally
            {
                unblockUI();
                Cursor = Cursors.Arrow;
            }
        }

        bool validateInput()
        {
            if (string.IsNullOrEmpty(emailTextbox.Text.Trim()) || string.IsNullOrWhiteSpace(emailTextbox.Text.Trim()))
            {
                MessageBox.Show("Email can not be blank.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(passwordBox.Password.Trim()) || string.IsNullOrWhiteSpace(passwordBox.Password.Trim()))
            {
                MessageBox.Show("Password can not be blank.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }


        void blockUI()
        {
            emailTextbox.IsEnabled = passwordBox.IsEnabled = loginButton.IsEnabled = false;
        }
        void unblockUI()
        {
            emailTextbox.IsEnabled = passwordBox.IsEnabled = loginButton.IsEnabled = true;
        }
    }
}
