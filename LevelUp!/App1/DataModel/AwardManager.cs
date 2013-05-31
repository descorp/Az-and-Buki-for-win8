using SQLite;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using levelupspace.DataModel;

namespace levelupspace
{
    public class AwardItem: ABCItem
    {
        public AwardItem(String uniqueId, String title, String imagePath, String description, int ID)
            : base(uniqueId, title, imagePath, description)
        {
            this._id = ID;
        }

        private int _id = 0;
        public int ID
        {
            get { return _id; }
            set { this.SetProperty( ref this._id, value); }
        }
    }

    public class AwardManager
    {
        public async static Task<ObservableCollection<AwardItem>> UsersAwards(int userId, String DBPath)
        {
            ObservableCollection<AwardItem> UserAwards = new ObservableCollection<AwardItem>();

            var db = new SQLiteAsyncConnection(DBPath);
            
            var AwardQuery = await db.QueryAsync<UserAward>("SELECT * FROM UserAward WHERE UserID=?", userId);
            var LPath = ApplicationData.Current.LocalFolder.Path;
            for (int i = 0; i < AwardQuery.Count; i++)
            {
                var languageID = System.Globalization.CultureInfo.CurrentCulture.Name;
                var LocalQuery = await db.QueryAsync<AwardLocalization>(
                                     "SELECT * FROM AwardLocalization WHERE AwardID=?", AwardQuery[i].AwardID);
                var localization = LocalQuery.Where(l => l.LanguageID.Contains(languageID)).First();
                if (localization == null) localization = LocalQuery.Where(l => l.LanguageID.Contains("en")).First();
                
                var AwardDataQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE ID=?", AwardQuery[i].AwardID);


                var AwardData = AwardDataQuery.FirstOrDefault();

                

                UserAwards.Add(new AwardItem(String.Concat("Award " + AwardQuery[i].AwardID.ToString()),
                                             localization.AwardName,
                                             Path.Combine(LPath, AwardData.LogoPath),
                                             localization.AwardDescription,
                                             AwardQuery[i].AwardID));

            }
            return UserAwards;
        }

        public async static Task<AwardItem> GetAward(int awardId, String DBPath)
        {
            var LPath = ApplicationData.Current.LocalFolder.Path;

            var db = new SQLiteAsyncConnection(DBPath);

            var AwardQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE ID=?", awardId);
            var Award = AwardQuery.FirstOrDefault();

            var languageID = System.Globalization.CultureInfo.CurrentCulture.Name;
            
            
            var LocalQuery = await db.QueryAsync<AwardLocalization>(
                                     "SELECT * FROM AwardLocalization WHERE AwardID=?", Award.ID);

            var localization = LocalQuery.Where(l => l.LanguageID.Contains(languageID)).First();
            if (localization == null) localization = LocalQuery.Where(l => l.LanguageID.Contains("en")).First();

            return new AwardItem(String.Concat("Award " + Award.ID.ToString()),
                                             localization.AwardName,
                                             Path.Combine(LPath, Award.LogoPath),
                                             localization.AwardDescription,
                                             Award.ID);
        }

        public async static Task<AwardItem> GetAwardForRate(int Rate, String DBPath)
        {
            if (Rate>5 || Rate <2)
                return null;
            var LPath = ApplicationData.Current.LocalFolder.Path;
            var db = new SQLiteAsyncConnection(DBPath);

            var AwardQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE Rate=?", Rate);
            var Award = AwardQuery.FirstOrDefault();
            
            var LocalQuery = await db.QueryAsync<AwardLocalization>(
                                     "SELECT * FROM AwardLocalization WHERE AwardID=?", Award.ID);
            var languageID = System.Globalization.CultureInfo.CurrentCulture.Name;
            var localization = LocalQuery.Where(l => l.LanguageID.Contains(languageID)).First();
            if (localization == null) localization = LocalQuery.Where(l => l.LanguageID.Contains("en")).First();

            return new AwardItem("Award " + Award.ID.ToString(),
                                localization.AwardName,
                                Path.Combine(LPath, Award.LogoPath),
                                localization.AwardDescription,
                                Award.ID);
        }

        public async static void AddUserAward(AwardItem award, int UserId, String DBPath)
        {
            var db = new SQLiteAsyncConnection(DBPath);

            await db.InsertAsync(new UserAward() { AwardID = award.ID, 
                                                    UserID = UserId });
        }
    }
}
