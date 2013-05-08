using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace LevelUP
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

    }
    #endregion



    #region Localization

    class Localization
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
}
