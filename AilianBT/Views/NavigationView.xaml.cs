using AilianBT.Helpers;
using AilianBT.ViewModels;
using System;
using System.Collections.Generic;
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
        public Controls.Notification Notification { get; set; }

        public NavigationView()
        {
            this.InitializeComponent();
            this.Loaded += _navigationViewLoaded;

            this.lbNavigationList.SelectionChanged += lbNavigationListSelectionChanged;

            NavigationVM.DetailFrame = this.DetailFrame;
            NavigationVM.FuncFrame = this.FuncFrame;

            DetailFrame.Navigated += _detailFrameNavigated;

            SystemNavigationManager.GetForCurrentView().BackRequested += _navigationViewBackRequested;

            Notification = this.notification;
        }

        private void _navigationViewBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.DetailFrame.BackStackDepth > 1)
            {
                this.DetailFrame.GoBack();
                e.Handled = true;
            }
            else if (this.DetailFrame.BackStackDepth == 1)
            {
                this.DetailFrame.GoBack();
                e.Handled = true;
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                var currentState = ShowModeVisualStateGroup.CurrentState;
                if (currentState == null || currentState == NarrowVisualState || currentState == NormalVisualState)
                {
                    VisualStateManager.GoToState(this, "NormalVisualState", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "WideVisualState", false);
                }
            }
            else
            {
            }
        }

        private void _detailFrameNavigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType != typeof(Views.DefaultDetailView))
            {
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                var currentState = ShowModeVisualStateGroup.CurrentState;
                if (currentState == null || currentState == NarrowVisualState || currentState == NormalVisualState)
                {
                    VisualStateManager.GoToState(this, "NarrowVisualState", false);
                }
            }
        }


        private IList<Visual> _selectedIndicatorVisuals = new List<Visual>();
        private void _navigationViewLoaded(object sender, RoutedEventArgs e)
        {
            var _indicators = VisualTreeHelperTool.FindAllNamedVisualChild<Rectangle>(splitView, "SelectedIndicator");
            foreach (var item in _indicators)
            {
                _selectedIndicatorVisuals.Add(ElementCompositionPreview.GetElementVisual(item));
            }
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
    }
}
