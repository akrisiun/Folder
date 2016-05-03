using MultiSelect;
using SharpShell.Helpers;
using SharpShell.Interop;
using SharpShell.Pidl;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Folder.FS
{
    public class TreeContext
    {
        public static void RighClick(string path, MultiSelectTreeView tree)
        {
            PidlData dataPidl = ShPidlSystem.FromPath(path);

            var pidl = dataPidl.Handle.pidl;
            var ctrl = new ControlWindow();
            var hWnd = ctrl.Handle;
            var itemHit = new ShellItem(dataPidl);

            Point pt = Mouse.GetPosition(tree);
            RighClick(path, ctrl, itemHit, (int)pt.X, (int)pt.Y);
        }

        public static void RighClick(string path, System.Windows.Forms.Control ctrl, // IntPtr Handle
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
            shellFolder.GetUIObjectOf(ctrl.Handle, 1,
                // (IntPtr[])(object)
                apidl,
                Shell32.IID_IContextMenu, 0,
                out ppv);

            //  If we have an item, cast it.
            if (ppv == IntPtr.Zero)
            {
                var menu = new SharpShell.SharpContextMenu.ShellContextMenu(itemHit);

                var pos = new System.Drawing.Point(x, y); //node.Bounds.Left, node.Bounds.Top);
                menu.ShowContextMenu(ctrl, pos);
            }
        }

        class ControlWindow : System.Windows.Forms.Control
        {
            public ControlWindow() // ShellContextMenu parent)
            {
                //m_Parent = parent;
            }

            protected override void WndProc(ref System.Windows.Forms.Message m)
            {
                //if (!m_Parent.HandleMenuMessage(ref m))
                {
                    base.WndProc(ref m);
                }
            }

            //ShellContextMenu m_Parent;
        }
    }
}
