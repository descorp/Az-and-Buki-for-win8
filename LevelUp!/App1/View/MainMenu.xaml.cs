using System;
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237
using levelupspace.Common;
using levelupspace.DataModel;

namespace levelupspace
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    
    public sealed partial class MainMenu : LayoutAwarePage
    {
        Popup LogInPopup;

        Popup MessageBoxPopup;

       

        public MainMenu()
        {         
            InitializeComponent();
            Current = this;
            cbLangs.ItemsSource = LanguageProvider.AllLanguages;
            cbLangs.SelectedIndex = 0;
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
                else
                {
                    var res = new ResourceLoader();

                    btnLogOn.Visibility = Visibility.Visible;
                    btnLogIn.Content = res.GetString("btnLogInContent");
                    tbUserName.Text = res.GetString("NotLogedIn");
                    imgProfile.Source = new BitmapImage(new Uri("ms-appx:///Assets/Userlogo.png"));
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
            Frame.Navigate(typeof(UserAlphabetsPage), "AllAlphabets");

        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChooseGamePage));
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
                    LogInPopup.Child = new AuthorizationControl();

                    // open the Popup
                    LogInPopup.IsOpen = true;
                }
            }
        }

        private void btnLogOn_Click(object sender, RoutedEventArgs e)
        {
            var res = new ResourceLoader();
            if (!HttpProvider.IsInternetConnection())
            {
                Logger.ShowMessage(res.GetString("NoInternetConnectionError"));
                return;
            }
            if (LogInPopup != null)
            {
                LogInPopup.IsOpen = false;
                LogInPopup.Closed += (senderPopup, argsPopup) =>
                {
                    LogInPopup = null;
                };
            }
            Frame.Navigate(typeof(UserSignOnPage));
        }

        public void UserLogIn()
        {
            var res = new ResourceLoader();

            btnLogIn.Content = res.GetString("btnLogOutContent");
            btnLogOn.Visibility = Visibility.Collapsed;
            var name = (String)ApplicationData.Current.LocalSettings.Values["UserName"];
            tbUserName.Text = String.Format(res.GetString("Greeting"),name);
            var logoPath = (String)ApplicationData.Current.LocalSettings.Values["UserLogo"];
            if (logoPath != "ms-appx:///Assets/Userlogo.png")
            {
                logoPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, logoPath);
            }

            imgProfile.Source = new BitmapImage(new Uri(ABCItem.BaseUri, logoPath));            
        }

        public void UserLogOut()
        {
            var res = new ResourceLoader();

            btnLogOn.Visibility = Visibility.Visible;
            btnLogIn.Content = res.GetString("btnLogInContent");
            tbUserName.Text = res.GetString("NotLogedIn");
            imgProfile.Source = new BitmapImage(new Uri("ms-appx:///Assets/Userlogo.png"));  
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
                Frame.Navigate(typeof(AchievementsPage));
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
                    var res = new ResourceLoader();
                    MessageBoxPopup.Child = new TextPopup(res.GetString("LogInPlease"));

                    // open the Popup
                    MessageBoxPopup.IsOpen = true;
                }
            }
        }

        private void cbLangs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LanguageProvider.CurrentLanguage = e.AddedItems[0] as LanguageItem;

            var _Frame = Window.Current.Content as Frame;
            if (_Frame != null) _Frame.Navigate(typeof(MainMenu));

            
        }

        private void BtnLoadAlphabets_OnClick(object sender, RoutedEventArgs e)
        {
            var _Frame = Window.Current.Content as Frame;
            if (_Frame != null) _Frame.Navigate(typeof(DownloadsPage), DownloadPageState.ChoosePacks);
        }
    }
}
