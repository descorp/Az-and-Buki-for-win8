using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace AzandBukiAdminApp
{
    //class ABCDataContext : DataContext
    //{
    //    public ABCDataContext() : base("Data Source=appdata:/ABCdb.sdf; File Mode=read only;") { }

    //    public Table<Alphabet> Alphabets;
    //    public Table<Letter> Letters;
    //    public Table<Word> Words;
    //}
    

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


        public string Sprite
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
