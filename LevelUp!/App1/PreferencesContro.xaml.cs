using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class PreferencesContro : UserControl
    {
        public PreferencesContro()
        {
            this.InitializeComponent();
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
