using System;
using SQLite;
using Newtonsoft.Json;

namespace levelupspace.DataModel
{

    #region Alphabet
    public class Alphabet
    {
        [PrimaryKey, AutoIncrement]
        [JsonProperty(PropertyName = "Id")]
        public int ID
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Guid")]        
        public long Guid
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Logo")]
        public string Logo
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Path")]
        public string Path
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Length")]
        public long Length
        {
            get;
            set;
        }

    }

    public class AlphabetLocalization
    {
        [JsonProperty(PropertyName = "Guid")]
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "AlphabetID")]
        public int AlphabetID
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "LanguageID")]
        public String LanguageID
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "LanguageName")]
        public String LanguageName
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Description")]
        public string Description
        {
            get;
            set;
        }
    }
    #endregion

    #region User
    public class User
    {
        [JsonProperty(PropertyName = "Guid")]
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "Name")]
        public string Name
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "Hash")]
        public string Hash
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "Avatar")]
        public string Avatar
        {
            get;
            set;
        }

    }
    #endregion

    #region Award
    public class UserAward
    {
        [JsonProperty(PropertyName = "Guid")]
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "UserID")]
        public int UserID
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "AwardID")]
        public int AwardID
        {
            get;
            set;
        }
    }

    public class Award
    {
        [JsonProperty(PropertyName = "Guid")]
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "LogoPath")]
        public string LogoPath
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "Rate")]
        public int Rate
        {
            get;
            set;
        }
    }

    public class AwardLocalization
    {
        [JsonProperty(PropertyName = "Guid")]
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "AwardId")]
        public int AwardId
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "LanguageID")]
        public String LanguageID
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "AwardName")]
        public String AwardName
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "AwardDescription")]
        public String AwardDescription
        {
            get;
            set;
        }
    }
    #endregion

    #region Letter
    public class Letter
    {
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        
        public int AlphabetID
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public string Logo
        {
            get;
            set;
        }

        public string Sound
        {
            get;
            set;
        }
    }
    #endregion

    #region Word
    
    public class Word
    {
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        public int LetterID
        {
            get;
            set;
        }

        public int AlphabetID
        {
            get;
            set;
        }

        public string Image
        {
            get;
            set;
        }

        public string ValueImg
        {
            get;
            set;
        }

        public string Sound
        {
            get;
            set;
        }

    }
    #endregion

}
