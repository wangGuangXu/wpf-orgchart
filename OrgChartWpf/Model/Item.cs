using System.Collections.ObjectModel;

namespace OrgChartWpf.Model
{
    public class Item
    {
        public int Index { get; set; }

        private ObservableCollection<Item> _items;
        public ObservableCollection<Item> Items
        {
            get { return _items ?? (_items = new ObservableCollection<Item>()); }
        }
    }
}