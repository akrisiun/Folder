using MultiSelect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using Forms = System.Windows.Forms;

namespace Folder
{
    public interface IFolderWindow : IFrameworkInputElement
    {
        TextBox txtFind { get; }

        Forms.TextBox txtPath { get; set; }
        WindowsFormsHost hostPath { get; }

        Button buttonProj { get; }

        MultiSelectTreeView tree { get; }
    }

    public class FWindow : Window // , IFolderWindow
    {
    }
}
