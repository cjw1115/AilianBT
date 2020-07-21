using AilianBT.Models;
using AilianBT.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace AilianBT.Views
{
    public sealed partial class MusicView : Page
    {
        public MusicVM MusicVM { get; private set; } = ViewModelLocator.Instance.MusicVM;

        public MusicView()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            this.Loaded += _musicViewLoaded;
        }
        
        private void _musicViewLoaded(object sender, RoutedEventArgs e)
        {
            MusicVM.Load();
        }

        private void _gridDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var grid=sender as FrameworkElement;
            MusicVM.ItemClicked(grid.DataContext as MusicModel);
        }
    }
}
