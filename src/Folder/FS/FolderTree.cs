using IOFile;
using MultiSelect;
using SharpShell.Helpers;
using SharpShell.Pidl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Folder.FS
{
    public static class FolderTree
    {
        public static void LoadDir(Window w, MultiSelectTreeView tree, string dir)
        {
            //SHSimpleIDListFromPath
            PidlData dataPidl = ShPidlSystem.FromPath(dir);

            ListData<DirectoryEnum.FileDataInfo> data = ReadDir(dir);
            if (!data.any)
                return;

            tree.Items.Clear();
            var itemRoot = new MultiSelectTreeViewItem { Header = dir };
            itemRoot.DataContext = dir;
            tree.Items.Add(itemRoot);
            itemRoot.IsExpanded = true;

            var numDir = data.numDir;
            while (numDir.Current != null)
            {
                var item = numDir.Current as DirectoryEnum.FileDataInfo;

                string displayName = Path.GetFileName(item.cFileName)
                    + (item.HasAttribute(FATTR.FILE_ATTRIBUTE_DIRECTORY) ? @"\" : String.Empty);

                var subItem = new MultiSelectTreeViewItem { Header = displayName };
                subItem.DataContext = Path.Combine(dir, item.cFileName);
                itemRoot.Items.Add(subItem);

                subItem.Items.Add(String.Empty);
                subItem.IsExpanded = false;

                if (!numDir.MoveNext())
                    break;
            }

            var num = data.num;
            do
            {
                var item = num.Current as DirectoryEnum.FileDataInfo;
                if (IsIgnore(item.cFileName))
                    continue;

                string displayName = Path.GetFileName(item.cFileName);
                
                var subItem = new MultiSelectTreeViewItem { Header = displayName };
                subItem.DataContext = Path.Combine(dir, item.cFileName);
                itemRoot.Items.Add(subItem);

            } while (num.MoveNext());

        }

        struct ListData<T>
        {
            public IEnumerable<T> ListDir;
            public IEnumerable<T> List;

            public IEnumerator num;
            public IEnumerator numDir;
            public bool any;
        }

        static ListData<DirectoryEnum.FileDataInfo> ReadDir(string dir)
        {
            var data = new ListData<DirectoryEnum.FileDataInfo>();
            bool any = false;
            try
            {
                data.List = DirectoryEnum.ReadFilesInfo(dir, searchOption: SearchOption.TopDirectoryOnly);
                data.num = data.List.GetEnumerator();
                any = data.num.MoveNext();

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

}