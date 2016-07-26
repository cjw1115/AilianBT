using AilianBT.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public event Action test;
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
            var list = await bll.GetResList(_pageIndex++);
            if (list != null)
            {
                foreach (var item in list)
                {
                    AilianRes.Add(item);
                }
            }
        }
        
        public async void LoadMore()
        {
            var newlist=await bll.GetResList(_pageIndex++);
            if (newlist != null||newlist.Count>0)
            {
                foreach(var item in newlist)
                {
                    AilianRes.Add(item);
                }
            }
        }
        public async void Refresh()
        {
            var newlist = await bll.GetResList(1);
            if (newlist != null && newlist.Count > 0)
            {
                AilianRes.Clear();
                foreach (var item in newlist)
                {
                    AilianRes.Add(item);
                }
            }
        }


        public  void ItemClick(object sender, ItemClickEventArgs e)
        {
            var model = e.ClickedItem as AilianResModel;
            NavigationVM.DetailFrame.Navigate(typeof(Views.ShowView), model);
        }
    }
}
