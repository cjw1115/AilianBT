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
        HttpRequestMessage request = null;
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
            scrollView.ViewChanged-= ScrollView_ViewChanged;
            scrollView.ViewChanged += ScrollView_ViewChanged;
            scrollView.ChangeView(null, 60, null);

            OnRefresh += MainVM.Refresh;
            OnLoadMore += MainVM.LoadMore;
        }

        public event Action OnRefresh;
        public event Action OnLoadMore;

        private bool isLoadMore = false;
        private bool isRefresh = false;
        private void ScrollView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrollView = sender as ScrollViewer;
            if (!e.IsIntermediate)
            {
                if (scrollView.VerticalOffset <=0)
                { //刷新
                    if (!isRefresh)
                    {
                        isRefresh = true;
                        Task.Run(async () =>
                        {
                            await scrollView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                OnRefresh.Invoke();
                                scrollView.ChangeView(null, 60, null);
                                
                            });
                            isRefresh = false;
                        });
                    }
                   
                    
                    
                }
                else if(scrollView.VerticalOffset < 60)
                {
                    scrollView.ChangeView(null, 60, null);
                }
                else if (scrollView.VerticalOffset > scrollView.ScrollableHeight-40)
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

        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            MainVM.Loaded();
            Init();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnLoadMore.Invoke();
        }

        
    }
}