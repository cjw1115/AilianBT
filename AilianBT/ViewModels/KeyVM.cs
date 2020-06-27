using AilianBT.Models;
using AilianBT.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AilianBT.ViewModels
{
    public class KeyVM:ViewModelBase
    {
        private AilianBTService _ailianBTService = SimpleIoc.Default.GetInstance<AilianBTService>();
        private NavigationVM _navigationVM = ViewModelLocator.Instance.NavigationVM;

        public ObservableCollection<KeyGroupModel> _groups = new ObservableCollection<KeyGroupModel>();
        public ObservableCollection<KeyGroupModel> Groups
        {
            get => _groups;
            set => Set(ref _groups, value);
        }

        public async void Loaded()
        {
            try
            {
                var newkeys = await _ailianBTService.GetNewKeyList();
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

        public void GridView_ItemClick(object param)
        {
            NewKeyModel item = param as NewKeyModel;

            Views.SearchViewParam searchViewParam = new Views.SearchViewParam();
            searchViewParam.IsTo = true;
            searchViewParam.SearchKey = item.Value;

            _navigationVM.DetailFrame.Navigate(typeof(Views.SearchView), searchViewParam);
        }
    }
}
