using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AilianBT.Common.Models
{
    [DataContract]
    public class MusicModel: INotifyPropertyChanged
    {
        #region for data bind
        public event PropertyChangedEventHandler PropertyChanged;
        public void Set<T>(ref T field, T newValue = default(T), [CallerMemberName] string propertyName = null)
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
        #endregion

        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public Uri Uri { get; set; }
        [DataMember]
        public Uri RealUri { get; set; }

        [IgnoreDataMember]
        private bool _hasCached = false;
        [DataMember]
        public bool HasCached
        {
            get=> _hasCached;
            set
            {
                Set(ref _hasCached, value);
            }
        }

        public override bool Equals(object obj)
        {
            var model = obj as MusicModel;
            if (model == null)
                return false;
            if (model.ID == ID &&
                model.Title == Title &&
                model.Uri == Uri &&
                model.RealUri == RealUri)
            {
                return true;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //public static bool operator ==(MusicModel obj1, MusicModel obj2)
        //{
        //    if (obj1 == null || obj2 == null)
        //        return false;
        //    return obj1.Equals(obj2);
        //}
        //public static bool operator !=(MusicModel obj1, MusicModel obj2)
        //{
        //    if(obj1==null&&obj2!=null)
        //    {
        //        return true;
        //    }
        //    if(obj1!=null&&obj2==null)
        //    {
        //        return true;
        //    }
        //    if (obj1 == null && obj2 == null)
        //        return false;
        //    return !obj1.Equals(obj2);
        //}

    }
}
