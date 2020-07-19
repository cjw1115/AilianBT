using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Xaml;

namespace AilianBT.ViewModels
{
    public class ViewModelLocator
    {
        private static ViewModelLocator _instance;
        public static ViewModelLocator Instance 
        {
            get
            {
                if(_instance==null)
                {
                    _instance = Application.Current.Resources["Locator"] as ViewModelLocator;
                }
                return _instance;
            }
        }
        
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<NavigationVM>();
            SimpleIoc.Default.Register<MainVM>();
            SimpleIoc.Default.Register<ShowVM>();
            SimpleIoc.Default.Register<SearchVM>();
            SimpleIoc.Default.Register<KeyVM>();
            SimpleIoc.Default.Register<MusicVM>();
            SimpleIoc.Default.Register<PlayerViewModel>();
            SimpleIoc.Default.Register<DownloadedViewModel>();
            SimpleIoc.Default.Register<DownloadingViewModel>();
            SimpleIoc.Default.Register<SettingViewModel>();
        }

        public NavigationVM NavigationVM
        {
            get { return SimpleIoc.Default.GetInstance<NavigationVM>(); }
        }
        public MainVM MainVM
        {
            get { return SimpleIoc.Default.GetInstance<MainVM>(); }
        }
        public ShowVM ShowVM
        {
            get { return SimpleIoc.Default.GetInstance<ShowVM>(); }
        }
        public SearchVM SearchVM
        {
            get { return SimpleIoc.Default.GetInstance<SearchVM>(); }
        }
        public KeyVM KeyVM
        {
            get { return SimpleIoc.Default.GetInstance<KeyVM>(); }
        }
        public MusicVM MusicVM
        {
            get { return SimpleIoc.Default.GetInstance<MusicVM>(); }
        }

        public PlayerViewModel PlayerVM
        {
            get { return SimpleIoc.Default.GetInstance<PlayerViewModel>(); }
        }

        public DownloadedViewModel DownloadedVM
        {
            get { return SimpleIoc.Default.GetInstance<DownloadedViewModel>(); }
        }
        public DownloadingViewModel DownloadingVM
        {
            get { return SimpleIoc.Default.GetInstance<DownloadingViewModel>(); }
        }
        public SettingViewModel SettingVM
        {
            get { return SimpleIoc.Default.GetInstance<SettingViewModel>(); }
        }
    }
}
