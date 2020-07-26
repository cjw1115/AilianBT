using AilianBT.Helpers;
using AilianBT.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AilianBT
{
    sealed partial class App : Application
    {
        public static Frame MainFrame { get; set; }
        private DbService _dbService;
        private StorageService _storageService;
        private SettingService _settingService;
        private LogService _logService;

        public App()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            InitializeComponent();
            UnhandledException += _appUnhandledException;

            _registerService();

            _logService = SimpleIoc.Default.GetInstance<LogService>();

            _dbService = SimpleIoc.Default.GetInstance<DbService>();
            _dbService.DownloadDbContext.Database.Migrate();

            _storageService = SimpleIoc.Default.GetInstance<StorageService>();
            _settingService = SimpleIoc.Default.GetInstance<SettingService>();

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
            _adjustUI();
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

        private void _initLivingMode()
        {
            if (_settingService.GetLiveMode() == null)
            {
                _settingService.SetLiveMode(true);
            }
        }

        private void _adjustUI()
        {
            _settingService.AdjustTitleBar();
            _settingService.SetThemeColor(_settingService.GetCachedTheme());
        }

        private void _registerService()
        {
            SimpleIoc.Default.Register<AppCenterService>();
            SimpleIoc.Default.Register(() => new LogService(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path, SimpleIoc.Default.GetInstance<AppCenterService>()));
            SimpleIoc.Default.Register<UtilityHelper>();
            SimpleIoc.Default.Register<DbService>();
            SimpleIoc.Default.Register<StorageService>();
            SimpleIoc.Default.Register<NotificationService>();
            SimpleIoc.Default.Register<DownloadService>();
            SimpleIoc.Default.Register<MusicService>();
            SimpleIoc.Default.Register<AilianBTService>();
            SimpleIoc.Default.Register<SettingService>();
        }
        
        private void _appUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            _logService.Error($"Unhandled exception:{e?.Message}", e.Exception);
        }
    }
}
