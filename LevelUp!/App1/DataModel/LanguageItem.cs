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
                bool IsLanguageSupported=false;
                int EngIndex=0;
                for (int i = 0; i < ApplicationLanguages.ManifestLanguages.Count; i++)
                {
                    var LItem = new LanguageItem();
                    CultureInfo cInfo = new CultureInfo(ApplicationLanguages.ManifestLanguages[i]);

                    LItem.LanguageName = cInfo.NativeName;
                    LItem.LanguageCode = cInfo.Name;
                    if (LItem.LanguageCode.Equals("en-US"))
                    {
                        EngIndex = i;
                    }
                    if (CurrentLanguage.LanguageCode.Equals(LItem.LanguageCode))
                    {
                        All.Insert(0, LItem);
                        IsLanguageSupported = true;
                    }
                    else All.Add(LItem);
                }

                if (!IsLanguageSupported) AllLanguages.Move(EngIndex,0);
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
