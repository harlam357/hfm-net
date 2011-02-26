
using System.Windows.Forms;

namespace HFM.Log.Tool
{
   public partial class TextDialog : Form
   {
      public TextDialog()
      {
         InitializeComponent();
      }
      
      public void SetText(string text)
      {
         textBox1.Text = text;
      }
   }
}
