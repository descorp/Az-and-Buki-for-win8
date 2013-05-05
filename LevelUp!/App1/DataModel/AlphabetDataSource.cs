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


namespace App1
{
    public abstract class ABCItem : App1.Common.BindableBase
    {
        public static Uri _baseUri = new Uri("ms-appx:///");

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
            WordItems.CollectionChanged += ItemsCollectionChanged;
        }

        private AlphabetItem _alphabet;
        public AlphabetItem Alphabet
        {
            get { return this._alphabet; }
            set { this.SetProperty(ref this._alphabet, value); }
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

        private ObservableCollection<WordItem> _topWordItems = new ObservableCollection<WordItem>();
        public ObservableCollection<WordItem> TopWordItems
        {
            get { return this._topWordItems; }
        }
        private ObservableCollection<WordItem> _wordItems = new ObservableCollection<WordItem>();
        public ObservableCollection<WordItem> WordItems
        {
            get { return this._wordItems; }
        }

        private int _id = 0;
        public int ID
        {
            get { return _id; }
            set { this.SetProperty( ref this._id, value); }
        }
    }

    public class WordItem : ABCItem
    {
        public WordItem(String uniqueId, String title, String imagePath, String description, int ID, AlphabetItem alphabet, String picturePath)
            : base(uniqueId, title, imagePath, description)
        {
            this._id = ID;
            this._alphabet = alphabet;
            this._PicturePath = picturePath;
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
            set { this.SetProperty(ref this._id, value); }
        }

        private ImageSource _imgPicture = null;
        private String _PicturePath = null;
        public ImageSource imgPicture
        {
            get
            {
                if (this._imgPicture == null && this._PicturePath != null)
                {
                    this._imgPicture = new BitmapImage(new Uri(WordItem._baseUri, this._PicturePath));
                }
                return this._imgPicture;
            }

            set
            {
                this._PicturePath = null;
                this.SetProperty(ref this._imgPicture, value);
            }
        }

        public void SetPicture(String path)
        {
            this._imgPicture = null;
            this._PicturePath = path;
            this.OnPropertyChanged("imgPicture");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }


    public class AlphabetItem: ABCItem
    {
        public AlphabetItem(String uniqueId, String title, String imagePath, String description, int ID)
            : base(uniqueId, title, imagePath, description)
        {
            this._id = ID;
            LetterItems.CollectionChanged += ItemsCollectionChanged;
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
                        TopLetterItems.Insert(e.NewStartingIndex,LetterItems[e.NewStartingIndex]);
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

        private ObservableCollection<LetterItem> _letterItems = new ObservableCollection<LetterItem>();
        public ObservableCollection<LetterItem> LetterItems
        {
            get { return this._letterItems; }
        }

       

        private ObservableCollection<LetterItem> _topLetterItem = new ObservableCollection<LetterItem>();
        public ObservableCollection<LetterItem> TopLetterItems
        {
            get {return this._topLetterItem; }
        }
    }



    public sealed class ABCDataSource
    {
        private static ABCDataSource _ABCDataSource = new ABCDataSource();
        

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
            if (matches.Count() == 1)
            {
                var _Path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db");
                SQLiteConnection db = new SQLiteConnection(_Path);

                var Aitem = matches.First();
                if (Aitem.LetterItems.Count == 0)
                {
                    var LetterQuery = db.Query<Letter>("SELECT * FROM Letter WHERE AlphabetID=?", Aitem.ID);

                    for (int j = 0; j < LetterQuery.Count; j++)
                        Aitem.LetterItems.Add(new LetterItem(
                                        String.Concat("Alphabet ", Aitem.ID.ToString(), " Letter ", LetterQuery[j].ID.ToString()),
                                        LetterQuery[j].Value,
                                        Path.Combine(ApplicationData.Current.LocalFolder.Path, LetterQuery[j].Logo),
                                        LetterQuery[j].Value,
                                        LetterQuery[j].ID,
                                        Aitem
                                        ));
                }
                return Aitem;
            }
            return null;
        }

        public static LetterItem GetItem(string uniqueId)
        {
            // Для небольших наборов данных можно использовать простой линейный поиск
            var matches = _ABCDataSource.AllAlphabets.SelectMany(group => group.LetterItems).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1)
            {
                var _Path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db");
                SQLiteConnection db = new SQLiteConnection(_Path);

                var Litem = matches.First();
                if (Litem.WordItems.Count == 0)
                {
                    var WordQuery = db.Query<Word>("SELECT * FROM Word WHERE AlphabetID=? AND LetterID=?", Litem.Alphabet.ID, Litem.ID);

                    for (int j = 0; j < WordQuery.Count; j++)
                        Litem.WordItems.Add(new WordItem(
                                            String.Concat("Alphabet ", Litem.Alphabet.ID.ToString(), " Letter ", Litem.ID.ToString(), WordQuery[j].ID.ToString()),
                                            " ",
                                            Path.Combine(ApplicationData.Current.LocalFolder.Path, WordQuery[j].ValueImg),
                                            " ",
                                            WordQuery[j].ID,
                                            Litem.Alphabet,
                                            Path.Combine(ApplicationData.Current.LocalFolder.Path,WordQuery[j].Image)
                                            ));
                }
                return Litem;
            }
            return null;
        }



        public ABCDataSource()
        {
            var _Path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db");
            SQLiteConnection db = new SQLiteConnection(_Path);
            
            
            var AlphabetQuery = db.Query<Alphabet>("SELECT * FROM Alphabet");
            
            for (int i = 0; i < AlphabetQuery.Count; i++)
            {
                var LocalQuery = db.Query<Localization>(
                                     "SELECT * FROM Localization WHERE AlphabetID=?", AlphabetQuery[i].ID);                  
                var localization = LocalQuery.FirstOrDefault(); 

                var Aitem = new AlphabetItem(
                        String.Concat("Alphabet ", AlphabetQuery[i].ID.ToString()),
                        LocalQuery.ElementAt(0).LanguageName,
                        Path.Combine(ApplicationData.Current.LocalFolder.Path, AlphabetQuery[i].Logo),
                        LocalQuery.ElementAt(0).Description,
                        AlphabetQuery[i].ID                    
                        );

                    _allAlphabets.Add(Aitem);
            }      
           
        }

        
    }

}
