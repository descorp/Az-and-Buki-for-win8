using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite;

namespace levelupspace.DataModel
{
    public sealed class ContentManager
    {
        public static ObservableCollection<AlphabetItem> AllAlphabets(String DataSourcePath)
        {
            var abcs = new ObservableCollection<AlphabetItem>();
            List<Alphabet> AlphabetQuery;

            using (var db = new SQLiteConnection(DataSourcePath))
            {
                try
                {
                    AlphabetQuery = db.Query<Alphabet>("SELECT * FROM Alphabet WHERE IsSystem = 0");
                }
                catch
                {
                    return null;
                }
                if (AlphabetQuery == null) return null;
            }

            foreach (var alphabet in AlphabetQuery)
            {
                List<AlphabetLocalization> localQuery;
                using (var db = new SQLiteConnection(DataSourcePath))
                {
                    localQuery = db.Query<AlphabetLocalization>(
                        "SELECT * FROM AlphabetLocalization WHERE AlphabetID=?", alphabet.Guid);
                }

                var languageID = CultureInfo.CurrentCulture.Name;
                var localization = localQuery.First(l => l.LanguageID.Contains(languageID)) ??
                                   localQuery.First(l => l.LanguageID.Contains("en"));

                var lPath = ApplicationData.Current.LocalFolder.Path;

                var aitem = new AlphabetItem(
                    String.Concat("Alphabet ", alphabet.Guid),
                    localization.LanguageName,
                    Path.Combine(lPath, alphabet.Logo),
                    localization.Description,
                    alphabet.Guid
                    );

                abcs.Add(aitem);
            }
            if (abcs.Count > 0)
                return abcs;
            return null;

        }

        public static IEnumerable<AlphabetItem> GetAlphabets(string uniqueId)
        {
            if (!uniqueId.Equals("AllAlphabets")) throw new ArgumentException("Only 'AllAlphabets' is supported as a collection of Alphabets");

            return AllAlphabets(DBconnectionPath.Local);
        }

        public static int AlphabetsCount(String DBPath)
        {
            using (var db = new SQLiteConnection(DBPath))
            {
                var command = db.CreateCommand("SELECT COUNT(ID) From Alphabet");
                return command.ExecuteScalar<int>();
            }
        }

        public static List<int> GetListOfDownloadedPackagesID(String DBPath)
        {
            using (var db = new SQLiteConnection(DBPath))
            {
                var command = db.CreateCommand("SELECT * From Alphabet");
                var query = command.ExecuteQuery<Alphabet>();
                return query.Select(a => a.Guid).ToList();
            }
        }

        public static AlphabetItem GetAlphabet(string uniqueId, String DBPath)
        {
            var aid = ParseAlphabetID(uniqueId);
            List<Letter> letterQuery;
            AlphabetItem aitem;
            var lPath = ApplicationData.Current.LocalFolder.Path;

            using (var db = new SQLiteConnection(DBPath))
            {
                var alphabetQuery =
                    db.Query<Alphabet>("SELECT * FROM Alphabet WHERE IsSystem = 0 AND GUID=?", aid).FirstOrDefault();

                var localQuery = db.Query<AlphabetLocalization>(
                    "SELECT * FROM AlphabetLocalization WHERE AlphabetID=?", alphabetQuery.Guid);

                var languageID = CultureInfo.CurrentCulture.Name;
                var localization = localQuery.First(l => l.LanguageID.Contains(languageID)) ??
                                   localQuery.First(l => l.LanguageID.Contains("en"));

                aitem = new AlphabetItem(
                    String.Concat("Alphabet ", alphabetQuery.Guid),
                    localization.LanguageName,
                    Path.Combine(lPath, alphabetQuery.Logo),
                    localization.Description,
                    alphabetQuery.Guid
                    );

                letterQuery = db.Query<Letter>("SELECT * FROM Letter WHERE AlphabetID=?", aitem.ID);
            }

            foreach (var letter in letterQuery)
            {
                var Litem = new LetterItem(
                    String.Concat("Alphabet ", aitem.ID.ToString(), " Letter ", letter.Guid.ToString()),
                    letter.Value,
                    Path.Combine(lPath, letter.Logo),
                    letter.Value,
                    letter.Guid,
                    aitem,
                    Path.Combine(lPath, letter.Sound)
                    );

                List<Word> WordQuery;
                using (var db = new SQLiteConnection(DBPath))
                 WordQuery = db.Query<Word>("SELECT * FROM Word WHERE AlphabetID=? AND LetterID=?", aitem.ID, letter.Guid);

                foreach (var word in WordQuery)
                {
                    string sound = word.Sound == null ? "none" : Path.Combine(lPath, word.Sound);

                    Litem.WordItems.Add(new WordItem(
                        String.Concat("Alphabet ", word.AlphabetID.ToString(), " Letter ", word.LetterID.ToString(), " Word ", word.ID.ToString()),
                        " ",
                        Path.Combine(lPath, word.ValueImg),
                        " ",
                        word.ID,
                        aitem,
                        Path.Combine(lPath, word.Image),
                        sound
                        ));
                }

                aitem.LetterItems.Add(Litem);
            }

            return aitem;
        }



