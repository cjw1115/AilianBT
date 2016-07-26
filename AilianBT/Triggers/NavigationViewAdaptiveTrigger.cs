
using AilianBT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace AilianBT.Triggers
{
    class NavigationViewAdaptiveTrigger : StateTriggerBase
    {
        public double MinWindowWidth
        {
            get { return (double)GetValue(MinWindowWidthProperty); }
            set { SetValue(MinWindowWidthProperty, value); }
        }
        public static DependencyProperty MinWindowWidthProperty = DependencyProperty.Register("MinWindowWidth", typeof(double), typeof(NavigationViewAdaptiveTrigger), new PropertyMetadata(null));
        public NavigationViewAdaptiveTrigger()
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (Window.Current.Bounds.Width > MinWindowWidth && (MinWindowWidth!=0||NavigationVM.DetailFrame.BackStackDepth>=1))
            {
                this.SetActive(true);
            }
            else
            {
                this.SetActive(false);
            }
        }
    }
}
