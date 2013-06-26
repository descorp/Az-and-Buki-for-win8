using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace levelupspace
{
    public sealed partial class GameParamsControl : UserControl
    {
        public GameParamsControl()
        {
            this.InitializeComponent();
            var res = new ResourceLoader();
            try
            {
                ObservableCollection<DifficultyItem> DLevels = new ObservableCollection<DifficultyItem>();
                DLevels.Add(new DifficultyItem("easy level", res.GetString("EasyLevel"), "ms-appx:///Assets/EasyLevel.png", " "));
                DLevels.Add(new DifficultyItem("medium level", res.GetString("MeduimLevel"), "ms-appx:///Assets/MediumLevel.png", " "));
                DLevels.Add(new DifficultyItem("genius level", res.GetString("HardLevel"), "ms-appx:///Assets/HardLevel.png", " "));

                cbDifficulty.ItemsSource = DLevels;
                cbDifficulty.SelectedIndex = 0;

                var abcs = ContentManager.GetAlphabets("AllAlphabets");


                cbAlphabet.ItemsSource = abcs;
                cbAlphabet.SelectedIndex = 0;
            }
            catch
            {
                //this.Frame.Navigate(typeof(MainMenu));
                Logger.ShowMessage(res.GetString("ConnectionError"));
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            Popup p = this.Parent as Popup;
            p.IsOpen = false; // close the Popup

            GamePage.Current.SetGameState(cbDifficulty.SelectedIndex, (cbAlphabet.SelectedItem as AlphabetItem).UniqueId);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Popup p = this.Parent as Popup;
            p.IsOpen = false; // close the Popup

            GamePage.Current.GoBack();
        }

        private void UserControl_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            if (e.Key == Windows.System.VirtualKey.Escape) this.backButton_Click(this, args);
            else if (e.Key == Windows.System.VirtualKey.Enter) this.btnPlay_Click(this, args);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cbDifficulty.Focus(Windows.UI.Xaml.FocusState.Pointer);
        }
    }
}
