using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace levelupspace.DataModel
{
    public class AlphabetItem : ABCItem
    {
        public AlphabetItem(String uniqueId, String title, String imagePath, String description, long ID)
            : base(uniqueId, title, imagePath, description)
        {
            _id = ID;
            LetterItems.CollectionChanged += ItemsCollectionChanged;
        }

        private long _id;
        public long ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }


        private void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Предоставляет подмножество полной коллекции элементов, привязываемой из объекта GroupedItemsPage
            // по двум причинам: GridView не виртуализирует большие коллекции элементов и оно
            // улучшает работу пользователей при просмотре групп с большим количеством
            // элементов.
            //
            // Отображается максимальное число столбцов (12), поскольку это приводит к заполнению столбцов сетки
            // сколько строк отображается: 1, 2, 3, 4 или 6

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopLetterItems.Insert(e.NewStartingIndex, LetterItems[e.NewStartingIndex]);
                        if (TopLetterItems.Count > 12)
                        {
                            TopLetterItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopLetterItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopLetterItems.RemoveAt(e.OldStartingIndex);
                        TopLetterItems.Add(LetterItems[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopLetterItems.Insert(e.NewStartingIndex, LetterItems[e.NewStartingIndex]);
                        TopLetterItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopLetterItems.RemoveAt(e.OldStartingIndex);
                        if (LetterItems.Count >= 12)
                        {
                            TopLetterItems.Add(LetterItems[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopLetterItems[e.OldStartingIndex] = LetterItems[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopLetterItems.Clear();
                    while (TopLetterItems.Count < LetterItems.Count && TopLetterItems.Count < 12)
                    {
                        TopLetterItems.Add(LetterItems[TopLetterItems.Count]);
                    }
                    break;
            }
        }

        private readonly ObservableCollection<LetterItem> _letterItems = new ObservableCollection<LetterItem>();
        public ObservableCollection<LetterItem> LetterItems
        {
            get { return _letterItems; }
        }

        private readonly ObservableCollection<LetterItem> _topLetterItem = new ObservableCollection<LetterItem>();
        public ObservableCollection<LetterItem> TopLetterItems
        {
            get { return _topLetterItem; }
        }
    }
}