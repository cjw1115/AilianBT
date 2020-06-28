using AilianBT.Helpers;
using AilianBT.Services;
using AilianBT.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace AilianBT.Views
{
    public sealed partial class ShowView : Page
    {
        public ShowVM ShowVM { get; set; } = ViewModelLocator.Instance.ShowVM;
        private LogService _logService = SimpleIoc.Default.GetInstance<LogService>();

        public ShowView()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            this.Loaded += _showViewLoaded;
            this.Unloaded += _showViewUnLoaded;
            this.SizeChanged += _showViewSizeChanged;
            webViewFile.NavigationCompleted += _webViewFileNavigationCompleted;
            webViewSummary.NavigationCompleted += _webViewSummaryNavigationCompleted;
            webViewSummary.NavigationStarting += webViewSummaryNavigationStarting;
            webViewFile.NavigationStarting += webViewSummaryNavigationStarting;
        }

        private void webViewSummaryNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri != null)
            {
                try
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Launcher.LaunchUriAsync(args.Uri);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
                catch(Exception e)
                {
                    _logService.Error($"Try to navigate to an uri: {args.Uri?.ToString()}", e);
                }
                args.Cancel = true;
            }
        }
            

        private bool hasNavigated = false;

        private async void _showViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(hasNavigated)
            {
                await _adjustWebView(webViewSummary);
                await _adjustWebView(webViewFile);
            }
        }

        private async void _showViewLoaded(object sender, RoutedEventArgs e)
        {
            _findIndicatorVisuals();
        }


        private void _showViewUnLoaded(object sender, RoutedEventArgs e)
        {
            Bindings.StopTracking();
            webViewSummary.SetValue(ShowView.DetailInfoProperty, null);

            webViewSummary.NavigationCompleted -= _webViewSummaryNavigationCompleted;
            webViewFile.NavigationCompleted -= _webViewFileNavigationCompleted;
        }

        private async void _webViewSummaryNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            hasNavigated = true;
            await _adjustWebView(webViewSummary);
            ShowVM.IsLoadingWebView = false;
        }

        private async void _webViewFileNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            await _adjustWebView(webViewFile);
        }

        private async Task _adjustWebView(WebView webView)
        {
            await _adjustWebviewContentZoom(webView, "100%");
            await _adjustWebviewContentHeight(webView);
            InitHeaderAnimation();
            await _adjustWebviewContentZoom(webView, "90%");
        }

        private string _getHeightJSContent;

        private async Task _adjustWebviewContentHeight(WebView webView)
        {
            if(string.IsNullOrEmpty(_getHeightJSContent))
            {
                var jsfile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/JS/GetHeight.js"));
                using (var stream = await jsfile.OpenStreamForReadAsync())
                {
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    _getHeightJSContent = Encoding.UTF8.GetString(buffer);
                }
            }
            
            string result = await webView.InvokeScriptAsync("eval", new[] { _getHeightJSContent });
            if (int.TryParse(result, out int contentHeight))
            {
                webView.Height = contentHeight;
            }
        }

        private async Task _adjustWebviewContentZoom(WebView webView, string zoomPercent)
        {
            await webView.InvokeScriptAsync("eval", new[] { $"document.body.style.zoom = \"{zoomPercent}\""});
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ShowVM.Loaded(e.Parameter);
        }

        public static string GetDetailInfo(DependencyObject o)
        {
            return o.GetValue(DetailInfoProperty) as string;
        }

        public static void SetDetailInfo(DependencyObject o,string value)
        {
            o.SetValue(DetailInfoProperty, value);
        }

        public static DependencyProperty DetailInfoProperty = DependencyProperty.RegisterAttached("DetailInfo", typeof(string), typeof(WebView),
            new PropertyMetadata(null, (o,e) =>
            {
                if (e.NewValue is string html && o is WebView webView)
                {
                    webView.NavigateToString(html);
                }
            }));

        private ExpressionAnimation _headerAnimation;
        public void InitHeaderAnimation()
        {
            if (_headerAnimation != null)
                _headerAnimation.Dispose();

            var compositor = Window.Current.Compositor;
            var visual = ElementCompositionPreview.GetElementVisual(panel);
            var scrollVisualSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollView);

            _headerAnimation = compositor.CreateExpressionAnimation();
            _headerAnimation.Expression = "((-scrollVisual.Translation.Y)>panelHeight)?(-scrollVisual.Translation.Y-panelHeight):0";

            _headerAnimation.SetScalarParameter("panelHeight", (float)(this.panel.ActualHeight - this.panelControl.ActualHeight));
            _headerAnimation.SetReferenceParameter("scrollVisual", scrollVisualSet);
            visual.StartAnimation("Offset.Y", _headerAnimation);
        }

        private IList<Visual> _selectedIndicatorVisuals = new List<Visual>();
        private void _findIndicatorVisuals()
        {
            var _indicators = VisualTreeHelperTool.FindAllNamedVisualChild<Rectangle>(lvNavigationBar, "SelectedIndicator");
            foreach (var item in _indicators)
            {
                _selectedIndicatorVisuals.Add(ElementCompositionPreview.GetElementVisual(item));
            }
        }

        private void _lvNavigationBarSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0 || e.RemovedItems.Count <= 0)
                return;

            var newIndx = lvNavigationBar.Items.IndexOf(e.AddedItems[0]);
            var oldIndex = lvNavigationBar.Items.IndexOf(e.RemovedItems[0]);

            var oldItem = lvNavigationBar.ContainerFromIndex(oldIndex) as FrameworkElement;
            var newItem = lvNavigationBar.ContainerFromIndex(newIndx) as FrameworkElement;

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
