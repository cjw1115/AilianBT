using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AilianBTShared.Models
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
    }
}
