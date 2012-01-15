namespace HFM.Forms
{
   partial class ProteinCalculatorForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProteinCalculatorForm));
         this.TimePerFrameMinuteTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.ProjectLabel = new HFM.Forms.Controls.LabelWrapper();
         this.ProjectComboBox = new HFM.Forms.Controls.ComboBoxWrapper();
         this.TimePerFrameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.TotalTimeCheckBox = new HFM.Forms.Controls.CheckBoxWrapper();
         this.TotalTimeMinuteTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.CoreNameTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.CoreNameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.TimePerFrameMinuteLabel = new HFM.Forms.Controls.LabelWrapper();
         this.TimePerFrameSecondTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.TimePerFrameSecondLabel = new HFM.Forms.Controls.LabelWrapper();
         this.TotalTimeMinuteLabel = new HFM.Forms.Controls.LabelWrapper();
         this.TotalTimeSecondTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.TotalTimeSecondLabel = new HFM.Forms.Controls.LabelWrapper();
         this.SlotTypeTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.SlotTypeLabel = new HFM.Forms.Controls.LabelWrapper();
         this.NumberOfAtomsLabel = new HFM.Forms.Controls.LabelWrapper();
         this.NumberOfAtomsTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.CompletionTimeTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.PreferredDeadlineTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.FinalDeadlineTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.KFactorTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.BonusMultiplierTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.BaseCreditTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.BonusCreditTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.TotalCreditTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.CompletionTimeLabel = new HFM.Forms.Controls.LabelWrapper();
         this.BonusMultiplierLabel = new HFM.Forms.Controls.LabelWrapper();
         this.BaseCreditLabel = new HFM.Forms.Controls.LabelWrapper();
         this.BonusCreditLabel = new HFM.Forms.Controls.LabelWrapper();
         this.BasePpdTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.TotalCreditLabel = new HFM.Forms.Controls.LabelWrapper();
         this.BasePpdLabel = new HFM.Forms.Controls.LabelWrapper();
         this.BonusPpdTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.BonusPpdLabel = new HFM.Forms.Controls.LabelWrapper();
         this.TotalPpdLabel = new HFM.Forms.Controls.LabelWrapper();
         this.TotalPpdTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.CalculateButton = new HFM.Forms.Controls.ButtonWrapper();
         this.PreferredDeadlineCheckBox = new HFM.Forms.Controls.CheckBoxWrapper();
         this.FinalDeadlineCheckBox = new HFM.Forms.Controls.CheckBoxWrapper();
         this.KFactorCheckBox = new HFM.Forms.Controls.CheckBoxWrapper();
         this.PreferredDeadlineDaysLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FinalDeadlineDaysLabel = new HFM.Forms.Controls.LabelWrapper();
         this.CompletionTimeDaysLabel = new HFM.Forms.Controls.LabelWrapper();
         this.SuspendLayout();
         // 
         // TimePerFrameMinuteTextBox
         // 
         this.TimePerFrameMinuteTextBox.Location = new System.Drawing.Point(136, 46);
         this.TimePerFrameMinuteTextBox.Name = "TimePerFrameMinuteTextBox";
         this.TimePerFrameMinuteTextBox.Size = new System.Drawing.Size(45, 20);
         this.TimePerFrameMinuteTextBox.TabIndex = 3;
         // 
         // ProjectLabel
         // 
         this.ProjectLabel.AutoSize = true;
         this.ProjectLabel.Location = new System.Drawing.Point(87, 20);
         this.ProjectLabel.Name = "ProjectLabel";
         this.ProjectLabel.Size = new System.Drawing.Size(43, 13);
         this.ProjectLabel.TabIndex = 0;
         this.ProjectLabel.Text = "Project:";
         // 
         // ProjectComboBox
         // 
         this.ProjectComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.ProjectComboBox.FormattingEnabled = true;
         this.ProjectComboBox.Location = new System.Drawing.Point(136, 17);
         this.ProjectComboBox.Name = "ProjectComboBox";
         this.ProjectComboBox.Size = new System.Drawing.Size(119, 21);
         this.ProjectComboBox.TabIndex = 1;
         // 
         // TimePerFrameLabel
         // 
         this.TimePerFrameLabel.AutoSize = true;
         this.TimePerFrameLabel.Location = new System.Drawing.Point(46, 49);
         this.TimePerFrameLabel.Name = "TimePerFrameLabel";
         this.TimePerFrameLabel.Size = new System.Drawing.Size(84, 13);
         this.TimePerFrameLabel.TabIndex = 2;
         this.TimePerFrameLabel.Text = "Time Per Frame:";
         // 
         // TotalTimeCheckBox
         // 
         this.TotalTimeCheckBox.AutoSize = true;
         this.TotalTimeCheckBox.Location = new System.Drawing.Point(32, 77);
         this.TotalTimeCheckBox.Name = "TotalTimeCheckBox";
         this.TotalTimeCheckBox.Size = new System.Drawing.Size(101, 17);
         this.TotalTimeCheckBox.TabIndex = 7;
         this.TotalTimeCheckBox.Text = "Total WU Time:";
         this.TotalTimeCheckBox.UseVisualStyleBackColor = true;
         // 
         // TotalTimeMinuteTextBox
         // 
         this.TotalTimeMinuteTextBox.Location = new System.Drawing.Point(136, 75);
         this.TotalTimeMinuteTextBox.Name = "TotalTimeMinuteTextBox";
         this.TotalTimeMinuteTextBox.Size = new System.Drawing.Size(45, 20);
         this.TotalTimeMinuteTextBox.TabIndex = 8;
         // 
         // CoreNameTextBox
         // 
         this.CoreNameTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.CoreNameTextBox.Location = new System.Drawing.Point(136, 153);
         this.CoreNameTextBox.Name = "CoreNameTextBox";
         this.CoreNameTextBox.ReadOnly = true;
         this.CoreNameTextBox.Size = new System.Drawing.Size(119, 20);
         this.CoreNameTextBox.TabIndex = 14;
         // 
         // CoreNameLabel
         // 
         this.CoreNameLabel.AutoSize = true;
         this.CoreNameLabel.Location = new System.Drawing.Point(67, 156);
         this.CoreNameLabel.Name = "CoreNameLabel";
         this.CoreNameLabel.Size = new System.Drawing.Size(63, 13);
         this.CoreNameLabel.TabIndex = 13;
         this.CoreNameLabel.Text = "Core Name:";
         // 
         // TimePerFrameMinuteLabel
         // 
         this.TimePerFrameMinuteLabel.AutoSize = true;
         this.TimePerFrameMinuteLabel.Location = new System.Drawing.Point(185, 49);
         this.TimePerFrameMinuteLabel.Name = "TimePerFrameMinuteLabel";
         this.TimePerFrameMinuteLabel.Size = new System.Drawing.Size(24, 13);
         this.TimePerFrameMinuteLabel.TabIndex = 4;
         this.TimePerFrameMinuteLabel.Text = "Min";
         // 
         // TimePerFrameSecondTextBox
         // 
         this.TimePerFrameSecondTextBox.Location = new System.Drawing.Point(210, 46);
         this.TimePerFrameSecondTextBox.Name = "TimePerFrameSecondTextBox";
         this.TimePerFrameSecondTextBox.Size = new System.Drawing.Size(45, 20);
         this.TimePerFrameSecondTextBox.TabIndex = 5;
         // 
         // TimePerFrameSecondLabel
         // 
         this.TimePerFrameSecondLabel.AutoSize = true;
         this.TimePerFrameSecondLabel.Location = new System.Drawing.Point(259, 49);
         this.TimePerFrameSecondLabel.Name = "TimePerFrameSecondLabel";
         this.TimePerFrameSecondLabel.Size = new System.Drawing.Size(26, 13);
         this.TimePerFrameSecondLabel.TabIndex = 6;
         this.TimePerFrameSecondLabel.Text = "Sec";
         // 
         // TotalTimeMinuteLabel
         // 
         this.TotalTimeMinuteLabel.AutoSize = true;
         this.TotalTimeMinuteLabel.Location = new System.Drawing.Point(185, 78);
         this.TotalTimeMinuteLabel.Name = "TotalTimeMinuteLabel";
         this.TotalTimeMinuteLabel.Size = new System.Drawing.Size(24, 13);
         this.TotalTimeMinuteLabel.TabIndex = 9;
         this.TotalTimeMinuteLabel.Text = "Min";
         // 
         // TotalTimeSecondTextBox
         // 
         this.TotalTimeSecondTextBox.Location = new System.Drawing.Point(210, 75);
         this.TotalTimeSecondTextBox.Name = "TotalTimeSecondTextBox";
         this.TotalTimeSecondTextBox.Size = new System.Drawing.Size(45, 20);
         this.TotalTimeSecondTextBox.TabIndex = 10;
         // 
         // TotalTimeSecondLabel
         // 
         this.TotalTimeSecondLabel.AutoSize = true;
         this.TotalTimeSecondLabel.Location = new System.Drawing.Point(259, 78);
         this.TotalTimeSecondLabel.Name = "TotalTimeSecondLabel";
         this.TotalTimeSecondLabel.Size = new System.Drawing.Size(26, 13);
         this.TotalTimeSecondLabel.TabIndex = 11;
         this.TotalTimeSecondLabel.Text = "Sec";
         // 
         // SlotTypeTextBox
         // 
         this.SlotTypeTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.SlotTypeTextBox.Location = new System.Drawing.Point(136, 179);
         this.SlotTypeTextBox.Name = "SlotTypeTextBox";
         this.SlotTypeTextBox.ReadOnly = true;
         this.SlotTypeTextBox.Size = new System.Drawing.Size(119, 20);
         this.SlotTypeTextBox.TabIndex = 16;
         // 
         // SlotTypeLabel
         // 
         this.SlotTypeLabel.AutoSize = true;
         this.SlotTypeLabel.Location = new System.Drawing.Point(75, 182);
         this.SlotTypeLabel.Name = "SlotTypeLabel";
         this.SlotTypeLabel.Size = new System.Drawing.Size(55, 13);
         this.SlotTypeLabel.TabIndex = 15;
         this.SlotTypeLabel.Text = "Slot Type:";
         // 
         // NumberOfAtomsLabel
         // 
         this.NumberOfAtomsLabel.AutoSize = true;
         this.NumberOfAtomsLabel.Location = new System.Drawing.Point(39, 208);
         this.NumberOfAtomsLabel.Name = "NumberOfAtomsLabel";
         this.NumberOfAtomsLabel.Size = new System.Drawing.Size(91, 13);
         this.NumberOfAtomsLabel.TabIndex = 17;
         this.NumberOfAtomsLabel.Text = "Number of Atoms:";
         // 
         // NumberOfAtomsTextBox
         // 
         this.NumberOfAtomsTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.NumberOfAtomsTextBox.Location = new System.Drawing.Point(136, 205);
         this.NumberOfAtomsTextBox.Name = "NumberOfAtomsTextBox";
         this.NumberOfAtomsTextBox.ReadOnly = true;
         this.NumberOfAtomsTextBox.Size = new System.Drawing.Size(119, 20);
         this.NumberOfAtomsTextBox.TabIndex = 18;
         // 
         // CompletionTimeTextBox
         // 
         this.CompletionTimeTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.CompletionTimeTextBox.Location = new System.Drawing.Point(136, 231);
         this.CompletionTimeTextBox.Name = "CompletionTimeTextBox";
         this.CompletionTimeTextBox.ReadOnly = true;
         this.CompletionTimeTextBox.Size = new System.Drawing.Size(119, 20);
         this.CompletionTimeTextBox.TabIndex = 20;
         // 
         // PreferredDeadlineTextBox
         // 
         this.PreferredDeadlineTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.PreferredDeadlineTextBox.Location = new System.Drawing.Point(136, 257);
         this.PreferredDeadlineTextBox.Name = "PreferredDeadlineTextBox";
         this.PreferredDeadlineTextBox.ReadOnly = true;
         this.PreferredDeadlineTextBox.Size = new System.Drawing.Size(119, 20);
         this.PreferredDeadlineTextBox.TabIndex = 22;
         // 
         // FinalDeadlineTextBox
         // 
         this.FinalDeadlineTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.FinalDeadlineTextBox.Location = new System.Drawing.Point(136, 283);
         this.FinalDeadlineTextBox.Name = "FinalDeadlineTextBox";
         this.FinalDeadlineTextBox.ReadOnly = true;
         this.FinalDeadlineTextBox.Size = new System.Drawing.Size(119, 20);
         this.FinalDeadlineTextBox.TabIndex = 24;
         // 
         // KFactorTextBox
         // 
         this.KFactorTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.KFactorTextBox.Location = new System.Drawing.Point(136, 309);
         this.KFactorTextBox.Name = "KFactorTextBox";
         this.KFactorTextBox.ReadOnly = true;
         this.KFactorTextBox.Size = new System.Drawing.Size(119, 20);
         this.KFactorTextBox.TabIndex = 26;
         // 
         // BonusMultiplierTextBox
         // 
         this.BonusMultiplierTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.BonusMultiplierTextBox.Location = new System.Drawing.Point(136, 335);
         this.BonusMultiplierTextBox.Name = "BonusMultiplierTextBox";
         this.BonusMultiplierTextBox.ReadOnly = true;
         this.BonusMultiplierTextBox.Size = new System.Drawing.Size(119, 20);
         this.BonusMultiplierTextBox.TabIndex = 28;
         // 
         // BaseCreditTextBox
         // 
         this.BaseCreditTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.BaseCreditTextBox.Location = new System.Drawing.Point(136, 361);
         this.BaseCreditTextBox.Name = "BaseCreditTextBox";
         this.BaseCreditTextBox.ReadOnly = true;
         this.BaseCreditTextBox.Size = new System.Drawing.Size(119, 20);
         this.BaseCreditTextBox.TabIndex = 30;
         // 
         // BonusCreditTextBox
         // 
         this.BonusCreditTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.BonusCreditTextBox.Location = new System.Drawing.Point(136, 387);
         this.BonusCreditTextBox.Name = "BonusCreditTextBox";
         this.BonusCreditTextBox.ReadOnly = true;
         this.BonusCreditTextBox.Size = new System.Drawing.Size(119, 20);
         this.BonusCreditTextBox.TabIndex = 32;
         // 
         // TotalCreditTextBox
         // 
         this.TotalCreditTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.TotalCreditTextBox.Location = new System.Drawing.Point(136, 413);
         this.TotalCreditTextBox.Name = "TotalCreditTextBox";
         this.TotalCreditTextBox.ReadOnly = true;
         this.TotalCreditTextBox.Size = new System.Drawing.Size(119, 20);
         this.TotalCreditTextBox.TabIndex = 34;
         // 
         // CompletionTimeLabel
         // 
         this.CompletionTimeLabel.AutoSize = true;
         this.CompletionTimeLabel.Location = new System.Drawing.Point(42, 234);
         this.CompletionTimeLabel.Name = "CompletionTimeLabel";
         this.CompletionTimeLabel.Size = new System.Drawing.Size(88, 13);
         this.CompletionTimeLabel.TabIndex = 19;
         this.CompletionTimeLabel.Text = "Completion Time:";
         // 
         // BonusMultiplierLabel
         // 
         this.BonusMultiplierLabel.AutoSize = true;
         this.BonusMultiplierLabel.Location = new System.Drawing.Point(46, 338);
         this.BonusMultiplierLabel.Name = "BonusMultiplierLabel";
         this.BonusMultiplierLabel.Size = new System.Drawing.Size(84, 13);
         this.BonusMultiplierLabel.TabIndex = 27;
         this.BonusMultiplierLabel.Text = "Bonus Multiplier:";
         // 
         // BaseCreditLabel
         // 
         this.BaseCreditLabel.AutoSize = true;
         this.BaseCreditLabel.Location = new System.Drawing.Point(66, 364);
         this.BaseCreditLabel.Name = "BaseCreditLabel";
         this.BaseCreditLabel.Size = new System.Drawing.Size(64, 13);
         this.BaseCreditLabel.TabIndex = 29;
         this.BaseCreditLabel.Text = "Base Credit:";
         // 
         // BonusCreditLabel
         // 
         this.BonusCreditLabel.AutoSize = true;
         this.BonusCreditLabel.Location = new System.Drawing.Point(60, 390);
         this.BonusCreditLabel.Name = "BonusCreditLabel";
         this.BonusCreditLabel.Size = new System.Drawing.Size(70, 13);
         this.BonusCreditLabel.TabIndex = 31;
         this.BonusCreditLabel.Text = "Bonus Credit:";
         // 
         // BasePpdTextBox
         // 
         this.BasePpdTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.BasePpdTextBox.Location = new System.Drawing.Point(136, 439);
         this.BasePpdTextBox.Name = "BasePpdTextBox";
         this.BasePpdTextBox.ReadOnly = true;
         this.BasePpdTextBox.Size = new System.Drawing.Size(119, 20);
         this.BasePpdTextBox.TabIndex = 36;
         // 
         // TotalCreditLabel
         // 
         this.TotalCreditLabel.AutoSize = true;
         this.TotalCreditLabel.Location = new System.Drawing.Point(66, 416);
         this.TotalCreditLabel.Name = "TotalCreditLabel";
         this.TotalCreditLabel.Size = new System.Drawing.Size(64, 13);
         this.TotalCreditLabel.TabIndex = 33;
         this.TotalCreditLabel.Text = "Total Credit:";
         // 
         // BasePpdLabel
         // 
         this.BasePpdLabel.AutoSize = true;
         this.BasePpdLabel.Location = new System.Drawing.Point(71, 442);
         this.BasePpdLabel.Name = "BasePpdLabel";
         this.BasePpdLabel.Size = new System.Drawing.Size(59, 13);
         this.BasePpdLabel.TabIndex = 35;
         this.BasePpdLabel.Text = "Base PPD:";
         // 
         // BonusPpdTextBox
         // 
         this.BonusPpdTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.BonusPpdTextBox.Location = new System.Drawing.Point(136, 465);
         this.BonusPpdTextBox.Name = "BonusPpdTextBox";
         this.BonusPpdTextBox.ReadOnly = true;
         this.BonusPpdTextBox.Size = new System.Drawing.Size(119, 20);
         this.BonusPpdTextBox.TabIndex = 38;
         // 
         // BonusPpdLabel
         // 
         this.BonusPpdLabel.AutoSize = true;
         this.BonusPpdLabel.Location = new System.Drawing.Point(65, 468);
         this.BonusPpdLabel.Name = "BonusPpdLabel";
         this.BonusPpdLabel.Size = new System.Drawing.Size(65, 13);
         this.BonusPpdLabel.TabIndex = 37;
         this.BonusPpdLabel.Text = "Bonus PPD:";
         // 
         // TotalPpdLabel
         // 
         this.TotalPpdLabel.AutoSize = true;
         this.TotalPpdLabel.Location = new System.Drawing.Point(71, 494);
         this.TotalPpdLabel.Name = "TotalPpdLabel";
         this.TotalPpdLabel.Size = new System.Drawing.Size(59, 13);
         this.TotalPpdLabel.TabIndex = 39;
         this.TotalPpdLabel.Text = "Total PPD:";
         // 
         // TotalPpdTextBox
         // 
         this.TotalPpdTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.TotalPpdTextBox.Location = new System.Drawing.Point(136, 491);
         this.TotalPpdTextBox.Name = "TotalPpdTextBox";
         this.TotalPpdTextBox.ReadOnly = true;
         this.TotalPpdTextBox.Size = new System.Drawing.Size(119, 20);
         this.TotalPpdTextBox.TabIndex = 40;
         // 
         // CalculateButton
         // 
         this.CalculateButton.Location = new System.Drawing.Point(111, 112);
         this.CalculateButton.Name = "CalculateButton";
         this.CalculateButton.Size = new System.Drawing.Size(93, 26);
         this.CalculateButton.TabIndex = 12;
         this.CalculateButton.Text = "Calculate";
         this.CalculateButton.UseVisualStyleBackColor = true;
         this.CalculateButton.Click += new System.EventHandler(this.CalculateButtonClick);
         // 
         // PreferredDeadlineCheckBox
         // 
         this.PreferredDeadlineCheckBox.AutoSize = true;
         this.PreferredDeadlineCheckBox.Location = new System.Drawing.Point(16, 260);
         this.PreferredDeadlineCheckBox.Name = "PreferredDeadlineCheckBox";
         this.PreferredDeadlineCheckBox.Size = new System.Drawing.Size(117, 17);
         this.PreferredDeadlineCheckBox.TabIndex = 41;
         this.PreferredDeadlineCheckBox.Text = "Preferred Deadline:";
         this.PreferredDeadlineCheckBox.UseVisualStyleBackColor = true;
         // 
         // FinalDeadlineCheckBox
         // 
         this.FinalDeadlineCheckBox.AutoSize = true;
         this.FinalDeadlineCheckBox.Location = new System.Drawing.Point(37, 285);
         this.FinalDeadlineCheckBox.Name = "FinalDeadlineCheckBox";
         this.FinalDeadlineCheckBox.Size = new System.Drawing.Size(96, 17);
         this.FinalDeadlineCheckBox.TabIndex = 42;
         this.FinalDeadlineCheckBox.Text = "Final Deadline:";
         this.FinalDeadlineCheckBox.UseVisualStyleBackColor = true;
         // 
         // KFactorCheckBox
         // 
         this.KFactorCheckBox.AutoSize = true;
         this.KFactorCheckBox.Location = new System.Drawing.Point(64, 311);
         this.KFactorCheckBox.Name = "KFactorCheckBox";
         this.KFactorCheckBox.Size = new System.Drawing.Size(69, 17);
         this.KFactorCheckBox.TabIndex = 43;
         this.KFactorCheckBox.Text = "K Factor:";
         this.KFactorCheckBox.UseVisualStyleBackColor = true;
         // 
         // PreferredDeadlineDaysLabel
         // 
         this.PreferredDeadlineDaysLabel.AutoSize = true;
         this.PreferredDeadlineDaysLabel.Location = new System.Drawing.Point(259, 260);
         this.PreferredDeadlineDaysLabel.Name = "PreferredDeadlineDaysLabel";
         this.PreferredDeadlineDaysLabel.Size = new System.Drawing.Size(31, 13);
         this.PreferredDeadlineDaysLabel.TabIndex = 44;
         this.PreferredDeadlineDaysLabel.Text = "Days";
         // 
         // FinalDeadlineDaysLabel
         // 
         this.FinalDeadlineDaysLabel.AutoSize = true;
         this.FinalDeadlineDaysLabel.Location = new System.Drawing.Point(259, 286);
         this.FinalDeadlineDaysLabel.Name = "FinalDeadlineDaysLabel";
         this.FinalDeadlineDaysLabel.Size = new System.Drawing.Size(31, 13);
         this.FinalDeadlineDaysLabel.TabIndex = 45;
         this.FinalDeadlineDaysLabel.Text = "Days";
         // 
         // CompletionTimeDaysLabel
         // 
         this.CompletionTimeDaysLabel.AutoSize = true;
         this.CompletionTimeDaysLabel.Location = new System.Drawing.Point(259, 234);
         this.CompletionTimeDaysLabel.Name = "CompletionTimeDaysLabel";
         this.CompletionTimeDaysLabel.Size = new System.Drawing.Size(31, 13);
         this.CompletionTimeDaysLabel.TabIndex = 46;
         this.CompletionTimeDaysLabel.Text = "Days";
         // 
         // ProteinCalculatorForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(314, 530);
         this.Controls.Add(this.CompletionTimeDaysLabel);
         this.Controls.Add(this.FinalDeadlineDaysLabel);
         this.Controls.Add(this.PreferredDeadlineDaysLabel);
         this.Controls.Add(this.KFactorCheckBox);
         this.Controls.Add(this.FinalDeadlineCheckBox);
         this.Controls.Add(this.PreferredDeadlineCheckBox);
         this.Controls.Add(this.CalculateButton);
         this.Controls.Add(this.TotalPpdTextBox);
         this.Controls.Add(this.TotalPpdLabel);
         this.Controls.Add(this.BonusPpdLabel);
         this.Controls.Add(this.BonusPpdTextBox);
         this.Controls.Add(this.BasePpdLabel);
         this.Controls.Add(this.TotalCreditLabel);
         this.Controls.Add(this.BasePpdTextBox);
         this.Controls.Add(this.BonusCreditLabel);
         this.Controls.Add(this.BaseCreditLabel);
         this.Controls.Add(this.BonusMultiplierLabel);
         this.Controls.Add(this.CompletionTimeLabel);
         this.Controls.Add(this.TotalCreditTextBox);
         this.Controls.Add(this.BonusCreditTextBox);
         this.Controls.Add(this.BaseCreditTextBox);
         this.Controls.Add(this.BonusMultiplierTextBox);
         this.Controls.Add(this.KFactorTextBox);
         this.Controls.Add(this.FinalDeadlineTextBox);
         this.Controls.Add(this.PreferredDeadlineTextBox);
         this.Controls.Add(this.CompletionTimeTextBox);
         this.Controls.Add(this.NumberOfAtomsTextBox);
         this.Controls.Add(this.NumberOfAtomsLabel);
         this.Controls.Add(this.SlotTypeLabel);
         this.Controls.Add(this.SlotTypeTextBox);
         this.Controls.Add(this.TotalTimeSecondLabel);
         this.Controls.Add(this.TotalTimeSecondTextBox);
         this.Controls.Add(this.TotalTimeMinuteLabel);
         this.Controls.Add(this.TimePerFrameSecondLabel);
         this.Controls.Add(this.TimePerFrameSecondTextBox);
         this.Controls.Add(this.TimePerFrameMinuteLabel);
         this.Controls.Add(this.CoreNameLabel);
         this.Controls.Add(this.CoreNameTextBox);
         this.Controls.Add(this.TotalTimeMinuteTextBox);
         this.Controls.Add(this.TotalTimeCheckBox);
         this.Controls.Add(this.TimePerFrameLabel);
         this.Controls.Add(this.ProjectComboBox);
         this.Controls.Add(this.ProjectLabel);
         this.Controls.Add(this.TimePerFrameMinuteTextBox);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.Name = "ProteinCalculatorForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Points Calculator";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private Controls.TextBoxWrapper TimePerFrameMinuteTextBox;
      private Controls.LabelWrapper ProjectLabel;
      private Controls.ComboBoxWrapper ProjectComboBox;
      private Controls.LabelWrapper TimePerFrameLabel;
      private Controls.CheckBoxWrapper TotalTimeCheckBox;
      private Controls.TextBoxWrapper TotalTimeMinuteTextBox;
      private Controls.TextBoxWrapper CoreNameTextBox;
      private Controls.LabelWrapper CoreNameLabel;
      private Controls.LabelWrapper TimePerFrameMinuteLabel;
      private Controls.TextBoxWrapper TimePerFrameSecondTextBox;
      private Controls.LabelWrapper TimePerFrameSecondLabel;
      private Controls.LabelWrapper TotalTimeMinuteLabel;
      private Controls.TextBoxWrapper TotalTimeSecondTextBox;
      private Controls.LabelWrapper TotalTimeSecondLabel;
      private Controls.TextBoxWrapper SlotTypeTextBox;
      private Controls.LabelWrapper SlotTypeLabel;
      private Controls.LabelWrapper NumberOfAtomsLabel;
      private Controls.TextBoxWrapper NumberOfAtomsTextBox;
      private Controls.TextBoxWrapper CompletionTimeTextBox;
      private Controls.TextBoxWrapper PreferredDeadlineTextBox;
      private Controls.TextBoxWrapper FinalDeadlineTextBox;
      private Controls.TextBoxWrapper KFactorTextBox;
      private Controls.TextBoxWrapper BonusMultiplierTextBox;
      private Controls.TextBoxWrapper BaseCreditTextBox;
      private Controls.TextBoxWrapper BonusCreditTextBox;
      private Controls.TextBoxWrapper TotalCreditTextBox;
      private Controls.LabelWrapper CompletionTimeLabel;
      private Controls.LabelWrapper BonusMultiplierLabel;
      private Controls.LabelWrapper BaseCreditLabel;
      private Controls.LabelWrapper BonusCreditLabel;
      private Controls.TextBoxWrapper BasePpdTextBox;
      private Controls.LabelWrapper TotalCreditLabel;
      private Controls.LabelWrapper BasePpdLabel;
      private Controls.TextBoxWrapper BonusPpdTextBox;
      private Controls.LabelWrapper BonusPpdLabel;
      private Controls.LabelWrapper TotalPpdLabel;
      private Controls.TextBoxWrapper TotalPpdTextBox;
      private Controls.ButtonWrapper CalculateButton;
      private Controls.CheckBoxWrapper PreferredDeadlineCheckBox;
      private Controls.CheckBoxWrapper FinalDeadlineCheckBox;
      private Controls.CheckBoxWrapper KFactorCheckBox;
      private Controls.LabelWrapper PreferredDeadlineDaysLabel;
      private Controls.LabelWrapper FinalDeadlineDaysLabel;
      private Controls.LabelWrapper CompletionTimeDaysLabel;
   }
}