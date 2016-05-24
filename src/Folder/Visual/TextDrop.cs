using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Forms = System.Windows.Forms;

using Folder.FS;
using MultiSelect;

namespace Folder
{
    public static class TextDrop
    {
        public static void Bind(this IFolderWindow w, Forms.TextBox txtFile)
        {
            txtFile.Tag = w;
            txtFile.PreviewKeyDown += txtFile_PreviewKeyDown;
        }

        static void txtFile_PreviewKeyDown(object sender, Forms.PreviewKeyDownEventArgs e)
        {
            var txt = sender as Forms.TextBox;
            if (e.KeyCode == Forms.Keys.Enter && txt != null)
            {
                Tree.LoadTree(txt.Tag as IFolderWindow, txt.Text);
            }
        }

        public static DragDropEffects DoDrag(this DependencyObject dragSource, string filePath)
        {
            string[] paths = new[] { filePath };
            var ret = DragDrop.DoDragDrop(dragSource, new DataObject(DataFormats.FileDrop, paths),
                      DragDropEffects.Copy);
            return ret;
        }

        //static void DragOver(object sender, DragEventArgs args)
        //{
        //    args.Effects = DragDropEffects.Copy;

        //    // Mark the event as handled, so TextBox's native DragOver handler is not called.
        //    args.Handled = true;
        //}

        //static void Drop(object sender, DragEventArgs args)
        //{
        //    args.Handled = true;

        //    var fileName = IsSingleFile(args);
        //    if (fileName == null) return;


        //    // var fileToLoad = new StreamReader(fileName); 
        //}

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

}