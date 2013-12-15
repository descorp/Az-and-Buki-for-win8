using System.Threading.Tasks;
using SQLite;
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace levelupspace.DataModel
{
    public class DBFiller
    {
        public static async void LoadPackageToDB(String pathToPack, String dbPath, EventHandler  packLoaded)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(pathToPack);
            var file = await folder.GetFileAsync("input.sql");
            var sqlStrings = await FileIO.ReadLinesAsync(file);

            InsertStringsToDB(sqlStrings, dbPath);

            if (packLoaded != null)
            {
                packLoaded(null, new EventArgs());
            }
        }

        private static void InsertStringsToDB(IEnumerable<string> ListString, String DBPath)
        {
            foreach (var rawCommand in ListString) //(int i = 0; i < ListString.Count; i++)
            {
                if (rawCommand.IndexOf(") VALUES (", StringComparison.Ordinal) > 5)
                {
                    var parametString = rawCommand.Substring(rawCommand.IndexOf(") VALUES (", StringComparison.Ordinal) + 10);
                    parametString = parametString.Remove(parametString.LastIndexOf(");", StringComparison.Ordinal) > 0 ? parametString.LastIndexOf(");", StringComparison.Ordinal) : parametString.LastIndexOf(") ;", StringComparison.Ordinal));

                    var rawArray = parametString.Split(new[] { ",       ", ",      ", ",     ", ",    ", ",   ", "       ,", "      ,", "     ,", "    ,", "   ," }, StringSplitOptions.RemoveEmptyEntries);
                    var paramsTemplate = "";
                    var array = new List<string>();

                    foreach (var s in rawArray)
                    {
                        const string trim = " \"";
                        var temp = s.Trim(trim.ToCharArray());
                        temp = temp.TrimEnd("\\".ToCharArray());
                        paramsTemplate += "?, ";
                        array.Add(temp);
                    }

                    paramsTemplate = paramsTemplate.Remove(paramsTemplate.Length - 2);
                    var command = rawCommand.Remove(rawCommand.IndexOf(") VALUES (", StringComparison.Ordinal) + 10) + paramsTemplate + " ); ";
                    using (var db = new SQLiteConnection(DBPath))
                        db.CreateCommand(command, array.ToArray()).ExecuteNonQuery();
                }
            }
        }

        public static void CreateDB(String DBPath)
        {
            using (var db = new SQLiteConnection(DBPath))
            {
                db.CreateTable<Alphabet>();
                db.CreateTable<Letter>();
                db.CreateTable<AlphabetLocalization>();
                db.CreateTable<Word>();
                db.CreateTable<User>();
                db.CreateTable<UserAward>();
                db.CreateTable<Award>();
                db.CreateTable<AwardLocalization>();
                var n = db.CreateCommand("PRAGMA win1251 = \"UTF-8\"").ExecuteNonQuery();
            }
        }
    }
}
