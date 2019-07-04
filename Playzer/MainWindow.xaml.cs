using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OpenQA.Selenium.Edge;
using Path = System.IO.Path;
using System.Collections.ObjectModel;
using Microsoft.Win32;

namespace Playzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<MyPlaylist> myPlaylist = new List<MyPlaylist>();
        public static List<MyPlaylist> allPL = new List<MyPlaylist>();
        public static bool changed = false;
        public List<Song> searched_results = new List<Song>();
        int cntPlaylist = 0;
        public static string sb = null;
        public static string Email = null;
        public static string Pass = null;



        public MainWindow()
        {
            InitializeComponent();
        }


        public bool CanExit { get; private set; } = false;

        int searchPage = 0;
        string lastSearchQuery = string.Empty;
        private List<Song> selectedTracks = new List<Song>();
        private int cnt = 0;

        private void addLoadingState(Control source)
        {
            if (source.Parent is Panel)
            {
                Panel parent = source.Parent as Panel;
                Grid indicatorBase = new Grid();
                indicatorBase.HorizontalAlignment = HorizontalAlignment.Stretch;
                indicatorBase.VerticalAlignment = VerticalAlignment.Stretch;
                ProgressBar loadingBar = new ProgressBar();
                loadingBar.HorizontalAlignment = HorizontalAlignment.Center;
                loadingBar.VerticalAlignment = VerticalAlignment.Center;
                loadingBar.Height = 20;
                loadingBar.Width = 100;
                loadingBar.IsIndeterminate = true;
                indicatorBase.Children.Add(loadingBar);
                indicatorBase.Visibility = Visibility.Visible;
                source.Visibility = Visibility.Hidden;
                parent.Children.Insert(0, indicatorBase);
            }
        }

        private void removeLoadingState(Control source)
        {
            if (source.Parent is Panel)
            {
                Panel parent = source.Parent as Panel;
                parent.Children.RemoveAt(0);
                source.Visibility = Visibility.Visible;
            }
        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (myPlaylistsTab.IsSelected)
            {
                ShowMyPlaylists();
            }
        }

        void ShowMyPlaylists()
        {
            addLoadingState(myPlaylistsScroller);

            loadMoreMyPlaylists.Visibility = Visibility.Visible;
            myPlaylistsContent.Children.Clear();
            LoadMyPlaylistsNextPage();

            removeLoadingState(myPlaylistsScroller);
        }


        void addMyPlaylists(List<MyPlaylist> data)
        {
            int perRow = 4;
            int inLastRow = 0;
            if (myPlaylistsContent.Children.Count > 0)
                inLastRow = (myPlaylistsContent.Children[myPlaylistsContent.Children.Count - 1] as StackPanel).Children.Count;

            bool attachToLastRow = false;
            int take = 0;
            if (inLastRow % perRow != 0)
                take = perRow - inLastRow;
            attachToLastRow = take != 0;

            if (attachToLastRow)
            {
                StackPanel last = (myPlaylistsContent.Children[myPlaylistsContent.Children.Count - 1] as StackPanel);

                foreach (var playlist in data.Take(take))
                {
                    PlaylistInfoControl control = new PlaylistInfoControl();
                    control.Playlist = playlist;
                    last.Children.Add(control);
                }
            }

            int num = -1;
            StackPanel panel = null;
            foreach (var playlist in data.Skip(take))
            {
                num++;
                if (num % perRow == 0)
                {
                    panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;
                }

                PlaylistInfoControl control = new PlaylistInfoControl();
                control.Playlist = playlist;
                panel.Children.Add(control);

                if (num % perRow == perRow - 1)
                    myPlaylistsContent.Children.Add(panel);
            }

            if (num % perRow != perRow - 1)
                myPlaylistsContent.Children.Add(panel);
        }


        public List<MyPlaylist> GetMyPlaylist()
        {
            List<MyPlaylist> myplaylists = new List<MyPlaylist>();
            MyPlaylist playlist1 = new MyPlaylist();


            var service = PhantomJSDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            IWebDriver driver = new PhantomJSDriver(service);
            driver.Url = "http://www.playzer.fr/customer/login";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            driver.FindElement(By.Id("customer_login_login")).SendKeys(Email);
            driver.FindElement(By.Id("customer_login_password")).SendKeys(Pass);
            IWebElement btn = driver.FindElements(By.XPath(".//button[@onclick]"))[1];
            js.ExecuteScript("arguments[0].click();", btn);

          

            // driver.Manage().Cookies.AddCookie(GetCookies());


            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            IWebElement nav_menu = driver.FindElement(By.Id("nav_menu"));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            js.ExecuteScript("arguments[0].click();", nav_menu);
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));


            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='nav_menu']/ul")));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='nav_menu']/ul/div[1]")));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='nav_menu']/ul/div[1]/div[2]")));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.Id("menu_myplaylists")));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

           

            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            
            IReadOnlyCollection<IWebElement> all_playlist = driver.FindElements(By.XPath("//div[@class='content home_playlist transition']"));
            Thread.Sleep(TimeSpan.FromSeconds(2));


            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            int i = 1;
            foreach (IWebElement playlist in all_playlist)
            {

                string id = playlist.GetAttribute("id");
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

               //Thread.Sleep(TimeSpan.FromSeconds(2));
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));
                IReadOnlyCollection<IWebElement> songs = song_items.FindElements(By.ClassName("content"));
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));

                playlist1 = new MyPlaylist();
                playlist1.Title = driver.FindElement(By.XPath("//*[@id='playlist_edit_name']")).GetAttribute("value");
                playlist1.SelectedId = i;

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
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                myplaylists.Add(playlist1);

                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                js.ExecuteScript("arguments[0].click();", driver.FindElement(By.Id("close_modify")));
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='playlist_options']/div[1]/img")));
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                i++;
            }
            driver.Close();
            driver.Dispose();
            driver.Quit();


            PlaylistDetailsWindow.all_playlists = myplaylists;
            myPlaylist = myplaylists;
            return myplaylists;
        }


        public void LoadMyPlaylistsNextPage()
        {

            Cursor = Cursors.Wait;
           
           addMyPlaylists(allPL);

                if (myPlaylist.Count < 50)
                    loadMoreMyPlaylists.Visibility = Visibility.Collapsed;

            cntPlaylist++;
            Cursor = Cursors.Arrow;

        }

        public List<Song> GetSearch()
        {
            List<Song> search_results = new List<Song>();
            IReadOnlyCollection<IWebElement> search_content;
            List<string> search_content_ids = new List<string>();

            var service = PhantomJSDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            IWebDriver driver = new PhantomJSDriver(service);
            driver.Url = "http://www.playzer.fr";

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;


            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='panel_search']/img")));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
              Thread.Sleep(TimeSpan.FromSeconds(1));
            IWebElement search =  driver.FindElement(By.Id("search_engine"));
            search.SendKeys(searchBox.Text);
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
           Thread.Sleep(TimeSpan.FromSeconds(2));
            js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='search_clips_tab']")));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
          // Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            IWebElement result = driver.FindElement(By.Id("search_results"));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
          //  Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            search_content = result.FindElements(By.XPath("//div[@class='content transition search_item_content']"));
         //   Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            int i = 1;

            foreach (IWebElement ids in search_content)
            {
                string song_ID = ids.GetAttribute("id");

                string title = driver.FindElement(By.XPath("//*[@id='" + song_ID + "']/div[2]")).Text;

                string artist = driver.FindElement(By.XPath("//*[@id='" + song_ID + "']/div[3]")).Text;

                string url = driver.FindElement(By.XPath("//*[@id='" + song_ID + "']/div[1]/img[2]")).GetAttribute("src");

                search_results.Add(new Song(title, artist, song_ID,
                url, i));
                i++;
            }

            driver.Close();
            driver.Quit();
            driver.Dispose();

            return search_results;

        }


        private async void loadMoreMyPlaylists_Click(object sender, RoutedEventArgs e)
        {
            LoadMyPlaylistsNextPage();
        }

        private async void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            if (string.IsNullOrEmpty(searchBox.Text.Trim()))
                return;

            searchMusicStack.Children.Clear();
            searchPage = 0;
            lastSearchQuery = searchBox.Text.Trim();
            sb = searchBox.Text;
            LoadNextSearchPage();
            Cursor = Cursors.Arrow;
        }

        void LoadNextSearchPage()
        {
           
            try
            {
           

                foreach (var song in GetSearch())
                {
                    
                    ListItem mc = new ListItem();
                    mc.Track = song;
                    mc.addBtn.Visibility = Visibility.Visible;
                    mc.HorizontalAlignment = HorizontalAlignment.Stretch;
                    searchMusicStack.Children.Add(mc);
                }


             
            }
            finally
            {
             
            }
        }

        public Cookie GetCookies()
        {
            string path1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            string text = File.ReadAllText(Path.Combine(path1,"Cookie.data"));

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

        private  void searchLoadMoreBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadNextSearchPage();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ObservableCollection<TableItem> _playList = new ObservableCollection<TableItem>();
            List<string> lines = new List<string>();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Load CSV";
            ofd.Filter = "CSV file|*.csv";

            ofd.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            bool? result = ofd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                Cursor = Cursors.Wait;


                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    while (sr.Peek() >= 0)
                    {
                        lines.Add(sr.ReadLine());
                    }

                }

                int i = 1;
                foreach (string rows in lines)
                {
                    if (i != 1)
                    {
                        TableItem song = new TableItem(rows.Split(',')[0], rows.Split(',')[1], rows.Split(',')[2],rows.Split(',')[3]);
                        _playList.Add(song);
                    }
                    i++;

                }

                LoadCSV win = new LoadCSV(_playList, ofd.SafeFileName);
                win.ShowDialog();

                Cursor = Cursors.Arrow;
            }
        }


    }
}