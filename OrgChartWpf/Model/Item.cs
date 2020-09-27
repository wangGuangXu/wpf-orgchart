using System.Collections.ObjectModel;

namespace OrgChartWpf.Model
{
    public class Item
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }

        private ObservableCollection<Item> _items;
        public ObservableCollection<Item> Items
        {
            get { return _items ?? (_items = new ObservableCollection<Item>()); }
        }

        public Item()
        {

        }

        public Item(int index, string name)
        {
            Index = index;
            Name = name;
        }
    }
}