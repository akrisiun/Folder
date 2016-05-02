using IOFile;
using MultiSelect;
using SharpShell.Helpers;
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
            PidlData dataPidl = NativePidl.PIDListFromPath(dir);

            IEnumerable<IOFile.DirectoryEnum.FileDataInfo> listDir = null;
            IEnumerable<IOFile.DirectoryEnum.FileDataInfo> list = null;
            bool any = false;
            IEnumerator num = null;
            IEnumerator numDir = null;
            try
            {
                list = DirectoryEnum.ReadFilesInfo(dir, searchOption: SearchOption.TopDirectoryOnly);
                num = list.GetEnumerator();
                any = num.MoveNext();

                listDir = DirectoryEnum.ReadDirectories(dir);
                numDir = listDir.GetEnumerator();
                numDir.MoveNext();
                any = any || numDir.Current != null;
            }
            catch { }
            if (!any)
                return;

            tree.Items.Clear();
            var itemRoot = new MultiSelectTreeViewItem { Header = dir };
            tree.Items.Add(itemRoot);
            itemRoot.IsExpanded = true;

            while (numDir.Current != null)
            {
                var item = numDir.Current as DirectoryEnum.FileDataInfo;

                string displayName = Path.GetFileName(item.cFileName)
                    + (item.HasAttribute(FATTR.FILE_ATTRIBUTE_DIRECTORY) ? @"\" : String.Empty);

                var subItem = new MultiSelectTreeViewItem { Header = displayName };
                itemRoot.Items.Add(subItem);

                if (!numDir.MoveNext())
                    break;
            }

            do
            {
                var item = num.Current as DirectoryEnum.FileDataInfo;
                if (IsIgnore(item.cFileName))
                    continue;

                string displayName = Path.GetFileName(item.cFileName);
                var subItem = new MultiSelectTreeViewItem { Header = displayName };
                itemRoot.Items.Add(subItem);

            } while (num.MoveNext());

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