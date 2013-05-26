using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;

namespace levelupspace
{

    public class UserManager
    {
        private static string DBPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db");
        UserManager()
        { 
        }

        public async static Task<int> AddUserAsync(User Newby)
        {

            Newby.Hash = ComputeMD5(Newby.Hash);
            var db = new SQLiteAsyncConnection(DBPath);

            var Result =await db.InsertAsync(Newby);
            if (Result > 0)
            {
                var u = db.QueryAsync<User>("SELECT * FROM User WHERE Name=?", Newby.Name);
                Newby.ID = u.Result[0].ID;
                if (Newby.Avatar != "ms-appx:///Assets/Userlogo.png")
                {

                    Newby.Avatar = String.Concat("Users/UL", Newby.ID.ToString(), ".png");

                    await db.UpdateAsync(Newby);
                }
                

                ApplicationData.Current.LocalSettings.Values["UserName"] = Newby.Name;
                ApplicationData.Current.LocalSettings.Values["UserLogo"] = Newby.Avatar;
                ApplicationData.Current.LocalSettings.Values["UserID"] = Newby.ID;
                
                return Newby.ID;
            }
            else return -1;
        }

        public async static Task<bool> Authorize(string Name,string Pass)
        {
            var db = new SQLiteAsyncConnection(DBPath);

            var User = await db.QueryAsync<User>("SELECT * FROM User WHERE Name=?", Name);
            if (User.Count == 0)
                return false;
            if (String.Compare( ComputeMD5(String.Concat(Name,Pass)), User[0].Hash)!=0)
                return false;
            
            ApplicationData.Current.LocalSettings.Values["UserName"]=Name;
            ApplicationData.Current.LocalSettings.Values["UserLogo"] = User[0].Avatar;
            ApplicationData.Current.LocalSettings.Values["UserID"] = User[0].ID;

            return true;
            
        }

        public static void LogOut()
        {
            ApplicationData.Current.LocalSettings.Values.Remove("UserName");
            ApplicationData.Current.LocalSettings.Values.Remove("UserLogo");
            ApplicationData.Current.LocalSettings.Values.Remove("UserID");
        }

        public static bool IsAutorized
        {
            get { return ApplicationData.Current.LocalSettings.Values.ContainsKey("UserName"); }
        }

        public static int UserId
        {
            get { return (int)ApplicationData.Current.LocalSettings.Values["UserID"]; }
        }

        public async static Task<bool> IsUniqueLoginAsync(String Login)
        {
            var db = new SQLiteAsyncConnection(DBPath);
            
            var UQuery = await db.QueryAsync<User>("SELECT * FROM User WHERE Name=?", Login);
            
            return UQuery.Count > 0 ? false : true;
        }

        private static string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
    }
}
