using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LevelUP
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
        public async static Task<ObservableCollection<AwardItem>> UsersAwards(int userId)
        {
            ObservableCollection<AwardItem> UserAwards = new ObservableCollection<AwardItem>();

            var db = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));

            var AwardQuery = await db.QueryAsync<UserAward>("SELECT * FROM UserAward WHERE UserID=?", userId);

            for (int i = 0; i < AwardQuery.Count; i++)
            {
                //TODO Do proper localization
                var LocalQuery = await db.QueryAsync<AwardLocalization>(
                                     "SELECT * FROM AwardLocalization WHERE AwardID=?", AwardQuery[i].AwardID);
                var AwardDataQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE ID=?", AwardQuery[i].AwardID);
                var AwardData = AwardDataQuery.FirstOrDefault();
                var localization = LocalQuery.FirstOrDefault();

                UserAwards.Add(new AwardItem(String.Concat("Award " + AwardQuery[i].AwardID.ToString()),
                                             localization.AwardName,
                                             Path.Combine(ApplicationData.Current.LocalFolder.Path, AwardData.LogoPath),
                                             localization.AwardDescription,
                                             AwardQuery[i].AwardID));

            }
            return UserAwards;
        }

        public async static Task<AwardItem> GetAward(int awardId)
        {
            

            var db = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));

            var AwardQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE ID=?", awardId);
            var Award = AwardQuery.FirstOrDefault();

            //TODO Do proper localization
            var LocalQuery = await db.QueryAsync<AwardLocalization>(
                                     "SELECT * FROM AwardLocalization WHERE AwardID=?", Award.ID);
            
            var localization = LocalQuery.FirstOrDefault();

            return new AwardItem(String.Concat("Award " + Award.ID.ToString()),
                                             localization.AwardName,
                                             Path.Combine(ApplicationData.Current.LocalFolder.Path, Award.LogoPath),
                                             localization.AwardDescription,
                                             Award.ID);
        }

        public async static Task<AwardItem> GetAwardForRate(int Rate)
        {
            if (Rate>5 || Rate <2)
                return null;

            var db = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));

            var AwardQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE Rate=?", Rate);
            var Award = AwardQuery.FirstOrDefault();
            //TODO Do proper localization
            var LocalQuery = await db.QueryAsync<AwardLocalization>(
                                     "SELECT * FROM AwardLocalization WHERE AwardID=?", Award.ID);
            var localization = LocalQuery.FirstOrDefault();

            return new AwardItem("Award " + Award.ID.ToString(),
                                localization.AwardName,
                                Path.Combine(ApplicationData.Current.LocalFolder.Path, Award.LogoPath),
                                localization.AwardDescription,
                                Award.ID);
        }

        public async static void AddUserAward(AwardItem award, int UserId)
        {
            var db = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));

            await db.InsertAsync(new UserAward() { AwardID = award.ID, 
                                                    UserID = UserId });
        }

        private static string DateTimeSQLite(DateTime datetime)
        {
            string dateTimeFormat = "{0}-{1}-{2} {3}:{4}:{5}.{6}";
            return string.Format(dateTimeFormat, datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond);
        }
    }
}
