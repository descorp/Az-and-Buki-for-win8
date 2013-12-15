using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237
using levelupspace.Common;
using levelupspace.DataModel;

namespace levelupspace
{
    enum Socials { VK, Facebook };
    
    
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    /// 
    public sealed partial class SharePage : LayoutAwarePage
    {
        String _imagePath;
        String _message;
        SocialProvider provider;
        int awardID=-1;
        int Net=-1;

        public static SharePage Current;

        public SharePage()
        {
            InitializeComponent();

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
                var parameters = ((String)navigationParameter).Split('/');

                if (pageState == null && parameters != null)
                {
                    int.TryParse(parameters[0], out awardID);
                    int.TryParse(parameters[1], out Net);
                }
                else
                {
                    if (pageState != null)
                    {
                        awardID = (int)pageState["SharePageAward"];
                        Net = (int)pageState["SocialNetworkID"];
                    }
                }

                AwardItem award = null;

                if (awardID > 0)
                    award = await AwardManager.GetAward(awardID, DBconnectionPath.Local);
                if (award != null)
                {

                    _imagePath = award.ImagePath;
                    _message = String.Format(res.GetString("PostMessage"), award.Title);
                    try
                    {

                        switch (Net)
                        {
                            case (int)Socials.VK:
                                provider = new VKProvider();
                                break;
                            case (int)Socials.Facebook:
                                provider = new FacebookProvider();
                                break;
                            default:
                                Frame.Navigate(typeof(AchievementsPage));
                                break;
                        }


                        WebPage.Navigate(provider.LoginUri);
                        WebPage.LoadCompleted += WebPage_LoadCompleted;

                    }
                    catch
                    {
                        Logger.ShowMessage(res.GetString("ConnectionError"));
                        Frame.Navigate(typeof(AchievementsPage));
                    }
                }
                else
                {
                    Logger.ShowMessage(res.GetString("NoInternetConnectionError"));
                    Frame.Navigate(typeof(AchievementsPage));
                }
            }
            else Frame.Navigate(typeof(AchievementsPage));
            
        }

        private void WebPage_LoadCompleted(object sender, NavigationEventArgs e)
        {
            tbProgress.Visibility = Visibility.Collapsed;
            PBLoad.Visibility = Visibility.Collapsed;

            provider.SentEvent += PostSending_Completed;

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

            Frame.Navigate(typeof(AchievementsPage), "WallPostSent");
        }

        public async void SendingCompleted()
        {
            await Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    tbProgress.Visibility = Visibility.Collapsed;
                    PBLoad.Visibility = Visibility.Collapsed;

                    Frame.Navigate(typeof(AchievementsPage), "WallPostSent");
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
            pageState["SharePageAward"] = awardID;
            pageState["SocialNetworkID"] = Net;
        }

        private void pageRoot_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var args = new RoutedEventArgs();
            if (e.Key == VirtualKey.Escape) GoBack(this, args);
        }
    }
}
