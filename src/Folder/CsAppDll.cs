using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Forms.Integration;
using System.Threading;
using System.Windows.Media.Imaging;

namespace Folder
{
    public class CsApp : Application
    {
        static CsApp()
        {
            // debugger entry
            //global::Folder.MainWindow.Dll = "/FolderLib";
        }

        static CsApp startRef;
        public static CsApp Instance
        {
            [DebuggerStepThrough]
            get { return startRef ?? Application.Current as CsApp; }
            set { startRef = value; }
        }

        public static IFolderWindow FolderWindow { get; set; }

        public static CsApp Ref()
        {
            return Instance ?? new CsApp();
        }

        public static bool StartupMode = false;
        public CsApp()
        {
            startRef = this;
            //if (!StartupMode)
            //    Startup += App_StartupLoad2;
        }

        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs args)
        {
            var ex = args.Exception;
            //if (CsApp.Instance.FormObject != null)
            //    CsApp.Instance.FormObject.LastError = ex;

            Trace.Write(ex.Message);
        }
         
        
        void App_Startup(object sender, StartupEventArgs e)
        {
            JumpList jumpList1 = JumpList.GetJumpList(CsApp.Current);

            //MainWindow window = new MainWindow();   // NoBorder 
            //window.AllowsTransparency = false;
            //window.Load(null);
            //window.Show();

            //if (FoxCmd.Attach())
            //    FoxCmd.AssignForm(window);
        }

        //public EditWindow ShowEditWindow(string file)
        //{
        //    var winEdit = new EditWindow();
        //    winEdit.ShowInTaskbar = true;
        //    if (file.Length > 0)
        //    {
        //        winEdit.txtPath.Text = file;
        //        winEdit.OpenFile();
        //    }

        //    winEdit.Show();
        //    return winEdit;
        //}
    }
}