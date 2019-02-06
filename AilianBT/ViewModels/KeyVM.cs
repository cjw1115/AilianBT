using AilianBT.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace AilianBT.ViewModels
{
    public class KeyVM:ViewModelBase
    {
        private BLL.AilianBTBLL _ailianBtBll;
        public KeyVM()
        {
            _ailianBtBll = new BLL.AilianBTBLL();
        }
        public async void Loaded()
        {
            try
            {
                var newkeys = await _ailianBtBll.GetNewKeyList();

                Groups.Clear();
                for (int i = 0; i < newkeys.Count; i++)
                {
                    Groups.Add(new KeyGroupModel { Day = (DayOfWeek)i, Keys = new List<NewKeysModels> { new NewKeysModels { NewKeyModel = newkeys[i] } } });
                }
            }
            catch(Exception e)
            {
                App.ShowNotification(e.Message);
            }
            
        }
       
        public ObservableCollection<KeyGroupModel> Groups { get; set; } = new ObservableCollection<KeyGroupModel>();

        public  void GridView_ItemClick(object param)
        {
            NewKeyModel item = param as NewKeyModel;

            Views.SearchViewParam searchViewParam = new Views.SearchViewParam();
            searchViewParam.IsTo = true;
            searchViewParam.SearchKey = item.Value;

            NavigationVM.DetailFrame.Navigate(typeof(Views.SearchView), searchViewParam);
        }

    }
}
