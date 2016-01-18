﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp2.Resources;

namespace PhoneApp2
{

    public partial class BrowserPage : PhoneApplicationPage
    {
        string link; 
        public BrowserPage()
        {
            link = (App.Current as App).link_pubblic;
            System.Diagnostics.Debug.WriteLine(link);
            //testo.Text = link;
            InitializeComponent();

            this.Browser.Loaded += BrowserGo;
        }

        private void BrowserGo(object sender, RoutedEventArgs e)
        {
            Browser.Navigate(new Uri(link));
            Browser.LoadCompleted += PageLoaded;
        }

        private void PageLoaded(object sender, NavigationEventArgs e)
        {
            MessageBox.Show("Pagina caricata");
        }
    }
}