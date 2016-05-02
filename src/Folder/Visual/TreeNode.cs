using MultiSelect;
using System;
using System.Windows; 

namespace Folder.Visual
{
    class TreeNode
    {
        public static void OnExpand(MultiSelectTreeViewItem item)
        {
            item.IsExpanded = true;
            var items = item.Items;
        } 
        
    }
}
