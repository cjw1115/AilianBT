using AilianBT.Helpers;
using AilianBT.Services;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace AilianBT.Views
{
    public sealed partial class MainView : Page
    {
        public ViewModels.MainVM MainVM { get; set; }
        public MainView()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
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
            this.imgbackground.Source = await AssertsHelper.GetRandomBackgroundImage();
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
            visual.CenterPoint = new System.Numerics.Vector3((float)(18), (float)(18), 0);
            visual.StartAnimation("RotationAngleInDegrees", angleAnimation);
        }
        #endregion
    }
}