using SharpShell.Helpers;
using System;

namespace Folder
{
    public class ProjItem
    {
        public string Name { get; set; }
        public bool Exclude { get; set; }
        public string Type { get; set; } // "P" - prg

        public PidlHandle HPidl { get; set; }
    }

}