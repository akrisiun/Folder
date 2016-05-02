using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Folder.Native
{
    public static class NativeAutocomplete
    {
        public static void SetFileAutoComplete(this TextBox ctrl)
        {
            AutoCompleteFlags flags = AutoCompleteFlags.SHACF_FILESYS_ONLY;

            Int32 res = SHAutoComplete(ctrl.Handle, (UInt32)flags);
        }

        [DllImport("shlwapi.dll", SetLastError = true)]
        public static extern Int32 SHAutoComplete(
                IntPtr hwndEdit, UInt32 dwFlags);

        [Flags]
        public enum AutoCompleteFlags : uint
        {
            SHACF_DEFAULT = 0x00000000,         // Currently (SHACF_FILESYSTEM | SHACF_URLALL)
            SHACF_FILESYSTEM = 0x00000001,      // This includes the File System as well as the rest of the shell (Desktop\My Computer\Control Panel\)
            SHACF_URLALL = (SHACF_URLHISTORY | SHACF_URLMRU),
            SHACF_URLHISTORY = 0x00000002,      // URLs in the User's History
            SHACF_URLMRU = 0x00000004,          // URLs in the User's Recently Used list.
            SHACF_USETAB = 0x00000008,          // Use the tab to move thru the autocomplete possibilities instead of to the next dialog/window control.
            SHACF_FILESYS_ONLY = 0x00000010,    // This includes the File System
            SHACF_FILESYS_DIRS = 0x00000020,    // Same as SHACF_FILESYS_ONLY except it only includes directories, UNC servers, and UNC server shares.
            SHACF_AUTOSUGGEST_FORCE_ON = 0x10000000,    // Ignore the registry default and force the feature on.
            SHACF_AUTOSUGGEST_FORCE_OFF = 0x20000000,   // Ignore the registry default and force the feature off.
            SHACF_AUTOAPPEND_FORCE_ON = 0x40000000,     // Ignore the registry default and force the feature on. (Also know as AutoComplete)
            SHACF_AUTOAPPEND_FORCE_OFF = 0x80000000,    // Ignore the registry default and force the feature off. (Also know as AutoComplete)
        }

    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("EAC04BC0-3791-11D2-BB95-0060977B464C")]
    public interface IAutoComplete2 /*: IAutoComplete */
    {
        // Initializes the autocomplete object.
        [PreserveSig]
        Int32 Init(
            IntPtr hwndEdit,                    // Handle to the window for the system 
            // edit control that is to 
            // have autocompletion enabled. 
            [MarshalAs(UnmanagedType.IUnknown)]
        Object punkACL,                     // Pointer to the IUnknown interface 
            // of the string list object that 
            // is responsible for generating 
            // candidates for the completed 
            // string. The object must expose
            // an IEnumString interface. 
            [MarshalAs(UnmanagedType.LPWStr)]
        String pwszRegKeyPath,              // Pointer to an optional null-
            // terminated Unicode string that 
            // gives the registry path, 
            // including the value name, where 
            // the format string is stored as 
            // a REG_SZ value. The 
            // autocomplete object first 
            // looks for the path under 
            // HKEY_CURRENT_USER . If it fails,
            // it then tries 
            // HKEY_LOCAL_MACHINE. 
            // For a discussion of the format 
            // string, see the definition of 
            // pwszQuickComplete. 
            [MarshalAs(UnmanagedType.LPWStr)]
        String pwszQuickComplete);          // Pointer to an optional string
        // that specifies the format to be
        // used if the user enters some text
        // and presses CTRL+ENTER. Set
        // this parameter to NULL to 
        // disable quick completion. 
        // Otherwise, the autocomplete 
        // object treats pwszQuickComplete 
        // as a sprintf format string, 
        // and the text in the
        // edit box as its associated 
        // argument, to produce a new 
        // string.
        [PreserveSig]
        Int32 Enable(
            Int32 fEnable);                     // Value that is set to 
        // TRUE to enable autocompletion, 
        // or to FALSE to disable it. 

        // Sets the current autocomplete options.
        [PreserveSig]
        Int32 SetOptions(
            UInt32 dwFlag);             // Flags that allow an application to specify 
        // autocomplete options. 

        // Retrieves the current autocomplete options.
        [PreserveSig]
        Int32 GetOptions(
            out UInt32 pdwFlag);    // that indicate the options that are 
        // currently set. 

    }

}
