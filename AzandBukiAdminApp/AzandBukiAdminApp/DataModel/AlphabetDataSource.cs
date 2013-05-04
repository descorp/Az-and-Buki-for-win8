using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using SQLite;
using System.IO;
using Windows.Storage;


namespace AzandBukiAdminApp
{
    public abstract class ABCItem : AzandBukiAdminApp.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public ABCItem(String uniqueId, String title, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._description = description;
            this._imagePath = imagePath;
        }
        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(ABCItem._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public class LetterItem : ABCItem
    {
        public LetterItem(String uniqueId, String title, String imagePath, String description, int ID, AlphabetItem alphabet)
            : base(uniqueId, title, imagePath, description)
        {
            this._id = ID;
            this._alphabet = alphabet;
        }

        private AlphabetItem _alphabet;
        public AlphabetItem Alphabet
        {
            get { return this._alphabet; }
            set { this.SetProperty(ref this._alphabet, value); }
        }

        private int _id = 0;
        public int ID
        {
            get { return _id; }
            set { this.SetProperty( ref this._id, value); }
        }

    }

    public class AlphabetItem: ABCItem
    {
        public AlphabetItem(String uniqueId, String title, String imagePath, String description, int ID)
            : base(uniqueId, title, imagePath, description)
        {
            this._id = ID;
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private int _id = 0;
        public int ID
        {
            get { return _id; }
            set { this.SetProperty(ref this._id, value); }
        }
        
        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
                        TopItems.Insert(e.NewStartingIndex,Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<LetterItem> _items = new ObservableCollection<LetterItem>();
        public ObservableCollection<LetterItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<LetterItem> _topItem = new ObservableCollection<LetterItem>();
        public ObservableCollection<LetterItem> TopItems
        {
            get {return this._topItem; }
        }
    }



    public sealed class ABCDataSouce
    {
        private static ABCDataSouce _ABCDataSource = new ABCDataSouce();
        private String _Path;

        private ObservableCollection<AlphabetItem> _allAlphabets = new ObservableCollection<AlphabetItem>();
        public ObservableCollection<AlphabetItem> AllAlphabets
        {
            get { return this._allAlphabets; }
            
        }

        public static IEnumerable<AlphabetItem> GetAlphabets(string uniqueId)
        {
            if (!uniqueId.Equals("AllAlphabets")) throw new ArgumentException("Only 'AllAlphabets' is supported as a collection of Alphabets");

            return _ABCDataSource.AllAlphabets;
        }

        public static AlphabetItem GetAlphabet(string uniqueId)
        {
            // Для небольших наборов данных можно использовать простой линейный поиск
            var matches = _ABCDataSource.AllAlphabets.Where((alphabet) => alphabet.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static LetterItem GetItem(string uniqueId)
        {
            // Для небольших наборов данных можно использовать простой линейный поиск
            var matches = _ABCDataSource.AllAlphabets.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public ABCDataSouce()
        {
            this._Path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db");
            SQLiteConnection db = new SQLiteConnection(_Path);

            db.CreateTable<Alphabet>();
            db.CreateTable<Letter>();
            db.CreateTable<Word>();

            var _allAlphabets = new ObservableCollection<AlphabetItem>();
            
            var AlphabetQuery = db.Query<Alphabet>("SELECT * FROM Alphabet");
            
            for (int i = 0; i < AlphabetQuery.Count - 1; i++)
            {
                var Aitem = new AlphabetItem(
                        String.Concat("Alphabet ", AlphabetQuery[i].ID.ToString()),
                        AlphabetQuery[i].Language,
                        AlphabetQuery[i].Description,
                        AlphabetQuery[i].Logo,
                        AlphabetQuery[i].ID                    
                        );
                
                var LetterQuery = db.Query<Letter>("SELECT * FROM Letter WHERE AlphabetID=?", AlphabetQuery[i].ID);
                
                for (int j = 0; j < LetterQuery.Count-1; j++)
                    Aitem.Items.Add(new LetterItem(
                                    String.Concat("Alphabet ", AlphabetQuery[i].ID.ToString(), " Letter ", LetterQuery[j].ToString()),
                                    LetterQuery[j].Value,
                                    LetterQuery[j].Logo,
                                    LetterQuery[j].Value,
                                    LetterQuery[j].ID,
                                    Aitem
                                    ));                
                
                _allAlphabets.Add(Aitem);
            }      
           
        }

        
    }

}
