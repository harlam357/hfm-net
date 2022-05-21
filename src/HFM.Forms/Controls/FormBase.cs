namespace HFM.Forms.Controls
{
    public partial class FormBase : Form
    {
        public FormBase()
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

        /// <summary>
        /// When the user presses the ESC key, close the form and return <see cref="DialogResult.Cancel" />.
        /// </summary>
        protected void EscapeKeyReturnsCancelDialogResult()
        {
            EscapeKeyReturnsDialogResult(DialogResult.Cancel);
        }

        /// <summary>
        /// When the user presses the ESC key, close the form and return <paramref name="dialogResult" />.
        /// </summary>
        protected void EscapeKeyReturnsDialogResult(DialogResult dialogResult)
        {
            var escapeKeyButton = new Button();
            escapeKeyButton.DialogResult = dialogResult;
            escapeKeyButton.Name = "escapeKeyButton";
            escapeKeyButton.TabStop = false;
            escapeKeyButton.Size = Size.Empty;
            escapeKeyButton.Location = Point.Empty;
            escapeKeyButton.Click += (s, e) => Close();
            Controls.Add(escapeKeyButton);
            EscapeKeyButton(escapeKeyButton);
        }

        /// <summary>
        /// When the user presses the ESC key, execute the <paramref name="button" /> Click event.
        /// </summary>
        protected void EscapeKeyButton(Button button) => CancelButton = button;
    }
}
