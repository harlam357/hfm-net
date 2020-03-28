
using System.Drawing;
using System.Windows.Forms;

namespace HFM.Forms
{
   internal static class WindowPosition
   {
      internal static Point Normalize(Point position, Size size)
      {
         return Normalize(position.X, position.Y, size.Width, size.Height);
      }

      internal static Point Normalize(int x, int y, int width, int height)
      {
         return SystemInformation.VirtualScreen.IntersectsWith(new Rectangle(x, y, width, height)) 
            ? new Point(x, y) 
            : CenterOnPrimaryScreen(width, height);
      }

      internal static Point CenterOnPrimaryScreen(Size size)
      {
         return CenterOnPrimaryScreen(size.Width, size.Height);
      }

      internal static Point CenterOnPrimaryScreen(int width, int height)
      {
         int x = (Screen.PrimaryScreen.Bounds.Size.Width - width) / 2;
         int y = (Screen.PrimaryScreen.Bounds.Size.Height - height) / 2;
         return new Point(x, y);
      }
   }
}
