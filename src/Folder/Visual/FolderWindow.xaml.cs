
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Forms = System.Windows.Forms;
using System.IO;
using System.Text;
using Folder.Native;
using System.Windows.Media.Imaging;
using VfpProj;
using Vfp;
using MultiSelect;

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
            // InitializeComponent();
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
            Tree.LoadFolder(this, txtPath.Text);
        }

        void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            var w = this;
            string dir = txtPath.Text.Trim();

            Tree.LoadFolder(this, dir);
        }

    }

}
