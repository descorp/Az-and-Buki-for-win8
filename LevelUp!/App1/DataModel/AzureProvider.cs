using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.MobileServices;

namespace levelupspace.DataModel
{
    public class AzureDBProvider
    {

        #region MobileService connection
        //public static MobileServiceClient MobileService = new MobileServiceClient(
        //"https://levelupbackend.azure-mobile.net/",
        //"hGDmdRlTFfjhgwkOzAAICUBJliMLzn13"
        //);

        private const string AppUri = "https://levelupbackend.azure-mobile.net/";
        private const string AppKey = "hGDmdRlTFfjhgwkOzAAICUBJliMLzn13";

        #endregion



        ////private static MobileServiceCollectionView<Alphabet> AlphabetItems;
        private static IMobileServiceTable<Alphabet> AlphabetTable
        {
            get { return new MobileServiceClient(AppUri, AppKey).GetTable<Alphabet>(); }
        }

        ////private static MobileServiceCollectionView<AlphabetLocalization> AlphabetLocalizationtItems;
        private static IMobileServiceTable<AlphabetLocalization> AlphabetLocalizationTable
        {
            get { return new MobileServiceClient(AppUri, AppKey).GetTable<AlphabetLocalization>(); }
        }

        ////private static MobileServiceCollectionView<User> UserItems;
        private static IMobileServiceTable<User> UserTable
        {
            get { return new MobileServiceClient(AppUri, AppKey).GetTable<User>(); }
        }

        ////private static MobileServiceCollectionView<UserAward> UserAwardItems;
        private static IMobileServiceTable<UserAward> UserAwardTable
        {
            get { return new MobileServiceClient(AppUri, AppKey).GetTable<UserAward>(); }
        }

        ////private static MobileServiceCollectionView<Award> AwardItems;
        private static IMobileServiceTable<Award> AwardTable
        {
            get { return new MobileServiceClient(AppUri, AppKey).GetTable<Award>(); }
        }

        ////private static MobileServiceCollectionView<AwardLocalization> AwardLocalizationItems;
        private static IMobileServiceTable<AwardLocalization> AwardLocalizationTable
        {
            get { return new MobileServiceClient(AppUri, AppKey).GetTable<AwardLocalization>(); }
        }



        #region Users data providing methods

        public async static void AddNewUser(User user)
        {
            var userTable = UserTable;
            await userTable.InsertAsync(user);
            await userTable.ToListAsync();
        }

        public async static Task<bool> UserUnique(string name)
        {
            var userTable = UserTable;
            var list = await userTable.Where(user => user.Name == name).ToListAsync();
            return list.Count == 0;
        }

        public static async Task<User> GetUser(string name, string hash)
        {
            var userTable = UserTable;
            var list = await userTable.Where(user => user.Name == name && user.Hash == hash).ToListAsync();
            if (list.Count > 0)
                return list.First();
            return null;
        }

        #endregion

        #region Packages data providing methods

        public async static Task<List<Alphabet>> GetAllPackages()
        {
            //// This code inserts a new TodoItem into the database. When the operation completes
            //// and Mobile Services has assigned an Guid, the item is added to the CollectionView
            var alphabetTable = AlphabetTable;
            return await alphabetTable.ToListAsync();
        }

        public async static Task<string> GetBlobName(int packageID)
        {
            var alphabetTable = AlphabetTable;
            var list = await alphabetTable.Where(pack => pack.ID == packageID).ToListAsync();
            var fullPath = list.First().Path;
            var lastSlash = fullPath.LastIndexOf("/", StringComparison.Ordinal);
            return fullPath.Remove(0, lastSlash + 1);
        }

        /// <summary>
        /// Returns Size of blobe containes package
        /// </summary>
        /// <param name="packageID">Package ID in table</param>
        /// <returns>Size of </returns>
        public static async Task<long> GetBlobSize(int packageID)
        {
            var alphabetTable = AlphabetTable;
            var list = await alphabetTable.Where(pack => pack.ID == packageID).ToListAsync();
            return list.First().Length;
        }

        public async static Task<AlphabetLocalization> GetPackageLocalization(Alphabet alphabet, String localizationId)
        {
            var alphabetLocalizationTable = AlphabetLocalizationTable;
            var localization = await alphabetLocalizationTable.Where(local => local.AlphabetID == alphabet.Guid && local.LanguageID.Contains(localizationId)).ToListAsync();
            return localization.First();
        }

        #endregion

        #region Awards data providing methods

        public async static void AddAwardToUser(User user, Award award)
        {
            //// This code inserts a new TodoItem into the database. When the operation completes
            //// and Mobile Services has assigned an Guid, the item is added to the CollectionView
            var userAwardTable = UserAwardTable;
            var newAward = new UserAward { AwardID = award.ID, UserID = user.ID };
            await userAwardTable.InsertAsync(newAward);
        }

        public async static Task<List<UserAward>> GetAwardsOfThisUser(User user)
        {
            var userAwardTable = UserAwardTable;
            return await userAwardTable.Where(u => u.UserID == user.ID).ToListAsync();
        }

