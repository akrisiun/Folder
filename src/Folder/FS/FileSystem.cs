using System;
using System.IO;

namespace VfpProj
{
    public static class FileSystem
    {
        public static string CurrentDirectory {
            get { return Directory.GetCurrentDirectory(); } // Environment.CurrentDirectory; } 
            set { System.IO.Directory.SetCurrentDirectory(value); }
        }


    }
}
