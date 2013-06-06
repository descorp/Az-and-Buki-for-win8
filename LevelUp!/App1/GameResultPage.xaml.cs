using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace levelupspace
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class GameResultPage : levelupspace.Common.LayoutAwarePage
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
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var rate = (double)navigationParameter;

            var res = new ResourceLoader();

            int StarRate = 0;

            tbStatus.Text = res.GetString("BadGameResult");
            if (rate > 0.2)
            {
                imgStar1.Source = new BitmapImage(new Uri(ABCItem._baseUri, "ms-appx:///Assets/FullStar.png"));
                StarRate = 1;
            }
            if (rate > 0.4)
            {
                imgStar2.Source = new BitmapImage(new Uri(ABCItem._baseUri, "ms-appx:///Assets/FullStar.png"));
                tbStatus.Text = res.GetString("LowAwerageGameResult");
                StarRate = 2;
            }
            if (rate > 0.6)
            {
                imgStar3.Source = new BitmapImage(new Uri(ABCItem._baseUri, "ms-appx:///Assets/FullStar.png"));
                tbStatus.Text = res.GetString("AwerageGameResult");
                StarRate = 3;
            }
            if (rate > 0.8)
            {
                imgStar4.Source = new BitmapImage(new Uri(ABCItem._baseUri, "ms-appx:///Assets/FullStar.png"));
                tbStatus.Text = res.GetString("GoodGameResult");
                StarRate = 4;
            }
            if (rate > 0.9)
            {
                imgStar5.Source = new BitmapImage(new Uri(ABCItem._baseUri, "ms-appx:///Assets/FullStar.png"));
                tbStatus.Text = res.GetString("PerfectGameResult");
                StarRate = 5;
            }

            if (!UserManager.IsAutorized && rate > 0.4)
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
                    MessageBoxPopup.Child = new TextPopup(res.GetString("LogInForPrizes"));

                    // open the Popup
                    MessageBoxPopup.IsOpen = true;
                }
            }
            else
            {
                AwardItem award=null;
                try
                {
                    award = await AwardManager.GetAwardForRate(StarRate, DBconnectionPath.Local);
                }
                catch { };
                if (award != null)
                {
                    AwardManager.AddUserAward(award, UserManager.UserId, DBconnectionPath.Local);
                    this.DefaultViewModel["Award"] = award;
                    tbAboutAchievements.Text = res.GetString("YouWonPrizeMessage");
                }
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

        private new void GoBack(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainMenu));
        }

        private void pageRoot_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            if (e.Key == Windows.System.VirtualKey.Escape) this.GoBack(this, args);
        }
    }
}
