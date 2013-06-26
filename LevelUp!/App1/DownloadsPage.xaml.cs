using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
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
using Windows.Networking.BackgroundTransfer;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace levelupspace
{
    public enum DownloadPageState { ChooseLang, ChoosePacks, Waiting, Downloading };
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    /// 
    public sealed partial class DownloadsPage : levelupspace.Common.LayoutAwarePage
    {
        
        private DownloadPageState state;
        private List<DownLoadAlphabetItem> DownloadingPackagesCollection = new List<DownLoadAlphabetItem>();
        private BackgroundDownloader downloader = new BackgroundDownloader();

        private async void ChangeState(DownloadPageState state)
        {
            this.state = state;
            var res = new ResourceLoader();
            switch (state)
            {
                case DownloadPageState.ChooseLang:
                    cbLangs.ItemsSource = LanguageProvider.AllLanguages;
                    cbLangs.SelectedIndex = 0;
                    cbLangs.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    btnChooseLang.Visibility = Windows.UI.Xaml.Visibility.Visible;
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
                    pageTitle.Text = res.GetString("WaitingAppTitle");
                    cbLangs.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    gwDownLoadItems.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    btnChooseLang.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    break;
                case DownloadPageState.ChoosePacks:
                    if (!HttpProvider.IsInternetConnection())
                    {
                        this.Frame.Navigate(typeof(MainMenu));
                        Logger.ShowMessage(res.GetString("NoInternetConnectionError"));
                        DBFiller.CreateDB(DBconnectionPath.Local);                        
                        return;
                    }

                    pageTitle.Text = res.GetString("DownloadingAppTitle");

                    try
                    {
                        var allPackagesFromAzure = await ContentManager.DownloadFromAzureDB();
                        List<DownLoadAlphabetItem> packageSholdBeShown = allPackagesFromAzure.ToList();
                        DBFiller.CreateDB(DBconnectionPath.Local);

                        var packagesAlreadyDownloaded = ContentManager.GetListOfDownloadedPackagesID(DBconnectionPath.Local);

                        foreach (int ID in packagesAlreadyDownloaded)
                            foreach (DownLoadAlphabetItem alph in allPackagesFromAzure)
                                if (alph.ID == ID)
                                    packageSholdBeShown.Remove(alph);

                        this.DefaultViewModel["ABCItems"] = packageSholdBeShown;

                        foreach (DownLoadAlphabetItem alph in packageSholdBeShown)
                            if (alph.IsSystem)
                                gwDownLoadItems.SelectedItems.Add(alph);


                        this.DefaultViewModel["HeaderText"] = res.GetString("ChoosePacksMessage");
                        tbStatus.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        pRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        cbLangs.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        gwDownLoadItems.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        btnChooseLang.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    catch
                    {
                        Logger.ShowMessage("ConnectionError");
                        this.Frame.Navigate(typeof(MainMenu));
                    }
                    break;
                case DownloadPageState.Downloading:
                    btnChooseLang.IsEnabled = false;
                    gwDownLoadItems.SelectionMode = ListViewSelectionMode.None;
                    break;
            }
        }

        public DownloadsPage()
        {
            this.InitializeComponent();
            downloader.Method = "GET";
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
            ChangeState(DownloadPageState.Waiting);
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
            if (_Frame != null)
            _Frame.Navigate(typeof(DownloadsPage));
        }

        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void btnChooseLang_Click(object sender, RoutedEventArgs e)
        {
            var res = new ResourceLoader();
            switch (state)
            {
                case DownloadPageState.ChooseLang:
                    ChangeState(DownloadPageState.Waiting);

                    ChangeState(DownloadPageState.ChoosePacks);

                    break;
                case DownloadPageState.ChoosePacks:
                    foreach (DownLoadAlphabetItem item in gwDownLoadItems.SelectedItems)
                    {
                        //http://levelupstorage.blob.core.windows.net/packages/A1.zip?st=2013-06-11T17%3A55%3A33Z&se=2013-06-11T18%3A55%3A33Z&sr=c&sp=r&sig=%2FtzEgXYJ3rudszpYS7l8KD%2Bs4tRO%2FlB7MZfGYV5BAoM%3D
                        try
                        {
                            DownloadingPackagesCollection.Add(item);

                            string blobName = await AzureDBProvider.GetBlobName((int)item.ID);
                            string sas = AzureStorageProvider.GetConnectionString(blobName);
                            Uri packUrl = new Uri(@"http://levelupstorage.blob.core.windows.net/packages/" + blobName + sas);


                            var Local = ApplicationData.Current.TemporaryFolder;
                            var file = await Local.CreateFileAsync(item.ID.ToString() + "_pack", CreationCollisionOption.ReplaceExisting);
                            
                            item.PackageFileName = file.DisplayName;
                            item.DownLoadProgressMax = 100 + 3;
                            item.DownLoadProcessVisible = Windows.UI.Xaml.Visibility.Visible;
                            item.DownLoadProgessPos = 0;
                            item.DownloadStatus = res.GetString("PackageDownloadMessage");

                            var stream = await file.OpenSequentialReadAsync();
                            var downloadOperation = await downloader.CreateDownloadAsync(packUrl, file, stream);
                            
                             StartDownloading(downloadOperation);
                            //item.SetDownloadOperation(downloadOperation);
                            //int numberOfParts = 20;
                            //AzureStorageProvider.DownloadPackageFromStorage(file, blobName, numberOfParts, item.DownLoadProgressMax, FileDownloaded, ProgressCallback);

                        }
                        catch
                        {
                            item.DownloadStatus = res.GetString("DownloadingError");
                        }
                    };
                    ChangeState(DownloadPageState.Downloading);

                    break;
            }
        }

        private void FileDownloaded(object sender, EventArgs args)
        {
            var res = new ResourceLoader();
            var argument = args as FilePartDownloadedEventArgs;
            DownLoadAlphabetItem item = DownloadingPackagesCollection.First(process => process.PackageFileName == argument.FileName);

            if (argument.Offset != -1)
            {
                item.DownloadStatus = res.GetString("PackageInstallingMessage");
                item.DownLoadProgessPos++;
                try
                {
                    UnZIPer.Unzip(sender as StorageFile, FileUnZIPed);
                }
                catch
                {
                    item.DownloadStatus = res.GetString("DownloadingError");
                }
            }
            else
                item.DownloadStatus = res.GetString("DownloadingError");
        }

        private async void StartDownloading(DownloadOperation operation)
        {
            var progress = new Progress<DownloadOperation>(ProgressCallback);
            await operation.StartAsync().AsTask(progress);
        }

        private void ProgressCallback(DownloadOperation obj)
        {
            double progress = ((double)obj.Progress.BytesReceived / obj.Progress.TotalBytesToReceive);
            string fileName = obj.ResultFile.Name;

            var res = new ResourceLoader();
            DownLoadAlphabetItem item = DownloadingPackagesCollection.First(process => process.PackageFileName == fileName);
            if (item != null)
                item.DownLoadProgessPos = (int)(progress * 100);

            if (progress >= 1.0)
            {
                this.FileDownloaded(obj.ResultFile, new FilePartDownloadedEventArgs(fileName, (long)obj.Progress.TotalBytesToReceive));
                obj = null;
            }
        }

        private void FileUnZIPed(object sender, EventArgs args)
        {
            var res = new ResourceLoader();
            var argument = args as FileUnzippedEventArgs;
            var item = DownloadingPackagesCollection.First(process => process.PackageFileName == argument.FileName);
            if (item != null)
            {
                item.DownLoadProgessPos++;
                DBFiller.LoadPackageToDB(argument.FolderPath, DBconnectionPath.Local);
                item.DownloadStatus = res.GetString("PackageInstalledMessage");

                bool isEveryDownloadingsCompleted = true;
                foreach (DownLoadAlphabetItem d in DownloadingPackagesCollection)
                    if (d.DownloadStatus != res.GetString("PackageInstalledMessage"))
                        isEveryDownloadingsCompleted = false;

                if (isEveryDownloadingsCompleted)
                    this.Frame.Navigate(typeof(MainMenu));
            }
        }

        private void gwDownLoadItems_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void gwDownLoadItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = e.RemovedItems;
            if (gwDownLoadItems.SelectionMode == ListViewSelectionMode.Multiple)
            foreach (DownLoadAlphabetItem item in items)
                if (item.IsSystem)
                    gwDownLoadItems.SelectedItems.Add(item);
        }
    }
}
