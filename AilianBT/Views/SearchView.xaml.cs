using AilianBT.Helpers;
using AilianBT.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace AilianBT.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchView : Page
    {
        public SearchVM SearchVM { get; set;}
        public SearchView()
        {
            var locator = App.Current.Resources["Locator"] as ViewModelLocator;
            SearchVM = locator.SearchVM;
            this.InitializeComponent();
            this.Loaded += SearchView_Loaded;
            

        }

        private ScrollViewer scrollView;
        public void Init()
        {
            scrollView = Helpers.VisualTreeHelperTool.FindVisualChild<ScrollViewer>(this.listView);
            scrollView.HorizontalScrollMode = ScrollMode.Disabled;
            scrollView.VerticalAlignment = VerticalAlignment.Stretch;
            scrollView.ViewChanged -= ScrollView_ViewChanged;
            scrollView.ViewChanged += ScrollView_ViewChanged;

            scrollView.ChangeView(null, 120, null);
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
                                await SearchVM.LoadMore();
                                IsLoadMore = false;
                            });
                        });
                    }
                }
            }
        }

        private SearchViewParam naviParam;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            naviParam = (SearchViewParam)e.Parameter;
        }
        private async void SearchView_Loaded(object sender, RoutedEventArgs e)
        {
            this.imgbackground.Source = await AssertsHelper.GetRandomBackgroundImage();
            Init();

            if (naviParam!=null&&naviParam.IsTo)
            {
                SearchVM.Loaded(naviParam.SearchKey);
            }

            InitLoadingMoreAnimation();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await SearchVM.LoadMore();
        }

        #region Animation
        private Compositor _compositor => Window.Current.Compositor;
        public void InitLoadingMoreAnimation()
        {
            var angleAnimation = _compositor.CreateScalarKeyFrameAnimation();
            angleAnimation.InsertKeyFrame(0, 0);
            angleAnimation.InsertKeyFrame(1, 360);
            angleAnimation.Duration = TimeSpan.FromSeconds(1);
            angleAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            var visual = ElementCompositionPreview.GetElementVisual(iconLoadingMore);
            visual.CenterPoint = new System.Numerics.Vector3((float)(18), (float)(18), 0);
            visual.StartAnimation("RotationAngleInDegrees", angleAnimation);
        }
        #endregion
    }
    public class SearchViewParam
    {
        public bool IsTo { get; set; } = false;
        public string SearchKey { get; set; }
    }
}
