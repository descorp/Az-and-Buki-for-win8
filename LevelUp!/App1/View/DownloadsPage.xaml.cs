using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using levelupspace.Common;
using levelupspace.DataModel;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Networking.BackgroundTransfer;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace levelupspace
{
    public enum DownloadPageState { ChoosePacks, Waiting, Downloading };
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    /// 
    public sealed partial class DownloadsPage : LayoutAwarePage
    {

        private DownloadPageState _state;
        private readonly List<DownLoadAlphabetItem> _downloadingPackagesCollection = new List<DownLoadAlphabetItem>();
        private readonly BackgroundDownloader _downloader = new BackgroundDownloader();

        private async void ChangeState(DownloadPageState state)
        {
            _state = state;
            var res = new ResourceLoader();
            switch (state)
            {
                case DownloadPageState.Waiting:
                    tbStatus.Text = res.GetString("PleaseWaitMessage");
                    tbStatus.Visibility = Visibility.Visible;
                    pRing.Visibility = Visibility.Visible;
                    pageTitle.Text = res.GetString("WaitingAppTitle");
                    gwDownLoadItems.Visibility = Visibility.Collapsed;
                    btnNext.Visibility = Visibility.Collapsed;
                    break;
                case DownloadPageState.ChoosePacks:
                    {
                        try
                        {
                            if (!HttpProvider.IsInternetConnection())
                            {
                                Logger.ShowMessage(res.GetString("NoInternetConnectionError"));
                                ToMainMenu();
                                DBFiller.CreateDB(DBconnectionPath.Local);
                                return;
                            }

                            var allPackagesFromAzure = await ContentManager.DownloadFromAzureDB();
                            var packageSholdBeShown = allPackagesFromAzure.ToList();

                            DBFiller.CreateDB(DBconnectionPath.Local);

                            var packagesAlreadyDownloaded =
                                ContentManager.GetListOfDownloadedPackagesID(DBconnectionPath.Local);

                            if (packageSholdBeShown.Count - packagesAlreadyDownloaded.Count == 0)
                            {
                                Logger.ShowMessage(res.GetString("NoNewPackagesAvailable"));
                                ToMainMenu();
                                return;
                            }

                            foreach (var ID in packagesAlreadyDownloaded)
                            {
                                var downLoadAlphabetItems = allPackagesFromAzure as IList<DownLoadAlphabetItem> ??
                                                            allPackagesFromAzure.ToList();
                                foreach (var alph in downLoadAlphabetItems)
                                    if (alph.ID == ID)
                                        packageSholdBeShown.Remove(alph);
                            }

                            DefaultViewModel["ABCItems"] = packageSholdBeShown;

                            foreach (var alph in packageSholdBeShown)
                                if (alph.IsSystem)
                                    gwDownLoadItems.SelectedItems.Add(alph);


                            DefaultViewModel["HeaderText"] = res.GetString("ChoosePacksMessage");
                            tbStatus.Visibility = Visibility.Collapsed;
                            pageTitle.Text = res.GetString("DownloadingAppTitle");
                            pRing.Visibility = Visibility.Collapsed;
                            gwDownLoadItems.Visibility = Visibility.Visible;
                            btnNext.Visibility = Visibility.Visible;

                        }
                        catch
                        {
                            Logger.ShowMessage("ConnectionError");
                            ToMainMenu();
                        }
                        break;
                    }
                case DownloadPageState.Downloading:
                    {
                        btnNext.Visibility = Visibility.Visible;
                        btnNext.IsEnabled = false;
                        gwDownLoadItems.SelectionMode = ListViewSelectionMode.None;
                        break;
                    }
            }

        }

        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Logger.ShowMessage(e.Exception.Message);
        }

        public DownloadsPage()
        {
            InitializeComponent();
            _downloader.Method = "GET";
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
            if (navigationParameter == null) ChangeState(DownloadPageState.ChoosePacks);
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


        private void pageRoot_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void btnChooseLang_Click(object sender, RoutedEventArgs e)
        {
            var res = new ResourceLoader();
            if (gwDownLoadItems.SelectedItems.Count == 0)
            {
                ToMainMenu();
                DBFiller.CreateDB(DBconnectionPath.Local);
                return;
            }

            foreach (DownLoadAlphabetItem item in gwDownLoadItems.SelectedItems)
            {
                //http://levelupstorage.blob.core.windows.net/packages/A1.zip?st=2013-06-11T17%3A55%3A33Z&se=2013-06-11T18%3A55%3A33Z&sr=c&sp=r&sig=%2FtzEgXYJ3rudszpYS7l8KD%2Bs4tRO%2FlB7MZfGYV5BAoM%3D
                try
                {
                    _downloadingPackagesCollection.Add(item);

                    var blobName = await AzureDBProvider.GetBlobName((int)item.ID);
                    var sas = AzureStorageProvider.GetConnectionString(blobName);
                    var packUrl = new Uri(@"http://levelupstorage.blob.core.windows.net/packages/" + blobName + sas);


                    var Local = ApplicationData.Current.TemporaryFolder;
                    var file = await Local.CreateFileAsync(item.ID + "_pack", CreationCollisionOption.ReplaceExisting);

                    item.PackageFileName = file.DisplayName;
                    item.DownLoadProgressMax = 100 + 3;
                    item.DownLoadProcessVisible = Visibility.Visible;
                    item.DownLoadProgessPos = 0;
                    item.DownloadStatus = res.GetString("PackageDownloadMessage");

                    var stream = await file.OpenSequentialReadAsync();
                    var downloadOperation = await _downloader.CreateDownloadAsync(packUrl, file, stream);

                    StartDownloading(downloadOperation);

                }
                catch
                {
                    item.DownloadStatus = res.GetString("DownloadingError");
                }
            }
            ChangeState(DownloadPageState.Downloading);

        }

        private void FileDownloaded(object sender, EventArgs args)
        {
            var res = new ResourceLoader();
            var argument = args as FilePartDownloadedEventArgs;
            var item = _downloadingPackagesCollection.First(process => argument != null && process.PackageFileName == argument.FileName);

            if (argument != null && argument.Offset != -1)
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
            var progress = ((double)obj.Progress.BytesReceived / obj.Progress.TotalBytesToReceive);
            var fileName = obj.ResultFile.Name;

            var item = _downloadingPackagesCollection.First(process => process.PackageFileName == fileName);
            if (item != null)
                item.DownLoadProgessPos = (int)(progress * 100);

            if (progress >= 1.0)
            {
                FileDownloaded(obj.ResultFile, new FilePartDownloadedEventArgs(fileName, (long)obj.Progress.TotalBytesToReceive));
            }
        }

        private void FileUnZIPed(object sender, EventArgs args)
        {
            var res = new ResourceLoader();
            var argument = args as FileUnzippedEventArgs;
            var item = _downloadingPackagesCollection.First(process => argument != null && process.PackageFileName == argument.FileName);
            if (item != null)
            {
                item.DownLoadProgessPos++;
                if (argument != null) DBFiller.LoadPackageToDB(argument.FolderPath, DBconnectionPath.Local, (a, e) =>
                {
                    item.DownloadStatus = res.GetString("PackageInstalledMessage");

                    var isEveryDownloadingsCompleted = true;
                    foreach (var d in _downloadingPackagesCollection)
                        if (d.DownloadStatus != res.GetString("PackageInstalledMessage"))
                            isEveryDownloadingsCompleted = false;

                    if (isEveryDownloadingsCompleted)
                        ToMainMenu();
                });
            }
        }

        private void ToMainMenu()
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Frame.Navigate(typeof(MainMenu)));
            ReleaseResources();
        }

        private void ReleaseResources()
        {
            _downloadingPackagesCollection.Clear();
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
