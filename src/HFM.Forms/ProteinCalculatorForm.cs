/*
 * HFM.NET - Protein Calculator Form
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System.Linq;
using System.Windows.Forms;

using HFM.Forms.Models;

namespace HFM.Forms
{
   public partial class ProteinCalculatorForm : Form
   {
      private readonly ProteinCalculatorModel _model;

      public ProteinCalculatorForm(ProteinCalculatorModel model)
      {
         _model = model;

         InitializeComponent();
         DataBind();
      }

      private void DataBind()
      {
         ProjectComboBox.DataSource = _model.Projects.ToList();
         ProjectComboBox.DataBindings.Add("SelectedItem", _model, "SelectedProject", false, DataSourceUpdateMode.OnPropertyChanged);

         TimePerFrameMinuteTextBox.BindText(_model, "TpfMinutes");
         TimePerFrameSecondTextBox.BindText(_model, "TpfSeconds");

         TotalTimeCheckBox.BindChecked(_model, "TotalWuTimeEnabled");
         TotalTimeMinuteTextBox.BindText(_model, "TotalWuTimeMinutes");
         TotalTimeMinuteTextBox.BindEnabled(_model, "TotalWuTimeEnabled");
         TotalTimeSecondTextBox.BindText(_model, "TotalWuTimeSeconds");
         TotalTimeSecondTextBox.BindEnabled(_model, "TotalWuTimeEnabled");

         CoreNameTextBox.BindText(_model, "CoreName");
         SlotTypeTextBox.BindText(_model, "SlotType");
         NumberOfAtomsTextBox.BindText(_model, "NumberOfAtoms");
         CompletionTimeTextBox.BindText(_model, "CompletionTime");

         PreferredDeadlineTextBox.BindText(_model, "PreferredDeadline");
         PreferredDeadlineTextBox.DataBindings.Add("ReadOnly", _model, "PreferredDeadlineIsReadOnly", false, DataSourceUpdateMode.OnPropertyChanged);
         PreferredDeadlineCheckBox.BindChecked(_model, "PreferredDeadlineChecked");

         FinalDeadlineTextBox.BindText(_model, "FinalDeadline");
         FinalDeadlineTextBox.DataBindings.Add("ReadOnly", _model, "FinalDeadlineIsReadOnly", false, DataSourceUpdateMode.OnPropertyChanged);
         FinalDeadlineCheckBox.BindChecked(_model, "FinalDeadlineChecked");

         KFactorTextBox.BindText(_model, "KFactor");
         KFactorTextBox.DataBindings.Add("ReadOnly", _model, "KFactorIsReadOnly", false, DataSourceUpdateMode.OnPropertyChanged);
         KFactorCheckBox.BindChecked(_model, "KFactorChecked");

         BonusMultiplierTextBox.BindText(_model, "BonusMultiplier");
         BaseCreditTextBox.BindText(_model, "BaseCredit");
         BonusCreditTextBox.BindText(_model, "BonusCredit");
         TotalCreditTextBox.BindText(_model, "TotalCredit");
         BasePpdTextBox.BindText(_model, "BasePpd");
         BonusPpdTextBox.BindText(_model, "BonusPpd");
         TotalPpdTextBox.BindText(_model, "TotalPpd");
      }

      private void CalculateButtonClick(object sender, System.EventArgs e)
      {
         _model.Calculate();
      }
   }
}
