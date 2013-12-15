using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using levelupspace.Common;
using levelupspace.DataModel;

// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace levelupspace
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class UserSignOnPage : LayoutAwarePage
    {
        String logofilePath;
        public UserSignOnPage()
        {
            InitializeComponent();

            var passitemssource1 = new PasswordBoxImageSource("ms-appx:///Icons/CH1.png", "ms-appx:///Icons/CH2.png",
                "ms-appx:///Icons/CH3.png", "ms-appx:///Icons/CH4.png", "ms-appx:///Icons/CH5.png", "ms-appx:///Icons/CH6.png");
            var passitemssource2 = new PasswordBoxImageSource("ms-appx:///Icons/CH1.png", "ms-appx:///Icons/CH2.png",
                "ms-appx:///Icons/CH3.png", "ms-appx:///Icons/CH4.png", "ms-appx:///Icons/CH5.png", "ms-appx:///Icons/CH6.png");
            var passitemssource3 = new PasswordBoxImageSource("ms-appx:///Icons/CH1.png", "ms-appx:///Icons/CH2.png",
                "ms-appx:///Icons/CH3.png", "ms-appx:///Icons/CH4.png", "ms-appx:///Icons/CH5.png", "ms-appx:///Icons/CH6.png");
            var passitemssource4 = new PasswordBoxImageSource("ms-appx:///Icons/CH1.png", "ms-appx:///Icons/CH2.png",
                "ms-appx:///Icons/CH3.png", "ms-appx:///Icons/CH4.png", "ms-appx:///Icons/CH5.png", "ms-appx:///Icons/CH6.png");

            PassBox.ApplyTemplate();
            PassBox.SMItemSource1 = passitemssource1.Items;
            PassBox.SMItemSource2 = passitemssource2.Items;
            PassBox.SMItemSource3 = passitemssource3.Items;
            PassBox.SMItemSource4 = passitemssource4.Items;

            logofilePath = "ms-appx:///Assets/Userlogo.png";
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
            if (pageState != null)
            {
                if (pageState.ContainsKey("SignOnLogin"))
                    tbName.Text = (string)pageState["SignOnLogin"];
                if (pageState.ContainsKey("SignOnLogofile"))
                {
                    logofilePath = (string)pageState["SignOnLogofile"];
                    imgUserLogo.Source = new BitmapImage(new Uri(logofilePath, UriKind.Absolute));
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
            if (tbName.Text.Length!=0)
                pageState["SignOnLogin"] = tbName.Text;
            if (logofilePath.Length != 0)
                pageState["SignOnLogofile"] = logofilePath;
        }

        private async void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var res = new ResourceLoader();

            if (tbName.Text.Length > 3)
            {
                var uniquelogin = false;

                try
                {
                    uniquelogin = await AzureDBProvider.UserUnique(tbName.Text);  //UserManager.IsUniqueLoginAsync(tbName.Text, DBconnectionPath.Local);
                }
                catch
                {
                    Frame.Navigate(typeof(MainMenu));
                    Logger.ShowMessage(res.GetString("ConnectionError"));
                }

                if (uniquelogin)
                {
                    if (logofilePath != "ms-appx:///Assets/Userlogo.png")
                    {
                        var file = await StorageFile.GetFileFromPathAsync(logofilePath);
                        logofilePath = String.Concat("Users/UL", tbName.Text, file.FileType);
                        await file.RenameAsync("UL"+tbName.Text+file.FileType);
                        //AzureStorageProvider.UploadAvatarToStorage(file, tbName.Text);
                    }

                    var newUserID = await UserManager.AddUserAsync(new User
                    {
                        Name = tbName.Text,
                        Avatar = logofilePath
                    },
                        PassBox.Key,
                        DBconnectionPath.Local);

                    if (newUserID > 0)
                    {

                        Frame.Navigate(typeof(MainMenu), "Autorized");
                        return;
                    }


                    Logger.ShowMessage(res.GetString("SignOnUnexpectedError"));

                    return;
                }
                Logger.ShowMessage(res.GetString("SignOnNotUniqueLoginError"));
                return;
            }
            Logger.ShowMessage(res.GetString("SignOnTooShortNameError"));
        }

        

        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var OpenPicker = new FileOpenPicker();
            OpenPicker.FileTypeFilter.Add(".png");
            OpenPicker.FileTypeFilter.Add(".jpg");
            OpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            var file = await OpenPicker.PickSingleFileAsync();
            
            
            if (file != null)
            {
                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Users",CreationCollisionOption.OpenIfExists);
                var logofile = await file.CopyAsync(folder, String.Concat("UL", file.FileType), NameCollisionOption.ReplaceExisting);

                logofilePath = logofile.Path;

                imgUserLogo.Source = new BitmapImage(new Uri(logofile.Path, UriKind.Absolute));
                
            }

        }

        private void pageRoot_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var args = new RoutedEventArgs();
            if (e.Key == VirtualKey.Escape) GoBack(this, args);
            else if (e.Key == VirtualKey.Enter) btnOk_Click(this, args);
        }

        private async void backButton_Click(object sender, RoutedEventArgs e)
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("User", CreationCollisionOption.OpenIfExists);
            try
            {
                var logofile = await folder.GetFileAsync("UL");
                await logofile.DeleteAsync();
                GoBack(this, e);
            }
            catch
            {
                GoBack(this, e);
            }

        }
    }
}
