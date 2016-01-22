using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Text;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Xml.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Phone.Shell;
using PhoneApp2.Resources;

namespace PhoneApp2
{

    public partial class App : Application
    {
        public string link_pubblic { get; set; }
        public string name_pubblic { get; set; }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        // builder
        int context = 0;
        int rectcontext = 0;
        int contanimation = 1;
        int contanimation_return = 0;
        bool IsNew;
        bool IsLateralOn;
        DispatcherTimer Time = new DispatcherTimer();
        string oldtitle;

        Thickness StandardMargin = new Thickness(); //decided to don't put pages in phone margin and align these after app's startup so i can edit it better
        

        public class Record
        {
            public string src { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();

            StandardMargin.Left = 6;
            StandardMargin.Top = 0;
            Autor.Margin = StandardMargin;

            Request();                                      //request the feed's list

            Time.Interval = new TimeSpan(0, 0, 0, 0, 30);       //initialize the animation's timer
            Time.Tick += new EventHandler(Timer_Tick);
            Time.Start();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //it's the bets way i found to let the animation start without waiting the end of the scrolling

            var scrollViewer = (ScrollViewer)Scroller;
            /*if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
            {
                System.Diagnostics.Debug.WriteLine("end of scroller");
            }*/

            if (scrollViewer.VerticalOffset > 50)
            {

                if (contanimation == 1)
                {
                    uptit.Begin();
                    titlerectin.Begin();
                }
            }
            else
            {
                if (contanimation_return == 0)
                {
                    downtit.Begin();
                    titlerectout.Begin();
                }
            }

        }

        string CutString(string ArtName, int num)
        {
            if (ArtName.Length > num) {
                ArtName = ArtName.Substring(0, Math.Min(num, ArtName.Length));
                ArtName = ArtName + "...";
            }
            return ArtName;

        }

        void CreateNew(string ArtName, System.DateTime currentdate, string creator, string description, string link)
        {

            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                string imageurl = description;
                //Title
                TextBlock text = new TextBlock();
                ArtName = CutString(ArtName, 38);
                text.Text = ArtName;
                text.FontFamily = new System.Windows.Media.FontFamily("Segoe WP Bold");
                text.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                text.FontSize = 26;
                Thickness margin = text.Margin;
                margin.Left = 20;
                margin.Top = 252 + context;
                text.Margin = margin;

                //grey rectangle
                Rectangle rect = new Rectangle();
                rect.Height = 85;
                rect.Width = 450;
                Thickness margin2 = rect.Margin;
                margin2.Left = 0;
                margin2.Top = -935 + rectcontext;
                rect.Margin = margin2;
                rect.Fill = new SolidColorBrush(Color.FromArgb(70, 0, 0, 0));
                rect.Stroke = new SolidColorBrush(Color.FromArgb(00, 00, 00, 00));

                //time and date
                TextBlock date = new TextBlock();
                date.Text = (creator + " " + currentdate.Day + "/" + currentdate.Month + "/" + currentdate.Year + " " + currentdate.Hour + ":" + currentdate.Minute);
                date.FontFamily = new System.Windows.Media.FontFamily("Segoe WP light");
                date.Foreground = new SolidColorBrush(Color.FromArgb(200, 245, 245, 245));
                date.FontSize = 15;
                Thickness margin4 = date.Margin;
                margin4.Left = 21;
                margin4.Top = 282 + context;
                date.Margin = margin4;

                //Description
                description = getBetween(description, "<p>", "</p>");
                description = SpliceText(description, 47);
                description = CutString(description, 85);
                TextBlock Description = new TextBlock();
                Description.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                Description.LineHeight = 18;
                Description.Text = description;
                Description.FontFamily = new System.Windows.Media.FontFamily("Segoe WP Bold");
                Description.Foreground = new SolidColorBrush(Color.FromArgb(235, 235, 235, 235));
                Description.FontSize = 19;
                Thickness margin5 = Description.Margin;
                margin5.Left = 21;
                margin5.Top = 302 + context;
                Description.Margin = margin5;
                System.Diagnostics.Debug.WriteLine(description);

                //image
                imageurl = getBetween(imageurl, "src=\"", "\"");
                System.Diagnostics.Debug.WriteLine("url dellimmagine : " + imageurl);
                Image image = new Image();
                image.Height = 203;
                image.Width = 450;
                image.Stretch = Stretch.Fill;
                Thickness margin6 = image.Margin;
                margin6.Left = 0;
                margin6.Top = -1052 + rectcontext;
                image.Margin = margin6;
                image.Source = new BitmapImage(new Uri(imageurl));

                //add everything to the main grid       note: first you add will be deeper, and so on..

                griglia.Children.Add(image);
                griglia.Children.Add(rect);
                //griglia.Children.Add(lateralRect);
                griglia.Children.Add(text);
                griglia.Children.Add(date);
                griglia.Children.Add(Description);

                text.Tag = link;
                text.Tap += new EventHandler<GestureEventArgs>(ArtTapped);

                rectcontext = rectcontext + 460;
                context = context + 230;

            });


        }

