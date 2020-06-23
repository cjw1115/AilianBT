using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AilianBT.Common
{
    public class ViewModelBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Set<T>(ref T field,T newValue = default(T),[CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return;
            }
            field = newValue;
            OnPropertyChanged(propertyName);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
