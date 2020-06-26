using AilianBT.Helpers;
using AilianBT.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AilianBT.Views
{
    public sealed partial class SettingView : Page
    {
        public SettingViewModel SettingVM { get; private set; } = ViewModelLocator.Instance.SettingVM;

        public SettingView()
        {
            this.InitializeComponent();
            this.Loaded += _settingViewLoaded;
        }

        private async void _settingViewLoaded(object sender, RoutedEventArgs e)
        {
            imgbackground.Source = await AssertsHelper.GetRandomBackgroundImage();
            SettingVM.Load();
        }
    }
}
