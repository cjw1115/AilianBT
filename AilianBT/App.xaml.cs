using AilianBT.Common.Services;
using AilianBT.Constant;
using AilianBT.Models;
using AilianBT.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AilianBT
{
    sealed partial class App : Application
    {
        public static Frame MainFrame { get; set; }
        private DbService _dbService;
        private StorageService _storageService;

        public App()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            InitializeComponent();

            _registerService();

            _dbService = SimpleIoc.Default.GetInstance<DbService>();
            _dbService.DownloadDbContext.Database.Migrate();

            _storageService = SimpleIoc.Default.GetInstance<StorageService>();

            EnteredBackground += _enteredBackground;
            LeavingBackground += _leavingBackground;
        }

        public bool IsInBackgroundMode { get; private set; }
        
        private void _enteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            IsInBackgroundMode = true;
        }

        private void _leavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            IsInBackgroundMode = false;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += _onNavigationFailed;
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(Views.NavigationView), e.Arguments);
                }
                Window.Current.Activate();
            }

            MainFrame = rootFrame;

            _adjustTitleBar();
            _loadBasicSettings();
            _initLivingMode();
        }

        

        private void _onNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        public static void ShowNotification(string message)
        {
            Views.NavigationView view = MainFrame.Content as Views.NavigationView;
            view.Notification.NotifyMessage = message;
            view.Notification.Show();
        }

        private void _adjustTitleBar()
        {
            var applicationView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            var colorBrush = Application.Current.Resources["AilianBtMainColor"] as SolidColorBrush;
            applicationView.TitleBar.ButtonBackgroundColor = colorBrush.Color;
            applicationView.TitleBar.BackgroundColor = colorBrush.Color;

            if ("Windows.Mobile" == Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.BackgroundOpacity = 1;
                statusBar.BackgroundColor = colorBrush.Color;
            }
        }

        private void _loadBasicSettings()
        {
            var model = _storageService.GetLocalSetting<ThemeColorModel>(nameof(ThemeColorModel));
            if (model != null)
                SettingService.SetThemeColor(model.ThemeColor);
            else
            {
                var brush = Application.Current.Resources["AilianBtMainColor"] as SolidColorBrush;
                SettingService.SetThemeColor(brush.Color);
            }
        }

        private void _initLivingMode()
        {
            if (_storageService.GetLocalSetting<bool?>(Definition.KISSSUB_LIVING_MODE) == null)
            {
                // Default turn on the livingmode
                _storageService.SetLocalSetting(Definition.KISSSUB_LIVING_MODE, true.ToString());
            }
        }

        private void _registerService()
        {
            SimpleIoc.Default.Register<DbService>();
            SimpleIoc.Default.Register<StorageService>();
            SimpleIoc.Default.Register<NotificationService>();
            SimpleIoc.Default.Register<DownloadService>();
            SimpleIoc.Default.Register<MusicService>();
            SimpleIoc.Default.Register<AilianBTService>();
        }
    }
}
