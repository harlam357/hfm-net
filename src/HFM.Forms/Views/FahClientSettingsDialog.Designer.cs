namespace HFM.Forms.Views
{
   partial class FahClientSettingsDialog
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
            this.DummyTextBox = new System.Windows.Forms.TextBox();
            this.DialogCancelButton = new System.Windows.Forms.Button();
            this.DialogOkButton = new System.Windows.Forms.Button();
            this.SetupTabControl = new System.Windows.Forms.TabControl();
            this.ConnectionTabPage = new System.Windows.Forms.TabPage();
            this.SlotsDataGridView = new System.Windows.Forms.DataGridView();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.AddressPortTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.PasswordTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.AddressPortLabel = new System.Windows.Forms.Label();
            this.AddressLabel = new System.Windows.Forms.Label();
            this.AddressTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.ClientNameLabel = new System.Windows.Forms.Label();
            this.ClientNameTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.SetupTabControl.SuspendLayout();
            this.ConnectionTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SlotsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // DummyTextBox
            // 
            this.DummyTextBox.Location = new System.Drawing.Point(331, 298);
            this.DummyTextBox.Name = "DummyTextBox";
            this.DummyTextBox.Size = new System.Drawing.Size(49, 20);
            this.DummyTextBox.TabIndex = 6;
            // 
            // DialogCancelButton
            // 
            this.DialogCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DialogCancelButton.CausesValidation = false;
            this.DialogCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.DialogCancelButton.Location = new System.Drawing.Point(304, 326);
            this.DialogCancelButton.Name = "DialogCancelButton";
            this.DialogCancelButton.Size = new System.Drawing.Size(81, 25);
            this.DialogCancelButton.TabIndex = 5;
            this.DialogCancelButton.Text = "Cancel";
            this.DialogCancelButton.UseVisualStyleBackColor = true;
            // 
            // DialogOkButton
            // 
            this.DialogOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DialogOkButton.Location = new System.Drawing.Point(212, 326);
            this.DialogOkButton.Name = "DialogOkButton";
            this.DialogOkButton.Size = new System.Drawing.Size(81, 25);
            this.DialogOkButton.TabIndex = 4;
            this.DialogOkButton.Text = "OK";
            this.DialogOkButton.UseVisualStyleBackColor = true;
            this.DialogOkButton.Click += new System.EventHandler(this.DialogOkButtonClick);
            // 
            // SetupTabControl
            // 
            this.SetupTabControl.Controls.Add(this.ConnectionTabPage);
            this.SetupTabControl.Location = new System.Drawing.Point(8, 12);
            this.SetupTabControl.Name = "SetupTabControl";
            this.SetupTabControl.SelectedIndex = 0;
            this.SetupTabControl.Size = new System.Drawing.Size(377, 284);
            this.SetupTabControl.TabIndex = 0;
            // 
            // ConnectionTabPage
            // 
            this.ConnectionTabPage.Controls.Add(this.SlotsDataGridView);
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
            this.ConnectionTabPage.Size = new System.Drawing.Size(369, 258);
            this.ConnectionTabPage.TabIndex = 0;
            this.ConnectionTabPage.Text = "Connection";
            this.ConnectionTabPage.UseVisualStyleBackColor = true;
            // 
            // SlotsDataGridView
            // 
            this.SlotsDataGridView.AllowUserToAddRows = false;
            this.SlotsDataGridView.AllowUserToDeleteRows = false;
            this.SlotsDataGridView.AllowUserToResizeRows = false;
            this.SlotsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SlotsDataGridView.Location = new System.Drawing.Point(19, 102);
            this.SlotsDataGridView.MultiSelect = false;
            this.SlotsDataGridView.Name = "SlotsDataGridView";
            this.SlotsDataGridView.ReadOnly = true;
            this.SlotsDataGridView.RowHeadersVisible = false;
            this.SlotsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.SlotsDataGridView.Size = new System.Drawing.Size(331, 137);
            this.SlotsDataGridView.TabIndex = 9;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(247, 69);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(103, 25);
            this.ConnectButton.TabIndex = 8;
            this.ConnectButton.Text = "Test Connection";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButtonClick);
            // 
            // AddressPortTextBox
            // 
            this.AddressPortTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.AddressPortTextBox.DoubleBuffered = true;
            this.AddressPortTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.AddressPortTextBox.ErrorToolTip = this.toolTip1;
            this.AddressPortTextBox.ErrorToolTipDuration = 5000;
            this.AddressPortTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.AddressPortTextBox.ErrorToolTipText = "";
            this.AddressPortTextBox.Location = new System.Drawing.Point(289, 44);
            this.AddressPortTextBox.Name = "AddressPortTextBox";
            this.AddressPortTextBox.Size = new System.Drawing.Size(61, 20);
            this.AddressPortTextBox.TabIndex = 5;
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.PasswordTextBox.DoubleBuffered = true;
            this.PasswordTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.PasswordTextBox.ErrorToolTip = this.toolTip1;
            this.PasswordTextBox.ErrorToolTipDuration = 5000;
            this.PasswordTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.PasswordTextBox.ErrorToolTipText = "";
            this.PasswordTextBox.Location = new System.Drawing.Point(99, 70);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.Size = new System.Drawing.Size(139, 20);
            this.PasswordTextBox.TabIndex = 7;
            this.PasswordTextBox.UseSystemPasswordChar = true;
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new System.Drawing.Point(16, 73);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(56, 13);
            this.PasswordLabel.TabIndex = 6;
            this.PasswordLabel.Text = "Password:";
            // 
            // AddressPortLabel
            // 
            this.AddressPortLabel.AutoSize = true;
            this.AddressPortLabel.Location = new System.Drawing.Point(254, 47);
            this.AddressPortLabel.Name = "AddressPortLabel";
            this.AddressPortLabel.Size = new System.Drawing.Size(29, 13);
            this.AddressPortLabel.TabIndex = 4;
            this.AddressPortLabel.Text = "Port:";
            // 
            // AddressLabel
            // 
            this.AddressLabel.AutoSize = true;
            this.AddressLabel.Location = new System.Drawing.Point(16, 47);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(48, 13);
            this.AddressLabel.TabIndex = 2;
            this.AddressLabel.Text = "Address:";
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.AddressTextBox.DoubleBuffered = true;
            this.AddressTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.AddressTextBox.ErrorToolTip = this.toolTip1;
            this.AddressTextBox.ErrorToolTipDuration = 5000;
            this.AddressTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.AddressTextBox.ErrorToolTipText = "";
            this.AddressTextBox.Location = new System.Drawing.Point(99, 44);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(139, 20);
            this.AddressTextBox.TabIndex = 3;
            // 
            // ClientNameLabel
            // 
            this.ClientNameLabel.AutoSize = true;
            this.ClientNameLabel.Location = new System.Drawing.Point(16, 21);
            this.ClientNameLabel.Name = "ClientNameLabel";
            this.ClientNameLabel.Size = new System.Drawing.Size(67, 13);
            this.ClientNameLabel.TabIndex = 0;
            this.ClientNameLabel.Text = "Client Name:";
            // 
            // ClientNameTextBox
            // 
            this.ClientNameTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.ClientNameTextBox.DoubleBuffered = true;
            this.ClientNameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.ClientNameTextBox.ErrorToolTip = this.toolTip1;
            this.ClientNameTextBox.ErrorToolTipDuration = 5000;
            this.ClientNameTextBox.ErrorToolTipPoint = new System.Drawing.Point(230, -20);
            this.ClientNameTextBox.ErrorToolTipText = "";
            this.ClientNameTextBox.Location = new System.Drawing.Point(99, 18);
            this.ClientNameTextBox.MaxLength = 100;
            this.ClientNameTextBox.Name = "ClientNameTextBox";
            this.ClientNameTextBox.Size = new System.Drawing.Size(251, 20);
            this.ClientNameTextBox.TabIndex = 1;
            // 
            // FahClientSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 361);
            this.Controls.Add(this.SetupTabControl);
            this.Controls.Add(this.DummyTextBox);
            this.Controls.Add(this.DialogCancelButton);
            this.Controls.Add(this.DialogOkButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FahClientSettingsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Client Setup";
            this.Load += new System.EventHandler(this.FahClientSettingsDialog_Load);
            this.Shown += new System.EventHandler(this.FahClientSettingsDialog_Shown);
            this.SetupTabControl.ResumeLayout(false);
            this.ConnectionTabPage.ResumeLayout(false);
            this.ConnectionTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SlotsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox DummyTextBox;
      private System.Windows.Forms.Button DialogCancelButton;
      private System.Windows.Forms.Button DialogOkButton;
      private System.Windows.Forms.TabControl SetupTabControl;
      private System.Windows.Forms.TabPage ConnectionTabPage;
      private System.Windows.Forms.Label ClientNameLabel;
      private HFM.Forms.Controls.DataErrorTextBox ClientNameTextBox;
      private System.Windows.Forms.Label AddressLabel;
      private HFM.Forms.Controls.DataErrorTextBox AddressTextBox;
      private System.Windows.Forms.Label AddressPortLabel;
      private HFM.Forms.Controls.DataErrorTextBox PasswordTextBox;
      private System.Windows.Forms.Label PasswordLabel;
      private HFM.Forms.Controls.DataErrorTextBox AddressPortTextBox;
      private System.Windows.Forms.ToolTip toolTip1;
      private System.Windows.Forms.Button ConnectButton;
      private System.Windows.Forms.DataGridView SlotsDataGridView;

   }
}
