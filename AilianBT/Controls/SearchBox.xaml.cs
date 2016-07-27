using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace AilianBT.Controls
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
