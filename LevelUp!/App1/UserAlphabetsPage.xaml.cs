using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// 
    public sealed partial class UserAlphabetsPage : levelupspace.Common.LayoutAwarePage
    {
        Popup DownLoadABCPopup;

        public UserAlphabetsPage()
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
            var res = new ResourceLoader();
            try
            {
                var abcs = ContentManager.GetAlphabets((String)navigationParameter);
                if (abcs == null||(abcs as ObservableCollection<AlphabetItem>).Count == 0)
                {
                    if (DownLoadABCPopup == null)
                    {
                        // create the Popup in code
                        DownLoadABCPopup = new Popup();


                        // we are creating this in code and need to handle multiple instances
                        // so we are attaching to the Popup.Closed event to remove our reference
                        DownLoadABCPopup.Closed += (senderPopup, argsPopup) =>
                        {
                            DownLoadABCPopup = null;
                        };

                        DownLoadABCPopup.HorizontalOffset = (Window.Current.Bounds.Width - 600) / 2;
                        DownLoadABCPopup.VerticalOffset = 350;

                        // set the content to our UserControl

                        var chidPopup = new TextPopup(res.GetString("ABCIsEmptyMessage"), true);
                        chidPopup.OKClickEvent += DownLoadABCClicked;
                        DownLoadABCPopup.Child = chidPopup;

                        // open the Popup
                        DownLoadABCPopup.IsOpen = true;
                    }
                }
                else this.DefaultViewModel["ABCItems"] = abcs;
            }
            catch
            {
                Logger.ShowMessage(res.GetString("ConnectionError"));
            }
        }

        private void DownLoadABCClicked(object sender, EventArgs args)
        {
            if (!HttpProvider.IsInternetConnection())
            {
                var res = new ResourceLoader();
                Logger.ShowMessage(res.GetString("NoInternetConnectionError"));
                return;
            }
            this.Frame.Navigate(typeof(DownloadsPage), (int)DownloadPageState.ChoosePacks);
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

        private void gvAlphabets_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((AlphabetItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(AlphabetDetailsPage), itemId);

        }

        private void pageRoot_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            if (e.Key == Windows.System.VirtualKey.Escape) this.GoBack(this, args);
        }
    }
}
