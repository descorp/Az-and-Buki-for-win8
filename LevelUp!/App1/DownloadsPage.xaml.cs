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
using Windows.ApplicationModel.Resources;
using Windows.Storage;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace levelupspace
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class DownloadsPage : levelupspace.Common.LayoutAwarePage
    {
        enum DownloadPageState { ChooseLang, ChoosePacks, Waiting };
        private DownloadPageState state;

        private void ChangeState(DownloadPageState state)
        {
            this.state = state;
            var res = new ResourceLoader();
            switch (state)
            {
                case DownloadPageState.ChooseLang:

                    pageTitle.Text = res.GetString("TuningAppTitle");
                    tbStatus.Text = res.GetString("DownLoadPageChooseLangText");
                    tbStatus.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    pRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    gwDownLoadItems.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                case DownloadPageState.Waiting:
                    tbStatus.Text = res.GetString("PleaseWaitMessage");
                    tbStatus.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    pRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    cbLangs.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    gwDownLoadItems.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    btnChooseLang.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                case DownloadPageState.ChoosePacks:
                    this.DefaultViewModel["HeaderText"] = res.GetString("ChoosePacksMessage");
                    tbStatus.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    pRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    cbLangs.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    gwDownLoadItems.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    btnChooseLang.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    break;
            }
        }

        public DownloadsPage()
        {
            this.InitializeComponent();
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
            if (navigationParameter == null) ChangeState(DownloadPageState.ChooseLang);
            else ChangeState((DownloadPageState)navigationParameter);
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

        private void LocalizationSelected(object sender, SelectionChangedEventArgs e)
        {
            LanguageProvider.CurrentLanguage = cbLangs.SelectedItem as LanguageItem;
            var _Frame = Window.Current.Content as Frame;
            _Frame.Navigate(typeof(DownloadsPage));
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {


        }

        private async void btnChooseLang_Click(object sender, RoutedEventArgs e)
        {
            switch (state)
            {
                case DownloadPageState.ChooseLang:
                    string localization = cbLangs.SelectedItem.ToString();

                    ChangeState(DownloadPageState.Waiting);
                    var ABCs = await ContentManager.DownloadFromAzureDB();
                    this.DefaultViewModel["ABCItems"] = ABCs;
                    ChangeState(DownloadPageState.ChoosePacks);
                    break;
                case DownloadPageState.ChoosePacks:
                    ChangeState(DownloadPageState.Waiting);
                    List<StorageFile> files = new List<StorageFile>();
                    foreach (AlphabetItem item in gwDownLoadItems.SelectedItems)
                    {
                        var Local = ApplicationData.Current.TemporaryFolder;
                        var file = await Local.CreateFileAsync(item.ID.ToString() + "_pack", CreationCollisionOption.ReplaceExisting);
                        files.Add(file);
                        string blobName = await AzureDBProvider.GetBlobName((int)item.ID);
                        AzureStorageProvider.DownloadPackageFromStorage(file, blobName, 5240420, FileDownloaded, FilePartDownloaded);
                        //TODO:  remove hardcode and set normal file length
                    };

                    break;
            }

        }

        private void FileDownloaded(object sender, EventArgs args)
        {
            var file = sender as StorageFile;
            tbStatus.Text = file.DisplayName + " downloaded\r\n";
        }

        private void FilePartDownloaded(object sender, EventArgs args)
        {
            var argument = args as FilePartDownloadedEvent;
            string offsetInKBytes = (argument.Offset / 1024) .ToString() + "KB ";
            if (argument.Offset > 1024 * 1024) offsetInKBytes = ((double)argument.Offset / 1024 / 1024).ToString("F1") + "MB ";
            double persent = (double)(argument.Offset) / (double)(5240420) * 100;
            tbStatus.Text = offsetInKBytes + " downloaded - " + persent.ToString("F1");
        }

    }
}
