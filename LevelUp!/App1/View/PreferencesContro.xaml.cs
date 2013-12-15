using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace levelupspace
{
    public sealed partial class PreferencesContro : UserControl
    {
        public PreferencesContro()
        {
            InitializeComponent();
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("Socials"))
                SwitchEnableSocials.IsOn = (bool)ApplicationData.Current.RoamingSettings.Values["Socials"];
            else SwitchEnableSocials.IsOn = false;
        }

        private void SwitchEnableSocials_Toggled_1(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.RoamingSettings.Values["Socials"] = SwitchEnableSocials.IsOn;
        }
    }
}
