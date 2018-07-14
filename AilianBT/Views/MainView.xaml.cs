using BtDownload.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace AilianBT.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainView : Page
    {
        public ViewModels.MainVM MainVM { get; set; }
        public MainView()
        {
            var locator = App.Current.Resources["Locator"] as ViewModels.ViewModelLocator;
            MainVM = locator.MainVM;
            this.InitializeComponent();
            this.Loaded += MainView_Loaded;
        }

      
        public void Init()
        {
            var scrollView = Helpers.VisualTreeHelperTool.FindVisualChild<ScrollViewer>(this.listView);
            scrollView.HorizontalScrollMode = ScrollMode.Disabled;
            scrollView.VerticalAlignment = VerticalAlignment.Stretch;
            scrollView.ViewChanged -= ScrollView_ViewChanged;
            scrollView.ViewChanged += ScrollView_ViewChanged;
        }
        private bool _isLoadMore = false;
        public bool IsLoadMore
        {
            get => _isLoadMore;
            set
            {
                if (value == true)
                {
                    iconNeedMore.Visibility = Visibility.Collapsed;
                    iconLoadingMore.Visibility = Visibility.Visible;
                }
                else
                {
                    iconNeedMore.Visibility = Visibility.Visible;
                    iconLoadingMore.Visibility = Visibility.Collapsed;
                }
            }
        }
        private ProgressRing _progressRing => Helpers.VisualTreeHelperTool.FindVisualChild<ProgressRing>(this.listView);
        private void ScrollView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollView = sender as ScrollViewer;
            if (!e.IsIntermediate)
            {
                if (scrollView.VerticalOffset > scrollView.ScrollableHeight - 40)
                {
                    if (!IsLoadMore)
                    {
                        Task.Run(async () =>
                        {
                            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                            {
                                IsLoadMore = true;
                                await MainVM.LoadMore();
                                IsLoadMore = false;
                            });
                            
                        });
                    }
                }
            }
        }


        private async void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            this.imgbackground.Source = await FileService.GetBackgroundImage();
            MainVM.Loaded();
            Init();

            InitLoadingMoreAnimation();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await MainVM.LoadMore();
        }
        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await MainVM.Refresh();
        }

        #region Animation
        private Compositor _compositor => Window.Current.Compositor;
        public void InitLoadingMoreAnimation()
        {
            var angleAnimation= _compositor.CreateScalarKeyFrameAnimation();
            angleAnimation.InsertKeyFrame(0, 0);
            angleAnimation.InsertKeyFrame(1, 360);
            angleAnimation.Duration = TimeSpan.FromSeconds(1);
            angleAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            var visual=ElementCompositionPreview.GetElementVisual(iconLoadingMore);
            visual.CenterPoint = new System.Numerics.Vector3((float)(iconLoadingMore.ActualWidth / 2), (float)(iconLoadingMore.ActualHeight/ 2f), 0);
            visual.StartAnimation("RotationAngleInDegrees", angleAnimation);
        }
        #endregion
    }
}