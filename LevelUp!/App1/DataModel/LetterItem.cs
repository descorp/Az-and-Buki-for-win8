using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace levelupspace.DataModel
{
    public class LetterItem : ABCItem
    {
        public LetterItem(String uniqueId, String title, String imagePath, String description, int id, AlphabetItem alphabet, String sound)
            : base(uniqueId, title, imagePath, description)
        {
            _id = id;
            _alphabet = alphabet;
            _sound = sound;
            WordItems.CollectionChanged += ItemsCollectionChanged;
        }

        private String _sound;
        public String Sound
        {
            get { return _sound; }
            set { SetProperty(ref _sound, value); }
        }

        private AlphabetItem _alphabet;
        public AlphabetItem Alphabet
        {
            get { return _alphabet; }
            set { SetProperty(ref _alphabet, value); }
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
                        TopWordItems.Insert(e.NewStartingIndex, WordItems[e.NewStartingIndex]);
                        if (TopWordItems.Count > 12)
                        {
                            TopWordItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopWordItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopWordItems.RemoveAt(e.OldStartingIndex);
                        TopWordItems.Add(WordItems[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopWordItems.Insert(e.NewStartingIndex, WordItems[e.NewStartingIndex]);
                        TopWordItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopWordItems.RemoveAt(e.OldStartingIndex);
                        if (WordItems.Count >= 12)
                        {
                            TopWordItems.Add(WordItems[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopWordItems[e.OldStartingIndex] = WordItems[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopWordItems.Clear();
                    while (TopWordItems.Count < WordItems.Count && TopWordItems.Count < 12)
                    {
                        TopWordItems.Add(WordItems[TopWordItems.Count]);
                    }
                    break;
            }
        }

        private readonly ObservableCollection<WordItem> _topWordItems = new ObservableCollection<WordItem>();
        public ObservableCollection<WordItem> TopWordItems
        {
            get { return _topWordItems; }
        }
        private readonly ObservableCollection<WordItem> _wordItems = new ObservableCollection<WordItem>();
        public ObservableCollection<WordItem> WordItems
        {
            get { return _wordItems; }
        }

        private int _id;
        public int ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
    }
}