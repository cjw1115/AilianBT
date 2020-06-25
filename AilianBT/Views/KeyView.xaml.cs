using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace AilianBT.Views
{
    public sealed partial class KeyView : Page
    {
        public ViewModels.KeyVM KeyVM { get; set; }

        public KeyView()
        {
            var locator = App.Current.Resources["Locator"] as ViewModels.ViewModelLocator;
            KeyVM = locator.KeyVM;
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            this.Loaded += KeyView_Loaded;
        }

        private  void KeyView_Loaded(object sender, RoutedEventArgs e)
        {
            KeyVM.Loaded();
            
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            KeyVM.GridView_ItemClick(e.ClickedItem);
        }
    }
}
