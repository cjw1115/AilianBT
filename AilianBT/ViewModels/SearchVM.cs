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
    public class SearchVM : ViewModelBase
    {
        BLL.AilianBTBLL bll;
        public SearchVM()
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
        public  void Loaded(object param)
        {
            SearchKey = (string)param;
            Search();
        }

        public async Task LoadMore()
        {
            IList<AilianResModel> newlist = null;
            try
            {
                newlist = await bll.SerachResList(SearchKey, _pageIndex++);
                if (newlist != null)
                {
                    foreach (var item in newlist)
                    {
                        AilianRes.Add(item);
                    }
                }
                else
                    AilianRes.Clear();
            }
            catch (Exceptions.NetworkException networkexception)
            {
                App.ShowNotification(networkexception.Message);
            }
            catch (Exceptions.ResolveException resolveException)
            {
                AilianRes.Clear();
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
                newlist = await bll.SerachResList(SearchKey, _pageIndex++);

                if (newlist != null && newlist.Count > 0)
                {
                    AilianRes.Clear();
                    foreach (var item in newlist)
                    {
                        AilianRes.Add(item);
                    }
                }

                AilianRes.Clear();
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
                AilianRes.Clear();
                App.ShowNotification(resolveException.Message);
            }
        }

        public void ItemClick(object sender, ItemClickEventArgs e)
        {
            var model = e.ClickedItem as AilianResModel;
            NavigationVM.DetailFrame.Navigate(typeof(Views.ShowView), model);
        }


        private string _searchKey;
        public string SearchKey
        {
            get { return _searchKey; }
            set { Set(ref _searchKey, value); }
        }
        
        
        public async void Search()
        {
            if(string.IsNullOrWhiteSpace(SearchKey))
            {
                App.ShowNotification("请输入搜索关键字");
                return;
            }
            //标识搜索状态
            _pageIndex = 1;
            try
            {
                var newlist = await bll.SerachResList(SearchKey, _pageIndex++);
                AilianRes.Clear();
                if (newlist != null && newlist.Count > 0)
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
                AilianRes.Clear();
                App.ShowNotification(resolveException.Message);
            }
        }
    }
}