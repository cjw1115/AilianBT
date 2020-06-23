using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace AilianBT.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NavigationView : Page
    {
        public ViewModels.NavigationVM NavigationVM { get; set; }
        public Controls.Notification Notification { get; set; }

        public NavigationView()
        {

            this.InitializeComponent();
            this.Loaded += NavigationView_Loaded;
            this.btnAbout.Click += BtnAbout_Click;
            var locator = Application.Current.Resources["Locator"] as ViewModels.ViewModelLocator;
            NavigationVM = locator.NavigationVM;
            ViewModels.NavigationVM.DetailFrame = this.DetailFrame;
            ViewModels.NavigationVM.FuncFrame = this.FuncFrame;


            DetailFrame.Navigated += DetailFrame_Navigated;

            SystemNavigationManager.GetForCurrentView().BackRequested += NavigationView_BackRequested;

            Notification = this.notification;
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            this.splitView.IsPaneOpen = false;
        }

        private void NavigationView_BackRequested(object sender, BackRequestedEventArgs e)
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

        private void btnHamber_Click(object sender, RoutedEventArgs e)
        {
            this.splitView.IsPaneOpen = !this.splitView.IsPaneOpen;
        }

        private void DetailFrame_Navigated(object sender, NavigationEventArgs e)
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

        Grid paneGrid;
        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            var findre = AilianBT.Helpers.VisualTreeHelperTool.FindNamedVisualChild<FrameworkElement>(this.splitView, "PaneRoot");
            paneGrid = findre as Grid;
            trans = paneGrid.RenderTransform as CompositeTransform;


            findre.ManipulationCompleted += Pane_ManipulationCompleted;
            findre.ManipulationMode = ManipulationModes.TranslateX;
            findre.ManipulationDelta += Pane_ManipulationDelta;


            var grid = Helpers.VisualTreeHelperTool.FindVisualChild<Grid>(splitView);
            var group = VisualStateManager.GetVisualStateGroups(grid).ToList();
            var transitons = group[0].Transitions;
            if (transitons != null)
            {
                var visualTrans = transitons.Where(m => m.From == "Closed" && m.To == "OpenOverlayLeft").FirstOrDefault();
                close_overlay = visualTrans;

                visualTrans = transitons.Where(m => m.From == "OpenOverlayLeft" && m.To == "Closed").FirstOrDefault();
                overlay_close = visualTrans;
            }

        }

        private void Pane_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;

            Debug.WriteLine("TranslateX:" + trans.TranslateX);
        }

        private void Pane_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.Cumulative.Translation.X < 0 && -e.Cumulative.Translation.X >= splitView.OpenPaneLength / 4)
            {
                splitView.IsPaneOpen = false;
                paneGrid.Visibility = Visibility.Collapsed;
                trans = paneGrid.RenderTransform as CompositeTransform;
                trans.TranslateX = 0;
            }
        }

        VisualTransition close_overlay;
        VisualTransition overlay_close;
        CompositeTransform trans = new CompositeTransform();
        private void swipeBorder_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;
            if (splitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                if (e.Cumulative.Translation.X > 0 && e.Cumulative.Translation.X <= splitView.OpenPaneLength)
                {

                    paneGrid.Visibility = Visibility.Visible;

                    trans = paneGrid.RenderTransform as CompositeTransform;
                    trans.TranslateX = (e.Cumulative.Translation.X - splitView.OpenPaneLength);

                    Debug.WriteLine("TranslateX:" + trans.TranslateX);
                }

            }
        }

        private void swipeBorder_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;
            if (splitView.DisplayMode == SplitViewDisplayMode.Overlay && paneGrid != null)
            {
                paneGrid.Visibility = Visibility.Collapsed;
                //paneGrid.Visibility = Visibility.Collapsed;
                if (e.Cumulative.Translation.X > splitView.OpenPaneLength / 2.0)
                {

                    splitView.IsPaneOpen = true;
                    //paneGrid.Visibility = Visibility.Visible ;

                    close_overlay?.Storyboard?.SkipToFill();
                    trans = paneGrid.RenderTransform as CompositeTransform;
                    trans.TranslateX = 0;
                }
                else
                {

                    trans = paneGrid.RenderTransform as CompositeTransform;
                    trans.TranslateX = 0;
                }
                //trans = paneGrid.RenderTransform as CompositeTransform;
                //trans.TranslateX = 0;
            }


        }

        private void navigationList_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.splitView.IsPaneOpen = false;
        }
    }
}