        public async static Task<List<Award>> GetAllAwards(User user)
        {
            var awardTable = AwardTable;
            return await awardTable.ToListAsync();
        }

        public async static Task<List<AwardLocalization>> GetAwardLocalization(Award award, String localizationId)
        {
            var awardLocalizationTable = AwardLocalizationTable;
            return await awardLocalizationTable.Where(local => local.AwardId == award.ID && local.LanguageID == localizationId).ToListAsync();
        }

        #endregion

    }

    public class AzureStorageProvider
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=http;AccountName=levelupstorage;AccountKey=k9OkEg5CQHVD415z+s8xD/zx4lCKyWdBgWrDxqUnCsVbohmxgYVUUs8q4ZknpJdpgOEikk0damf2/lTSksSTZg==";

        private static readonly CloudStorageAccount StorageAccount =
            CloudStorageAccount.Parse(ConnectionString);

        public static async void UploadAvatarToStorage(StorageFile file, String username)
        {
            // Retrieve reference to a previously created container.
            var container = StorageAccount.CreateCloudBlobClient().GetContainerReference("userpic");
            
            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference("UL" + username);

            // Create or overwrite the "myblob" blob with contents from a local file.

            var stream = await file.OpenAsync(FileAccessMode.Read);

            await blockBlob.UploadFromStreamAsync(stream.GetInputStreamAt(0));

        }

        public static async void DownloadPackageFromStorage(StorageFile file, String packageName, EventHandler downloadCompletedEvent = null)
        {
            // Retrieve reference to a previously created container.
            var container = StorageAccount.CreateCloudBlobClient().GetContainerReference("packages");

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference(packageName);
            // Create or overwrite the "myblob" blob with contents from a local file.

            using (var fileStream = await file.OpenStreamForWriteAsync())
            {

                await blockBlob.DownloadToStreamAsync(fileStream.AsOutputStream());
            }

            var args = new EventArgs();

            if (downloadCompletedEvent != null) downloadCompletedEvent(null, args);
        }

        public static async void DownloadPackageFromStorage(StorageFile file, String packageNameInBlob, int numOfParts, long Length, EventHandler downloadCompletedEvent = null, EventHandler downloadPartEvent = null)
        {
            long offset = 0;
            var length = Length / numOfParts;
            if (length < 4096)
                length = 4096;
            try
            {
                // Retrieve reference to a previously created container.
                var container = StorageAccount.CreateCloudBlobClient().GetContainerReference("packages");

                // Retrieve reference to a blob named "myblob".
                var blockBlob = container.GetBlockBlobReference(packageNameInBlob);
                // Create or overwrite the "myblob" blob with contents from a local file.
                using (var fileStream = await file.OpenStreamForWriteAsync())
                {
                    while (offset < Length)
                    {
                        if (Length - length < offset)
                            length = Length - offset;

                        await blockBlob.DownloadRangeToStreamAsync(fileStream.AsOutputStream(), offset, length);
                        offset += length;
                        if (downloadPartEvent != null)
                        {
                            downloadPartEvent(null, new FilePartDownloadedEventArgs(file.DisplayName, 0));
                        }

                    }
                }
            }
            catch
            {
                if (downloadCompletedEvent != null) downloadCompletedEvent(file, new FilePartDownloadedEventArgs(file.DisplayName, -1));
            }

            var args = new FilePartDownloadedEventArgs(file.DisplayName, offset);
            if (downloadCompletedEvent != null) downloadCompletedEvent(file, args);
        }

        public static async void DownloadAvatarFromStorage(StorageFile file, String userID)
        {
            // Retrieve reference to a previously created container.
            var container = StorageAccount.CreateCloudBlobClient().GetContainerReference("userpic");

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference("UL" + userID);
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = await file.OpenStreamForWriteAsync())
            {
                await blockBlob.DownloadToStreamAsync(fileStream.AsOutputStream());
            }

        }

        public static string GetConnectionString(string packageNameInBlob)
        {
            var container = StorageAccount.CreateCloudBlobClient().GetContainerReference("packages");

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference(packageNameInBlob);

            var policy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTimeOffset.Now.AddMinutes(10)
            };

            var url = blockBlob.GetSharedAccessSignature(policy);
            return url;
        }

    }

    public class FilePartDownloadedEventArgs : EventArgs
    {
        string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
        long _offset;
        public long Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public FilePartDownloadedEventArgs(string FileName, long Offset)
        {
            _offset = Offset;
            _fileName = FileName;
        }

    }

    public class FileUnzippedEventArgs : EventArgs
    {
        string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public FileUnzippedEventArgs(string FileName, string folderPath)
        {
            _fileName = FileName;
            _folderPath = folderPath;
        }

        private String _folderPath;

        public String FolderPath
        {
            get
            {
                return _folderPath;
            }

            set
            {
                _folderPath = value;
            }
        }
    }

}
