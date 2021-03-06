﻿using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using levelupspace;
using System.Threading.Tasks;
using levelupspace.DataModel;
using SQLite;
using AsyncAssert = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer.Assert;

namespace UnitTestLibrary
{

    [TestClass]
    public class UserManagerClassTest
    {

        [TestMethod]

        public async Task AddUserAsyncEmptyName()
        {
            User newUser = new User();
            newUser.Name = "";
            String Pass = "123";
            newUser.Avatar = "ms-appx:///Assets/Userlogo.png";

            await AsyncAssert.ThrowsException<ArgumentOutOfRangeException>(async () =>
                {
                    await UserManager.AddUserAsync(newUser, Pass, DBconnectionPath.Test);
                });
        }

        public async Task AddUserAsyncEmptyPass()
        {
            User newUser = new User();
            newUser.Name = "Sasha";
            newUser.Avatar = "ms-appx:///Assets/Userlogo.png";
            String Pass = "";

            await AsyncAssert.ThrowsException<ArgumentOutOfRangeException>(async () =>
            {
                await UserManager.AddUserAsync(newUser, Pass, DBconnectionPath.Test);
            });
        }

        [TestMethod]
        public async Task AddUserAsyncNewUserCorrectData()
        {
            var db = new SQLiteAsyncConnection(DBconnectionPath.Test);
            await db.CreateTableAsync<User>();

            User newUser = new User();
            newUser.Name = "Sasha";
            newUser.Avatar = "ms-appx:///Assets/Userlogo.png";
            String Pass = "123";

            var Result = await UserManager.AddUserAsync(newUser, Pass, DBconnectionPath.Test);

            Assert.IsTrue(Result>0);
            UserManager.LogOut();
            await db.DropTableAsync<User>();

        }


        [TestMethod]
        public async Task IsUniqueLoginAsyncOccupiedUserName()
        {
            var db = new SQLiteAsyncConnection(DBconnectionPath.Test);
            await db.CreateTableAsync<User>();

            User newUser = new User();
            newUser.Name = "Sasha";
            newUser.Avatar = "ms-appx:///Assets/Userlogo.png";
            String Pass = "123";
            await UserManager.AddUserAsync(newUser, Pass, DBconnectionPath.Test);
            var Result = await UserManager.IsUniqueLoginAsync(newUser.Name, DBconnectionPath.Test);

            Assert.AreEqual(Result, false );
            UserManager.LogOut();
            await db.DropTableAsync<User>();
        }

        [TestMethod]
        public async Task IsUniqueLoginAsyncFreeUserName()
        {
            var db = new SQLiteAsyncConnection(DBconnectionPath.Test);
            await db.CreateTableAsync<User>();

            String Name = "Sasha";

            var Result = await UserManager.IsUniqueLoginAsync(Name, DBconnectionPath.Test);

            Assert.AreEqual(Result, true);

            await db.DropTableAsync<User>();
        }

        [TestMethod]
        public async Task AuthorizeCorrectNamePass()
        {
            var db = new SQLiteAsyncConnection(DBconnectionPath.Test);
            await db.CreateTableAsync<User>();
            User newUser = new User();
            newUser.Name = "Sasha";
            newUser.Avatar = "ms-appx:///Assets/Userlogo.png";
            String Pass = "123";

            await UserManager.AddUserAsync(newUser, Pass, DBconnectionPath.Test);

            var Result = await UserManager.Authorize(newUser.Name, Pass, DBconnectionPath.Test);

            UserManager.LogOut();

            Assert.IsTrue(Result);

            await db.DropTableAsync<User>();
        }

        [TestMethod]
        public async Task AuthorizeWrongNamePass()
        {
            var db = new SQLiteAsyncConnection(DBconnectionPath.Test);
            await db.CreateTableAsync<User>();
            User newUser = new User();
            newUser.Name = "Sasha";
            newUser.Avatar = "ms-appx:///Assets/Userlogo.png";
            String Pass = "123";

            await UserManager.AddUserAsync(newUser, Pass, DBconnectionPath.Test);

            var Result = await UserManager.Authorize(newUser.Name, "1", DBconnectionPath.Test);

            Assert.IsFalse(Result);
            UserManager.LogOut();
            await db.DropTableAsync<User>();
        }

        public async Task AuthorizeEmptyName()
        {
            String Name = "";

            await AsyncAssert.ThrowsException<ArgumentOutOfRangeException>(async () =>
            {
                await UserManager.Authorize(Name, "123", DBconnectionPath.Test);
            });
        }

        public async Task AuthorizeEmptyPass()
        {
            String Name = "Sasha";

            await AsyncAssert.ThrowsException<ArgumentOutOfRangeException>(async () =>
            {
                await UserManager.Authorize(Name, "", DBconnectionPath.Test);
            });
        }

        [TestMethod]
        public async Task UserIdCorrect()
        {
            var db = new SQLiteAsyncConnection(DBconnectionPath.Test);
            await db.CreateTableAsync<User>();
            User newUser = new User();
            newUser.Name = "Sasha";
            newUser.Avatar = "ms-appx:///Assets/Userlogo.png";
            String Pass = "123";

            var Expected = await UserManager.AddUserAsync(newUser, Pass, DBconnectionPath.Test);

            await UserManager.Authorize(newUser.Name, Pass, DBconnectionPath.Test);

            var Real = UserManager.UserId;

            Assert.AreEqual(Expected, Real);
            UserManager.LogOut();
            await db.DropTableAsync<User>();
        }

        [TestMethod]
        public async Task IsAutorizedUserAuthorized()
        {
            var db = new SQLiteAsyncConnection(DBconnectionPath.Test);
            await db.CreateTableAsync<User>();
            User newUser = new User();
            newUser.Name = "Sasha";
            newUser.Avatar = "ms-appx:///Assets/Userlogo.png";
            String Pass = "123";

            await UserManager.AddUserAsync(newUser, Pass, DBconnectionPath.Test);

            await UserManager.Authorize(newUser.Name, Pass, DBconnectionPath.Test);

            var Result = UserManager.IsAutorized;

            Assert.IsTrue(Result);

            UserManager.LogOut();
            await db.DropTableAsync<User>();
        }

        [TestMethod]
        public void IsAutorizedUserNotAuthorized()
        {
            var Result = UserManager.IsAutorized;
            Assert.IsFalse(Result);
        }
    }

}

