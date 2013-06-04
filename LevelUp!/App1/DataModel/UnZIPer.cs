using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace levelupspace.DataModel
{
    public class UnZIPer
    {
        public static async void Unzip(StorageFile file)
        {
            Stream stream = await file.OpenStreamForReadAsync();
            var rootdir = ApplicationData.Current.LocalFolder;
            Dictionary<String, StorageFolder> folderTree = new Dictionary<string, StorageFolder>();
            using (ZipArchive archive = new ZipArchive(stream))
            {
                foreach (ZipArchiveEntry folderEntry in archive.Entries.Where(u => u.Length == 0))
                {
                    string path = folderEntry.FullName;
                    string finalFolderName = path.Substring(0, path.Length - 1);

                    if (finalFolderName.LastIndexOf("/") > 0)
                    {
                        string subFolderName = path.Substring(0, finalFolderName.LastIndexOf("/") + 1);
                        finalFolderName = finalFolderName.Substring(subFolderName.Length);
                        var subFolder = folderTree.First(u => u.Key == subFolderName).Value;
                        folderTree.Add(path, await subFolder.CreateFolderAsync(finalFolderName, CreationCollisionOption.OpenIfExists));
                    }
                    else
                    {
                        folderTree.Add(path, await rootdir.CreateFolderAsync(finalFolderName, CreationCollisionOption.OpenIfExists));
                    }
                }

                foreach (ZipArchiveEntry entry in archive.Entries.Where(u => u.Length > 0 && !u.Name.Contains("Thumbs.db")))
                {
                    string dirName = entry.FullName.Substring(0, entry.FullName.LastIndexOf("/") + 1);
                    var newFile = await folderTree.First(u => u.Key == dirName).Value.CreateFileAsync(entry.Name, CreationCollisionOption.ReplaceExisting);
                    using (Stream streamForWriting = await newFile.OpenStreamForWriteAsync())
                    {
                        using (Stream streamForRead = entry.Open())
                        {
                            long length = entry.Length;
                            int n = 0;
                            int offset = 0;
                            do
                            {
                                byte[] bytes = new byte[length];
                                n = await streamForRead.ReadAsync(bytes, 0, (int)length);
                                await streamForWriting.WriteAsync(bytes, 0, n);
                                offset += n;
                            }
                            while (offset < length);
                        }
                    }
                }
            }
        }

        public static async void Unzip(StorageFile file, EventHandler EventHandler)
        {
            Stream stream = await file.OpenStreamForReadAsync();
            var rootdir = ApplicationData.Current.LocalFolder;
            Dictionary<String, StorageFolder> folderTree = new Dictionary<string, StorageFolder>();
            using (ZipArchive archive = new ZipArchive(stream))
            {
                foreach (ZipArchiveEntry folderEntry in archive.Entries.Where(u => u.Length == 0))
                {
                    string path = folderEntry.FullName;
                    string finalFolderName = path.Substring(0, path.Length - 1);

                    if (finalFolderName.LastIndexOf("/") > 0)
                    {
                        string subFolderName = path.Substring(0, finalFolderName.LastIndexOf("/") + 1);
                        finalFolderName = finalFolderName.Substring(subFolderName.Length);
                        var subFolder = folderTree.First(u => u.Key == subFolderName).Value;
                        folderTree.Add(path, await subFolder.CreateFolderAsync(finalFolderName, CreationCollisionOption.OpenIfExists));
                    }
                    else
                    {
                        folderTree.Add(path, await rootdir.CreateFolderAsync(finalFolderName, CreationCollisionOption.OpenIfExists));
                    }
                }

                foreach (ZipArchiveEntry entry in archive.Entries.Where(u => u.Length > 0))
                {
                    string dirName = entry.FullName.Substring(0, entry.FullName.LastIndexOf("/") + 1);
                    var newFile = await folderTree.First(u => u.Key == dirName).Value.CreateFileAsync(entry.Name, CreationCollisionOption.ReplaceExisting);
                    using (Stream streamForWriting = await newFile.OpenStreamForWriteAsync())
                    {
                        using (Stream streamForRead = entry.Open())
                        {
                            long length = entry.Length;
                            int n = 0;
                            int offset = 0;
                            do
                            {
                                byte[] bytes = new byte[length];
                                n = await streamForRead.ReadAsync(bytes, 0, (int)length);
                                await streamForWriting.WriteAsync(bytes, 0, n);
                                offset += n;
                            }
                            while (offset < length);
                        }
                    }
                }
            }

            if (EventHandler != null)
                EventHandler(null, new FileUnzippedEventArgs(file.Name, folderTree.First().Value.Path));
        }
    }

}
