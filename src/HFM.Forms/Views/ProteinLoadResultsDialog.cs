using System.Collections.Generic;

using HFM.Forms.Controls;
using HFM.Proteins;

namespace HFM.Forms.Views
{
    public partial class ProteinLoadResultsDialog : FormBase
    {
        public ProteinLoadResultsDialog()
        {
            InitializeComponent();
        }

        public void DataBind(IEnumerable<ProteinDictionaryChange> loadResults)
        {
            foreach (var loadResult in loadResults)
            {
                if (!loadResult.Result.Equals(ProteinDictionaryChangeResult.NoChange))
                {
                    ProteinListBox.Items.Add(loadResult.ToString());
                }
            }
        }

        private void DialogOkButtonClick(object sender, System.EventArgs e)
        {
            Close();
        }
    }
}
