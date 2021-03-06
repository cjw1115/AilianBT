﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AilianBT.Views.Controls
{
    public sealed partial class SearchBox : UserControl
    {
        public SearchBox()
        {
            this.InitializeComponent();
            this.btnSearch.Click += BtnSearch_Click;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            string key = this.tbSearch.Text.Trim();
            if (key != null)
            {
                SearchKey = key;
                Search.Invoke(key);
            }
        }

        public string SearchKey
        {
            get { return (string)this.GetValue(SearchKeyProperty); }
            set { SetValue(SearchKeyProperty, value); }
        }
        public static readonly DependencyProperty SearchKeyProperty = DependencyProperty.Register("SearchKey", typeof(string), typeof(SearchBox),new PropertyMetadata(null,(o,e)=> {
            SearchBox searchbox = o as SearchBox;
            searchbox.SearchKey = e.NewValue as string;
        }));

        public bool IsVisible
        {
            get { return (bool)this.GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof(bool), typeof(SearchBox), new PropertyMetadata(false, (o, e) =>
         {
             var searchbox = o as SearchBox;
             if (searchbox.IsVisible)
                 searchbox.Visibility = Visibility.Visible;
             else
                 searchbox.Visibility = Visibility.Collapsed;
         }));

        public event Action<string> Search;
    }
}
