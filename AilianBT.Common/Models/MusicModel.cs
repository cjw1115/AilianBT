using GalaSoft.MvvmLight;
using System;
using System.Runtime.Serialization;

namespace AilianBT.Common.Models
{
    [DataContract]
    public class MusicModel: ViewModelBase
    {
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
            set => Set(ref _hasCached, value);
        }

        public override bool Equals(object obj)
        {
            var model = obj as MusicModel;
            if (model == null)
            {
                return false;
            }
            if (this == model)
            {
                return true;
            }
            return model.ID == ID 
                && model.Title == Title 
                && model.Uri == Uri
                && model.RealUri == RealUri;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