        private void ArtTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (App.Current as App).link_pubblic = ((string)((TextBlock)sender).Tag);
            (App.Current as App).name_pubblic = ((string)((TextBlock)sender).Text);
            MessageBox.Show((string)((TextBlock)sender).Tag);
            NavigationService.Navigate(new Uri("/BrowserPage.xaml", UriKind.Relative));
        }



        public static string SpliceText(string text, int lineLength)
        {
            var charCount = 0;
            var lines = text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                            .GroupBy(w => (charCount += w.Length + 1) / lineLength)
                            .Select(g => string.Join(" ", g));

            return String.Join("\n", lines.ToArray());
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }




        void Request()
        {
            var link = new Uri("http://d-25.net/feed");
            var request = (HttpWebRequest)WebRequest.Create(link);
            request.Method = "GET";
            request.BeginGetResponse(Responsetocken, request);
        }


        void Responsetocken(IAsyncResult result)
        {
            try
            {
                HttpWebRequest richiesta = (HttpWebRequest)result.AsyncState;
                HttpWebResponse risposta = (HttpWebResponse)richiesta.EndGetResponse(result);

                using (StreamReader stream = new StreamReader(risposta.GetResponseStream()))
                {
                    string xml = stream.ReadToEnd();
                    var document = XDocument.Parse(xml);
                    XNamespace dcNamespace = "http://purl.org/dc/elements/1.1/";


                    var posts = (from p in document.Descendants("item")
                                 select new
                                 {

                                     Title = p.Element("title").Value,
                                     Link = p.Element("link").Value,
                                     Description = p.Element("description").Value,
                                     Comments = p.Element("comments").Value,
                                     creator = p.Element(dcNamespace + "creator").Value,
                                     PubDate = DateTime.Parse(p.Element("pubDate").Value)
                                 }).ToList();

                    foreach (var post in posts)
                    {

                        System.Diagnostics.Debug.WriteLine(post.Title);
                        System.Diagnostics.Debug.WriteLine(post.Link);
                        System.Diagnostics.Debug.WriteLine(post.Comments);
                        System.Diagnostics.Debug.WriteLine(post.PubDate);
                        System.Diagnostics.Debug.WriteLine(post.Description);
                        System.Diagnostics.Debug.WriteLine(post.creator);
                        IsNew = CheckIfNew(post.Title);
                        if (IsNew == true)
                        {
                            CreateNew(post.Title, post.PubDate, post.creator, post.Description, post.Link);
                        }
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                Dispatcher.BeginInvoke(() =>
               ErrorLoading()

                );
            }
        }

        void ErrorLoading()
        {
            ErrorGrid.Visibility = Visibility.Visible;
            ErrorGrid.IsHitTestVisible = true;
            ErrorIn.Begin();
        }

        bool CheckIfNew(string currentitle)
        {

            if (currentitle != oldtitle)
            {
                oldtitle = currentitle;
                return true;
            }
            else {
                oldtitle = currentitle;
                return false;
            }
        }



        private void transform_complete(object sender, EventArgs e)
        {
            contanimation = 0;
            contanimation_return = 0;
        }

        private void back_complete(object sender, EventArgs e)
        {
            contanimation_return = 1;
            contanimation = 1;
        }


        private void LateralTransComplete(object sender, EventArgs e)
        {
            IsLateralOn = true;
        }

        private void barpress_complete(object sender, EventArgs e)
        {
            rotate.Begin();
        }

        private void rotateback_complete(object sender, EventArgs e)
        {
            barpressback.Begin();
        }

        private void HamButton_tap(object sender, GestureEventArgs e)
        {
            if (IsLateralOn == false)
            {
                lateralOn();
            }
            else {
                lateralOff();
            }
        }

        void lateralOn()
        {
            titlerectout.Begin();
            translateral.Begin();
            translprinc.Begin();
            transtitle.Begin();
            IsLateralOn = true;
            barpress.Begin();
            Time.Stop();
            uptit.Begin();
            titleleft.Begin();
        }

        void lateralOff()
        {
            translateralback.Begin();
            translprincback.Begin();
            transtitleback.Begin();
            rotateback.Begin();
            IsLateralOn = false;
            titleright.Begin();
            Time.Start();

        }

        private void ReloadPage(object sender, GestureEventArgs e)
        {
            ErrorOut.Begin();
            ErrorGrid.Visibility = Visibility.Collapsed;
            ErrorGrid.IsHitTestVisible = false;
            Request();

        }

        void FocusChange()
        {
            Autor.Visibility = Visibility.Collapsed;
            //software.Visibility = Visibility.Collapsed;
            Scroller.Visibility = Visibility.Collapsed;
            //about.Visibility = Visibility.Collapsed;

            Autor.IsHitTestVisible = false;
            //software.IsHitTestVisible = false;
            Scroller.IsHitTestVisible = false;
            //about.IsHitTestVisible = false;
        }

        private void AutorChange(object sender, GestureEventArgs e)
        {
            FocusChange();
            Autor.Visibility = Visibility.Visible;
            Autor.IsHitTestVisible = true;

            lateralOff();
        }

        private void NewsTapped(object sender, GestureEventArgs e)
        {
            FocusChange();
            Scroller.Visibility = Visibility.Visible;
            Scroller.IsHitTestVisible = true;

            lateralOff();
        }
        
        #region Feedreader Links
        private void OpenBrowser(string url)
        {
            Microsoft.Phone.Tasks.WebBrowserTask WebBrowserTask = new Microsoft.Phone.Tasks.WebBrowserTask();
            WebBrowserTask.Uri = new Uri(url);
            WebBrowserTask.Show();
        }

        private void Github_click1(object sender, GestureEventArgs e)
        {
            OpenBrowser("https://github.com/D-25/");
        }

        private void Github_click2(object sender, GestureEventArgs e)
        {
            OpenBrowser("https://www.github.com/AlessioGiacobbe");
        }

        private void GPlus_Click1(object sender, GestureEventArgs e)
        {
            OpenBrowser("https://plus.google.com/100725403409398223348/posts");
        }

        private void GPlus_Click2(object sender, GestureEventArgs e)
        {
            OpenBrowser("https://plus.google.com/+AlessioGiacobbeProfile/posts");
        }

        private void Fb_Click1(object sender, GestureEventArgs e)
        {
            OpenBrowser("https://www.facebook.com/d25social");
        }

        private void Twitter_Click1(object sender, GestureEventArgs e)
        {
            OpenBrowser("https://twitter.com/giacobbealessi0");
        }

        private void Twitter_Click2(object sender, GestureEventArgs e)
        {
            OpenBrowser("https://twitter.com/D_25T");
        }

        private void GPlus_Click3(object sender, GestureEventArgs e)
        {
            OpenBrowser("https://plus.google.com/u/0/103048757221183397796/posts");
        }

        private void Fb_Click2(object sender, GestureEventArgs e)
        {
            OpenBrowser("https://www.facebook.com/giovanni.sganga.75?fref=ts");
        }

        #endregion
        
    }
}