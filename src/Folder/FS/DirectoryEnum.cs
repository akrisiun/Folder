using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IOFile
{
    public static class DirectoryEnum
    {
        public static IEnumerable<string> ReadFiles(string path,
               String searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var resultHandler = new StringResultHandler(true, includeDirs: false);
            var iterator = new Win32FileSystemEnumerableIterator<string>(path, null, searchPattern, searchOption, resultHandler);
            var numer = iterator.GetEnumerator();

            while (numer.MoveNext())
                yield return Path.Combine(path, numer.Current);
        }

        public static IEnumerable<FileDataInfo> ReadDirectories(string path,
                SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var resultHandler = new FileDataInfoResultHandler(FATTR.FILE_ATTRIBUTE_DIRECTORY | FATTR.FILE_ATTRIBUTE_READONLY);
            var searchPattern = "*.*";
            return ReadHandler(resultHandler, path, searchPattern, searchOption);
        }

        public static IEnumerable<FileDataInfo> ReadFilesInfo(string path,
               String searchPattern = "*.*",
               SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var resultHandler = new FileDataInfoResultHandler(FATTR.FILE_ATTRIBUTE_NORMAL | FATTR.FILE_ATTRIBUTE_READONLY);
            return ReadHandler(resultHandler, path, searchPattern, searchOption);
        }

        static IEnumerable<FileDataInfo> ReadHandler(this FileDataInfoResultHandler resultHandler, string path,
               String searchPattern, SearchOption searchOption)
        {
            var iterator = new Win32FileSystemEnumerableIterator<FileDataInfo>(path, null, searchPattern, searchOption, resultHandler);
            var numer = iterator.GetEnumerator();

            while (numer.MoveNext())
                yield return numer.Current;
        }

        public class FileDataInfo
        {
            internal uint dwFileAttributes;
            internal Win32FindFile.FILE_TIME ftLastWriteTime;
            internal uint nFileSizeLow;

            // [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string cFileName;
            long? ticks;

            public string Name { get { return cFileName; } set { cFileName = value; } }
            public int Length { get { return (int)nFileSizeLow; } set { nFileSizeLow = (uint)value; } }
            public DateTime LastWriteTime
            {
                get { return ticks.HasValue ? DateTime.FromBinary(ticks.Value) : DateTime.FromFileTime(ftLastWriteTime.ToTicks()); }
                set { ticks = value.ToBinary(); }
            }

            public bool HasAttribute(FATTR attr) { return (dwFileAttributes & (uint)attr) != 0; }
        }

        internal class FileDataInfoResultHandler : SearchResultHandler<FileDataInfo>
        {
            public FATTR Filters { get; set; }

            public FileDataInfoResultHandler(FATTR filter)
            {
                Filters = filter;
            }

            [System.Security.SecurityCritical]
            internal override bool IsResultIncluded(SearchResult result)
            {
                bool allowHidden = (Filters & FATTR.FILE_ATTRIBUTE_READONLY) != 0;
                if (!allowHidden && !result.FindData.HasAttribute(FATTR.FILE_ATTRIBUTE_READONLY))
                    return false;

                if ((Filters & FATTR.FILE_ATTRIBUTE_DIRECTORY) != 0)
                {
                    if (result.FindData.cFileName == "." || result.FindData.cFileName == "..")
                        return false;

                    if (result.FindData.HasAttribute(FATTR.FILE_ATTRIBUTE_DIRECTORY))
                        return true;
                }

                if ((Filters & FATTR.FILE_ATTRIBUTE_NORMAL) != 0)
                    return Win32FileSystemEnumerableHelpers.IsFile(result.FindData);

                return false;
            }

            [System.Security.SecurityCritical]
            internal override FileDataInfo CreateObject(SearchResult result)
            {
                Win32FindFile.WIN32_FIND_DATA data = result.FindData;
                var build = new StringBuilder(data.cFileName);
                FileDataInfo fi = new FileDataInfo
                {
                    cFileName = build.ToString(),
                    dwFileAttributes = data.dwFileAttributes,
                    nFileSizeLow = data.nFileSizeLow,
                    ftLastWriteTime = data.ftLastWriteTime
                };
                return fi;
            }
        }
    }

}