using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace LevelUP
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class GameResultPage : LevelUP.Common.LayoutAwarePage
    {
        Popup MessageBoxPopup;
        public GameResultPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации. Также предоставляется любое сохраненное состояние
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="navigationParameter">Значение параметра, передаваемое
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы.
        /// </param>
        /// <param name="pageState">Словарь состояния, сохраненного данной страницей в ходе предыдущего
        /// сеанса. Это значение будет равно NULL при первом посещении страницы.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var rate = (double)navigationParameter;

            if ( !UserManager.IsAutorized && rate>0.4 )
            {
                if (MessageBoxPopup == null)
                {
                    // create the Popup in code
                    MessageBoxPopup = new Popup();


                    // we are creating this in code and need to handle multiple instances
                    // so we are attaching to the Popup.Closed event to remove our reference
                    MessageBoxPopup.Closed += (senderPopup, argsPopup) =>
                    {
                        MessageBoxPopup = null;
                    };

                    MessageBoxPopup.HorizontalOffset = (Window.Current.Bounds.Width - 600) / 2;
                    MessageBoxPopup.VerticalOffset = 350;

                    // set the content to our UserControl
                    MessageBoxPopup.Child = new TextPopup("Войди под своим именем, чтобы получать награды за блестящую игру");

                    // open the Popup
                    MessageBoxPopup.IsOpen = true;
                }
            }

            

            tbStatus.Text = "Кажется, тебе нужно еще поучиться";
            if (rate > 0.2)            
                imgStar1.Source = new BitmapImage(new Uri(ABCItem._baseUri, "ms-appx:///Assets/FullStar.png"));
            if (rate > 0.4)
            {
                imgStar2.Source = new BitmapImage(new Uri(ABCItem._baseUri, "ms-appx:///Assets/FullStar.png"));
                tbStatus.Text = "Уже неплохо! Я верю, ты можешь лучше!";
            }
            if (rate > 0.6)
            {
                imgStar3.Source = new BitmapImage(new Uri(ABCItem._baseUri, "ms-appx:///Assets/FullStar.png"));
                tbStatus.Text = "Так держать!";
            }
            if (rate > 0.8)
            {
                imgStar4.Source = new BitmapImage(new Uri(ABCItem._baseUri, "ms-appx:///Assets/FullStar.png"));
                tbStatus.Text = "Молодец! Отличный результат!";
            }
            if (rate > 0.9)
            {
                imgStar5.Source = new BitmapImage(new Uri(ABCItem._baseUri, "ms-appx:///Assets/FullStar.png"));
                tbStatus.Text = "Умница, похоже, ты гений!";
            }
            
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации. Значения должны соответствовать требованиям сериализации
        /// <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">Пустой словарь, заполняемый сериализуемым состоянием.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }
    }
}
