using Folder.Native;
using IOFile;
using MultiSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VisualFoxpro;

namespace VfpProj
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
            treeView.DragOver += TreeViewDrag.treeView_DragOver;
            treeView.Drop += TreeViewDrag.treeView_Drop;

            w.buttonProj.Click += (s, e) => GoUp(w, w.buttonProj);
        }

        public static void GoUp(this Folder.FolderWindow w, Button btn)
        {
            var dir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "..";
            var fullDir = Path.GetFullPath(dir);
            Directory.SetCurrentDirectory(fullDir);

            w.txtPath.Text = Directory.GetCurrentDirectory();
            LoadFolder(w, w.txtPath.Text);
        }

        public static void LoadFolder(this Folder.FolderWindow w, string dir)
        {
            if (File.Exists(dir))
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

    public static class FolderTree
    {
        public static void LoadDir(Window w, MultiSelectTreeView tree, string dir)
        {
            IEnumerable<IOFile.DirectoryEnum.FileDataInfo> list = null;
            try
            {
                list = DirectoryEnum.ReadFilesInfo(dir, searchOption: SearchOption.TopDirectoryOnly);
                var num = list.GetEnumerator();
                if (num.MoveNext())
                { 
                    tree.Items.Clear();
                    do
                    {
                        var item = num.Current;
                        if (IsIgnore(item.cFileName))
                            continue;

                        string filePrg = Path.GetFileName(item.cFileName);
                        tree.Items.Add(filePrg);

                    } while (num.MoveNext());
                }
            }
            catch { }
        }


        public static bool IsIgnore(string fileName)
        {
            if (fileName.StartsWith("."))
                return true;
            var ext = Path.GetExtension(fileName);
            if (ext.Length == 0)
                return false;

            ext = ext.ToLower();
            if (ext == ".exe" || ext == ".dll" || ext == ".metagen"
                || ext == ".fxp" || ext == ".obj"
                || ext == ".cdx" || ext == ".fpt"
                || ext == ".prt" || ext == ".sct" || ext == ".vct"
                || ext == ".bsc" || ext == ".lib" || ext == ".exp"
                || ext == ".sdf" || ext == ".suo"
                || ext == ".pdb" || fileName.Contains(".vshost."))
                return true;

            return false;
        }
    }

    public static class ProjTree
    {
        public static void LoadPrj(Window w, TreeView tree, VisualFoxpro.IFoxProject proj, string file)
        {
            if (proj == null)
                return;

            string main = null;
            IList<ProjItem> list = null;
            try
            {
                main = proj.MainFile;

                list = GetList(proj);

                tree.Items.Clear();
                foreach (var item in list)
                {
                    if (!"P".Equals(item.Type))
                        continue;

                    string filePrg = Path.GetFileName(item.Name);
                    tree.Items.Add(filePrg);
                }
            }
            catch { }
        }

        static IList<ProjItem> GetList(IFoxProject proj)
        {
            var list = new List<ProjItem>();

            IFoxPrjFiles files = proj.Files;
            IEnumerator filesNum = files.GetEnumerator();
            while (filesNum.MoveNext())
            {
                dynamic elem = filesNum.Current;

                var item = new ProjItem { Name = elem.Name, Exclude = elem.Exclude, Type = elem.Type };
                list.Add(item);
            }

            return list;
        }
    }
}
