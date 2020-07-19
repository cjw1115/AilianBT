using GalaSoft.MvvmLight;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AilianBT.Models
{
    [DataContract]
    public class MusicModel: ViewModelBase
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public Uri Uri { get; set; }

        private bool _hasCached = false;
        [JsonIgnore]
        public bool HasCached
        {
            get=> _hasCached;
            set => Set(ref _hasCached, value);
        }

        private TimeSpan _position;
        [JsonIgnore]
        public TimeSpan Position
        {
            get => _position;
            set => Set(ref _position, value);
        }

        private TimeSpan _length;
        [JsonIgnore]
        public TimeSpan Length
        {
            get => _length;
            set => Set(ref _length, value);
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
            return model.Title == Title
                && model.Uri == Uri;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Update(MusicModel source)
        {
            if (!this.Equals(source))
            {
                this.Title = source.Title;
                this.Uri = source.Uri;
                return true;
            }
            return false;
        }
    }
}
