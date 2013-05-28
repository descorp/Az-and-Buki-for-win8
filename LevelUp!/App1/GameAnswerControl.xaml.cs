using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace levelupspace
{
    public sealed partial class GameAnswerControl : UserControl
    {
        bool status;
        public GameAnswerControl(bool Status)
        {
            this.InitializeComponent();
            this.status = Status;
        }       

        private void UserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var res = new ResourceLoader();

            if (status)
            {
                tbMessage.Text = res.GetString("RightAnswer");
                ImgEmotion.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/HappyFace.png", UriKind.Absolute));

                

            }
            else
            {
                tbMessage.Text = res.GetString("WrongAnswer");
                ImgEmotion.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/SadFace.png", UriKind.Absolute));               

            }

             
        }
    }
}
