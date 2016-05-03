using SharpShell.Pidl;
using System; 

namespace Folder.FS
{
    public class ShPidlSystem
    {
        public static PidlData FromPath(string dir)
        {
            return NativePidl.PIDListFromPath(dir);
        }
    }
}
