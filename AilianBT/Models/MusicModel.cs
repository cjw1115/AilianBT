using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AilianBT.Models
{
    [DataContract]
    public class MusicModel
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public Uri Uri { get; set; }
        [DataMember]
        public Uri RealUri { get; set; }

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
    }
}
