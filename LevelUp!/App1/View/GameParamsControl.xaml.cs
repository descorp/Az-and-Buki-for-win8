using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
using Windows.UI.Xaml.Input;
using levelupspace.DataModel;

namespace levelupspace
{
    public sealed partial class GameParamsControl : UserControl
    {
        public GameParamsControl()
        {
            InitializeComponent();
            var res = new ResourceLoader();
            try
            {
                var DLevels = new ObservableCollection<DifficultyItem>
                {
                    new DifficultyItem("easy level", res.GetString("EasyLevel"), "ms-appx:///Assets/EasyLevel.png", " "),
                    new DifficultyItem("medium level", res.GetString("MeduimLevel"), "ms-appx:///Assets/MediumLevel.png",
                        " "),
                    new DifficultyItem("genius level", res.GetString("HardLevel"), "ms-appx:///Assets/HardLevel.png",
                        " ")
                };

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
            var p = Parent as Popup;
            if (p != null) p.IsOpen = false; // close the Popup

            var alphabetItem = cbAlphabet.SelectedItem as AlphabetItem;
            if (alphabetItem != null)
                GamePage.Current.SetGameState(cbDifficulty.SelectedIndex, alphabetItem.UniqueId);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            var p = Parent as Popup;
            if (p != null) p.IsOpen = false; // close the Popup

            GamePage.Current.GoBack();
        }

        private void UserControl_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var args = new RoutedEventArgs();
            if (e.Key == VirtualKey.Escape) backButton_Click(this, args);
            else if (e.Key == VirtualKey.Enter) btnPlay_Click(this, args);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cbDifficulty.Focus(FocusState.Pointer);
        }
    }
}
