using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Path = System.IO.Path;

namespace Playzer
{
    /// <summary>
    /// Interaction logic for MyPlaylists.xaml
    /// </summary>
    public partial class MyPlaylists : Window
    {
        public static List<Song> results = new List<Song>();

        public MyPlaylists()
        {
            InitializeComponent();
            Loaded += async (s, e) =>
            {
                await loadNextPage();
            };
        }

        int page = 0;
        public Song Track { get; set; } = null;
        public Song[] Tracks { get; set; } = null;
  

        async Task loadNextPage()
        {
            page++;

            var data = MainWindow.allPL;
            MainWindow.changed = false;//
            if (data==null)
                return;

            addPlaylists(data);

            loadMoreButton.IsEnabled = data.Count >= 50;

           

            if (Track != null)
            {
                var data1 = MainWindow.allPL;

                if (data1 == null)
                    return;

                foreach (var control in stack.Children)
                {
                    if (control is TrackInPlaylistControl)
                    {
                        var ctrl = control as TrackInPlaylistControl;
                        if (data1.Where(x => x.SelectedId == ctrl.Playlist.SelectedId).Any()) ;
                         
                    }
                }
            }

        }

        void addPlaylists(List<MyPlaylist> data)
        {
            List<Song> search_results = new List<Song>();
            IReadOnlyCollection<IWebElement> search_content;
            List<string> search_content_ids = new List<string>();
            List<MyPlaylist> plejliste = new List<MyPlaylist>();
            MyPlaylist pl = new MyPlaylist();
            List<MyPlaylist> lmp = new List<MyPlaylist>();

            foreach (var playlist in data)
            {
                TrackInPlaylistControl control = new TrackInPlaylistControl();
                control.Playlist = playlist;  

                control.OnPlaylistSelected += async (s, e) =>
                {
                    Cursor = Cursors.Wait;

                    try
                    {
                        if (!control.Added)
                        {
                          
                            var service = PhantomJSDriverService.CreateDefaultService();
                            service.HideCommandPromptWindow = true;
                            IWebDriver driver = new PhantomJSDriver(service);
                            driver.Url = "http://www.playzer.fr/customer/login";

                            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                            driver.FindElement(By.Id("customer_login_login")).SendKeys(MainWindow.Email);
                            driver.FindElement(By.Id("customer_login_password")).SendKeys(MainWindow.Pass);
                            IWebElement btn = driver.FindElements(By.XPath(".//button[@onclick]"))[1];
                            js.ExecuteScript("arguments[0].click();", btn);

 
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='panel_search']/img")));
                            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            driver.FindElement(By.Id("search_engine")).SendKeys(MainWindow.sb);
                            Thread.Sleep(TimeSpan.FromSeconds(2));
                            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                            IWebElement result = driver.FindElement(By.Id("search_results"));
                            search_content = result.FindElements(By.XPath("//div[@class='content transition search_item_content']"));
                            int i = 1;

                            foreach (IWebElement ids in search_content)
                            {
                                string song_id = ids.GetAttribute("id");
                                if (i == Track.Selected)
                                {
                                    js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='" + song_id + "']")));
                               
                                    js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='panel_history']")));
                                    Thread.Sleep(TimeSpan.FromSeconds(1));
                                    string songID = driver.FindElements(By.ClassName("bloc"))[0].GetAttribute("id");
                                    js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='" + songID + "_options']")));

                                    js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//div[@class='add transition']")));

                                    driver.FindElement(By.Id("content_add_form"));


                                    IReadOnlyCollection<IWebElement> all_plist = driver.FindElements(By.XPath("//div[@class='item choice']"));

                                    int j = 1;

                                    foreach (IWebElement choice_id in all_plist)
                                    {
                                        if (j == control.Playlist.SelectedId)
                                        {
                                            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                                            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='" + choice_id.GetAttribute("id") + "_status']")));                                      
                                            break;
                                        }
                                        j++;
                                    }
                                    break;
                                  
                                }
                                i++;
                            }

                           

