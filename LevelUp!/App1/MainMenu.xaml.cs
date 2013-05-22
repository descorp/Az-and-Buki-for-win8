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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace LevelUP
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    
    public sealed partial class MainMenu : LevelUP.Common.LayoutAwarePage
    {
        Popup LogInPopup;

        Popup MessageBoxPopup;

        public MainMenu()
        {
            this.InitializeComponent();
            Current = this;
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

            if (((String)navigationParameter) == "Autorized")
            {
                UserLogIn();
            }
            else if (((String)navigationParameter) == null)
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("UserName"))
                {
                    UserLogIn();
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

        private void btnAlphabets_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserAlphabetsPage), "AllAlphabets");

        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChooseGamePage));
        }
        public static MainMenu Current;


        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (UserManager.IsAutorized)
            {
                UserLogOut();
            }
            else
            {
                if (LogInPopup == null)
                {
                    // create the Popup in code
                    LogInPopup = new Popup();
                                       

                    // we are creating this in code and need to handle multiple instances
                    // so we are attaching to the Popup.Closed event to remove our reference
                    LogInPopup.Closed += (senderPopup, argsPopup) =>
                    {
                        LogInPopup = null;
                    };

                    LogInPopup.HorizontalOffset = (Window.Current.Bounds.Width - 600) / 2;
                    LogInPopup.VerticalOffset = (Window.Current.Bounds.Height - 440) / 2;

                    // set the content to our UserControl
                    LogInPopup.Child = new AutorizationControl();

                    // open the Popup
                    LogInPopup.IsOpen = true;
                }
            }
        }

        private void btnLogOn_Click(object sender, RoutedEventArgs e)
        {
            if (LogInPopup != null)
            {
                LogInPopup.IsOpen = false;
                LogInPopup.Closed += (senderPopup, argsPopup) =>
                {
                    LogInPopup = null;
                };
            }
            this.Frame.Navigate(typeof(UserSignOnPage));
        }

        public void UserLogIn()
        {            

            btnLogIn.Content = "Выход";
            btnLogOn.Visibility = Visibility.Collapsed;
            var Name = (String)ApplicationData.Current.LocalSettings.Values["UserName"];
            tbUserName.Text = "Привет, " + Name + "!";
            var LogoPath = (String)ApplicationData.Current.LocalSettings.Values["UserLogo"];
            if (LogoPath != "ms-appx:///Assets/Userlogo.png")
            {
                LogoPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, LogoPath);
            }

            imgProfile.Source = new BitmapImage(new Uri(ABCItem._baseUri, LogoPath));            
        }

        public void UserLogOut()
        {
            btnLogOn.Visibility = Visibility.Visible;
            btnLogIn.Content = "Вход";
            tbUserName.Text = "Вход не выполнен";
            UserManager.LogOut();
            
        }

        private void btnHighScores_Click(object sender, RoutedEventArgs e)
        {
            if (LogInPopup != null)
            {
                LogInPopup.IsOpen = false;
                LogInPopup.Closed += (senderPopup, argsPopup) =>
                {
                    LogInPopup = null;
                };
            }

            if (UserManager.IsAutorized)
                this.Frame.Navigate(typeof(AchievementsPage), UserManager.UserId);
            else
            {
                if ( MessageBoxPopup == null)
                {
                    // create the Popup in code
                    MessageBoxPopup = new Popup();


                    // we are creating this in code and need to handle multiple instances
                    // so we are attaching to the Popup.Closed event to remove our reference
                    MessageBoxPopup.Closed += (senderPopup, argsPopup) =>
                    {
                        MessageBoxPopup = null;
                    };

                    MessageBoxPopup.HorizontalOffset = (Window.Current.Bounds.Width - 600)/2;
                    MessageBoxPopup.VerticalOffset = 350;

                    // set the content to our UserControl
                    MessageBoxPopup.Child = new TextPopup("Войди под своим именем, чтобы просматривать и получать достижения");

                    // open the Popup
                    MessageBoxPopup.IsOpen = true;
                }
            }
        }
    }
}
