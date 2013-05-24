using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            if (status)
            {
                tbMessage.Text = "Молодец!";
                ImgEmotion.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/HappyFace.png", UriKind.Absolute));

                

            }
            else
            {
                tbMessage.Text = "Ой, кажется, ты ответил неправильно = (";
                ImgEmotion.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/SadFace.png", UriKind.Absolute));               

            }

             
        }
    }
}
