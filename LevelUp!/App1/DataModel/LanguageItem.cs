using System.Collections.ObjectModel;
using System.Globalization;
using Windows.Globalization;

namespace levelupspace
{
    public class LanguageItem
    {
        private string _langname;
        public string LanguageName
        {
            get { return _langname; }
            set { _langname = value; }
        }

        private string _langcode;
        
        public string LanguageCode
        {
            get { return _langcode; }
            set { _langcode = value; }
        }

    }

    public static class LanguageProvider
    {
        public static ObservableCollection<LanguageItem> AllLanguages
        {
            get
            {
                var all = new ObservableCollection<LanguageItem>();
                var isLanguageSupported=false;
                var engIndex=0;
                for (var i = 0; i < ApplicationLanguages.ManifestLanguages.Count; i++)
                {
                    var lItem = new LanguageItem();
                    var cInfo = new CultureInfo(ApplicationLanguages.ManifestLanguages[i]);

                    lItem.LanguageName = cInfo.NativeName;
                    lItem.LanguageCode = cInfo.Name;
                    if (lItem.LanguageCode.Equals("en-US"))
                    {
                        engIndex = i;
                    }
                    if (CurrentLanguage.LanguageCode.Equals(lItem.LanguageCode))
                    {
                        all.Insert(0, lItem);
                        isLanguageSupported = true;
                    }
                    else all.Add(lItem);
                }

                if (!isLanguageSupported) AllLanguages.Move(engIndex,0);
                return all;
            }
        }

        public static LanguageItem CurrentLanguage
        {
            get
            {
                var LItem = new LanguageItem();
                if (ApplicationLanguages.PrimaryLanguageOverride.Length == 0)
                    ApplicationLanguages.PrimaryLanguageOverride = "en-US";
                LItem.LanguageCode = ApplicationLanguages.PrimaryLanguageOverride;
                var cInfo = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride);
                LItem.LanguageName = cInfo.NativeName;
                return LItem;
            }

            set
            {
                ApplicationLanguages.PrimaryLanguageOverride = value.LanguageCode;
            }
        }
    }
}
