using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using levelupspace.Common;

namespace levelupspace.DataModel
{
    public abstract class ABCItem : BindableBase
    {
        public static Uri BaseUri = new Uri("ms-appx:///");

        protected ABCItem(String uniqueId, String title, String imagePath, String description)
        {
            _uniqueId = uniqueId;
            _title = title;
            _description = description;
            ImagePath = imagePath;
        }
        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return _uniqueId; }
            set { SetProperty(ref _uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return _subtitle; }
            set { SetProperty(ref _subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private ImageSource _image;
        public String ImagePath = null;
        public ImageSource Image
        {
            get
            {
                if (_image == null && ImagePath != null)
                {
                    _image = new BitmapImage(new Uri(BaseUri, ImagePath));
                }
                return _image;
            }

            set
            {
                ImagePath = null;
                SetProperty(ref _image, value);
            }
        }

        public void SetImage(String path)
        {
            _image = null;
            ImagePath = path;
            OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return Title;
        }
    }
}