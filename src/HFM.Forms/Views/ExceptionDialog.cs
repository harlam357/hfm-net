
using System;
using System.Windows.Forms;

namespace HFM.Forms
{
    public partial class ExceptionDialog : Form, IWin32Dialog
    {
        private readonly ExceptionPresenter _presenter;

        public ExceptionDialog(ExceptionPresenter presenter)
        {
            _presenter = presenter;

            InitializeComponent();

            if (_presenter.MustTerminate)
            {
                btnExit.Visible = false;
                btnContinue.Text = btnExit.Text;
                btnContinue.Left -= btnExit.Width - btnContinue.Width;
                btnContinue.Width = btnExit.Width;
            }

            exceptionTextBox.Text = _presenter.BuildExceptionText();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            _presenter.ReportClicked(copyErrorCheckBox.Checked);
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            _presenter.ContinueClicked();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _presenter.ExitClicked();
        }
    }
}
