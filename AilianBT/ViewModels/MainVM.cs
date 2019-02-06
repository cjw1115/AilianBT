using AilianBT.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace AilianBT.ViewModels
{
    public class MainVM:ViewModelBase
    {
        BLL.AilianBTBLL bll;
        public MainVM()
        {
            bll = new BLL.AilianBTBLL();
            AilianRes = new ObservableCollection<AilianResModel>();
        }
        private ObservableCollection<AilianResModel> _ailianRes;
        public ObservableCollection<AilianResModel> AilianRes
        {
            get { return _ailianRes; }
            set
            {
                Set(ref _ailianRes, value);
            }
        }

        private int _pageIndex = 1;
        public async void Loaded()
        {
            _pageIndex = 1;

            try
            {
                var list = await bll.GetResList(_pageIndex++);
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
                newlist = await bll.GetResList(_pageIndex++);

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
                newlist = await bll.GetResList(1);
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

        public  void ItemClick(object sender, ItemClickEventArgs e)
        {
            var model = e.ClickedItem as AilianResModel;
            NavigationVM.DetailFrame.Navigate(typeof(Views.ShowView), model);
        }

        public void Search_Click()
        {
            NavigationVM.DetailFrame.Navigate(typeof(Views.SearchView));
        }
    }
}
