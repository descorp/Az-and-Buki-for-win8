using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LevelUP
{
    public class UserManager
    {
        private bool IsInternetConnection()
        {
            var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null);
        }

        UserManager()
        { 
        }

        public async static Task<int> AddUserAsync(User Newby)
        {
            
            var db = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));

            var ID = await db.InsertAsync(Newby);

            return ID;
            
        }

        public static bool Authorize(User user)
        {
            SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));
            //ПРоверяем наличие пользователя в базе
            //Если есть - Ура!
            //Иначе - требуем интернет - соединения, либо ввести данные еще раз
            //Если есть интернет - спрашиваем у сервера, копируем данные в локальную бд
            return false;
        }

        public static bool EditUserData(User olduser, User newuser)
        {
            //Пытаемся отправить на сервер
            //Если успешно - обновляем и локальную бд
            return false;
        }

        public static bool IsUniqueLogin(String Login)
        {
            SQLiteConnection db = new SQLiteConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db"));
            db.CreateTable<User>();
            var UQuery = db.Query<User>("SELECT * FROM User WHERE Name=?", Login);
            return UQuery.Count > 0 ? false : true;
        }
    }
}
