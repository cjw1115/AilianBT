using AilianBT.Models;
using AilianBT.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace AilianBT.ViewModels
{
    public class MainVM:ViewModelBase
    {
        private AilianBTService _ailianBTService = SimpleIoc.Default.GetInstance<AilianBTService>();
        private NavigationVM _navigationVM = ViewModelLocator.Instance.NavigationVM;

        private ObservableCollection<AilianResModel> _ailianRes = new ObservableCollection<AilianResModel>();
        public ObservableCollection<AilianResModel> AilianRes
        {
            get => _ailianRes;
            set => Set(ref _ailianRes, value);
        }

        private int _pageIndex = 1;

        public async void Loaded()
        {
            _pageIndex = 1;

            try
            {
                var list = await _ailianBTService.GetResList(_pageIndex++);
                if (list != null)
                {
                    AilianRes.Clear();
                    foreach (var item in list)
                    {
                        AilianRes.Add(item);
                    }
                }
            }
            catch (Exceptions.NetworkException networkexception)
            {
                App.ShowNotification(networkexception.Message);
            }
            catch (Exceptions.ResolveException resolveException)
            {
                App.ShowNotification(resolveException.Message);
            }

        }
        
        public async Task LoadMore()
        {
            IList<AilianResModel> newlist = null;
            try
            {
                newlist = await _ailianBTService.GetResList(_pageIndex++);

                if (newlist != null)
                {
                    foreach (var item in newlist)
                    {
                        AilianRes.Add(item);
                    }
                }
            }
            catch (Exceptions.NetworkException networkexception)
            {
                App.ShowNotification(networkexception.Message);
            }
            catch (Exceptions.ResolveException resolveException)
            {
                App.ShowNotification(resolveException.Message);
            }
        }

        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { Set(ref _isRefreshing, value); }
        }

        public async Task Refresh()
        {
            _pageIndex = 1;
            IList<AilianResModel> newlist = null;
            try
            {
                newlist = await _ailianBTService.GetResList(1);
                if (newlist != null && newlist.Count > 0)
                {
                    AilianRes.Clear();
                    foreach (var item in newlist)
                    {
                        AilianRes.Add(item);
                    }
                }

                if (newlist != null)
                {
                    AilianRes.Clear();
                    foreach (var item in newlist)
                    {
                        AilianRes.Add(item);
                    }
                }
            }
            catch (Exceptions.NetworkException networkexception)
            {
                App.ShowNotification(networkexception.Message);
            }
            catch (Exceptions.ResolveException resolveException)
            {
                App.ShowNotification(resolveException.Message);
            }
        }

        public void ItemClick(object sender, ItemClickEventArgs e)
        {
            var model = e.ClickedItem as AilianResModel;
            _navigationVM.DetailFrameNavigate(typeof(Views.ShowView), model);
        }

        public void Search_Click()
        {
            _navigationVM.DetailFrameNavigate(typeof(Views.SearchView));
        }
    }
}
