using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Folder.Native
{
    public class NativeMethods
    {
        public delegate int EnumWindowsProc(IntPtr hwnd, int lParam);

        [DllImport("user32")]
        public extern static int EnumWindows(
            EnumWindowsProc lpEnumFunc, int lParam);

        [DllImport("user32")]
        public extern static int EnumChildWindows(
            IntPtr hWndParent, EnumWindowsProc lpEnumFunc, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        static private int WindowEnum(IntPtr hWnd, int lParam)
        {
            if (OnWindowEnum(hWnd))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        static bool OnWindowEnum(IntPtr hWnd)
        {
            items.Add(hWnd);
            return true;
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        public static string GetClassName(IntPtr hWnd)
        {
            System.Text.StringBuilder ClassName = new System.Text.StringBuilder(256);
            int nRet = GetClassName(hWnd, ClassName, ClassName.Capacity);
            if (nRet != 0)
                return ClassName.ToString();

            return string.Empty;
        }

        public static string GetWindowText(IntPtr hWnd)
        {
            System.Text.StringBuilder WindowName = new System.Text.StringBuilder(256);
            int nRet = GetWindowText(hWnd, WindowName, WindowName.Capacity);
            if (nRet != 0)
                return WindowName.ToString();

            return string.Empty;
        }

        static System.Collections.ObjectModel.Collection<IntPtr> items;

        public static System.Collections.ObjectModel.Collection<IntPtr> GetWindows(IntPtr handle)
        {
            items = new System.Collections.ObjectModel.Collection<IntPtr>();
            if (handle != IntPtr.Zero)
                EnumChildWindows(handle, new EnumWindowsProc(WindowEnum), 0);
            return items;
        }

    }

    // http://www.zorched.net/2009/01/01/register-and-unregister-com-dll-from-net-code/
    public class Registrar : IDisposable
    {
        #region Init

        private IntPtr hLib;

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        internal delegate int PointerToMethodInvoker();

        public Registrar(string filePath)
        {
            hLib = LoadLibrary(filePath);
            if (IntPtr.Zero == hLib)
            {
                int errno = Marshal.GetLastWin32Error();
                throw new Win32Exception(errno, "Failed to load library.");
            }
        }

        #endregion

        public void RegisterComDLL()
        {
            CallPointerMethod("DllRegisterServer");
        }

        public void UnRegisterComDLL()
        {
            CallPointerMethod("DllUnregisterServer");
        }

        private void CallPointerMethod(string methodName)
        {
            IntPtr dllEntryPoint = GetProcAddress(hLib, methodName);
            if (IntPtr.Zero == dllEntryPoint)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            PointerToMethodInvoker drs =
            (PointerToMethodInvoker)Marshal.GetDelegateForFunctionPointer(dllEntryPoint,
            typeof(PointerToMethodInvoker));
            drs();
        }

        public void Dispose()
        {
            if (IntPtr.Zero != hLib)
            {
                UnRegisterComDLL();
                FreeLibrary(hLib);
                hLib = IntPtr.Zero;
            }
        }
    }

}
