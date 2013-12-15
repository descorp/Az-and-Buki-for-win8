using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace levelupspace.DataModel
{
    public class UnZIPer
    {
        public static async void Unzip(StorageFile file)
        {
            using (var stream = await file.OpenStreamForReadAsync())
            {
                var rootdir = ApplicationData.Current.LocalFolder;
                var folderTree = new Dictionary<string, StorageFolder>();
                using (var archive = new ZipArchive(stream))
                {
                    foreach (var folderEntry in archive.Entries.Where(u => u.Length == 0))
                    {
                        var path = folderEntry.FullName;
                        var finalFolderName = path.Substring(0, path.Length - 1);

                        if (finalFolderName.LastIndexOf("/", StringComparison.Ordinal) > 0)
                        {
                            var subFolderName = path.Substring(0,
                                finalFolderName.LastIndexOf("/", StringComparison.Ordinal) + 1);
                            finalFolderName = finalFolderName.Substring(subFolderName.Length);
                            var subFolder = folderTree.First(u => u.Key == subFolderName).Value;
                            folderTree.Add(path,
                                await subFolder.CreateFolderAsync(finalFolderName, CreationCollisionOption.OpenIfExists));
                        }
                        else
                        {
                            folderTree.Add(path,
                                await rootdir.CreateFolderAsync(finalFolderName, CreationCollisionOption.OpenIfExists));
                        }
                    }


                    foreach (var entry in archive.Entries.Where(u => u.Length > 0 && !u.Name.Contains("Thumbs.db")))
                    {
                        var dirName = entry.FullName.Substring(0,
                            entry.FullName.LastIndexOf("/", StringComparison.Ordinal) + 1);
                        var newFile =
                            await
                                folderTree.First(u => u.Key == dirName)
                                    .Value.CreateFileAsync(entry.Name, CreationCollisionOption.ReplaceExisting);
                        using (var streamForWriting = await newFile.OpenStreamForWriteAsync())
                        {
                            using (var streamForRead = entry.Open())
                            {
                                var length = entry.Length;
                                var offset = 0;
                                do
                                {
                                    var bytes = new byte[length];
                                    var n = await streamForRead.ReadAsync(bytes, 0, (int) length);
                                    await streamForWriting.WriteAsync(bytes, 0, n);
                                    offset += n;
                                } while (offset < length);
                            }
                        }
                    }
                }
            }
        }

        public static async void Unzip(StorageFile file, EventHandler EventHandler)
        {
            Stream stream = null;

            do
            {
                try
                {
                    var asyncStream = await file.OpenReadAsync();
                    stream = asyncStream.AsStream();
                }
                catch (Exception)
                {
                }
            }
            while (stream != null && stream.Length == 0);

            var rootdir = ApplicationData.Current.LocalFolder;
            var folderTree = new Dictionary<string, StorageFolder>();
            using (var archive = new ZipArchive(stream))
            {
                foreach (var folderEntry in archive.Entries.Where(u => u.Length == 0))
                {
                    var path = folderEntry.FullName;
                    var finalFolderName = path.Substring(0, path.Length - 1);

                    if (finalFolderName.LastIndexOf("/", StringComparison.Ordinal) > 0)
                    {
                        var subFolderName = path.Substring(0, finalFolderName.LastIndexOf("/", StringComparison.Ordinal) + 1);
                        finalFolderName = finalFolderName.Substring(subFolderName.Length);
                        var subFolder = folderTree.First(u => u.Key == subFolderName).Value;
                        folderTree.Add(path, await subFolder.CreateFolderAsync(finalFolderName, CreationCollisionOption.OpenIfExists));
                    }
                    else
                    {
                        folderTree.Add(path, await rootdir.CreateFolderAsync(finalFolderName, CreationCollisionOption.OpenIfExists));
                    }
                }

                foreach (var entry in archive.Entries.Where(u => u.Length > 0))
                {
                    var dirName = entry.FullName.Substring(0, entry.FullName.LastIndexOf("/", StringComparison.Ordinal) + 1);
                    var folder = folderTree.First(u => u.Key == dirName).Value;
                    var newFile = await folder.CreateFileAsync(entry.Name, CreationCollisionOption.ReplaceExisting);

                    using (var streamForRead = entry.Open())
                    {
                        var length = entry.Length;

                        using (var streamForWriting = await newFile.OpenStreamForWriteAsync())
                        {
                            do
                            {
                                var bytes = new byte[length];
                                var n = await streamForRead.ReadAsync(bytes, 0, (int)length);
                                await streamForWriting.WriteAsync(bytes, 0, n);
                            }
                            while (streamForWriting.Length < length);
                        }
                    }
                }
            }
            stream.Dispose();
            
            if (EventHandler != null)
                EventHandler(null, new FileUnzippedEventArgs(file.Name, folderTree.First().Value.Path));
        }
    }

}
