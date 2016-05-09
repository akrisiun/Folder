using Folder.FS;
using MultiSelect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Folder.Visual
{
    public static class TreeViewDrag
    {
        static bool _isDragging;
        public static void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Released)
            { _isDragging = false; }

            MultiSelectTreeView treeView = sender as MultiSelectTreeView;
            if (treeView == null)
            {
                return;
            }

            if (!_isDragging && e.LeftButton == MouseButtonState.Pressed
                && treeView.SelectedItems.Count > 0)
            {
                if (!treeView.AllowDragDropState)
                    return;

                var SelectedItems = treeView.SelectedItems;
                _isDragging = true;
                if (Mouse.Captured != treeView)
                    Mouse.Capture(treeView, CaptureMode.Element);

                string[] paths = new string[SelectedItems.Count];
                for (int i = 0;
                    i < SelectedItems.Count && SelectedItems[i] as IconItem != null; i++)
                {
                    paths[i] = Path.GetFullPath((SelectedItems[i] as IconItem).Path);
                }

                if (paths[0] != null)
                {
                    DragDrop.DoDragDrop(treeView, new DataObject(DataFormats.FileDrop, paths),
                        DragDropEffects.Link);
                }
            }
        }

        public static void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Released)
            {
                _isDragging = false;

                // ReleaseMouseCapture 
                if (Mouse.Captured != null)
                    Mouse.Capture(null, CaptureMode.None);

                e.Handled = true;
            }
        }

        public static void treeView_MouseRightClick(object sender, MouseEventArgs e)
        {
            var treeItem = sender as MultiSelectTreeViewItem;
            var treeView = treeItem.ParentTreeView as MultiSelectTreeView;
            if (treeView == null || e.RightButton != MouseButtonState.Pressed)
                return;

            var item = treeItem.DataContext as IconItem;
            var fullPath = item.Path;
            if (fullPath == null)
                return;

            TreeContext.RighClick(fullPath, treeView);
        }

        public static void treeView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)
                || e.Data.GetDataPresent(typeof(Task)))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        public static void treeView_Drop(object sender, DragEventArgs e)
        {
            if (Mouse.Captured != null)
                Mouse.Capture(null, CaptureMode.None);

            var treeView = sender as MultiSelectTreeView;
            if (treeView == null)
                return;

            var obj = e.Data as System.Windows.DataObject;
            var formats = e.Data.GetFormats();
            if (obj.GetDataPresent(DataFormats.FileDrop, true))
            {
                var list = obj.GetData(DataFormats.FileDrop, true) as string[];
                if (list == null || list.Length == 0)
                    return;
                //  File drop
                string firstFile = list[0];
                e.Handled = true;
            }
        }

        // Do a VisualTreeHelper.HitTest(reference,location) call, a visual gets returned that represents the control you clicked on. 
        // You can cast it to something a bit more meaninful and extract the DataContext from there.
        public static T GetItemAtLocation<T>(TreeView treeView, Point location)
        {
            T foundItem = default(T);
            HitTestResult hitTestResults = VisualTreeHelper.HitTest(treeView, location);

            if (hitTestResults.VisualHit is FrameworkElement)
            {
                object dataObject = (hitTestResults.VisualHit as
                    FrameworkElement).DataContext;

                if (dataObject is T)
                {
                    foundItem = (T)dataObject;
                }
            }

            return foundItem;
        }
    }

    public class MouseUtilities
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Win32Point pt);
        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hwnd, ref Win32Point pt);

        public static Point GetMousePosition()
        {
            Win32Point mouse = new Win32Point();
            GetCursorPos(ref mouse);
            return new Point((double)mouse.X, (double)mouse.Y);
        }

        /// <summary>
        /// Returns the mouse cursor location.  This method is necessary during 
        /// a drag-drop operation because the WPF mechanisms for retrieving the
        /// cursor coordinates are unreliable.
        /// </summary>
        /// <param name="relativeTo">The Visual to which the mouse coordinates will be relative.</param>
        public static Point GetMousePosition(System.Windows.Media.Visual relativeTo)
        {
            var mousePoint = GetMousePosition();

            return relativeTo.PointFromScreen(mousePoint);

            #region Commented Out
            //System.Windows.Interop.HwndSource presentationSource =
            //    (System.Windows.Interop.HwndSource)PresentationSource.FromVisual( relativeTo );
            //ScreenToClient( presentationSource.Handle, ref mouse );
            //GeneralTransform transform = relativeTo.TransformToAncestor( presentationSource.RootVisual );
            //Point offset = transform.Transform( new Point( 0, 0 ) );
            //return new Point( mouse.X - offset.X, mouse.Y - offset.Y );
            #endregion // Commented Out
        }
    }
}
