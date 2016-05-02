using System;
using System.Collections.Generic;
using Folder.Native;

namespace VfpInterop
{

    public class NativeWndInfo
    {
        public IntPtr ptr;
        public string text;
        public string textorig;
        public string cls;

        public string item;     // unique record

        public NativeWndInfo(IntPtr ptr)
        {
            this.ptr = ptr;
            item = ptr.ToString("X8");

            textorig = NativeMethods.GetWindowText(ptr);
            text = textorig.StartsWith("Properties") ? "Properties" :
                        textorig.Equals("Data Session") ? "View" : textorig;
            cls = NativeMethods.GetClassName(ptr);
        }

    }

}
