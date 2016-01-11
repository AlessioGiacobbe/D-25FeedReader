﻿using System;
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
    }

    public partial class MainPage : PhoneApplicationPage
    {
        // builder
        int context = 0;
        int rectcontext = 0;
        double[] startXy = new double[2];
        int contanimation = 1;
        int contanimation_return = 0;
        bool IsNew;
        bool IsLateralOn;
        DispatcherTimer Time = new DispatcherTimer();
        string oldtitle;
        string bau;



        string html = String.Empty;
        public class Record
        {
            public string src { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
            Request();                                      //request the feed's list

            Time.Interval = new TimeSpan(0,0,0,0,30);       //initialize the animation's timer
            Time.Tick += new EventHandler(Timer_Tick);
            Time.Start();

        }

        private void Timer_Tick(object sender, EventArgs e) 
        {
            //it's the bets way i found to let the animation start without waiting the end of the scrolling
            
         var scrollViewer = (ScrollViewer)Scroller;
         if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
         {
             System.Diagnostics.Debug.WriteLine("end of scroller");
         }

         if (scrollViewer.VerticalOffset > 80)
             {
                 if (contanimation == 1)
                 {
                     trasformX.Begin();
                     trasformY.Begin();
                 }
             }
             else
             {
                 if (contanimation_return == 0)
                 {
                     backX.Begin();
                     backY.Begin();
                 }
             }
         
        }

        string CutString(string ArtName, int num)
        {
            if(ArtName.Length> num) { 
            ArtName = ArtName.Substring(0, Math.Min(num, ArtName.Length));
                ArtName = ArtName + "...";
                    }
            return ArtName;

        }

        void CreateNew(string ArtName, System.DateTime currentdate, string creator, string description, string link)
        {

            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                
                //Title
                TextBlock text = new TextBlock();
                ArtName = CutString(ArtName, 38);
                text.Text = ArtName;
                text.FontFamily = new System.Windows.Media.FontFamily("Segoe WP");
                text.Foreground = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
                text.FontSize = 25;
                Thickness margin = text.Margin;
                margin.Left = 20;
                margin.Top = 155 + context;
                text.Margin = margin;
                
                //grey rectangle
                Rectangle rect = new Rectangle();
                rect.Height = 173;
                rect.Width = 450;
                Thickness margin2 = rect.Margin;
                margin2.Left = -20;
                margin2.Top = -1052 + rectcontext;
                rect.Margin = margin2;
                rect.Fill = new SolidColorBrush(Color.FromArgb(200, 244, 244 , 245));
                rect.Stroke = new SolidColorBrush(Color.FromArgb(00, 00, 00, 00));
                
                //lateral gray rectangle
                Rectangle lateralRect = new Rectangle();
                lateralRect.Height = 173;
                lateralRect.Width = 10;
                Thickness margin3 = lateralRect.Margin;
                margin3.Left = -452;
                margin3.Top = -1052 + rectcontext;
                lateralRect.Margin = margin3;
                lateralRect.Fill = new SolidColorBrush(Color.FromArgb(250, 104, 104, 104));
                lateralRect.Stroke = new SolidColorBrush(Color.FromArgb(00, 00, 00, 00));

                //time and date
                TextBlock date = new TextBlock();
                date.Text = (creator + " " + currentdate.Day + "/" + currentdate.Month + "/" + currentdate.Year + " " + currentdate.Hour + ":" + currentdate.Minute);
                date.FontFamily = new System.Windows.Media.FontFamily("Segoe WP");
                date.Foreground = new SolidColorBrush(Color.FromArgb(200, 15, 15, 15));
                date.FontSize = 17.5;
                Thickness margin4 = date.Margin;
                margin4.Left = 20;
                margin4.Top = 190 + context;
                date.Margin = margin4;

                //Description
                description = getBetween(description, "<p>", "</p>");
                description = SpliceText(description, 46);
                description = CutString(description, 170);
                TextBlock Description = new TextBlock();
                Description.Text = description;
                Description.FontFamily = new System.Windows.Media.FontFamily("Segoe WP");
                Description.Foreground = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
                Description.FontSize = 20;
                Thickness margin5 = Description.Margin;
                margin5.Left = 20;
                margin5.Top = 215 + context;
                Description.Margin = margin5;
                System.Diagnostics.Debug.WriteLine(description);

                //add everything to the main grid       note: first you add will be deeper, and so on..
                griglia.Children.Add(rect);
                griglia.Children.Add(lateralRect);
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

        private void lateral_tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            translateral.Begin();
            translprinc.Begin();
            transtitle.Begin();
        }

        private void GestureStart(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            startXy[0] = e.ManipulationOrigin.X;
            startXy[1] = e.ManipulationOrigin.Y;
        }


        private void GestureEnd(object sender, ManipulationDeltaEventArgs e)
        {
            if ((startXy[1] - e.ManipulationOrigin.Y) > -3)
            {
                if (IsLateralOn == false) { 
                translateral.Begin(); 
                translprinc.Begin();
                transtitle.Begin();
                IsLateralOn = true;
                }
                System.Diagnostics.Debug.WriteLine("a");
            }
            else
            {
                if (IsLateralOn == true)
                {
                    translateralback.Begin();
                    translprincback.Begin();
                    transtitleback.Begin();
                    
                IsLateralOn = false;
                }

                System.Diagnostics.Debug.WriteLine("b");
            }
           
            System.Diagnostics.Debug.WriteLine((startXy[1] - e.ManipulationOrigin.Y));
        }

        private void LateralTransComplete(object sender, EventArgs e)
        {
            IsLateralOn = true;
        }

        

        
    }
}