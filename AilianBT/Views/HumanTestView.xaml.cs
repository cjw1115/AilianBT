using AilianBT.Exceptions;
using AilianBT.Helpers;
using AilianBT.Services;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;

namespace AilianBT.Views
{
    public sealed partial class HumanTestView : Page
    {
        private AilianBTService _ailianBTService = SimpleIoc.Default.GetInstance<AilianBTService>();

        public HumanTestView()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            this.Loaded += HumanTestView_Loaded;
        }

        private async void HumanTestView_Loaded(object sender, RoutedEventArgs e)
        {
            await testWebView.EnsureCoreWebView2Async();
            testWebView.CoreWebView2.Navigate("http://m.kisssub.org/");
            testWebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
        }

        private async void CoreWebView2_NavigationCompleted(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            var cookieManager = testWebView.CoreWebView2.CookieManager;

            var cookies = await cookieManager.GetCookiesAsync("http://www.kisssub.org");

            foreach (var cookie in cookies)
            {
                if (cookie.Name == "visitor_test")
                {
                    Cookie humanCookie = new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
                    humanCookie.Expires = DateTimeOffset.FromUnixTimeSeconds((long)cookie.Expires).UtcDateTime;
                    humanCookie.HttpOnly = cookie.IsHttpOnly;
                    humanCookie.Secure = cookie.IsSecure;
                    _ailianBTService.SetHumanTestCookie(humanCookie);

                    App.ShowNotification("人类行为检测成功，你可以访问其他页面了！");
                }
                Debug.WriteLine($"Name: {cookie.Name}, Value: {cookie.Value}");
            }
        }
    }
}