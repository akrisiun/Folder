
using System;
using System.IO;
using System.Text;
using System.Windows;
using Folder;

namespace VfpProj
{
    public struct VfpFileInfo
    {
        public VisualFoxpro.IFoxProject vfpProj;
        public FileInfo f;

        public bool Success { get { return f != null && f.Exists && vfpProj != null; } } 
        public string Ext {get {return f == null ? null : Path.GetExtension(f.Name).ToLower(); }}

        public override string ToString()
        {
            return f.Name;
        }
    }

    public static class TextRead
    {
        public static void ReadVfpInfo(ref VfpFileInfo info, ref string fileName, Action<String> Open)
        {
            VisualFoxpro.IFoxProject vfpProj = null;
            FileInfo f = null;

            try
            {
                f = new FileInfo(fileName);
                if (!f.Exists && vfpProj == null)
                    return;
            }
            catch { }

            string ext = "";

            try
            {
                fileName = fileName ?? vfpProj.Name;

                Directory.SetCurrentDirectory(Path.GetDirectoryName(fileName));
                ext = Path.GetExtension(fileName).ToUpperInvariant();

                Open(fileName);

            }
            catch (Exception ex)
            {
                if (ext.Length > 0 && ext != ".PJX")
                    MessageBox.Show("File " + fileName + " error\n" + ex);
            }
        }
    }
}
