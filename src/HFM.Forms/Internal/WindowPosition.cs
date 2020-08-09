using System;
using System.Drawing;
using System.Windows.Forms;

using HFM.Forms.Views;

namespace HFM.Forms.Internal
{
    internal static class WindowPosition
    {
        internal static (Point Location, Size Size) Normalize(IWin32Form form, Point restoreLocation, Size restoreSize)
        {
            var size = restoreSize == Size.Empty ? form.Size : EnsureMinimumSize(form.MinimumSize, restoreSize);
            var location = restoreLocation;
            if (restoreLocation == Point.Empty || !SystemInformation.VirtualScreen.IntersectsWith(new Rectangle(location, size)))
            {
                location = CenterOnPrimaryScreen(size);
            }
            return (location, size);
        }

        private static Point CenterOnPrimaryScreen(Size restoreSize)
        {
            var primaryScreenSize = Screen.PrimaryScreen.Bounds.Size;
            var size = EnsureMaximumSize(primaryScreenSize, restoreSize);

            int x = (primaryScreenSize.Width - size.Width) / 2;
            int y = (primaryScreenSize.Height - size.Height) / 2;
            return new Point(x, y);
        }

        private static Size EnsureMinimumSize(Size minimumSize, Size restoreSize)
        {
            return new Size(Math.Max(minimumSize.Width, restoreSize.Width), Math.Max(minimumSize.Height, restoreSize.Height));
        }

        private static Size EnsureMaximumSize(Size maximumSize, Size restoreSize)
        {
            return new Size(Math.Min(maximumSize.Width, restoreSize.Width), Math.Min(maximumSize.Height, restoreSize.Height));
        }
    }
}
