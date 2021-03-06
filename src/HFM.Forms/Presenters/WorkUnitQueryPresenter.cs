﻿using System.Windows.Forms;

using HFM.Core.Data;
using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    public class WorkUnitQueryPresenter : DialogPresenter
    {
        public WorkUnitQuery Query { get; set; }

        public WorkUnitQueryPresenter(WorkUnitQuery query)
        {
            Query = query;
        }

        protected override IWin32Dialog OnCreateDialog() => new WorkUnitQueryDialog(this);

        public void OKClicked()
        {
            Dialog.DialogResult = DialogResult.OK;
            Dialog.Close();
        }

        public void CancelClicked()
        {
            Dialog.DialogResult = DialogResult.Cancel;
            Dialog.Close();
        }
    }
}
