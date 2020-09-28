using OrgChartWpf.Command;
using OrgChartWpf.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OrgChartWpf.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region 变量

        private int _index = 0;
        private Random _rnd = new Random();

        #endregion

        #region 属性

        private ObservableCollection<Item> _items;
        public ObservableCollection<Item> Items
        {
            get { return _items ?? (_items = new ObservableCollection<Item>()); }
        }

        #endregion

        #region 命令

        private RelayCommand _refreshCommand;
        /// <summary>
        /// 刷新命令
        /// </summary>
        public RelayCommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(p => CreateItems(), p => true)); }
        }

        #endregion

        #region 构造函数

        public MainViewModel()
        {
            CreateItems();
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 创建数据项
        /// </summary>
        private void CreateItems()
        {
            //_index = 0;
            //Items.Clear();

            //var level = _rnd.Next(2, 6);
            //var item = CreateItem();

            //CreateItems(item, level);
            //Items.Add(item);

            Items.Clear();

            var item0 = new Item(0, "祖先");
            var item01 = new Item(1, "子:朱合祥");


            var item011 = new Item(2, "长子:朱长锁");
            var item0111 = new Item(1, "子:朱军");
            var item01111 = new Item(1, "女:朱怡菁");
            item0111.Items.Add(item01111);

            var item0112 = new Item(1, "女:朱红");
            item011.Items.Add(item0111);
            item011.Items.Add(item0112);

            var item012 = new Item(2, "次子:朱锁堂");
            var item0121 = new Item(3, "子:朱军伟");
            var item0122 = new Item(3, "女:朱秋红");

            item011.Items.Add(item0121);
            item011.Items.Add(item0122);

            var item013 = new Item(2, "长子:朱秀绒");
            var item014 = new Item(2, "次女:朱翠花");
            var item015 = new Item(2, "三女:朱小琴");

            item01.Items.Add(item011);
            item01.Items.Add(item012);
            item01.Items.Add(item013);
            item01.Items.Add(item014);
            item01.Items.Add(item015);

            var item02 = new Item(1, "女:(名字不详)");

            item0.Items.Add(item01);
            item0.Items.Add(item02);

            Items.Add(item0);
        }

        private Item CreateItem()
        {
            return new Item
            {
                Index = _index++
            };
        }

        private void CreateItems(Item item, int level)
        {
            if (level > 0)
            {
                for (int i = 0; i < _rnd.Next(1, 3); i++)
                {
                    var childItem = CreateItem();
                    item.Items.Add(childItem);

                    CreateItems(childItem, level - 1);
                }
            }
        }

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}