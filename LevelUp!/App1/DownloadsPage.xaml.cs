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
using levelupspace.DataModel;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace levelupspace
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class DownloadsPage : levelupspace.Common.LayoutAwarePage
    {

        private int state = 0;

        private void ChangeState(int state)
        {
            switch (state)
            {
                case 0: 
                    pageTitle.Text = "Language";
                    tbStatus.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    pRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                case 1: 
                    pageTitle.Text = "Loading";
                    tbStatus.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    pRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    cbLangs.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                case 2: break;
            }
        }

        public DownloadsPage()
        {
            this.InitializeComponent();
            gwDownLoadItems.Visibility = Windows.UI.Xaml.Visibility.Visible;
            
           

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

        private async void LocalizationSelected(object sender, SelectionChangedEventArgs e)
        {
            string localization = cbLangs.SelectedItem.ToString();
            state++;
            ChangeState(state);

            List<Alphabet> packageList = await AzureDBProvider.GetAllPackages();

            foreach (Alphabet package in packageList)
            {

            }
        }

        private async void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            var ABCs = await ContentManager.DownloadFromAzureDB();
            this.DefaultViewModel["ABCItems"] = ABCs;
            this.pRing.Visibility = Visibility.Collapsed;
            this.gwDownLoadItems.Visibility = Visibility.Visible;
        }
    }
}
