using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace levelupspace.DataModel
{
    public class DBFiller
    {
        public static async void LoadPackageToDB(String PathToPack, String DBPath)
        {
            
            var folder = await StorageFolder.GetFolderFromPathAsync(PathToPack);
            var file = await folder.GetFileAsync("input.sql");
            var SQLStrings = await FileIO.ReadLinesAsync(file);
           
            DBFiller.InsertStringsToDB(SQLStrings, DBPath);           
                      
        }

        private static void InsertStringsToDB(IList<string> ListString, String DBPath)
        {
            var db = new SQLiteConnection(DBPath);

            for (int i = 0; i < ListString.Count; i++)
            {
                var command = db.CreateCommand(ListString[i]);
                var result = command.ExecuteNonQuery();
                if (result <= 0)
                {
                    //Какая-то ошибка
                }
            }
        }

        public static void CreateDB(String DBPath)
        {
            var db = new SQLiteConnection(DBPath);
            db.CreateTable<Alphabet>();
            db.CreateTable<Letter>();
            db.CreateTable<AlphabetLocalization>();
            db.CreateTable<Word>();
            db.CreateTable<User>();
            db.CreateTable<Award>();
            db.CreateTable<AwardLocalization>();
        }
    }
}
