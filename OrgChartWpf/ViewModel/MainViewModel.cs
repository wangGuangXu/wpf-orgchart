using OrgChartWpf.Command;
using OrgChartWpf.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OrgChartWpf.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Variable

        private int _index = 0;
        private Random _rnd = new Random();

        #endregion

        #region Property

        #region Items

        private ObservableCollection<Item> _items;
        public ObservableCollection<Item> Items
        {
            get { return _items ?? (_items = new ObservableCollection<Item>()); }
        }

        #endregion

        #endregion

        #region Command

        #region Refresh

        private RelayCommand _refreshCommand;
        public RelayCommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = 
                    new RelayCommand(p => CreateItems(), p => true)); }
        }

        #endregion

        #endregion

        #region Constructor

        public MainViewModel()
        {
            CreateItems();
        }

        #endregion

        #region Private Method

        private void CreateItems()
        {
            _index = 0;
            Items.Clear();

            var level = _rnd.Next(2, 6);
            var item = CreateItem();

            CreateItems(item, level);
            Items.Add(item);
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