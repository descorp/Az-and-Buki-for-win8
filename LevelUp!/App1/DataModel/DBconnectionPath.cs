using System;
using System.IO;
using Windows.Storage;

namespace levelupspace
{
    public class DBconnectionPath
    {
        public static String Local = Path.Combine(ApplicationData.Current.LocalFolder.Path, "ABCdb.db");
        
        public static String Test = "Test.db";
    }
}
