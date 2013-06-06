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

            foreach (string rawCommand in ListString) //(int i = 0; i < ListString.Count; i++)
            {
                if (rawCommand.IndexOf(") VALUES (") > 5)
                {
                    string parametString = rawCommand.Substring(rawCommand.IndexOf(") VALUES (") + 10);
                    if (parametString.LastIndexOf(");") > 0)
                        parametString = parametString.Remove(parametString.LastIndexOf(");"));
                    else
                        parametString = parametString.Remove(parametString.LastIndexOf(") ;"));

                    string[] rawArray = parametString.Split(new string[] { ",       ", ",      ", ",     ", ",    ", ",   ", "       ,", "      ,", "     ,", "    ,", "   ," }, StringSplitOptions.RemoveEmptyEntries);
                    string paramsTemplate = "";
                    List<string> array = new List<string>();

                    foreach (string s in rawArray)
                    {
                        string trim = " \"";
                        string temp = s.Trim(trim.ToCharArray());
                        temp = temp.TrimEnd("\\".ToCharArray());
                        paramsTemplate += "?, ";
                        array.Add(temp);
                    }

                    paramsTemplate = paramsTemplate.Remove(paramsTemplate.Length - 2);
                    string command = rawCommand.Remove(rawCommand.IndexOf(") VALUES (") + 10) + paramsTemplate + " ); ";
                    db.CreateCommand(command, array.ToArray()).ExecuteNonQuery();
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
            db.CreateTable<UserAward>();
            db.CreateTable<Award>();
            db.CreateTable<AwardLocalization>();
            int n = db.CreateCommand("PRAGMA win1251 = \"UTF-8\"").ExecuteNonQuery();
        }
    }
}
