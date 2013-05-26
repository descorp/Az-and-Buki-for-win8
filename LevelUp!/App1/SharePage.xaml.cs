using Facebook;
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

namespace levelupspace
{
    enum Socials { VK, Facebook };
    
    
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    /// 
    public sealed partial class SharePage : levelupspace.Common.LayoutAwarePage
    {
        String _imagePath;
        String _message;
        SocialProvider provider;

        public static SharePage Current;

        public SharePage()
        {
            this.InitializeComponent();
            tbProgress.Text = "Подключение к серверу...";
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
        protected async override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (HttpProvider.IsInternetConnection())
            {
                var parameters = (Dictionary<string, int>)navigationParameter;

                var award = await AwardManager.GetAward((int)parameters["award"]);

                _imagePath = award.ImagePath;
                _message = String.Concat("Мой ребенок получил ", award.Title, " за успехи в изучении английского алфавита");
                try
                {  
                    switch (parameters["social"])
                    {
                    case (int)Socials.VK:
                        provider = new VKProvider();                        
                        break;
                    case (int)Socials.Facebook:
                        provider = new FacebookProvider(); 
                        break;
                    default:
                        this.Frame.Navigate(typeof(AchievementsPage));
                        break;
                    }
                    
                    
                    WebPage.Navigate(provider.LoginUri);
                    WebPage.LoadCompleted += new LoadCompletedEventHandler(WebPage_LoadCompleted);

                }
                catch (FormatException ex)
                {
                    Logger.ShowMessage("Проблемы с соединением!");
                    this.Frame.Navigate(typeof(AchievementsPage));
                }
            }
            else
            {
                Logger.ShowMessage("Нет интернет-соединения!");
                this.Frame.Navigate(typeof(AchievementsPage));
            }
            
        }

        private void WebPage_LoadCompleted(object sender, NavigationEventArgs e)
        {
            tbProgress.Visibility = Visibility.Collapsed;
            PBLoad.Visibility = Visibility.Collapsed;

            string Address = e.Uri.OriginalString.ToLower();
            
            provider.SentEvent += new EventHandler(PostSending_Completed);


            if (provider.URLParser(e.Uri))
            {
                tbProgress.Text = "Отправка сообщения...";
                tbProgress.Visibility = Visibility.Visible;
                PBLoad.Visibility = Visibility.Visible;

                provider.WallPost(_message, _imagePath);
            }
                
        }

        private void PostSending_Completed(object sender, EventArgs e)
        {
            tbProgress.Visibility = Visibility.Collapsed;
            PBLoad.Visibility = Visibility.Collapsed;

            this.Frame.Navigate(typeof(AchievementsPage), "WallPostSent");
        }

        public async void SendingCompleted()
        {
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    tbProgress.Visibility = Visibility.Collapsed;
                    PBLoad.Visibility = Visibility.Collapsed;

                    this.Frame.Navigate(typeof(AchievementsPage), "WallPostSent");
                });
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
