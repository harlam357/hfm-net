
using System.Drawing;
using System.Windows.Forms;

namespace HFM.Forms
{
   internal static class WindowPosition
   {
      public static Point Normalize(Point position, Size size)
      {
         return Normalize(position.X, position.Y, size.Width, size.Height);
      }

      public static Point Normalize(int x, int y, int width, int height)
      {
         if (SystemInformation.VirtualScreen.IntersectsWith(new Rectangle(x, y, width, height)))
         {
            return new Point(x, y);
         }
         x = (Screen.PrimaryScreen.Bounds.Size.Width - width) / 2;
         y = (Screen.PrimaryScreen.Bounds.Size.Height - height) / 2;
         return new Point(x, y);
      }
   }
}
