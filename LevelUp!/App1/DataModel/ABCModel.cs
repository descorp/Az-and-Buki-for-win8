using System;
using SQLite;

namespace levelupspace
{
    

    #region Alphabet

    
    public class Alphabet
    {

        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        
        public string Logo
        {
            get;
            set;
        }

        public bool IsNative
        {
            get;
            set;
        }

    }

    class AlphabetLocalization
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
        
        public String LanguageID
        {
            get;
            set;
        }
        public String LanguageName
        {
            get;
            set;
        }

        public string Description
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

        public string Value
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

    #region User
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Hash
        {
            get;
            set;
        }
        
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
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        public int UserID
        {
            get;
            set;
        }

        public int AwardID
        {
            get;
            set;
        }
    }

    public class Award
    {
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        public string LogoPath
        {
            get;
            set;
        }

        public int Rate
        {
            get;
            set;
        }
    }

    public class AwardLocalization
    {
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get;
            set;
        }

        public int AwardId
        {
            get;
            set;
        }

        public String LanguageID
        {
            get;
            set;
        }

        public String AwardName
        {
            get;
            set;
        }

        public String AwardDescription
        {
            get;
            set;
        }
    }
    #endregion
}
