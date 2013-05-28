using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace levelupspace
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

        private void UserControl_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            if (e.Key == Windows.System.VirtualKey.Escape||e.Key == Windows.System.VirtualKey.Enter) this.btnNext_Click(this, args);
        }
    }
}
