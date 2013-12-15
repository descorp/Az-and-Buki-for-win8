using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.UI.Xaml.Input;
using levelupspace.Common;
using levelupspace.DataModel;

namespace levelupspace
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class AchievementsPage : LayoutAwarePage
    {

        Popup _messageBoxPopup;

        public AchievementsPage()
        {
            InitializeComponent();
            
            
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
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var res = new ResourceLoader();
            try
            {
                DefaultViewModel["UAwards"] = await AwardManager.UsersAwards(UserManager.UserId, DBconnectionPath.Local);

                if ((string)navigationParameter == "WallPostSent")
                {

                    if (_messageBoxPopup == null)
                    {
                        // create the Popup in code
                        _messageBoxPopup = new Popup();


                        // we are creating this in code and need to handle multiple instances
                        // so we are attaching to the Popup.Closed event to remove our reference
                        _messageBoxPopup.Closed += (senderPopup, argsPopup) =>
                        {
                            _messageBoxPopup = null;
                        };

                        _messageBoxPopup.HorizontalOffset = (Window.Current.Bounds.Width - 600) / 2;
                        _messageBoxPopup.VerticalOffset = 350;

                        // set the content to our UserControl
                        _messageBoxPopup.Child = new TextPopup(res.GetString("WallPostSentMessage"));

                        // open the Popup
                        _messageBoxPopup.IsOpen = true;
                    }

                }
            }
            catch
            {
                Frame.Navigate(typeof(MainMenu));
                Logger.ShowMessage(res.GetString("ConnectionError"));
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

        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var parameters = (button.Tag + "/" + (int)Socials.VK);

                Frame.Navigate(typeof(SharePage), parameters);
            }
        }

        private void btnShareFB_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var parameters = (button.Tag + "/" + (int)Socials.Facebook);
            
                Frame.Navigate(typeof(SharePage), parameters);
            }
        }

        private void pageRoot_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var args = new RoutedEventArgs();
            if (e.Key == VirtualKey.Escape) GoBack(this, args);
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainMenu));
        }
    }
}
