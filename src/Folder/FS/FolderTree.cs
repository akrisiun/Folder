using Folder.Visual;
using IOFile;
using MultiSelect;
using SharpShell.Helpers;
using SharpShell.Pidl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Folder.FS
{
    public static class FolderTree
    {
        public static void BindTree(Window w, MultiSelectTreeView tree)
        {
            // tree.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }

        /* static void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            var ItemContainerGenerator = sender as ItemContainerGenerator;
            if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return;

            var sally = ItemContainerGenerator.ContainerFromIndex(0); // .Items[0];
            // var sallyItem = ItemContainerGenerator.ContainerFromItem(sally) as MultiSelectTreeViewItem;
            // ReadOnlyCollection<object> Items
        } */

        public static void LoadDir(Window w, MultiSelectTreeView tree, string dir)
        {
            //SHSimpleIDListFromPath
            PidlData dataPidl = ShPidlSystem.FromPath(dir);

            ListData<DirectoryEnum.FileDataInfo> data = ReadDir(dir);
            if (!data.any)
                return;

            tree.Items.Clear();

            var imageItem = new IconItem { Name = dir, Path = dir, IsNodeExpanded = true };
            imageItem.Bind(dataPidl);
            tree.Items.Add(imageItem);

            LoadSubDirData(w, imageItem, data);
        }

        public static void LoadSubDir(Window w,
            MultiSelectTreeViewItem itemRoot, string dir)
        {
            IconItem item = itemRoot.DataContext as IconItem;
            if (item == null)
                return;

            ListData<DirectoryEnum.FileDataInfo> data = ReadDir(dir);
            if (!data.any)
                return;

            itemRoot.IsExpanded = true;
            item.Items.Clear();
            LoadSubDirData(w, item, data);
        }

        public static void LoadSubDir(Window w,
            IconItem itemRoot, string dir)
        {
            ListData<DirectoryEnum.FileDataInfo> data = ReadDir(dir);
            if (!data.any)
                return;

            LoadSubDirData(w, itemRoot, data);
        }

        static void LoadSubDirData(Window w,
           IconItem itemRoot,
            ListData<DirectoryEnum.FileDataInfo> data)
        {
            string dir = data.Dir;

            var numDir = data.numDir;
            do
            {
                var item = numDir.Current as DirectoryEnum.FileDataInfo;
                if (item == null)
                    break;

                string displayName = Path.GetFileName(item.cFileName)
                    + (item.HasAttribute(FATTR.FILE_ATTRIBUTE_DIRECTORY) ? @"\" : String.Empty);

                var subItem = new IconItem
                {
                    Path = Path.Combine(dir, item.cFileName),
                    Name = displayName,
                    IsNodeExpanded = false,
                    Parent = itemRoot
                };
                subItem.Items.Add(IconItem.Empty);
                subItem.Bind();

                itemRoot.Items.Add(subItem);
            }
            while (numDir.MoveNext());

            var num = data.numFiles;
            do
            {
                var item = num.Current as DirectoryEnum.FileDataInfo;
                if (item == null || IsIgnore(item.cFileName))
                    continue;

                string displayName = Path.GetFileName(item.cFileName);

                //var subItem = new MultiSelectTreeViewItem { Header = displayName };
                //subItem.DataContext = Path.Combine(dir, item.cFileName);
                var subItem = new IconItem
                {
                    Path = Path.Combine(dir, item.cFileName),
                    Name = displayName,
                    IsNodeExpanded = false,
                    Parent = itemRoot
                };
                subItem.Bind();

                itemRoot.Items.Add(subItem);

            } while (num.MoveNext());

        }

        static ListData<DirectoryEnum.FileDataInfo> ReadDir(string dir)
        {
            var data = new ListData<DirectoryEnum.FileDataInfo>();
            data.Dir = dir;
            if (!Directory.Exists(dir))
                throw new DirectoryNotFoundException(dir ?? "-");

            bool any = false;
            try
            {
                data.ListFiles = DirectoryEnum.ReadFilesInfo(dir, searchOption: SearchOption.TopDirectoryOnly);
                data.numFiles = data.ListFiles.GetEnumerator();
                any = data.numFiles.MoveNext();

                data.ListDir = DirectoryEnum.ReadDirectories(dir);
                data.numDir = data.ListDir.GetEnumerator();
                data.numDir.MoveNext();
                any = any || data.numDir.Current != null;
            }
            catch { }

            data.any = any;
            return data;
        }

        public static bool IsIgnore(string fileName)
        {
            if (fileName.StartsWith("."))
                return true;
            var ext = Path.GetExtension(fileName);
            if (ext.Length == 0)
                return false;

            ext = ext.ToLower();

            // TODO: git ignore
            if (// ext == ".exe" || 
                ext == ".dll" || ext == ".metagen"
                || ext == ".fxp" || ext == ".obj" || ext == ".bak"
                || ext == ".cdx" || ext == ".fpt"
                || ext == ".prt" || ext == ".sct" || ext == ".vct"
                || ext == ".bsc" || ext == ".lib" || ext == ".exp"
                || ext == ".sdf" || ext == ".suo"
                || ext == ".pdb" || fileName.Contains(".vshost."))
                return true;

            return false;
        }
    }

    public struct ListData<T>
    {
        public string Dir { get; set; }
        public IEnumerable<T> ListDir;
        public IEnumerable<T> ListFiles;

        // Numerators
        public IEnumerator numDir;
        public IEnumerator numFiles;

        public bool any; // any subdirectories or files are found
    }

}