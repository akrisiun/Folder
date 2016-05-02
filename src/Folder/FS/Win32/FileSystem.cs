using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using IO = System.IO;

namespace IOFile
{
    // https://github.com/dotnet/corefx/blob/master/src/System.IO.FileSystem/src/System/IO/FileSystem.Current.MuxWin32WinRT.cs

    // internal 
    public abstract partial class FileSystem
    {
        private static readonly FileSystem s_current = new MultiplexingWin32WinRTFileSystem();
        public static FileSystem Current { get { return s_current; } }

        // Directory
        public abstract void CreateDirectory(string fullPath);
        public abstract bool DirectoryExists(string fullPath);
        // public abstract void MoveDirectory(string sourceFullPath, string destFullPath);
        // public abstract void RemoveDirectory(string fullPath, bool recursive);

        // File
        public abstract void CopyFile(string sourceFullPath, string destFullPath, bool overwrite);
        public abstract void DeleteFile(string fullPath);
        public abstract bool FileExists(string fullPath);
        // public abstract FileStreamBase Open(string fullPath, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, FileStream parent);
        public abstract void MoveFile(string sourceFullPath, string destFullPath);

        public abstract IO.FileAttributes GetAttributes(string fullPath);
        public abstract void SetAttributes(string fullPath, IO.FileAttributes attributes);
    }

    internal interface IFileSystemObject
    {
        FileAttributes Attributes { get; set; }
        DateTimeOffset CreationTime { get; set; }
        bool Exists { get; }
        DateTimeOffset LastAccessTime { get; set; }
        DateTimeOffset LastWriteTime { get; set; }
        long Length { get; }

        void Refresh();
    }

    internal class MultiplexingWin32WinRTFileSystem : FileSystem
    {
        // private readonly FileSystem _win32FileSystem = new Win32FileSystem();
        //public override int MaxPath { get { return Interop.mincore.MAX_PATH; } }
        //public override int MaxDirectoryPath { get { return Interop.mincore.MAX_DIRECTORY_PATH; } }

        public override void CopyFile(string sourceFullPath, string destFullPath, bool overwrite)
        {
            IO.File.Copy(sourceFullPath, destFullPath, overwrite);
        }

        public override void CreateDirectory(string fullPath)
        {
            IO.Directory.CreateDirectory(fullPath);
        }

        public override void DeleteFile(string fullPath)
        {
            IO.File.Delete(fullPath);
        }

        public override bool DirectoryExists(string fullPath)
        {
            return IO.Directory.Exists(fullPath);
        }

        public override bool FileExists(string fullPath) { return IO.File.Exists(fullPath); }
        // public abstract FileStreamBase Open(string fullPath, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, FileStream parent);
        public override void MoveFile(string sourceFullPath, string destFullPath) { IO.File.Move(sourceFullPath, destFullPath); }

        public override IO.FileAttributes GetAttributes(string fullPath)
        { return IO.File.GetAttributes(fullPath); }
        public override void SetAttributes(string fullPath, IO.FileAttributes attributes)
        { IO.File.SetAttributes(fullPath, attributes); }
    }

    // Provides attributes for files and directories.
    [ComVisible(true)]
    [Flags]
    public enum FileAttributes
    {
        //
        // Summary:
        //     The file is read-only.
        ReadOnly = 1,
        //
        // Summary:
        //     The file is hidden, and thus is not included in an ordinary directory listing.
        Hidden = 2,
        //
        // Summary:
        //     The file is a system file. That is, the file is part of the operating system
        //     or is used exclusively by the operating system.
        System = 4,
        //
        // Summary:
        //     The file is a directory.
        Directory = 16,
        //
        // Summary:
        //     The file is a candidate for backup or removal.
        Archive = 32,
        //
        // Summary:
        //     Reserved for future use.
        Device = 64,
        //
        // Summary:
        //     The file is a standard file that has no special attributes. This attribute is
        //     valid only if it is used alone.
        Normal = 128,
        //
        // Summary:
        //     The file is temporary. A temporary file contains data that is needed while an
        //     application is executing but is not needed after the application is finished.
        //     File systems try to keep all the data in memory for quicker access rather than
        //     flushing the data back to mass storage. A temporary file should be deleted by
        //     the application as soon as it is no longer needed.
        Temporary = 256,
        //
        // Summary:
        //     The file is a sparse file. Sparse files are typically large files whose data
        //     consists of mostly zeros.
        SparseFile = 512,
        //
        // Summary:
        //     The file contains a reparse point, which is a block of user-defined data associated
        //     with a file or a directory.
        ReparsePoint = 1024,
        //
        // Summary:
        //     The file is compressed.
        Compressed = 2048,
        //
        // Summary:
        //     The file is offline. The data of the file is not immediately available.
        Offline = 4096,
        //
        // Summary:
        //     The file will not be indexed by the operating system's content indexing service.
        NotContentIndexed = 8192,
        //
        // Summary:
        //     The file or directory is encrypted. For a file, this means that all data in the
        //     file is encrypted. For a directory, this means that encryption is the default
        //     for newly created files and directories.
        Encrypted = 16384,
        //
        // Summary:
        //     The file or directory includes data integrity support. When this value is applied
        //     to a file, all data streams in the file have integrity support. When this value
        //     is applied to a directory, all new files and subdirectories within that directory,
        //     by default, include integrity support.
        IntegrityStream = 32768,
        //
        // Summary:
        //     The file or directory is excluded from the data integrity scan. When this value
        //     is applied to a directory, by default, all new files and subdirectories within
        //     that directory are excluded from data integrity.
        NoScrubData = 131072
    }

}

