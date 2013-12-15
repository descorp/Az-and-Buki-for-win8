using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace levelupspace.DataModel
{
    public class WordItem : ABCItem
    {
        public WordItem(String uniqueId, String title, String imagePath, String description, int id, AlphabetItem alphabet, String picturePath, String sound)
            : base(uniqueId, title, imagePath, description)
        {
            _id = id;
            _alphabet = alphabet;
            _picturePath = picturePath;
            _sound = sound;
        }

        private AlphabetItem _alphabet;
        public AlphabetItem Alphabet
        {
            get { return _alphabet; }
            set { SetProperty(ref _alphabet, value); }
        }

        private String _sound;
        public String Sound
        {
            get { return _sound; }
            set { SetProperty(ref _sound, value); }
        }

        private int _id;
        public int ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private ImageSource _imgPicture;
        private String _picturePath;
        public ImageSource ImgPicture
        {
            get
            {
                if (_imgPicture == null && _picturePath != null)
                {
                    _imgPicture = new BitmapImage(new Uri(BaseUri, _picturePath));
                }
                return _imgPicture;
            }

            set
            {
                _picturePath = null;
                SetProperty(ref _imgPicture, value);
            }
        }

        public void SetPicture(String path)
        {
            _imgPicture = null;
            _picturePath = path;
            OnPropertyChanged("imgPicture");
        }

        public override string ToString()
        {
            return Title;
        }

        public WordItem Clone()
        {
            return new WordItem(UniqueId, Title, ImagePath, Description, _id, _alphabet, _picturePath, _sound);
        }
    }
}