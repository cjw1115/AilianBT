using AilianBT.ViewModels;
using BtDownload.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        
        public void Init()
        {
            var scrollView = Helpers.VisualTreeHelperTool.FindVisualChild<ScrollViewer>(this.listView);
            scrollView.HorizontalScrollMode = ScrollMode.Disabled;
            scrollView.VerticalAlignment = VerticalAlignment.Stretch;
            scrollView.ViewChanged -= ScrollView_ViewChanged;
            scrollView.ViewChanged += ScrollView_ViewChanged;
            scrollView.Loaded += ScrollView_Loaded;

            scrollView.ChangeView(null, 120, null);

            OnRefresh += SearchVM.Refresh;
            OnLoadMore += SearchVM.LoadMore;
        }

        private void ScrollView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public event Action OnRefresh;
        public event Action OnLoadMore;

        private bool isLoadMore = false;
        private bool isRefresh = false;
        private ProgressRing _progressRing => Helpers.VisualTreeHelperTool.FindVisualChild<ProgressRing>(this.listView);
        private void ScrollView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollView = sender as ScrollViewer;
            if (!e.IsIntermediate)
            {
                if (scrollView.VerticalOffset <= 0)
                { //刷新
                    if (!isRefresh)
                    {
                        _progressRing.IsActive = true;
                        Task.Run(async () =>
                        {
                            await scrollView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                OnRefresh.Invoke();
                                scrollView.ChangeView(null, 120, null);

                            });
                            isRefresh = false;
                            _progressRing.IsActive = false;
                        });
                    }

                }
                else if (scrollView.VerticalOffset < 120)
                {
                    scrollView.ChangeView(null, 120, null);
                }
                else if (scrollView.VerticalOffset > scrollView.ScrollableHeight - 40)
                {
                    if (!isLoadMore)
                    {
                        Task.Run(async () =>
                        {
                            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                OnLoadMore.Invoke();

                            });
                            isLoadMore = false;
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
            this.imgbackground.Source = await FileService.GetBackgroundImage();
            Init();

            if (naviParam!=null&&naviParam.IsTo)
            {
                SearchVM.Loaded(naviParam.SearchKey);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnLoadMore.Invoke();
        }
    }
    public class SearchViewParam
    {
        public bool IsTo { get; set; } = false;
        public string SearchKey { get; set; }
    }
}
