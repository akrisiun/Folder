using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
using VfpInterop;
using Folder;
using System.Windows.Input;

namespace Folder.Native
{
    [ToolboxItem(true)]
    public class WindowsEvents : ISupportInitialize
    {
        //CsForm form;
        //public MainWindow Form
        //{
        //    [DebuggerStepThrough]
        //    get { return form.Form; }
        //}

        public FolderWindow FormPrj
        {
            [DebuggerStepThrough]
            get;
            protected set;
        }

        public OpenFileDialog dlg;
        public string directory;
        public string file;
        public string ext;
        public Collection<NativeWndInfo> listWI;

        public System.Windows.Controls.TextBox txtFile;

        #region Init, Bind
        
        public WindowsEvents(Window form)
        {
             
        }

        void ISupportInitialize.BeginInit() { }
        void ISupportInitialize.EndInit()
        {
            //   form.tabWI.TabPages.Clear();
            Bind();
        }

        public bool IsBound;
        void Bind()
        {
           
        }
        #endregion 
    }

}
