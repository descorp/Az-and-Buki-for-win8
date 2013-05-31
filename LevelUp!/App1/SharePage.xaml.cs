using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

            var res = new ResourceLoader();

            tbProgress.Text = res.GetString("ConnectingToServerMessage");
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
            var res = new ResourceLoader();
            if (HttpProvider.IsInternetConnection())
            {
                var parameters = (Dictionary<string, int>)navigationParameter;

                var award = await AwardManager.GetAward((int)parameters["award"], DBconnectionPath.Local);

                

                _imagePath = award.ImagePath;
                _message = String.Format(res.GetString("PostMessage"), award.Title);
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
                    Logger.ShowMessage(res.GetString("ConnectionError"));
                    this.Frame.Navigate(typeof(AchievementsPage));
                }
            }
            else
            {
                Logger.ShowMessage(res.GetString("NoInternetConnectionError"));
                this.Frame.Navigate(typeof(AchievementsPage));
            }
            
        }

        private void WebPage_LoadCompleted(object sender, NavigationEventArgs e)
        {
            tbProgress.Visibility = Visibility.Collapsed;
            PBLoad.Visibility = Visibility.Collapsed;

            string Address = e.Uri.OriginalString.ToLower();
            
            provider.SentEvent += new EventHandler(PostSending_Completed);

            var res = new ResourceLoader();

            if (provider.URLParser(e.Uri))
            {
                tbProgress.Text = res.GetString("SendingMessage");
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

        private void pageRoot_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            if (e.Key == Windows.System.VirtualKey.Escape) this.GoBack(this, args);
        }
    }
}
