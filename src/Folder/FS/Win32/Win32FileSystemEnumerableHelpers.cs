using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IOFile
{
    internal static class Win32FileSystemEnumerableHelpers
    {
        [System.Security.SecurityCritical]  // auto-generated
        internal static bool IsDir(Win32FindFile.WIN32_FIND_DATA data)
        {
            // Don't add "." nor ".."
            return (0 != (data.dwFileAttributes & Win32FindFile.FileAttributes.FILE_ATTRIBUTE_DIRECTORY))
                                                && !data.cFileName.Equals(".") && !data.cFileName.Equals("..");
        }

        [System.Security.SecurityCritical]  // auto-generated
        internal static bool IsFile(Win32FindFile.WIN32_FIND_DATA data)
        {
            return 0 == (data.dwFileAttributes & Win32FindFile.FileAttributes.FILE_ATTRIBUTE_DIRECTORY);
        }
    }

    internal partial class Interop
    {
        internal partial class mincore
        {
            internal partial class FileAttributes
            {
                internal const int FILE_ATTRIBUTE_NORMAL = 0x00000080;
                internal const int FILE_ATTRIBUTE_READONLY = 0x00000001;
                internal const int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
                internal const int FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
            }


            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            [BestFitMapping(false)]
            internal unsafe struct WIN32_FIND_DATA
            {
                internal uint dwFileAttributes;
                internal FILE_TIME ftCreationTime;
                internal FILE_TIME ftLastAccessTime;
                internal FILE_TIME ftLastWriteTime;
                internal uint nFileSizeHigh;
                internal uint nFileSizeLow;
                internal uint dwReserved0;
                internal uint dwReserved1;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                internal string cFileName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
                internal string cAlternateFileName;
            }

            internal struct FILE_TIME
            {
                internal uint dwLowDateTime;
                internal uint dwHighDateTime;

                internal FILE_TIME(long fileTime)
                {
                    dwLowDateTime = (uint)fileTime;
                    dwHighDateTime = (uint)(fileTime >> 32);
                }

                internal long ToTicks()
                {
                    return ((long)dwHighDateTime << 32) + dwLowDateTime;
                }
            }

        }
    }

}

