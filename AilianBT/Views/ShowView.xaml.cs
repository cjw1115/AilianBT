using AilianBT.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace AilianBT.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ShowView : Page
    {
        public ShowVM ShowVM { get; set; }
        public ShowView()
        {
            var locator = App.Current.Resources["Locator"] as ViewModelLocator;
            ShowVM = locator.ShowVM;
            this.Loaded += ShowView_Loaded;
            this.InitializeComponent();
        }

        private object naviParam;
        private void ShowView_Loaded(object sender, RoutedEventArgs e)
        {
            ShowVM.Loaded(naviParam);

            this.webViewSummary.NavigationCompleted += WebViewSummary_NavigationCompleted;

            
        }

        private async void WebViewSummary_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            var webView=sender as WebView;
            var jsfile=await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/JS/GetHeight.js"));
            string jsContent = "";
            using (var stream = await jsfile.OpenStreamForReadAsync())
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                jsContent=Encoding.UTF8.GetString(buffer);
            }
            string result = await webView.InvokeScriptAsync("eval", new[] { jsContent });
            var re=int.TryParse(result, out int contentHeight);
            if(re)
            {
                webView.Height = contentHeight;
            }

            InitHeaderAnimation();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            naviParam = e.Parameter;
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
                var html = e.NewValue as string;
                if (html != null)
                {
                    var webview = o as WebView;
                    webview.NavigateToString(html);
                }
            }));


        public void InitHeaderAnimation()
        {
            var compositor = Window.Current.Compositor;
            var visual = ElementCompositionPreview.GetElementVisual(panel);
            var scrollVisualSet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollView);

            var scrollAnimation=compositor.CreateExpressionAnimation();
            scrollAnimation.Expression = "((-scrollVisual.Translation.Y)>panelHeight)?(-scrollVisual.Translation.Y-panelHeight):0";

            scrollAnimation.SetScalarParameter("panelHeight", (float)(this.panel.ActualHeight - this.panelControl.ActualHeight));
            scrollAnimation.SetReferenceParameter("scrollVisual", scrollVisualSet);
            visual.StartAnimation("Offset.Y", scrollAnimation);
        }
    }
}
