using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace levelupspace
{
    public sealed partial class AuthorizationControl : UserControl
    {
        MainMenu rootPage = MainMenu.Current;
        public AuthorizationControl()
        {
            this.InitializeComponent();
            var passitemssource1 = new PasswordBoxImageSource("ms-appx:///Icons/CH1.png", "ms-appx:///Icons/CH2.png",
                "ms-appx:///Icons/CH3.png", "ms-appx:///Icons/CH4.png", "ms-appx:///Icons/CH5.png", "ms-appx:///Icons/CH6.png");
            var passitemssource2 = new PasswordBoxImageSource("ms-appx:///Icons/CH1.png", "ms-appx:///Icons/CH2.png",
                "ms-appx:///Icons/CH3.png", "ms-appx:///Icons/CH4.png", "ms-appx:///Icons/CH5.png", "ms-appx:///Icons/CH6.png");
            var passitemssource3 = new PasswordBoxImageSource("ms-appx:///Icons/CH1.png", "ms-appx:///Icons/CH2.png",
                "ms-appx:///Icons/CH3.png", "ms-appx:///Icons/CH4.png", "ms-appx:///Icons/CH5.png", "ms-appx:///Icons/CH6.png");
            var passitemssource4 = new PasswordBoxImageSource("ms-appx:///Icons/CH1.png", "ms-appx:///Icons/CH2.png",
                "ms-appx:///Icons/CH3.png", "ms-appx:///Icons/CH4.png", "ms-appx:///Icons/CH5.png", "ms-appx:///Icons/CH6.png");

            this.PassBox.ApplyTemplate();
            this.PassBox.SMItemSource1 = passitemssource1.Items;
            this.PassBox.SMItemSource2 = passitemssource2.Items;
            this.PassBox.SMItemSource3 = passitemssource3.Items;
            this.PassBox.SMItemSource4 = passitemssource4.Items;
            
        }


        private async void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (tbName.Text.Length > 0)
            {
                var result = await UserManager.Authorize(tbName.Text, PassBox.Key);
                if (result)
                {
                    rootPage.UserLogIn();
                    Popup p = this.Parent as Popup;
                    p.IsOpen = false; // close the Popup
                }
                else
                {
                    var res = new ResourceLoader();
                    Logger.ShowMessage(res.GetString("ErrorWrongLogIn"));
                    return;
                }
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Popup p = this.Parent as Popup;
            p.IsOpen = false; // close the Popup
        }

        private void UserControl_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            if (e.Key == Windows.System.VirtualKey.Escape) this.backButton_Click(this, args);
            else if (e.Key == Windows.System.VirtualKey.Enter) this.btnSignIn_Click(this, args);
        }
    }
}
