using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Forms.Integration;
using System.Threading;
using System.Windows.Media.Imaging;
using VfpProj;

namespace Folder
{
    /// <summary>
    /// Interaction logic for CsApp.xaml
    /// </summary>
    public partial class CsApp : Application
    {
        static CsApp() { } // debugger entry

        static CsApp startRef;
        public static CsApp Instance
        {
            [DebuggerStepThrough]
            get { return startRef ?? Application.Current as CsApp; }
            set { startRef = value; }
        }

        // public MainWindow Window { get; set; }
        public FolderWindow Window { get; set; }

        public static CsApp Ref()
        {
            return Instance ?? new CsApp();
        }

        public static bool StartupMode = false;
        public CsApp()
        {
            startRef = this;
            // Startup += App_Startup;
            if (!StartupMode)
                Startup += App_StartupLoad2;
        }

        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs args)
        {
            var ex = args.Exception;
            //if (CsApp.Instance.Window.FormObject != null)
            //    CsApp.Instance.Window.FormObject.LastError = ex;

            Trace.Write(ex.Message);
        }

        //void App_StartupLoad(object sender, StartupEventArgs e)
            //Vfp.Startup.Instance.LoadMain(null);

        void App_StartupLoad2(object sender, StartupEventArgs e)
        {
            //Vfp.Startup.Instance.LoadMain(null);

            //var app = new VisualFoxpro.FoxApplication();
            //app.Visible = true;
            //FoxCmd.SetApp(app);
            //FoxCmd.SetVar();

            //Vfp.Startup.Instance.Show(app);

            MainWindow = new FolderWindow();

            MainWindow.Show();
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            JumpList jumpList1 = JumpList.GetJumpList(CsApp.Current);

            var window = new FolderWindow();   // NoBorder 
            window.AllowsTransparency = false;
            //window.Load(null);
            window.Show();

            //if (FoxCmd.Attach())
            //    FoxCmd.AssignForm(window);
        }

        //public EditWindow ShowEditWindow(string file)
        //{
        //    var winEdit = new EditWindow();

    }
}
