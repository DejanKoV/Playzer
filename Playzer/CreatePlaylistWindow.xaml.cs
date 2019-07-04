using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Playzer
{
    /// <summary>
    /// Interaction logic for CreatePlaylistWindow.xaml
    /// </summary>
    public partial class CreatePlaylistWindow : Window
    {
        public CreatePlaylistWindow()
        {
            InitializeComponent();
        }

        public int MusicId { get; set; } 
     

        private async void createBtn_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            List<Song> search_results = new List<Song>();
            List<MyPlaylist> myPL = new List<MyPlaylist>();
            IReadOnlyCollection<IWebElement> search_content;
            List<string> search_content_ids = new List<string>();
            MyPlaylist pl = new MyPlaylist();
            List<MyPlaylist> lmp = new List<MyPlaylist>();

            if (string.IsNullOrEmpty(titleTextbox.Text.Trim()))
            {
                MessageBox.Show("Title can not be blank", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
         

            createBtn.IsEnabled = false;
            if (MusicId!=0)
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

                driver.Manage().Window.Maximize();
                js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='panel_search']/img")));
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                Thread.Sleep(TimeSpan.FromSeconds(5));
                driver.FindElement(By.Id("search_engine")).SendKeys(MainWindow.sb);
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                Thread.Sleep(TimeSpan.FromSeconds(5));
                js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='search_clips_tab']")));
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                // Thread.Sleep(TimeSpan.FromSeconds(2));
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                IWebElement result = driver.FindElement(By.Id("search_results"));
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));


                search_content = result.FindElements(By.XPath("//div[@class='content transition search_item_content']"));
                int i = 1;
             
                foreach (IWebElement ids in search_content)
                {
                    string song_id = ids.GetAttribute("id");
                    if (i == MusicId)
                    {
                        js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='" + song_id + "']")));

                        js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='panel_history']")));
                        string songID = driver.FindElements(By.ClassName("bloc"))[0].GetAttribute("id");
                        js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='" + songID + "_options']")));

                        js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//div[@class='add transition']")));

                        driver.FindElement(By.Id("content_add_form"));
                     
                   
                        break;
                       
                    }
                    i++;
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));

                driver.FindElement(By.XPath("//*[@id='new_playlist']")).SendKeys(titleTextbox.Text);
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                Thread.Sleep(TimeSpan.FromSeconds(2));

                js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='new_playlist_button']")));
                Cursor = Cursors.Arrow;

                driver.Navigate().Refresh();

                js.ExecuteScript("arguments[0].click();", driver.FindElement(By.Id("menu_myplaylists")));
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                IReadOnlyCollection<IWebElement> all_playlist = driver.FindElements(By.XPath("//div[@class='content home_playlist transition']"));
                Thread.Sleep(TimeSpan.FromSeconds(2));

                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
                MyPlaylist playlist1 = new MyPlaylist();

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

                    myPL = LoadPlaylist();
                    myPL.Add(playlist1);

                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                    js.ExecuteScript("arguments[0].click();", driver.FindElement(By.Id("close_modify")));
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

                    js.ExecuteScript("arguments[0].click();", driver.FindElement(By.XPath("//*[@id='playlist_options']/div[1]/img")));
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                    break;
                }

                SaveToFile(myPL);

                MessageBox.Show("Song was added to created playlist", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow.changed = true;
               // MainWindow.allPL = myPL;
                driver.Close();
                driver.Quit();

            }

            createBtn.IsEnabled = true;
            DialogResult = true;
            this.Close();
            
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

    }
}
