using Folder.FS;
using MultiSelect;
using System;
using System.Windows; 

namespace Folder.Visual
{
    class TreeNode
    {
        public static void OnExpand(Window w, MultiSelectTreeViewItem item)
        {
            item.IsExpanded = true;
            var items = item.Items;
            string fullPath = item.DataContext as string;

            if (fullPath != null)
                FolderTree.LoadSubDir(w, item, fullPath);
        } 
        
    }
}
