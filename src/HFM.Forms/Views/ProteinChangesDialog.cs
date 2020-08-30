using System;
using System.Collections.Generic;

using HFM.Forms.Controls;
using HFM.Proteins;

namespace HFM.Forms.Views
{
    public partial class ProteinChangesDialog : FormBase
    {
        private readonly IEnumerable<ProteinChange> _proteinChanges;

        public ProteinChangesDialog(IEnumerable<ProteinChange> proteinChanges)
        {
            _proteinChanges = proteinChanges ?? throw new ArgumentNullException(nameof(proteinChanges));

            InitializeComponent();
            EscapeKeyReturnsCancelDialogResult();
        }

        private void ProteinLoadResultsDialog_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            foreach (var change in _proteinChanges)
            {
                changesListBox.Items.Add(change.ToString());
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
