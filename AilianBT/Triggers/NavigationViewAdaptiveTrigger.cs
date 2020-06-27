using AilianBT.Helpers;
using AilianBT.Models;
using AilianBT.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Xaml;

namespace AilianBT.Triggers
{
    public class NavigationViewAdaptiveTrigger : StateTriggerBase
    {
        private UtilityHelper _utilityHelper = SimpleIoc.Default.GetInstance<UtilityHelper>();

        public WindowMode Mode { get; set; }

        public NavigationViewAdaptiveTrigger()
        {
            Window.Current.SizeChanged += _windowSizeChanged;
        }

        private void _windowSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            SetActive(_utilityHelper.GetWindowMode() == Mode);
        }
    }
}
