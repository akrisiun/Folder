using SharpShell.Helpers;
using SharpShell.Interop;
using SharpShell.Pidl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Folder.FS
{
    class TreeContext
    {
        public static void RighClick(string path, IntPtr Handle,
            ShellItem itemHit, int x, int y)
        {
            var shellFolder = itemHit.IsFolder ? itemHit.ShellFolderInterface : itemHit.ParentItem.ShellFolderInterface;

            //  The item pidl is either the folder if the item is a folder, or the combined pidl otherwise.
            var fullIdList = itemHit.IsFolder
                ? PidlManager.PidlToIdlist(itemHit.PIDL)
                : PidlManager.Combine(PidlManager.PidlToIdlist(itemHit.ParentItem.PIDL),
                    PidlManager.PidlToIdlist(itemHit.RelativePIDL));


            //  Get the UI object of the context menu.
            IntPtr apidl = PidlManager.PidlsToAPidl(new IntPtr[] { PidlManager.IdListToPidl(fullIdList) });

            IntPtr ppv = IntPtr.Zero;
            shellFolder.GetUIObjectOf(Handle, 1,
                // (IntPtr[])(object)
                apidl,
                Shell32.IID_IContextMenu, 0,
                out ppv);

            //  If we have an item, cast it.
            if (ppv == IntPtr.Zero)
            {
                //var menu = new SharpShell.SharpContextMenu.ShellContextMenu(itemHit);

                //var pos = new Point(node.Bounds.Left, node.Bounds.Top);
                //menu.ShowContextMenu(tree, pos);
            }
        }
    }
}
