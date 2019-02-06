using AilianBT.Services;
using BtDownload.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AilianBT
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        public static Frame MainFrame { get; set; }
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            this.InitializeComponent();
            this.Suspending += OnSuspending;

            //using (var dbcontext = new BtDownload.Models.DownloadDbContext())
            //{
            //    dbcontext.Database.Migrate();
            //}
            
            BtDownload.Services.SimpleIoc.Register<BtDownload.VIewModels.DownloadedVM>();
            BtDownload.Services.SimpleIoc.Register<BtDownload.VIewModels.DownloadingVM>();
            BtDownload.Services.SimpleIoc.Register<Services.DbService>();

            var dbservice = BtDownload.Services.SimpleIoc.GetInstance<DbService>();
            dbservice.DownloadDbContext.Database.Migrate();

            this.EnteredBackground += App_EnteredBackground;
            this.LeavingBackground += App_LeavingBackground;
        }
        private bool _isInBackgroundMode;

        public bool IsInBackgroundMode => _isInBackgroundMode;
        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            _isInBackgroundMode = false;
        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            _isInBackgroundMode = true;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
//#if DEBUG
//            if (System.Diagnostics.Debugger.IsAttached)
//            {
//                this.DebugSettings.EnableFrameRateCounter = true;
//            }
//#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(Views.NavigationView), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }


            MainFrame = rootFrame;


            var view = ApplicationView.GetForCurrentView();
            var brush = (App.Current.Resources["AilianBtMainColor"]) as SolidColorBrush;
            view.TitleBar.ButtonBackgroundColor = brush.Color;
            view.TitleBar.BackgroundColor = brush.Color;

            if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = brush.Color;
            }

            LoadBasicSetting();
            InitLivingMode();
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }


        public static void ShowNotification(string message)
        {
            Views.NavigationView view = MainFrame.Content as Views.NavigationView;
            view.Notification.NotifyMessage = message;
            view.Notification.Show();
        }

        public async void LoadBasicSetting()
        {
            
            AilianBT.DAL.LocalSetting setting = new DAL.LocalSetting();
            var model = await setting.GetLocalInfo<Models.ThemeColorModel>(typeof(Models.ThemeColorModel).Name);
            if (model != null)
                Services.SettingService.SetThemeColor(model.ThemeColor);
            else
            {
                var brush = Application.Current.Resources["AilianBtMainColor"] as SolidColorBrush;
                Services.SettingService.SetThemeColor(brush.Color);
            }
        }
        public void InitLivingMode()
        {
            var livingmode = FileService.GetLocalSetting<bool?>("livingmode");
            if (livingmode == null)
            {
                FileService.SetLocalSetting<bool?>("livingmode", true);
            }
        }
    }
}
