using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace levelupspace
{
    public sealed partial class GameParamsControl : UserControl
    {
        public GameParamsControl()
        {
            this.InitializeComponent();

            ObservableCollection<DifficultyItem> DLevels = new ObservableCollection<DifficultyItem>();
            DLevels.Add(new DifficultyItem("easy level", "Легкий", "ms-appx:///Assets/EasyLevel.png"," "));
            DLevels.Add(new DifficultyItem("medium level", "Средний", "ms-appx:///Assets/MediumLevel.png", " "));
            DLevels.Add(new DifficultyItem("genius level", "Гений", "ms-appx:///Assets/HardLevel.png", " "));

            cbDifficulty.ItemsSource = DLevels;
            cbDifficulty.SelectedIndex = 0;

            var abcs = ContentManager.GetAlphabets("AllAlphabets");

            cbAlphabet.ItemsSource = abcs;
            cbAlphabet.SelectedIndex = 0;
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
    }
}
