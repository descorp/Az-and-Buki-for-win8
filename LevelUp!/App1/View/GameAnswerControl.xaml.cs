using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
using Windows.UI.Xaml.Media.Imaging;

namespace levelupspace
{
    public sealed partial class GameAnswerControl : UserControl
    {
        readonly bool _status;
        public GameAnswerControl(bool Status)
        {
            InitializeComponent();
            _status = Status;
        }       

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var res = new ResourceLoader();

            if (_status)
            {
                tbMessage.Text = res.GetString("RightAnswer");
                ImgEmotion.Source = new BitmapImage(new Uri("ms-appx:///Assets/HappyFace.png", UriKind.Absolute));

                

            }
            else
            {
                tbMessage.Text = res.GetString("WrongAnswer");
                ImgEmotion.Source = new BitmapImage(new Uri("ms-appx:///Assets/SadFace.png", UriKind.Absolute));               

            }

             
        }
    }
}
