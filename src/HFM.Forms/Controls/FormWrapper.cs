using System.Windows.Forms;

namespace HFM.Forms.Controls
{
    public partial class FormWrapper : Form
    {
        public FormWrapper()
        {
            InitializeComponent();
            Icon = Properties.Resources.hfm_48_48;
        }

        // https://stackoverflow.com/questions/76993/how-to-double-buffer-net-controls-on-a-form
        // TODO: Determine if this is still necessary.  Getting scroll bar artifacts on Main window RichTextBox on Windows 10.
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
    }
}
