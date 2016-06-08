using System;
using System.Windows.Controls;
using Folder.Native;
using Folder.FS;
using System.IO;
using Folder.Visual;
using MultiSelect;
using Forms = System.Windows.Forms;

namespace Folder.FS
{
    public static class Tree
    {
        public static void Bind(this IFolderWindow w, Forms.TextBox Child)
        {
            w.txtPath = Child; // w.hostPath.Child as System.Windows.Forms.TextBox;
            w.txtPath.Text = FileSystem.CurrentDirectory;
            NativeAutocomplete.SetFileAutoComplete(w.txtPath);

            var treeView = w.tree;
            treeView.MouseMove += TreeViewDrag.treeView_MouseMove;
            treeView.MouseLeftButtonUp += TreeViewDrag.treeView_MouseDown;
            treeView.Selection.MouseButtonDown += TreeViewDrag.treeView_MouseRightClick;
            treeView.DragOver += TreeViewDrag.treeView_DragOver;
            treeView.Drop += TreeViewDrag.treeView_Drop;
            treeView.PreviewSelectionChanged += treeView_PreviewSelectionChanged;

            w.buttonProj.Click += (s, e) => GoUp(w, w.buttonProj);

            FolderTree.BindTree(w, w.tree);
        }

        static void treeView_PreviewSelectionChanged(object sender, PreviewSelectionChangedEventArgs e)
        {
            var tree = sender as MultiSelectTreeView;
            var w = CsApp.FolderWindow;

            var select = e.Item as IconItem;
            if (w != null && select != null && select.Path != null)
                w.txtFind.Text = select.Path;
        }

        public static void GoUp(this IFolderWindow w, Button btn)
        {
            var dir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "..";
            var fullDir = Path.GetFullPath(dir);
            Directory.SetCurrentDirectory(fullDir);

            w.txtPath.Text = Directory.GetCurrentDirectory();
            LoadTree(w, w.txtPath.Text);
        }

        public static void LoadTree(this IFolderWindow w, string dir)
        {
            var ext = FileSystem.SafeGetExtensionLower(dir);
            var directory = Path.GetDirectoryName(dir);

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
            else if (Directory.Exists(directory))
                FileSystem.CurrentDirectory = directory;

            if (File.Exists(dir) && ext == ".sln")
            { 
                // TODO
            }

            string projName = FileSystem.CurrentDirectory;

            FolderTree.LoadDir(w, w.tree, projName);
        }

    }

}
