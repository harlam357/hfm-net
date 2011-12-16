namespace HFM.Forms
{
   partial class FahClientSetupDialog
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
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FahClientSetupDialog));
         this.DummyTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.ClientTimeOffsetLabel = new HFM.Forms.Controls.LabelWrapper();
         this.ClientTimeOffsetUpDown = new System.Windows.Forms.NumericUpDown();
         this.ClientNoUtcOffsetCheckBox = new HFM.Forms.Controls.CheckBoxWrapper();
         this.DialogCancelButton = new HFM.Forms.Controls.ButtonWrapper();
         this.DialogOkButton = new HFM.Forms.Controls.ButtonWrapper();
         this.SetupTabControl = new System.Windows.Forms.TabControl();
         this.ConnectionTabPage = new System.Windows.Forms.TabPage();
         this.ConnectButton = new HFM.Forms.Controls.ButtonWrapper();
         this.AddressPortTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.PasswordTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.PasswordLabel = new HFM.Forms.Controls.LabelWrapper();
         this.AddressPortLabel = new HFM.Forms.Controls.LabelWrapper();
         this.AddressLabel = new HFM.Forms.Controls.LabelWrapper();
         this.AddressTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.ClientNameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.ClientNameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.SlotsTabPage = new System.Windows.Forms.TabPage();
         this.SlotsDataGridView = new System.Windows.Forms.DataGridView();
         this.EditButton = new HFM.Forms.Controls.ButtonWrapper();
         this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
         ((System.ComponentModel.ISupportInitialize)(this.ClientTimeOffsetUpDown)).BeginInit();
         this.SetupTabControl.SuspendLayout();
         this.ConnectionTabPage.SuspendLayout();
         this.SlotsTabPage.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.SlotsDataGridView)).BeginInit();
         this.SuspendLayout();
         // 
         // DummyTextBox
         // 
         this.DummyTextBox.Location = new System.Drawing.Point(331, 324);
         this.DummyTextBox.Name = "DummyTextBox";
         this.DummyTextBox.Size = new System.Drawing.Size(49, 20);
         this.DummyTextBox.TabIndex = 26;
         // 
         // ClientTimeOffsetLabel
         // 
         this.ClientTimeOffsetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.ClientTimeOffsetLabel.AutoSize = true;
         this.ClientTimeOffsetLabel.Location = new System.Drawing.Point(64, 355);
         this.ClientTimeOffsetLabel.Name = "ClientTimeOffsetLabel";
         this.ClientTimeOffsetLabel.Size = new System.Drawing.Size(136, 13);
         this.ClientTimeOffsetLabel.TabIndex = 23;
         this.ClientTimeOffsetLabel.Text = "Client Time Offset (Minutes)";
         // 
         // ClientTimeOffsetUpDown
         // 
         this.ClientTimeOffsetUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.ClientTimeOffsetUpDown.Location = new System.Drawing.Point(8, 352);
         this.ClientTimeOffsetUpDown.Name = "ClientTimeOffsetUpDown";
         this.ClientTimeOffsetUpDown.Size = new System.Drawing.Size(54, 20);
         this.ClientTimeOffsetUpDown.TabIndex = 22;
         // 
         // ClientNoUtcOffsetCheckBox
         // 
         this.ClientNoUtcOffsetCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.ClientNoUtcOffsetCheckBox.AutoSize = true;
         this.ClientNoUtcOffsetCheckBox.Location = new System.Drawing.Point(12, 326);
         this.ClientNoUtcOffsetCheckBox.Name = "ClientNoUtcOffsetCheckBox";
         this.ClientNoUtcOffsetCheckBox.Size = new System.Drawing.Size(144, 17);
         this.ClientNoUtcOffsetCheckBox.TabIndex = 21;
         this.ClientNoUtcOffsetCheckBox.Text = "Client has no UTC offset.";
         this.ClientNoUtcOffsetCheckBox.UseVisualStyleBackColor = true;
         // 
         // DialogCancelButton
         // 
         this.DialogCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.DialogCancelButton.CausesValidation = false;
         this.DialogCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.DialogCancelButton.Location = new System.Drawing.Point(304, 350);
         this.DialogCancelButton.Name = "DialogCancelButton";
         this.DialogCancelButton.Size = new System.Drawing.Size(81, 25);
         this.DialogCancelButton.TabIndex = 25;
         this.DialogCancelButton.Text = "Cancel";
         this.DialogCancelButton.UseVisualStyleBackColor = true;
         // 
         // DialogOkButton
         // 
         this.DialogOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.DialogOkButton.Location = new System.Drawing.Point(212, 350);
         this.DialogOkButton.Name = "DialogOkButton";
         this.DialogOkButton.Size = new System.Drawing.Size(81, 25);
         this.DialogOkButton.TabIndex = 24;
         this.DialogOkButton.Text = "OK";
         this.DialogOkButton.UseVisualStyleBackColor = true;
         this.DialogOkButton.Click += new System.EventHandler(this.DialogOkButtonClick);
         // 
         // SetupTabControl
         // 
         this.SetupTabControl.Controls.Add(this.ConnectionTabPage);
         this.SetupTabControl.Controls.Add(this.SlotsTabPage);
         this.SetupTabControl.Location = new System.Drawing.Point(8, 12);
         this.SetupTabControl.Name = "SetupTabControl";
         this.SetupTabControl.SelectedIndex = 0;
         this.SetupTabControl.Size = new System.Drawing.Size(377, 306);
         this.SetupTabControl.TabIndex = 27;
         // 
         // ConnectionTabPage
         // 
         this.ConnectionTabPage.Controls.Add(this.ConnectButton);
         this.ConnectionTabPage.Controls.Add(this.AddressPortTextBox);
         this.ConnectionTabPage.Controls.Add(this.PasswordTextBox);
         this.ConnectionTabPage.Controls.Add(this.PasswordLabel);
         this.ConnectionTabPage.Controls.Add(this.AddressPortLabel);
         this.ConnectionTabPage.Controls.Add(this.AddressLabel);
         this.ConnectionTabPage.Controls.Add(this.AddressTextBox);
         this.ConnectionTabPage.Controls.Add(this.ClientNameLabel);
         this.ConnectionTabPage.Controls.Add(this.ClientNameTextBox);
         this.ConnectionTabPage.Location = new System.Drawing.Point(4, 22);
         this.ConnectionTabPage.Name = "ConnectionTabPage";
         this.ConnectionTabPage.Padding = new System.Windows.Forms.Padding(3);
         this.ConnectionTabPage.Size = new System.Drawing.Size(369, 280);
         this.ConnectionTabPage.TabIndex = 0;
         this.ConnectionTabPage.Text = "Connection";
         this.ConnectionTabPage.UseVisualStyleBackColor = true;
         // 
         // ConnectButton
         // 
         this.ConnectButton.Location = new System.Drawing.Point(269, 240);
         this.ConnectButton.Name = "ConnectButton";
         this.ConnectButton.Size = new System.Drawing.Size(81, 25);
         this.ConnectButton.TabIndex = 28;
         this.ConnectButton.Text = "Connect";
         this.ConnectButton.UseVisualStyleBackColor = true;
         this.ConnectButton.Click += new System.EventHandler(this.ConnectButtonClick);
         // 
         // AddressPortTextBox
         // 
         this.AddressPortTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.AddressPortTextBox.DoubleBuffered = true;
         this.AddressPortTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.AddressPortTextBox.ErrorState = false;
         this.AddressPortTextBox.ErrorToolTip = null;
         this.AddressPortTextBox.ErrorToolTipDuration = 5000;
         this.AddressPortTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.AddressPortTextBox.ErrorToolTipText = "";
         this.AddressPortTextBox.Location = new System.Drawing.Point(289, 44);
         this.AddressPortTextBox.Name = "AddressPortTextBox";
         this.AddressPortTextBox.Size = new System.Drawing.Size(61, 20);
         this.AddressPortTextBox.TabIndex = 10;
         this.AddressPortTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // PasswordTextBox
         // 
         this.PasswordTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.PasswordTextBox.DoubleBuffered = true;
         this.PasswordTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.PasswordTextBox.ErrorState = false;
         this.PasswordTextBox.ErrorToolTip = null;
         this.PasswordTextBox.ErrorToolTipDuration = 5000;
         this.PasswordTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.PasswordTextBox.ErrorToolTipText = "";
         this.PasswordTextBox.Location = new System.Drawing.Point(99, 70);
         this.PasswordTextBox.Name = "PasswordTextBox";
         this.PasswordTextBox.Size = new System.Drawing.Size(251, 20);
         this.PasswordTextBox.TabIndex = 9;
         this.PasswordTextBox.UseSystemPasswordChar = true;
         this.PasswordTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // PasswordLabel
         // 
         this.PasswordLabel.AutoSize = true;
         this.PasswordLabel.Location = new System.Drawing.Point(16, 73);
         this.PasswordLabel.Name = "PasswordLabel";
         this.PasswordLabel.Size = new System.Drawing.Size(56, 13);
         this.PasswordLabel.TabIndex = 8;
         this.PasswordLabel.Text = "Password:";
         // 
         // AddressPortLabel
         // 
         this.AddressPortLabel.AutoSize = true;
         this.AddressPortLabel.Location = new System.Drawing.Point(254, 47);
         this.AddressPortLabel.Name = "AddressPortLabel";
         this.AddressPortLabel.Size = new System.Drawing.Size(29, 13);
         this.AddressPortLabel.TabIndex = 6;
         this.AddressPortLabel.Text = "Port:";
         // 
         // AddressLabel
         // 
         this.AddressLabel.AutoSize = true;
         this.AddressLabel.Location = new System.Drawing.Point(16, 47);
         this.AddressLabel.Name = "AddressLabel";
         this.AddressLabel.Size = new System.Drawing.Size(48, 13);
         this.AddressLabel.TabIndex = 4;
         this.AddressLabel.Text = "Address:";
         // 
         // AddressTextBox
         // 
         this.AddressTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.AddressTextBox.DoubleBuffered = true;
         this.AddressTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.AddressTextBox.ErrorState = false;
         this.AddressTextBox.ErrorToolTip = null;
         this.AddressTextBox.ErrorToolTipDuration = 5000;
         this.AddressTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.AddressTextBox.ErrorToolTipText = "Must be a valid host name or IP address.";
         this.AddressTextBox.Location = new System.Drawing.Point(99, 44);
         this.AddressTextBox.Name = "AddressTextBox";
         this.AddressTextBox.Size = new System.Drawing.Size(139, 20);
         this.AddressTextBox.TabIndex = 5;
         this.AddressTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // ClientNameLabel
         // 
         this.ClientNameLabel.AutoSize = true;
         this.ClientNameLabel.Location = new System.Drawing.Point(16, 21);
         this.ClientNameLabel.Name = "ClientNameLabel";
         this.ClientNameLabel.Size = new System.Drawing.Size(67, 13);
         this.ClientNameLabel.TabIndex = 2;
         this.ClientNameLabel.Text = "Client Name:";
         // 
         // ClientNameTextBox
         // 
         this.ClientNameTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.ClientNameTextBox.DoubleBuffered = true;
         this.ClientNameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.ClientNameTextBox.ErrorState = false;
         this.ClientNameTextBox.ErrorToolTip = null;
         this.ClientNameTextBox.ErrorToolTipDuration = 5000;
         this.ClientNameTextBox.ErrorToolTipPoint = new System.Drawing.Point(230, -20);
         this.ClientNameTextBox.ErrorToolTipText = "Client name can contain only letters, numbers,\r\nand basic symbols (+=-_$&^.[]). I" +
    "t must be at\r\nleast three characters long and must not begin or\r\nend with a dot " +
    "(.) or a space.";
         this.ClientNameTextBox.Location = new System.Drawing.Point(99, 18);
         this.ClientNameTextBox.MaxLength = 100;
         this.ClientNameTextBox.Name = "ClientNameTextBox";
         this.ClientNameTextBox.Size = new System.Drawing.Size(251, 20);
         this.ClientNameTextBox.TabIndex = 3;
         this.ClientNameTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // SlotsTabPage
         // 
         this.SlotsTabPage.Controls.Add(this.SlotsDataGridView);
         this.SlotsTabPage.Controls.Add(this.EditButton);
         this.SlotsTabPage.Location = new System.Drawing.Point(4, 22);
         this.SlotsTabPage.Name = "SlotsTabPage";
         this.SlotsTabPage.Padding = new System.Windows.Forms.Padding(3);
         this.SlotsTabPage.Size = new System.Drawing.Size(369, 280);
         this.SlotsTabPage.TabIndex = 1;
         this.SlotsTabPage.Text = "Slots";
         this.SlotsTabPage.UseVisualStyleBackColor = true;
         // 
         // SlotsDataGridView
         // 
         this.SlotsDataGridView.AllowUserToAddRows = false;
         this.SlotsDataGridView.AllowUserToDeleteRows = false;
         this.SlotsDataGridView.AllowUserToResizeRows = false;
         this.SlotsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.SlotsDataGridView.Location = new System.Drawing.Point(16, 18);
         this.SlotsDataGridView.MultiSelect = false;
         this.SlotsDataGridView.Name = "SlotsDataGridView";
         this.SlotsDataGridView.ReadOnly = true;
         this.SlotsDataGridView.RowHeadersVisible = false;
         this.SlotsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
         this.SlotsDataGridView.Size = new System.Drawing.Size(334, 213);
         this.SlotsDataGridView.TabIndex = 30;
         // 
         // EditButton
         // 
         this.EditButton.Location = new System.Drawing.Point(269, 240);
         this.EditButton.Name = "EditButton";
         this.EditButton.Size = new System.Drawing.Size(81, 25);
         this.EditButton.TabIndex = 29;
         this.EditButton.Text = "Edit";
         this.EditButton.UseVisualStyleBackColor = true;
         this.EditButton.Visible = false;
         // 
         // FahClientSetupDialog
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(392, 385);
         this.Controls.Add(this.SetupTabControl);
         this.Controls.Add(this.DummyTextBox);
         this.Controls.Add(this.ClientTimeOffsetLabel);
         this.Controls.Add(this.ClientTimeOffsetUpDown);
         this.Controls.Add(this.ClientNoUtcOffsetCheckBox);
         this.Controls.Add(this.DialogCancelButton);
         this.Controls.Add(this.DialogOkButton);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "FahClientSetupDialog";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Client Setup";
         this.Shown += new System.EventHandler(this.FahClientSetupDialogShown);
         ((System.ComponentModel.ISupportInitialize)(this.ClientTimeOffsetUpDown)).EndInit();
         this.SetupTabControl.ResumeLayout(false);
         this.ConnectionTabPage.ResumeLayout(false);
         this.ConnectionTabPage.PerformLayout();
         this.SlotsTabPage.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.SlotsDataGridView)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private Controls.TextBoxWrapper DummyTextBox;
      private Controls.LabelWrapper ClientTimeOffsetLabel;
      private System.Windows.Forms.NumericUpDown ClientTimeOffsetUpDown;
      private Controls.CheckBoxWrapper ClientNoUtcOffsetCheckBox;
      private Controls.ButtonWrapper DialogCancelButton;
      private Controls.ButtonWrapper DialogOkButton;
      private System.Windows.Forms.TabControl SetupTabControl;
      private System.Windows.Forms.TabPage ConnectionTabPage;
      private System.Windows.Forms.TabPage SlotsTabPage;
      private Controls.LabelWrapper ClientNameLabel;
      private harlam357.Windows.Forms.ValidatingTextBox ClientNameTextBox;
      private Controls.LabelWrapper AddressLabel;
      private harlam357.Windows.Forms.ValidatingTextBox AddressTextBox;
      private Controls.LabelWrapper AddressPortLabel;
      private harlam357.Windows.Forms.ValidatingTextBox PasswordTextBox;
      private Controls.LabelWrapper PasswordLabel;
      private harlam357.Windows.Forms.ValidatingTextBox AddressPortTextBox;
      private System.Windows.Forms.ToolTip toolTip1;
      private Controls.ButtonWrapper ConnectButton;
      private Controls.ButtonWrapper EditButton;
      private System.Windows.Forms.DataGridView SlotsDataGridView;

   }
}