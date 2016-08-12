using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
namespace AilianBT.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<ViewModels.NavigationVM>();
            SimpleIoc.Default.Register<ViewModels.MainVM>();
            SimpleIoc.Default.Register<ViewModels.ShowVM>();
            SimpleIoc.Default.Register<ViewModels.SearchVM>();
            SimpleIoc.Default.Register<ViewModels.KeyVM>();

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
    }
}
