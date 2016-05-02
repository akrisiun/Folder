using Folder;
using MultiSelect;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Forms = System.Windows.Forms;

namespace VfpProj
{
    public static class TextDrop
    {
        public static void Bind(this FolderWindow w, Forms.TextBox txtFile)
        {
            txtFile.Tag = w;
            txtFile.PreviewKeyDown += txtFile_PreviewKeyDown;
        }

        static void txtFile_PreviewKeyDown(object sender, Forms.PreviewKeyDownEventArgs e)
        {
            var txt = sender as Forms.TextBox;
            if (e.KeyCode == Forms.Keys.Enter && txt != null)
            {
                Tree.LoadFolder(txt.Tag as FolderWindow, txt.Text);
            }
        }

        public static DragDropEffects DoDrag(this DependencyObject dragSource, string filePath)
        {
            string[] paths = new[] { filePath };
            var ret = DragDrop.DoDragDrop(dragSource, new DataObject(DataFormats.FileDrop, paths),
                      DragDropEffects.Copy);
            return ret;
        }

        static void DragOver(object sender, DragEventArgs args)
        {
            args.Effects = DragDropEffects.Copy; // IsSingleFile(args) != null ?  : DragDropEffects.None;

            // Mark the event as handled, so TextBox's native DragOver handler is not called.
            args.Handled = true;
        }

        static void Drop(object sender, DragEventArgs args)
        {
            args.Handled = true;

            var fileName = IsSingleFile(args);
            if (fileName == null) return;

            //var fileNames = args.Data.GetData(DataFormats.FileDrop, true) as string[];
            //// Check fo a single file or folder.
            //if (fileNames.Length >= 1)

            var fileToLoad = new StreamReader(fileName);

            //EditWindow w = (sender as FrameworkElement).Tag as EditWindow;
            //if (w == null)
            //    return;

            //w.txtPath.Text = fileName;
            //try
            //{
            //    w.editor.Text = fileToLoad.ReadToEnd();
            //    fileToLoad.Close();
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // If the data object in args is a single file, this method will return the filename. Otherwise, it returns null.
        static string IsSingleFile(DragEventArgs args)
        {
            // Check for files in the hovering data object.
            if (!args.Data.GetDataPresent(DataFormats.FileDrop, true))
                return null;

            var fileNames = args.Data.GetData(DataFormats.FileDrop, true) as string[];
            // Check fo a single file or folder.
            if (fileNames.Length >= 1 && File.Exists(fileNames[0]))
            {
                // At this point we know there single or more files. returns first
                return fileNames[0];
            }
            return null;
        }

    }

    public static class TreeViewDrag
    {
        static bool _isDragging;
        public static void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            MultiSelectTreeView treeView = sender as MultiSelectTreeView;
            if (_isDragging && e.LeftButton == MouseButtonState.Released)
                _isDragging = false;

            if (treeView != null
                && !_isDragging && e.LeftButton == MouseButtonState.Pressed
                && treeView.SelectedItems.Count > 0)
            {
                var SelectedItems = treeView.SelectedItems;
                _isDragging = true;
                if (Mouse.Captured != treeView)
                    Mouse.Capture(treeView, CaptureMode.Element);

                //string[] paths = new string[] { Path.GetFullPath(treeView.SelectedValue as string) };
                string[] paths = new string[SelectedItems.Count];
                for(int i = 0; i < SelectedItems.Count; i++)
                    paths[i] = Path.GetFullPath(SelectedItems[i] as string);

                DragDrop.DoDragDrop(treeView, new DataObject(DataFormats.FileDrop, paths),
                    DragDropEffects.Link);
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

        public static void treeView_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Task)))
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
            if (e.Data.GetDataPresent(typeof(Task)))
            {
                Task sourceTask = (Task)e.Data.GetData(typeof(Task));
                //Task<TreeViewItem> targetTask = GetItemAtLocation<Task<TreeViewItem>>(treeView, MouseUtilities.GetMousePosition());

                //// Code to move the item in the model is placed here...
                //targetTask.Wait();
                //if (targetTask.IsCompleted)
                //{
                //    e.Handled = true;
                //}

                return;
            }

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
