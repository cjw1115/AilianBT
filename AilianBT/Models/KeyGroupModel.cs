using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AilianBT.Models
{
    public class KeyGroupModel
    {
        public DayOfWeek Day { get; set; }
        public List<NewKeysModels> Keys { get; set; }
    }

    public class NewKeysModels
    {
        public ObservableCollection<Models.NewKeyModel> NewKeyModel { get; set; }
    }
}
