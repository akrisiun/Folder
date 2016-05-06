using SharpShell.Pidl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Folder.Visual
{
    public class IconItem
    {
        public IconItem()
        {
            Items = new List<IconItem>();
        }

        public string Name { get; set; }

        public bool IsNodeExpanded
        {
            get;
            set;
        }

        public string Path { get; set; }
        public IList<IconItem> Items { get; set; }
        public IconItem Parent { get; set; }

        #region Image

        public PidlData Data { get; set; }
        public ImageSource ImageSource { get; protected set; }

        public void Bind(PidlData dataPidl = null)
        {
            Data = dataPidl ?? ShPidlSystem.FromPath(Path);

            IntPtr icon = Data.CreateOverlayIconPtr();
            // Icon.ExtractAssociatedIcon(@"Filename.extension");

            SetItemage(icon);
        }

        //http://stackoverflow.com/questions/14358553/binding-image-source-through-property-in-wpf
        public void SetItemage(IntPtr hIcon)
        {
            if (hIcon != IntPtr.Zero) // !string.IsNullOrEmpty(url))
            {
                var image = Imaging.CreateBitmapSourceFromHIcon(
                        hIcon,
                        Int32Rect.Empty,
                        BitmapSizeOptions); // .FromEmptyOptions());
                image.Freeze(); // -> to prevent error: "Must create DependencySource on same Thread as the DependencyObject"
                ImageSource = image;
            }
            else
            {
                ImageSource = null;
            }
        }

        static BitmapSizeOptions BitmapSizeOptions = BitmapSizeOptions.FromWidthAndHeight(16, 16);

        #endregion

        public static IconItem Empty = new IconItem { Name = String.Empty, Path = string.Empty };

    }
}
