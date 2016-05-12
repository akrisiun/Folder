using Folder.FS;
using MultiSelect;
using System;
using System.IO;
using System.Windows;

namespace Folder.Visual
{
    class TreeNode
    {
        public static void OnExpand(Window w, MultiSelectTreeViewItem item)
        {
            item.IsExpanded = true;
            var items = item.Items;
            var node = item.DataContext as IconItem;

            if (node != null && Directory.Exists(node.Path))
            {
                (w as FolderWindow).txtFind.Text = node.Path;
                FolderTree.LoadSubDir(w, item, node.Path);
            }
        } 
        
    }
}
