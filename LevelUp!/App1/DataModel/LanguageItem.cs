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
            get { return this._langname; }
            set { this._langname = value; }
        }

        private string _langcode;
        
        public string LanguageCode
        {
            get { return this._langcode; }
            set { this._langcode = value; }
        }

    }

    public static class LanguageProvider
    {
        public static ObservableCollection<LanguageItem> AllLanguages
        {
            get
            {
                ObservableCollection<LanguageItem> All = new ObservableCollection<LanguageItem>();
                
                for (int i = 0; i < ApplicationLanguages.ManifestLanguages.Count; i++)
                {
                    var LItem = new LanguageItem();
                    CultureInfo cInfo = new CultureInfo(ApplicationLanguages.ManifestLanguages[i]);
                    LItem.LanguageName = cInfo.NativeName;
                    LItem.LanguageCode = cInfo.Name;
                    if (CurrentLanguage.LanguageCode.Equals(LItem.LanguageCode))
                        All.Insert(0, LItem);
                    else All.Add(LItem);
                }
                return All;
            }
        }

        public static LanguageItem CurrentLanguage
        {
            get
            {
                var LItem = new LanguageItem();
                LItem.LanguageCode = ApplicationLanguages.PrimaryLanguageOverride;
                CultureInfo cInfo = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride);
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
