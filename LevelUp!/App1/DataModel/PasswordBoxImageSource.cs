using System;
using System.Collections.ObjectModel;

namespace levelupspace.DataModel
{
    public class PasswordBoxImageSource
    {
        public PasswordBoxImageSource(params String[] imagePath)
        {
            _items = new ObservableCollection<PasswordBoxItem>();
            foreach (string path in imagePath)
            {
                _items.Add(new PasswordBoxItem(path));
            }
        }

        private readonly ObservableCollection<PasswordBoxItem> _items;

        public ObservableCollection<PasswordBoxItem> Items
        {
            get { return _items; }

        }
    }
}