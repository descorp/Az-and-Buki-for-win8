using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace levelupspace
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class AchievementsPage : levelupspace.Common.LayoutAwarePage
    {

        Popup MessageBoxPopup;

        public AchievementsPage()
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
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            this.DefaultViewModel["UAwards"] = await AwardManager.UsersAwards(UserManager.UserId);

            if ((string)navigationParameter == "WallPostSent")
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
                    var res = new ResourceLoader();
                    MessageBoxPopup.Child = new TextPopup(res.GetString("WallPostSentMessage"));

                    // open the Popup
                    MessageBoxPopup.IsOpen = true;
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

        private void btnShare_Click(object sender, RoutedEventArgs e)
        {
            
            var parameters = new Dictionary<string, int>();
            parameters["social"] = (int)Socials.VK;
            parameters["award"] = (int)((sender as Button).Tag);
            this.Frame.Navigate(typeof(SharePage), parameters);
            
        }

        private void btnShareFB_Click(object sender, RoutedEventArgs e)
        {
            var parameters = new Dictionary<string, int>();
            parameters["social"] = (int)Socials.Facebook;
            parameters["award"] = (int)((sender as Button).Tag);
            this.Frame.Navigate(typeof(SharePage), parameters);

        }
    }
}
