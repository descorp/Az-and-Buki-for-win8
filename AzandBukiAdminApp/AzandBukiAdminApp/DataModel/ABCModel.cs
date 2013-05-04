using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace AzandBukiAdminApp
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

        public string Language
        {
            get;
            set;
        }

        public string Description
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

        public string Sprite
        {
            get;
            set;
        }

    }
    #endregion

}
