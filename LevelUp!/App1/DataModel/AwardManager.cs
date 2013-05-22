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
        public AwardItem(String uniqueId, String title, String imagePath, String description, int ID, DateTime Date)
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

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { this.SetProperty(ref this._date, value); }
        }

    }

    public class AwardManager
    {
        //public async static Task<ObservableCollection<AwardItem>> UsersAwards(int userId)
        //{
        //    ObservableCollection<AwardItem> UserAwards = new ObservableCollection<AwardItem>();

        //    var db = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));

        //    var AwardQuery = await db.QueryAsync<UserAward>("SELECT * FROM UserAward WHERE UserID=?", userId);

        //    for (int i = 0; i < AwardQuery.Count; i++)
        //    {
        //        //TODO Do proper localization
        //        var LocalQuery = await db.QueryAsync<AwardLocalization>(
        //                             "SELECT * FROM AwardLocalization WHERE AwardID=?", AwardQuery[i].AwardID);
        //        var AwardDataQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE ID=?", AwardQuery[i].AwardID);
        //        var AwardData = AwardDataQuery.FirstOrDefault();
        //        var localization = LocalQuery.FirstOrDefault();

        //        UserAwards.Add(new AwardItem(String.Concat("Award " + AwardQuery[i].AwardID.ToString()),
        //                                     localization.AwardName,
        //                                     Path.Combine(ApplicationData.Current.LocalFolder.Path, AwardData.LogoPath),
        //                                     localization.AwardDescription,
        //                                     AwardQuery[i].AwardID,
        //                                     AwardQuery[i].Date));

        //    }
        //}

        public async static Task<Award> GetAwardForRate(double Rate)
        {
            if (Rate>10)
                return null;

            var db = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));

            var AwardQuery = await db.QueryAsync<Award>("SELECT * FROM Award WHERE UserID=?", userId);
        }

    }
}
