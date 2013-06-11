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
using System.Threading.Tasks;
using levelupspace.DataModel;
using Windows.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace levelupspace
{
    public abstract class ABCItem : levelupspace.Common.BindableBase
    {
        public static Uri _baseUri = new Uri("ms-appx:///");

        public ABCItem(String uniqueId, String title, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._description = description;
            this.ImagePath = imagePath;
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
        public String ImagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this.ImagePath != null)
                {
                    this._image = new BitmapImage(new Uri(ABCItem._baseUri, this.ImagePath));
                }
                return this._image;
            }

            set
            {
                this.ImagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this.ImagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }


    public class LetterItem : ABCItem
    {
        public LetterItem(String uniqueId, String title, String imagePath, String description, int ID, AlphabetItem alphabet, String sound)
            : base(uniqueId, title, imagePath, description)
        {
            this._id = ID;
            this._alphabet = alphabet;
            this._sound = sound;
            WordItems.CollectionChanged += ItemsCollectionChanged;
        }

        private String _sound;
        public String Sound
        {
            get { return this._sound; }
            set { this.SetProperty(ref this._sound, value); }
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
        public WordItem(String uniqueId, String title, String imagePath, String description, int ID, AlphabetItem alphabet, String picturePath, String sound)
            : base(uniqueId, title, imagePath, description)
        {
            this._id = ID;
            this._alphabet = alphabet;
            this._PicturePath = picturePath;
            this._sound = sound;
        }

        private AlphabetItem _alphabet;
        public AlphabetItem Alphabet
        {
            get { return this._alphabet; }
            set { this.SetProperty(ref this._alphabet, value); }
        }

        private String _sound;
        public String Sound
        {
            get { return this._sound; }
            set { this.SetProperty(ref this._sound, value); }
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

        public WordItem Clone()
        {
            return new WordItem(this.UniqueId, this.Title, this.ImagePath, this.Description, this._id, this._alphabet, this._PicturePath, this._sound); 
        }
    }


    public class AlphabetItem: ABCItem
    {
        public AlphabetItem(String uniqueId, String title, String imagePath, String description, long ID)
            : base(uniqueId, title, imagePath, description)
        {
            this._id = ID;
            LetterItems.CollectionChanged += ItemsCollectionChanged;
        }

        private long _id = 0;
        public long ID
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

    public class DownLoadAlphabetItem : AlphabetItem, INotifyPropertyChanged
    {
        public DownLoadAlphabetItem(String uniqueId, String title, String imagePath, String description, long ID, long Length, bool IsSystem)
            : base(uniqueId, title, imagePath, description, ID)
        {
            this._downloadVisible = Visibility.Collapsed;
            this.DownLoadProgressMax = Length;
            this.IsSystem = IsSystem;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public String SizeInBytes
        {
            get
            {
                string offsetInKBytes = (this._LoadMax / 1024).ToString() + "KB ";
                if (this._LoadMax > 1024 * 1024)
                    return ((double)this._LoadMax / 1024 / 1024).ToString("F1") + "MB ";
                else return offsetInKBytes;
            }
        }

        private string _packageFileName;
        public string PackageFileName
        {
            get { return _packageFileName; }
            set { _packageFileName = value; }
        }

        private bool _isSystem;
        public bool IsSystem
        {
            get { return _isSystem; }
            set { _isSystem = value; }
        }

        private Visibility _downloadVisible;
        public Visibility DownLoadProcessVisible
        {
            get { return this._downloadVisible; }
            set
            {
                this._downloadVisible = value;
                NotifyPropertyChanged();
            }
        }

        private String _downloadStat;
        public String DownloadStatus
        {
            get { return this._downloadStat; }
            set 
            { 
                this._downloadStat = value;
                NotifyPropertyChanged();
            }
        }

        private long _LoadPos;
        public long DownLoadProgessPos
        {
            get { return this._LoadPos; }
            set 
            {
                this._LoadPos = value;
                NotifyPropertyChanged();
            }
        }

        private long _LoadMax;
        public long DownLoadProgressMax
        {
            get { return this._LoadMax; }
            set 
            {
                this._LoadMax = value; 
                NotifyPropertyChanged();
            }
        }
    }

    public sealed class ContentManager
    {
        private static ContentManager _ABCDataSource = new ContentManager(DBconnectionPath.Local);
        
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

        public static int AlphabetsCount(String DBPath)
        {
            SQLiteConnection db = new SQLiteConnection(DBPath);
            var command = db.CreateCommand("SELECT COUNT(ID) From Alphabet");
            return command.ExecuteScalar<int>();
        }

        public static AlphabetItem GetAlphabet(string uniqueId, String DBPath)
        {
            // Для небольших наборов данных можно использовать простой линейный поиск
            var matches = _ABCDataSource.AllAlphabets.Where((alphabet) => alphabet.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1)
            {
                
                SQLiteConnection db = new SQLiteConnection(DBPath);

                var Aitem = matches.First();
                if (Aitem.LetterItems.Count == 0)
                {
                    
                    var LetterQuery = db.Query<Letter>("SELECT * FROM Letter WHERE AlphabetID=?", Aitem.ID);

                    var LPath = ApplicationData.Current.LocalFolder.Path;

                    for (int j = 0; j < LetterQuery.Count; j++)
                    {
                        
                        
                             
                        var Litem = new LetterItem(
                                        String.Concat("Alphabet ", Aitem.ID.ToString(), " Letter ", LetterQuery[j].Guid.ToString()),
                                        LetterQuery[j].Value,
                                        Path.Combine(LPath, LetterQuery[j].Logo),
                                        LetterQuery[j].Value,
                                        LetterQuery[j].Guid,
                                        Aitem,
                                        Path.Combine(LPath, LetterQuery[j].Sound)
                                        );

                        var WordQuery = db.Query<Word>("SELECT * FROM Word WHERE AlphabetID=? AND LetterID=?", Aitem.ID, LetterQuery[j].Guid);

                        for (int k = 0; k < WordQuery.Count; k++)
                        {
                            String sound;
                            if (WordQuery[k].Sound == null) sound = "none";
                            else sound = Path.Combine(LPath, WordQuery[k].Sound);
                            
                            Litem.WordItems.Add(new WordItem(
                                            String.Concat("Alphabet ", WordQuery[k].AlphabetID.ToString(), " Letter ", WordQuery[k].LetterID.ToString(), " Word ", WordQuery[k].ID.ToString()),
                                            " ",
                                            Path.Combine(LPath, WordQuery[k].ValueImg),
                                            " ",
                                            WordQuery[k].ID,
                                            Aitem,
                                            Path.Combine(LPath, WordQuery[k].Image),
                                            sound
                                            ));
                        }

                        Aitem.LetterItems.Add(Litem);
                    }
                }
                return Aitem;
            }
            return null;
        }

        

        public static LetterItem GetItem(string uniqueId)
        {
            // Для небольших наборов данных можно использовать простой линейный поиск
            var matches = _ABCDataSource.AllAlphabets.SelectMany(group => group.LetterItems).Where((item) => item.UniqueId.Equals(uniqueId));
            return matches.First();           
            
        }

        public static WordItem GetWordItem(string uniqueId)
        {
            var AlphabetID = ParseAlphabetID(uniqueId);
            var LetterID = ParseLetterID(uniqueId);
            var WordID = ParseWordID(uniqueId);

            if (AlphabetID < 0 || LetterID < 0 || WordID < 0)
                return null;

            var alpha = _ABCDataSource.AllAlphabets.Where(alphabet => alphabet.ID == AlphabetID).First();
            var let = alpha.LetterItems.Where(letter=>letter.ID==LetterID).First();
            var w = let.WordItems.Where(word=>word.ID==WordID).First();

            return w;            
        }

        public static async Task<IEnumerable<DownLoadAlphabetItem>> DownloadFromAzureDB()
        {
            var AItems = new ObservableCollection<DownLoadAlphabetItem>();
            
                var packages = await AzureDBProvider.GetAllPackages();

                foreach (var pack in packages)
                {
                    var local = await AzureDBProvider.GetPackageLocalization(pack, LanguageProvider.CurrentLanguage.LanguageCode);

                    AItems.Add(new DownLoadAlphabetItem(String.Concat("Alphabet ", pack.Guid.ToString()),
                            local.LanguageName,
                            pack.Logo,
                            local.Description,
                            pack.Guid,
                            pack.Length,
                            pack.IsSystem
                        ));
                }

                return AItems;
        }

        public ContentManager(String DataSourcePath)
        {
            
               SQLiteConnection db = new SQLiteConnection(DataSourcePath);

               var AlphabetQuery = db.Query<Alphabet>("SELECT * FROM Alphabet WHERE IsSystem = 0");

                for (int i = 0; i < AlphabetQuery.Count; i++)
                {
                    var LocalQuery = db.Query<AlphabetLocalization>(
                                         "SELECT * FROM AlphabetLocalization WHERE AlphabetID=?", AlphabetQuery[i].Guid);

                    var languageID = System.Globalization.CultureInfo.CurrentCulture.Name;
                    var localization = LocalQuery.Where(l => l.LanguageID.Contains(languageID)).First();
                    if (localization == null) localization = LocalQuery.Where(l => l.LanguageID.Contains("en")).First();

                    var LPath = ApplicationData.Current.LocalFolder.Path;

                    var Aitem = new AlphabetItem(
                            String.Concat("Alphabet ", AlphabetQuery[i].Guid),
                            localization.LanguageName,
                            Path.Combine(LPath, AlphabetQuery[i].Logo),
                            localization.Description,
                            AlphabetQuery[i].Guid
                            );

                    _allAlphabets.Add(Aitem);
                }
           
        }

        public static int ParseAlphabetID(string uniqueID)
        {            
            if (uniqueID.Contains("Alphabet") == true)
            {
                string[] pars = uniqueID.Split(' ');
                return int.Parse(pars[1]);
            }
            else return -1;
        }

        public static int ParseLetterID(string uniqueID)
        {
            if (uniqueID.Contains("Letter") == true)
            {
                string[] pars = uniqueID.Split(' ');
                return int.Parse(pars[3]);
            }
            else return -1;
        }

        public static int ParseWordID(string uniqueID)
        {
            if (uniqueID.Contains("Word") == true)
            {
                string[] pars = uniqueID.Split(' ');
                return int.Parse(pars[5]);
            }
            else return -1;
        }

        public async static Task<bool> IsContentDownloaded(String DBPath)
        {
            try
            {
                await StorageFile.GetFileFromPathAsync(DBPath);
            } 
            catch
            {
                return false;
            }
            return true;
        }
    }


    public class PasswordBoxItem : levelupspace.Common.BindableBase
    {
        public PasswordBoxItem(String ImagePath)
        {
            this._imagePath = ImagePath;
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


        
        
    }

    public class PasswordBoxImageSource
    {
        public PasswordBoxImageSource(params String[] ImagePath)
        {
            _items = new ObservableCollection<PasswordBoxItem>();
            for (int i = 0; i < ImagePath.Length; i++)
            {
                _items.Add(new PasswordBoxItem(ImagePath[i]));
            }
        }
        private ObservableCollection<PasswordBoxItem> _items;
        
        public ObservableCollection<PasswordBoxItem> Items
        {
            get { return this._items; }
            
        }


    }

    

}
