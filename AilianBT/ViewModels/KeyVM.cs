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
            //CollectionViewSource = new CollectionViewSource();
            //CollectionViewSource.IsSourceGrouped = true;
            //CollectionViewSource.ItemsPath = new Windows.UI.Xaml.PropertyPath("Keys");
            //CollectionViewSource.Source = Groups;
        }
        public async void Loaded()
        {
            var newkeys = await _ailianBtBll.GetNewKeyList();

            Groups.Clear();
            for (int i = 0; i < newkeys.Count; i++)
            {
                Groups.Add(new KeyGroupModel { Day = (DayOfWeek)i, Keys = new List<NewKeysModels> { new NewKeysModels { NewKeyModel= newkeys[i] }  } });
            }
        }
       
        //private CollectionViewSource _collectionViewSource;
        //public CollectionViewSource CollectionViewSource
        //{
        //    get { return _collectionViewSource; }
        //    set { Set(ref _collectionViewSource, value); }
        //}

        
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