                            driver.Navigate().Refresh();

                            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.Id("menu_myplaylists")));
                            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                            IReadOnlyCollection<IWebElement> all_playlist = driver.FindElements(By.XPath("//div[@class='content home_playlist transition']"));
                            Thread.Sleep(TimeSpan.FromSeconds(2));

                            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                            MyPlaylist playlist1 = new MyPlaylist();
                            int cnt = 1;

                            foreach (MyPlaylist mp in LoadPlaylist())
                            {
                                if (mp.SelectedId != control.Playlist.SelectedId)
                                {
                                    plejliste.Add(mp);
                                }
                            }
                            foreach (IWebElement playL in all_playlist)
                            {
                              
                                    try
                                    {

                                        string id = playL.GetAttribute("id");
                                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));
                                        IWebElement playlist_option_btn = driver.FindElement(By.XPath("//*[@id='" + id + "_options" + "']"));
                                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));
                                        js.ExecuteScript("arguments[0].click();", playlist_option_btn);
                                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));
                                        IWebElement edit_btn = driver.FindElement(By.XPath("//div[@class='edit transition']"));
                                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));
                                        js.ExecuteScript("arguments[0].click();", edit_btn);
                                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));

                                        IWebElement song_items = driver.FindElement(By.XPath("//ul[@id='playlist_items']"));
                                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));
                                        IReadOnlyCollection<IWebElement> songs = song_items.FindElements(By.ClassName("content"));
                                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));

                                        playlist1 = new MyPlaylist();
                                        playlist1.Title = driver.FindElement(By.XPath("//*[@id='playlist_edit_name']")).GetAttribute("value");
                                        playlist1.SelectedId = 1;

                                        foreach (IWebElement song in songs)
                                        {
                                            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));
                                            string id_song = song.GetAttribute("id");
                                            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                                            string title = driver.FindElement(By.XPath("//*[@id='" + id_song + "']/div[4]")).Text;
                                            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                                            string artist = driver.FindElement(By.XPath("//*[@id='" + id_song + "']/div[5]")).Text;
                                            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                                            string url = driver.FindElement(By.XPath("//*[@id='" + id_song + "']/div[3]/img")).GetAttribute("src");
                                            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                                            Song track = new Song(title, artist, id_song, url, i);
                                            playlist1.Songs.Add(track);
                                            playlist1.NumOfSongs = songs.Count;
                                            playlist1.Url = url;


                                        }

                                        plejliste.Add(playlist1);
                                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                                        js.ExecuteScript("arguments[0].click();", driver.FindElement(By.Id("close_modify")));
                                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                                        js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='playlist_options']/div[1]/img")));
                                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                break;
                               
                            }
                            driver.Close();
                            driver.Quit();
                        
                            SaveToFile(plejliste);


                            MessageBox.Show("Added to playlist " + playlist.Title, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                            control.Added = true;
                            MainWindow.changed = true;
                          //  MainWindow.allPL = plejliste;
                        }
                      
                    }
                    finally
                    {
                        Cursor = Cursors.Arrow;
                    }
                };
                stack.Children.Add(control);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreatePlaylistWindow create = new CreatePlaylistWindow();
            if (Track != null)
            {
                create.MusicId = Track.Selected;
                create.ShowDialog();
                this.Close();
            }
          
            


        }

        private  void loadMoreButton_Click(object sender, RoutedEventArgs e)
        {
             loadNextPage();
        }

        private List<MyPlaylist> LoadPlaylist()
        {
            List<MyPlaylist> plejliste = new List<MyPlaylist>();
            string path1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string text = File.ReadAllText(Path.Combine(path1, MainWindow.Email + "Playlists.txt"));

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

            return plejliste;
        }

        public void SaveToFile(List<MyPlaylist> myPL)
        {
            using (StreamWriter theWriter = new StreamWriter(MainWindow.Email + "Playlists.txt"))
            {
                try
                {
                    int cntlist = 0;
                    foreach (MyPlaylist mp in myPL)
                    {
                        cntlist++;
                        theWriter.WriteLine("PLAYLIST " + cntlist + "*#" + mp.Title + "*#" + mp.Url);
                        theWriter.WriteLine();
                        int cntSong = 0;
                        foreach (Song song in mp.Songs)
                        {
                            cntSong++;
                            theWriter.WriteLine("SONG " + cntSong + "|?" + song.Id + "|?" + song.Title + "|?" + song.Artist + "|?" + song.Url);
                        }

                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public Cookie GetCookies()
        {
            string path1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string text = File.ReadAllText(Path.Combine(path1, "Cookie.data"));

            string[] parse = text.Split(';');
            String name = parse[0];
            String value = parse[1];
            String domain = parse[2];
            String path = parse[3];
            DateTime expiry = DateTime.Now;
            if (!(parse[4].Equals("null")))
            {
                expiry = DateTime.Parse(parse[4]);
            }
            Cookie ck = new Cookie(name, value, domain, path, expiry);
            return ck;
        }

     
    }
}
