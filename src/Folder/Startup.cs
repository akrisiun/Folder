using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;
using System.ComponentModel;
using VfpProj;
using Folder;
using VisualFoxpro;
using Application = System.Windows.Application;
using System.Runtime.InteropServices;

namespace Vfp
{
    public class Startup : IComponent, IDisposable // _Startup
    {
        static Startup()
        {
            if (CsApp.Instance == null)
                CsApp.StartupMode = true;
            Instance = new Startup();
        }

        public void Dispose()
        {
            appCur = null;
            App = null;
        }

        ISite IComponent.Site { get; set; }

#pragma warning disable 0067
        public event EventHandler Disposed;

        public static string Dll { get; set; }
        public static Startup Instance { get; protected set; }
        public static Startup Load() { return Instance; }
        public FoxApplication App { get; set; }

        protected CsApp appCur;

        [STAThread]
        public void Main(FoxApplication app = null, bool lRun = false)
        {
            this.App = app;
            Dll = "/Folder";

            appCur = Application.Current as CsApp ?? Folder.CsApp.Instance;
            try
            {
                if (appCur == null)
                    appCur = Folder.CsApp.Ref();

                appCur.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }
            catch (Exception) { }

            var mainWnd = appCur.Window;

            if (mainWnd == null)
                mainWnd = new Folder.FolderWindow();

            WindowLoad(mainWnd, app, lRun);
        }

        public void WindowLoad(FolderWindow mainWnd, FoxApplication app, bool lRun = false)
        {
            string dir = Environment.CurrentDirectory;

            if (lRun)
                appCur.Run();
        }
    }
}