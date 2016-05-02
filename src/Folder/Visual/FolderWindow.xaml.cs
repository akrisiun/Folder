
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Forms = System.Windows.Forms;
using System.IO;
using System.Text;
using Folder.Native;
using MultiSelect;
using VfpProj;
using Folder.FS;
using Folder.Visual;

namespace Folder
{
    public partial class FolderWindow : Window
    {
        public string FileName { get; set; }
        public Forms.TextBox txtPath { get; set; }
        internal MultiSelectTreeView treeObj;

        public FolderWindow()
        {
            // Uri iconUri = new Uri("pack://application:,,,/PRG.ICO", UriKind.RelativeOrAbsolute);
            //Icon = MainWindow.PrgIco; //  BitmapFrame.Create(iconUri);
            if (Startup.Dll == null)
                Startup.Dll = "Folder";

            FileName = string.Empty;
            if (!_contentLoaded)
            {
                _contentLoaded = true;
                System.Uri resourceLocater = new System.Uri(Startup.Dll + ";component/visual/folderwindow.xaml", System.UriKind.Relative);
                System.Windows.Application.LoadComponent(this, resourceLocater);
            }

            treeObj = this.tree;
            PostLoad();
        }

        void PostLoad()
        {
            Tree.Bind(this);

            TextDrop.Bind(this, this.txtPath);
            Tree.LoadTree(this, txtPath.Text);
        }

        void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            var w = this;
            string dir = txtPath.Text.Trim();

            Tree.LoadTree(this, dir);
        }

        //<ItemsControl.ItemContainerStyle>
        //            <Style TargetType="tree:MultiSelectTreeViewItem">
        //                <EventSetter Event="TreeViewItem.Expanded" Handler="tree_OnExpanded" />
        //            </Style>
        //</ItemsControl.ItemContainerStyle>

        void tree_OnExpanded(object sender, RoutedEventArgs e)
        {
            var tvi = e.Source as MultiSelectTreeViewItem;
            if (tvi != null)
            {
                TreeNode.OnExpand(tvi);

                e.Handled = true;
            }
        }
    }

}
