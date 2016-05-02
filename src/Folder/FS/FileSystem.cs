using System;
using System.IO;

namespace Folder.FS
{
    public static class FileSystem
    {
        public const int MAX_PATH = 260;

        public static string CurrentDirectory
        {
            get { return Directory.GetCurrentDirectory(); } // Environment.CurrentDirectory; } 
            set { System.IO.Directory.SetCurrentDirectory(value); }
        }

        // not null
        public static string SafeGetExtensionLower(string file)
        {
            var ext = Path.GetExtension(file ?? String.Empty);
            return ext.Length == 0 ? ext : ext.ToLower();
        }

    }
}
