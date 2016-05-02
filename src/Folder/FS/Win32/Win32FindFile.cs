using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IOFile
{
    public static class Win32FindFile
    {
        internal static IntPtr FindFirstFile(string fileName, ref WIN32_FIND_DATA data)
        {
            // use FindExInfoBasic since we don't care about short name and it has better perf
            return FindFirstFileEx(fileName, FINDEX_INFO_LEVELS.FindExInfoBasic, out data,
                FINDEX_SEARCH_OPS.FindExSearchNameMatch, IntPtr.Zero, 0);
        }

        internal static bool FindNextFile(IntPtr hFindFile, ref WIN32_FIND_DATA lpFindFileData)
        {
            IntPtr result = FindNextFileW(hFindFile, out lpFindFileData);
            return result != IntPtr.Zero;
        }

        #region WINAPI Find struct 

        //// C++
        //            WIN32_FIND_DATA fd;
        //            HANDLE hFind = FindFirstFile(szWild, &fd);
        //if (INVALID_HANDLE_VALUE != hFind)
        //{
        //   do {
        //   TCHAR szFileName[MAX_PATH];
        //   PathCombine(szFileName, masterfolders, fd.cFileName);

        //            // write szFilename to output stream..

        //        } while (FindNextFile(hFind, &fd));

        //   FindClose(hFind);
        //    }

        public partial class FileAttributes
        {
            internal const int FILE_ATTRIBUTE_NORMAL = 0x00000080;
            internal const int FILE_ATTRIBUTE_READONLY = 0x00000001;
            internal const int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
            internal const int FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        [BestFitMapping(false)]
        public unsafe struct WIN32_FIND_DATA
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

        internal static uint SetErrorMode(uint uMode)
        {
            // Prompting behavior no longer occurs in all platforms supported
            return 0;
        }

        #endregion

        // http://www.pinvoke.net/default.aspx/kernel32.findfirstfileex
        [DllImport(Libraries.Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindFirstFileEx(
            string lpFileName, FINDEX_INFO_LEVELS fInfoLevelId,
            out WIN32_FIND_DATA lpFindFileData,
            FINDEX_SEARCH_OPS fSearchOp, IntPtr lpSearchFilter, int dwAdditionalFlags);

        // pinvoke FindNextFile
        [DllImport(Libraries.Kernel32, EntryPoint = "FindNextFile", SetLastError = true
                   , CharSet = CharSet.Unicode, BestFitMapping = false)]
        static extern IntPtr FindNextFileW(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll")]
        internal static extern bool FindClose(IntPtr handle);

        #region other WINAPI structs

        private static class Libraries
        {
            internal const string Crypt32 = "crypt32.dll";
            internal const string Kernel32 = "kernel32.dll";
            internal const string NtDll = "ntdll.dll";
            internal const string OleAut32 = "oleaut32.dll";
            internal const string Sspi = "sspicli.dll";
            internal const string User32 = "user32.dll";
        }

        internal enum FINDEX_INFO_LEVELS : uint
        {
            FindExInfoStandard = 0x0u,
            FindExInfoBasic = 0x1u,
            FindExInfoMaxInfoLevel = 0x2u,
        }

        internal enum FINDEX_SEARCH_OPS : uint
        {
            FindExSearchNameMatch = 0x0u,
            FindExSearchLimitToDirectories = 0x1u,
            FindExSearchLimitToDevices = 0x2u,
            FindExSearchMaxSearchOp = 0x3u,
        }


        // internal
        public struct FILE_TIME
        {
            internal uint dwLowDateTime;
            internal uint dwHighDateTime;

            public FILE_TIME(long fileTime)
            {
                dwLowDateTime = (uint)fileTime;
                dwHighDateTime = (uint)(fileTime >> 32);
            }

            internal long ToTicks()
            {
                return ((long)dwHighDateTime << 32) + dwLowDateTime;
            }
        }

        // internal 
        public partial class Errors
        {
            internal const int ERROR_SUCCESS = 0x0;
            internal const int ERROR_INVALID_FUNCTION = 0x1;
            internal const int ERROR_FILE_NOT_FOUND = 0x2;
            internal const int ERROR_PATH_NOT_FOUND = 0x3;
            internal const int ERROR_ACCESS_DENIED = 0x5;
            internal const int ERROR_INVALID_HANDLE = 0x6;
            internal const int ERROR_NOT_ENOUGH_MEMORY = 0x8;
            internal const int ERROR_INVALID_DATA = 0xD;
            internal const int ERROR_INVALID_DRIVE = 0xF;
            internal const int ERROR_NO_MORE_FILES = 0x12;
            internal const int ERROR_NOT_READY = 0x15;
            internal const int ERROR_BAD_LENGTH = 0x18;
            internal const int ERROR_SHARING_VIOLATION = 0x20;
            internal const int ERROR_LOCK_VIOLATION = 0x21;
            internal const int ERROR_HANDLE_EOF = 0x26;
            internal const int ERROR_FILE_EXISTS = 0x50;
            internal const int ERROR_INVALID_PARAMETER = 0x57;
            internal const int ERROR_BROKEN_PIPE = 0x6D;
            internal const int ERROR_INSUFFICIENT_BUFFER = 0x7A;
            internal const int ERROR_INVALID_NAME = 0x7B;
            internal const int ERROR_NEGATIVE_SEEK = 0x83;
            internal const int ERROR_DIR_NOT_EMPTY = 0x91;
            internal const int ERROR_BAD_PATHNAME = 0xA1;
            internal const int ERROR_LOCK_FAILED = 0xA7;
            internal const int ERROR_BUSY = 0xAA;
            internal const int ERROR_ALREADY_EXISTS = 0xB7;
            internal const int ERROR_BAD_EXE_FORMAT = 0xC1;
            internal const int ERROR_ENVVAR_NOT_FOUND = 0xCB;
            internal const int ERROR_FILENAME_EXCED_RANGE = 0xCE;
            internal const int ERROR_EXE_MACHINE_TYPE_MISMATCH = 0xD8;
            internal const int ERROR_PIPE_BUSY = 0xE7;
            internal const int ERROR_NO_DATA = 0xE8;
            internal const int ERROR_PIPE_NOT_CONNECTED = 0xE9;
            internal const int ERROR_MORE_DATA = 0xEA;
            internal const int ERROR_NO_MORE_ITEMS = 0x103;
            internal const int ERROR_PARTIAL_COPY = 0x12B;
            internal const int ERROR_ARITHMETIC_OVERFLOW = 0x216;
            internal const int ERROR_PIPE_CONNECTED = 0x217;
            internal const int ERROR_PIPE_LISTENING = 0x218;
            internal const int ERROR_OPERATION_ABORTED = 0x3E3;
            internal const int ERROR_IO_PENDING = 0x3E5;
            internal const int ERROR_DLL_INIT_FAILED = 0x45A;
            internal const int ERROR_NOT_FOUND = 0x490;
            internal const int ERROR_BAD_IMPERSONATION_LEVEL = 0x542;
            internal const int ERROR_RESOURCE_LANG_NOT_FOUND = 0x717;
            internal const int ERROR_NO_TOKEN = 0x3f0;
            internal const int ERROR_NON_ACCOUNT_SID = 0x4E9;
            internal const int ERROR_INVALID_SID = 0x539;
            internal const int ERROR_TRUSTED_RELATIONSHIP_FAILURE = 0x6FD;
            internal const int EFail = unchecked((int)0x80004005);
        }

        #endregion
    }
}
