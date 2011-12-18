
using System.Collections.Generic;
using System.Windows.Forms;

using HFM.Proteins;

namespace HFM.Forms
{
   public partial class ProteinLoadResultsDialog : Form
   {
      public ProteinLoadResultsDialog()
      {
         InitializeComponent();
      }

      public void DataBind(IEnumerable<ProteinLoadInfo> loadResults)
      {
         foreach (var loadResult in loadResults)
         {
            if (!loadResult.Result.Equals(ProteinLoadResult.NoChange))
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
