using MultiSelect;
using SharpShell.Helpers;
using SharpShell.Interop;
using SharpShell.Pidl;
using SharpShell.SharpContextMenu;
using Shell.SharpContextMenu;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Folder.FS
{
    public class TreeContext
    {
        public static void RighClick(string path, MultiSelectTreeView tree)
        {
            var itemHit = ShPidlSystem.ItemFromPath(path);
            var ctrl = ShPidlSystem.GetControlWindow();
            Point pt = Mouse.GetPosition(tree);

            IShellFolder shellFolder = ShPidlSystem.ShellFolderInterface(itemHit);

            if (ShPidlSystem.ParseDesktopContext(itemHit, shellFolder, ctrl) != IntPtr.Zero)
            {
                // TODO desktop ContextMenu
                return;
            }

            string directory = Path.GetDirectoryName(path);
            Directory.SetCurrentDirectory(directory);

            var menu = new ShellContextMenu(itemHit);

            ShPidlSystem.ShowMenu(menu, shellFolder, itemHit, ctrl, new System.Drawing.Point((int)pt.X, (int)pt.Y));
        }
    }
}
