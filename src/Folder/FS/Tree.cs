using System;
using System.Windows.Controls;
using Folder.Native;
using Folder.FS;
using System.IO;
using Folder.Visual;

namespace Folder.FS
{
    public static class Tree
    {
        public static void Bind(this Folder.FolderWindow w)
        {
            w.txtPath = w.hostPath.Child as System.Windows.Forms.TextBox;
            w.txtPath.Text = FileSystem.CurrentDirectory;
            NativeAutocomplete.SetFileAutoComplete(w.txtPath);

            var treeView = w.tree;
            treeView.MouseMove += TreeViewDrag.treeView_MouseMove;
            treeView.MouseLeftButtonUp += TreeViewDrag.treeView_MouseDown;
            treeView.Selection.MouseButtonDown += TreeViewDrag.treeView_MouseRightClick;
            treeView.DragOver += TreeViewDrag.treeView_DragOver;
            treeView.Drop += TreeViewDrag.treeView_Drop;

            w.buttonProj.Click += (s, e) => GoUp(w, w.buttonProj);

            FolderTree.BindTree(w, w.tree);
        }

        public static void GoUp(this Folder.FolderWindow w, Button btn)
        {
            var dir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "..";
            var fullDir = Path.GetFullPath(dir);
            Directory.SetCurrentDirectory(fullDir);

            w.txtPath.Text = Directory.GetCurrentDirectory();
            LoadTree(w, w.txtPath.Text);
        }

        public static void LoadTree(this Folder.FolderWindow w, string dir)
        {
            var ext = FileSystem.SafeGetExtensionLower(dir);
            if (File.Exists(dir) && ext == ".pjx")
            {
                // project file
                var info = new VfpFileInfo();

                TextRead.ReadVfpInfo(ref info, ref dir, (f) =>
                {
                }
                );
                return;
            }
            else if (Directory.Exists(dir))
                FileSystem.CurrentDirectory = dir;

            string projName = FileSystem.CurrentDirectory;

            FolderTree.LoadDir(w, w.tree, projName);
        }

    }

}
