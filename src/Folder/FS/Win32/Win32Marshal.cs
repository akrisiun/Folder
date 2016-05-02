using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Errors = IOFile.Win32FindFile.Errors;

namespace IOFile
{
    internal static class Win32Marshal
    {
        /// <summary>
        ///     Converts, resetting it, the last Win32 error into a corresponding <see cref="Exception"/> object.
        /// </summary>
        internal static Exception GetExceptionForLastWin32Error()
        {
            int errorCode = Marshal.GetLastWin32Error();
            return GetExceptionForWin32Error(errorCode, String.Empty);
        }

        /// <summary>
        ///     Converts, resetting it, the last Win32 error into a corresponding <see cref="Exception"/> object, optionally 
        ///     including the specified path in the error message.
        /// </summary>
        internal static Exception GetExceptionForLastWin32Error(string path)
        {
            int errorCode = Marshal.GetLastWin32Error();
            return GetExceptionForWin32Error(errorCode, path);
        }

        /// <summary>
        ///     Converts the specified Win32 error into a corresponding <see cref="Exception"/> object.
        /// </summary>
        internal static Exception GetExceptionForWin32Error(int errorCode)
        {
            return GetExceptionForWin32Error(errorCode, string.Empty);
        }

        /// <summary>
        ///     Converts the specified Win32 error into a corresponding <see cref="Exception"/> object, optionally 
        ///     including the specified path in the error message.
        /// </summary>
        internal static Exception GetExceptionForWin32Error(int errorCode, string path)
        {
            switch (errorCode)
            {
                case Errors.ERROR_FILE_NOT_FOUND:
                    if (path.Length == 0)
                        return new FileNotFoundException("FileNotFound");
                    else
                        return new FileNotFoundException(String.Format("FileNotFound %1", path), path);

                case Errors.ERROR_PATH_NOT_FOUND:
                    if (path.Length == 0)
                        return new DirectoryNotFoundException("PathNotFound_NoPathName");
                    else
                        return new DirectoryNotFoundException(String.Format("PathNotFound %1", path));

                case Errors.ERROR_ACCESS_DENIED:
                    if (path.Length == 0)
                        return new UnauthorizedAccessException("UnauthorizedAccess_IODenied_NoPathName");
                    else
                        return new UnauthorizedAccessException(String.Format("UnauthorizedAccess_IODenied %1", path));

                case Errors.ERROR_ALREADY_EXISTS:
                    if (path.Length == 0)
                        goto default;

                    return new IOException(String.Format("AlreadyExists %1", path), MakeHRFromErrorCode(errorCode));

                case Errors.ERROR_FILENAME_EXCED_RANGE:
                    return new PathTooLongException("PathTooLong");

                case Errors.ERROR_INVALID_PARAMETER:
                    return new IOException(GetMessage(errorCode), MakeHRFromErrorCode(errorCode));

                case Errors.ERROR_SHARING_VIOLATION:
                    if (path.Length == 0)
                        return new IOException("SharingViolation_NoFileName", MakeHRFromErrorCode(errorCode));
                    else
                        return new IOException(String.Format("SharingViolation %1", path), MakeHRFromErrorCode(errorCode));

                case Errors.ERROR_FILE_EXISTS:
                    if (path.Length == 0)
                        goto default;

                    return new IOException(String.Format("FileExists %1", path), MakeHRFromErrorCode(errorCode));

                case Errors.ERROR_OPERATION_ABORTED:
                    return new OperationCanceledException();

                default:
                    return new IOException(GetMessage(errorCode), MakeHRFromErrorCode(errorCode));
            }
        }

        /// <summary>
        ///     Returns a HRESULT for the specified Win32 error code.
        /// </summary>
        internal static int MakeHRFromErrorCode(int errorCode)
        {
            Contract.Assert((0xFFFF0000 & errorCode) == 0, "This is an HRESULT, not an error code!");

            return unchecked(((int)0x80070000) | errorCode);
        }

        /// <summary>
        ///     Returns a Win32 error code for the specified HRESULT if it came from FACILITY_WIN32
        ///     If not, returns the HRESULT unchanged
        /// </summary>
        internal static int TryMakeWin32ErrorCodeFromHR(int hr)
        {
            if ((0xFFFF0000 & hr) == 0x80070000)
            {
                // Win32 error, Win32Marshal.GetExceptionForWin32Error expects the Win32 format
                hr &= 0x0000FFFF;
            }

            return hr;
        }

        ///// <summary>
        /////     Returns a string message for the specified Win32 error code.
        ///// </summary>
        internal static string GetMessage(int errorCode)
        {
            //  return Interop.mincore.GetMessage(errorCode);
            return "Erorr code " + errorCode.ToString();
        }

    }

}
