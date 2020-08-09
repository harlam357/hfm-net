using System;
using System.Windows.Forms;

using HFM.Forms.Models;
using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    public class MessagesPresenter : FormPresenter<MessagesModel>
    {
        public MessagesModel Model { get; }

        public MessagesPresenter(MessagesModel model) : base(model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public override void Show()
        {
            if (Form is null)
            {
                Model.Load();

                Form = OnCreateForm();
                Form.Closed += OnClosed;
            }

            Form.Show();
            if (Form.WindowState == FormWindowState.Minimized)
            {
                Form.WindowState = FormWindowState.Normal;
            }
            else
            {
                Form.BringToFront();
            }
        }

        protected override IWin32Form OnCreateForm()
        {
            return new MessagesForm(this);
        }
    }
}
