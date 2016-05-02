using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Folder
{
    /// <summary>
    /// Interaction logic for NoBorder.xaml
    /// </summary>
    public partial class NoBorder : Window
    {
        public NoBorder()
        {
            InitializeComponent();

            Native.WpfNoBorder.Init(this, titleBar,
                topLeft, top, topRight, right, bottomRight, bottom, bottomLeft, left);
            cmdClose.Click += (s, e) => Close();
        }
    }

}
