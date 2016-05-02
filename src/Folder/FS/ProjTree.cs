using Folder.FS;
using Folder.Native;
using IOFile;
using MultiSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VisualFoxpro;

namespace Folder.FS
{
    public static class ProjTree
    {
        public static void LoadPrj(Window w, TreeView tree, VisualFoxpro.IFoxProject proj, string file)
        {
            if (proj == null)
                return;

            string main = null;
            IList<ProjItem> list = null;
            try
            {
                main = proj.MainFile;

                list = GetList(proj);

                tree.Items.Clear();
                foreach (var item in list)
                {
                    if (!"P".Equals(item.Type))
                        continue;

                    string filePrg = Path.GetFileName(item.Name);
                    tree.Items.Add(filePrg);
                }
            }
            catch { }
        }

        static IList<ProjItem> GetList(IFoxProject proj)
        {
            var list = new List<ProjItem>();

            IFoxPrjFiles files = proj.Files;
            IEnumerator filesNum = files.GetEnumerator();
            while (filesNum.MoveNext())
            {
                dynamic elem = filesNum.Current;

                var item = new ProjItem { Name = elem.Name, Exclude = elem.Exclude, Type = elem.Type };
                list.Add(item);
            }

            return list;
        }
    }
}
