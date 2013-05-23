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
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace LevelUP
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class VKAuthPage : LevelUP.Common.LayoutAwarePage
    {
        String _imagePath;
        String _message;

        public VKAuthPage()
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

            var award = await AwardManager.GetAward((int) navigationParameter);
            _imagePath = award.ImagePath;
            _message = String.Concat("Мой ребенок получил ",award.Title," за успехи в изучении английского алфавита");
                try
                {
                    WebPage.Navigate(VKProvider.AuthorizationUri);
                    WebPage.LoadCompleted += new LoadCompletedEventHandler(WebPage_LoadCompleted);

                }
                catch (FormatException ex)
                {
                    Logger.ShowMessage("Проблемы с соединением!");
                }
        }

        private void WebPage_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string Address = e.Uri.OriginalString.ToLower();
            VKProvider provider = new VKProvider();
            if (provider.URLParser(e.Uri))
                provider.WallPost(_message, _imagePath);
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
    }
}
