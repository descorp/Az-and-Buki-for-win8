using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236
using Windows.UI.Xaml.Input;

namespace levelupspace
{
    public sealed partial class TextPopup : UserControl
    {
        public EventHandler OKClickEvent=null;

        public TextPopup(String Text, bool IsBackEnable = false)
        {
            InitializeComponent();
            tbMessage.Text = Text;
            if (!IsBackEnable) backButton.Visibility = Visibility.Collapsed;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (OKClickEvent != null)
            {
                var args = new EventArgs();
                OKClickEvent(this, args);
            }

                var p = Parent as Popup;
            if (p != null) p.IsOpen = false; // close the Popup
        }

        private void UserControl_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var args = new RoutedEventArgs();
            if (e.Key == VirtualKey.Escape||e.Key == VirtualKey.Enter) btnNext_Click(this, args);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            btnNext.Focus(FocusState.Keyboard);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            var p = Parent as Popup;
            if (p != null) p.IsOpen = false; // close the Popup
        }
    }
}
