using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using SQLite;

namespace levelupspace.DataModel
{
    public class AwardItem : ABCItem
    {
        public AwardItem(String uniqueId, String title, String imagePath, String description, int id)
            : base(uniqueId, title, imagePath, description)
        {
            _id = id;
        }

        private int _id;
        public int ID
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        public bool SocialsEnabled
        {
            get
            {
                if (!ApplicationData.Current.RoamingSettings.Values.ContainsKey("Socials")) return false;
                return (bool)ApplicationData.Current.RoamingSettings.Values["Socials"];
            }
        }
    }

    public class AwardManager
    {
        public async static Task<ObservableCollection<AwardItem>> UsersAwards(int userId, String dbPath)
        {
            var userAwards = new ObservableCollection<AwardItem>();

            var db = new SQLiteAsyncConnection(dbPath);

            var awardQuery = await db.QueryAsync<UserAward>("SELECT * FROM UserAward WHERE UserID=?", userId);
            var lPath = ApplicationData.Current.LocalFolder.Path;
            foreach (var award in awardQuery)
            {
                var languageID = CultureInfo.CurrentCulture.Name;

                var localQuery = await db.QueryAsync<AwardLocalization>(
                    "SELECT * FROM AwardLocalization WHERE AwardID=?", award.AwardID);
                var localization = localQuery.First(l => l.LanguageID.Contains(languageID)) ??
                                   localQuery.First(l => l.LanguageID.Contains("en"));

                var awardDataQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE Id=?", award.AwardID);


                var awardData = awardDataQuery.FirstOrDefault();



                userAwards.Add(new AwardItem(String.Concat("Award ", award.AwardID),
                    localization.AwardName,
                    Path.Combine(lPath, awardData.LogoPath),
                    localization.AwardDescription,
                    award.AwardID));
            }
            return userAwards;
        }

        public async static Task<AwardItem> GetAward(int awardId, String dbPath)
        {
            var lPath = ApplicationData.Current.LocalFolder.Path;

            var db = new SQLiteAsyncConnection(dbPath);

            var awardQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE Id=?", awardId);
            var award = awardQuery.FirstOrDefault();

            var languageID = CultureInfo.CurrentCulture.Name;


            var localQuery = await db.QueryAsync<AwardLocalization>(
                                     "SELECT * FROM AwardLocalization WHERE AwardID=?", award.ID);

            var localization = localQuery.First(l => l.LanguageID.Contains(languageID)) ??
                               localQuery.First(l => l.LanguageID.Contains("en"));

            return new AwardItem("Award " + award.ID,
                                             localization.AwardName,
                                             Path.Combine(lPath, award.LogoPath),
                                             localization.AwardDescription,
                                             award.ID);
        }

        public async static Task<AwardItem> GetAwardForRate(int rate, String dbPath)
        {
            if (rate > 5 || rate < 2)
                return null;
            var lPath = ApplicationData.Current.LocalFolder.Path;
            var db = new SQLiteAsyncConnection(dbPath);

            var awardQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE Rate=?", rate);
            var award = awardQuery.FirstOrDefault();

            var localQuery = await db.QueryAsync<AwardLocalization>(
                                     "SELECT * FROM AwardLocalization WHERE AwardID=?", award.ID);
            var languageID = CultureInfo.CurrentCulture.Name;
            var localization = localQuery.First(l => l.LanguageID.Contains(languageID)) ??
                               localQuery.First(l => l.LanguageID.Contains("en"));

            return new AwardItem("Award " + award.ID.ToString(),
                                localization.AwardName,
                                Path.Combine(lPath, award.LogoPath),
                                localization.AwardDescription,
                                award.ID);
        }

        public async static void AddUserAward(AwardItem award, int userId, String dbPath)
        {
            var db = new SQLiteAsyncConnection(dbPath);

            await db.InsertAsync(new UserAward
            {
                AwardID = award.ID,
                UserID = userId
            });
        }
    }
}
