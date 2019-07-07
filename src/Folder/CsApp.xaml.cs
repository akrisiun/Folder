using System.Diagnostics;
using System.Windows;
using System.Windows.Shell;
using System.Threading;

namespace Folder
{
    public class Startup
    {
        public static string Dll { get; set; }
    }

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

        public static new FolderWindow MainWindow { get { return CsApp.Current.MainWindow as FolderWindow; } }
        public FolderWindow Window { get; set; }

        public static CsApp Ref()
        {
            return Instance ?? new CsApp();
        }

        public static bool StartupMode = false;
        public CsApp()
        {
            startRef = this;
            if (!StartupMode)
                Startup += App_StartupLoad;
        }

        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs args)
        {
            var ex = args.Exception;

            Trace.Write(ex.Message);
        }

        void App_StartupLoad(object sender, StartupEventArgs e)
        {
            (this as Application).MainWindow = new FolderWindow();

            MainWindow.Show();
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            JumpList jumpList1 = JumpList.GetJumpList(CsApp.Current);

            var window = new FolderWindow();   // NoBorder 
            window.AllowsTransparency = false;
            window.Show();
        }

    }
}
