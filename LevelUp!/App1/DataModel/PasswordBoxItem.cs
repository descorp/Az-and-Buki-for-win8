using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using levelupspace.Common;

namespace levelupspace.DataModel
{
    public class PasswordBoxItem : BindableBase
    {
        public PasswordBoxItem(String ImagePath)
        {
            _imagePath = ImagePath;
        }

        private ImageSource _image;
        private String _imagePath;
        public ImageSource Image
        {
            get
            {
                if (_image == null && _imagePath != null)
                {
                    _image = new BitmapImage(new Uri(ABCItem.BaseUri, _imagePath));
                }
                return _image;
            }

            set
            {
                _imagePath = null;
                SetProperty(ref _image, value);
            }
        }




    }
}