using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Playzer
{
    public class Populate : IPopulate
    {


        public bool Login(string email1, string password)
        {

            List<MyPlaylist> myplaylists = new List<MyPlaylist>();
            MyPlaylist playlist1 = new MyPlaylist();
            List<MyPlaylist> myPlaylist = new List<MyPlaylist>();

            var service = PhantomJSDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            IWebDriver driver = new PhantomJSDriver(service);
            driver.Url = "http://www.playzer.fr/customer/login";
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            Thread.Sleep(TimeSpan.FromSeconds(1));

            driver.FindElement(By.Id("customer_login_login")).SendKeys(email1);
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            driver.FindElement(By.Id("customer_login_password")).SendKeys(password);
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

            IWebElement btn = driver.FindElements(By.XPath(".//button[@onclick]"))[1];
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", btn);
            Thread.Sleep(TimeSpan.FromSeconds(1));
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));


            driver.Navigate().GoToUrl("http://www.playzer.fr/customer/account");
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));

           
            string s = driver.FindElement(By.XPath("//*[@id='account']/div/h2[1]")).Text;
            Thread.Sleep(TimeSpan.FromSeconds(1));

           driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
            if ("You are not logged in" == s)
            {
                return false;  
            }
            else
            {
               
                if (!File.Exists(email1 + "Playlists.txt"))
                {

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
                    Thread.Sleep(TimeSpan.FromSeconds(1));


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

                         Thread.Sleep(TimeSpan.FromSeconds(1));

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
                    MainWindow.allPL = myplaylists;

                    using (StreamWriter theWriter = new StreamWriter(email1 + "Playlists.txt"))
                    {
                        try
                        {
                            int cntlist = 0;
                           foreach(MyPlaylist mp in myplaylists)
                            {
                                cntlist++;
                                theWriter.WriteLine("PLAYLIST "+ cntlist + "*#" + mp.Title+"*#"+mp.Url);
                                theWriter.WriteLine();
                                int cntSong = 0;
                                foreach(Song song in mp.Songs)
                                {
                                    cntSong++;
                                    theWriter.WriteLine("SONG "+cntSong + "|?" + song.Id + "|?" + song.Title + "|?" + song.Artist + "|?" + song.Url);
                                }
                              
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    using (StreamWriter theWriter = new StreamWriter(email1 + ".txt"))
                    {
                        try
                        {
                            theWriter.Write(password);
                        }
                        catch (Exception ex)
                        {

                        }
                    }




                }
                else
                {
                        List<MyPlaylist> plejliste = new List<MyPlaylist>();
                        string path1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                        string text = File.ReadAllText(Path.Combine(path1, email1 + "Playlists.txt"));

                        string[] plist =  text.Split(new string[] { "PLAYLIST " }, StringSplitOptions.None);

                        foreach(string lists in plist)
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
                }


    



                driver.Quit();
              
                return true;
            }
          
        }

      

    }
}
