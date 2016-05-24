
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
using Folder;
using Folder.FS;
using Folder.Visual;
using System.Windows.Forms.Integration;

namespace Folder
{
    public partial class FolderWindow : Window, IFolderWindow
    {
        public string FileName { get; set; }
        public Forms.TextBox txtPath { get; set; }

        //IFolderWindow
        TextBox IFolderWindow.txtFind { get { return this.txtFind; } }

        WindowsFormsHost IFolderWindow.hostPath { get { return this.hostPath; } } 

        Button IFolderWindow.buttonProj { get { return this.buttonProj; }}
        MultiSelectTreeView IFolderWindow.tree { get { return treeObj; } }

        internal MultiSelectTreeView treeObj;

        public FolderWindow()
        {
            Uri iconUri = new Uri("pack://application:,,,/pjx.ico", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(iconUri);

            //if (Startup.Dll == null)
            //    Startup.Dll = "Folder";
            var Dll = "FolderLib";

            FileName = string.Empty;
            if (!_contentLoaded)
            {
                _contentLoaded = true;
                System.Uri resourceLocater = new System.Uri(Dll + ";component/visual/folderwindow.xaml", System.UriKind.Relative);
                System.Windows.Application.LoadComponent(this, resourceLocater);
            }

            treeObj = this.tree;
            PostLoad();
        }

        void PostLoad()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                string dir = args[1];
                try
                {
                    Directory.SetCurrentDirectory(dir);
                }
                catch
                {
                    // breakpoint
                }
            }

            var folder = this as IFolderWindow;
            //Tree.Bind(folder, hostPath.Child);

            TextDrop.Bind(folder, folder.txtPath);
            Tree.LoadTree(folder, folder.txtPath.Text);
        }

        void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            var w = this;
            string dir = this.txtPath.Text.Trim();

            Tree.LoadTree(this as IFolderWindow, dir);
        }

        void tree_OnExpanded(object sender, RoutedEventArgs e)
        {
            var tvi = e.Source as MultiSelectTreeViewItem;
            if (tvi != null)
            {
                TreeNode.OnExpand(this, tvi);

                e.Handled = true;
            }
        }
    }

}