        public static LetterItem GetLetterItem(string uniqueId, String DBPath)
        {
            var alphabetID = ParseAlphabetID(uniqueId);
            var alpha = GetAlphabet("Alphabet " + alphabetID.ToString(), DBPath);

            return alpha.LetterItems.FirstOrDefault(item => item.UniqueId.Equals(uniqueId));
        }

        public static WordItem GetWordItem(string uniqueId)
        {
            var alphabetID = ParseAlphabetID(uniqueId);
            var letterID = ParseLetterID(uniqueId);
            var wordID = ParseWordID(uniqueId);

            if (alphabetID < 0 || letterID < 0 || wordID < 0)
                return null;

            var alpha = GetAlphabet("Alphabet " + alphabetID, DBconnectionPath.Local);
            var let = alpha.LetterItems.First(letter => letter.ID == letterID);
            var w = let.WordItems.First(word => word.ID == wordID);

            return w;
        }

        public static async Task<IEnumerable<DownLoadAlphabetItem>> DownloadFromAzureDB()
        {
            var aItems = new ObservableCollection<DownLoadAlphabetItem>();

            var packages = await AzureDBProvider.GetAllPackages();

            foreach (var pack in packages)
            {
                var local = await AzureDBProvider.GetPackageLocalization(pack, LanguageProvider.CurrentLanguage.LanguageCode);

                aItems.Add(new DownLoadAlphabetItem(String.Concat("Alphabet ", pack.Guid.ToString()),
                        local.LanguageName,
                        pack.Logo,
                        local.Description,
                        pack.Guid,
                        pack.Length,
                        pack.IsSystem
                    ));
            }

            return aItems;
        }

        public static int ParseAlphabetID(string uniqueID)
        {
            if (uniqueID.Contains("Alphabet"))
            {
                var pars = uniqueID.Split(' ');
                return int.Parse(pars[1]);
            }
            return -1;
        }

        public static int ParseLetterID(string uniqueID)
        {
            if (uniqueID.Contains("Letter"))
            {
                var pars = uniqueID.Split(' ');
                return int.Parse(pars[3]);
            }
            return -1;
        }

        public static int ParseWordID(string uniqueID)
        {
            if (uniqueID.Contains("Word"))
            {
                var pars = uniqueID.Split(' ');
                return int.Parse(pars[5]);
            }
            return -1;
        }

        public async static Task<bool> IsContentDownloaded(String dbPath)
        {
            try
            {
                if (AlphabetsCount(dbPath) > 0)
                    return true;
            }
            catch (Exception)
            {

                return false;
            }
            return false;
        }
    }
}
