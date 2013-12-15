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
    public sealed partial class AuthorizationControl : UserControl
    {
        readonly MainMenu _rootPage = MainMenu.Current;
        public AuthorizationControl()
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
            
        }


        private async void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (tbName.Text.Length > 0)
            {
                var result = await UserManager.Authorize(tbName.Text, PassBox.Key, DBconnectionPath.Local);
                if (result)
                {
                    _rootPage.UserLogIn();
                    var p = Parent as Popup;
                    if (p != null) p.IsOpen = false; // close the Popup
                }
                else
                {
                    var res = new ResourceLoader();
                    Logger.ShowMessage(res.GetString("ErrorWrongLogIn"));
                }
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            var p = Parent as Popup;
            if (p != null) p.IsOpen = false; // close the Popup
        }

        private void UserControl_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var args = new RoutedEventArgs();
            if (e.Key == VirtualKey.Escape) backButton_Click(this, args);
            else if (e.Key == VirtualKey.Enter) btnSignIn_Click(this, args);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            tbName.Focus(FocusState.Pointer);
        }
    }
}
