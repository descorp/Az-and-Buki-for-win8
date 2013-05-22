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

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace LevelUP
{
    public sealed partial class TextPopup : UserControl
    {
        public TextPopup(String Text)
        {
            this.InitializeComponent();
            tbMessage.Text = Text;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Popup p = this.Parent as Popup;
            p.IsOpen = false; // close the Popup
        }
    }
}
