using AilianBT.Helpers;
using AilianBT.Services;
using AilianBT.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace AilianBT.Views
{
    public sealed partial class NavigationView : Page
    {
        public NavigationVM NavigationVM { get; private set; } = ViewModelLocator.Instance.NavigationVM;
        private UtilityHelper _utilityHelper = SimpleIoc.Default.GetInstance<UtilityHelper>();
        private LogService _logService = SimpleIoc.Default.GetInstance<LogService>();

        public Controls.Notification Notification { get; set; }

        public NavigationView()
        {
            this.InitializeComponent();
            this.Loaded += _navigationViewLoaded;

            this.lbNavigationList.SelectionChanged += lbNavigationListSelectionChanged;

            NavigationVM.DetailFrame = this.DetailFrame;
            NavigationVM.FuncFrame = this.FuncFrame;

            DetailFrame.Navigated += _detailFrameNavigated;

            Notification = this.notification;

            _customizeTitleBar();
        }

        private void _navigationViewBackRequested(object sender, RoutedEventArgs e)
        {
            DetailFrame.GoBack();
        }

        private void _detailFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType != typeof(DefaultDetailView))
            {
                btnGoBack.Visibility =  Visibility.Visible;
            }
            else
            {
                btnGoBack.Visibility = Visibility.Collapsed;
            }
            _adjustWindowMode();
        }

        private IList<Visual> _selectedIndicatorVisuals = new List<Visual>();
        private void _navigationViewLoaded(object sender, RoutedEventArgs e)
        {
            var _indicators = VisualTreeHelperTool.FindAllNamedVisualChild<Rectangle>(splitView, "SelectedIndicator");
            foreach (var item in _indicators)
            {
                _selectedIndicatorVisuals.Add(ElementCompositionPreview.GetElementVisual(item));
            }

            _adjustWindowMode();
        }

        private void btnHamberClicked(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void lbNavigationFooterListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.RemovedItems.Count <= 0)
            {
                lbNavigationList.SelectedIndex = -1;

                NavigationVM.SettingClicked();
            }
        }

        private void lbNavigationListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.RemovedItems.Count <= 0)
            {
                lbNavigationFooterList.SelectedIndex = -1;
                return;
            }

            if (e.AddedItems.Count <= 0 || e.RemovedItems.Count <= 0)
                return;

            var newIndx = lbNavigationList.Items.IndexOf(e.AddedItems[0]);
            var oldIndex = lbNavigationList.Items.IndexOf(e.RemovedItems[0]);

            var oldItem = lbNavigationList.ContainerFromIndex(oldIndex) as FrameworkElement;
            var newItem = lbNavigationList.ContainerFromIndex(newIndx) as FrameworkElement;

            var transform = oldItem.TransformToVisual(newItem);
            var point = transform.TransformPoint(new Windows.Foundation.Point(0, 0));

            Compositor _compositor = Window.Current.Compositor;
            var offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();

            var visual = _selectedIndicatorVisuals[newIndx];
            var finalOffset = visual.Offset;
            var beginOffset = visual.Offset + new System.Numerics.Vector3((float)point.X, (float)point.Y, 0);
            offsetAnimation.InsertKeyFrame(0, beginOffset);
            offsetAnimation.InsertKeyFrame(1, finalOffset);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(600);

            visual.StartAnimation("Offset", offsetAnimation);
        }

        private void _customizeTitleBar()
        {
            _logService.Debug("Try to use customized title bar");
            CoreApplication.MainView.TitleBar.LayoutMetricsChanged += (o, e) =>
            {
                panelTitilBar.Height = o.Height;
            };
            Window.Current.SetTitleBar(_gridDragBar);
        }

        private void _adjustWindowMode()
        {
            var viewState = $"{_utilityHelper.GetWindowMode()}VisualState";
            _logService.Debug($"Set view state to {viewState}");
            // Here is an appointment, the view state name should be "{WindowMode}VisualState"
            VisualStateManager.GoToState(this, viewState, false);
        }
    }
}
