namespace HFM.Client.Tool
{
   partial class MainForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
         this.HostAddressTextBox = new System.Windows.Forms.TextBox();
         this.HostAddressLabel = new System.Windows.Forms.Label();
         this.PortTextBox = new System.Windows.Forms.TextBox();
         this.PortLabel = new System.Windows.Forms.Label();
         this.PasswordTextBox = new System.Windows.Forms.TextBox();
         this.PasswordLabel = new System.Windows.Forms.Label();
         this.ConnectButton = new System.Windows.Forms.Button();
         this.MessageDisplayTextBox = new System.Windows.Forms.TextBox();
         this.CommandTextBox = new System.Windows.Forms.TextBox();
         this.CommandLabel = new System.Windows.Forms.Label();
         this.SendCommandButton = new System.Windows.Forms.Button();
         this.StatusStrip = new System.Windows.Forms.StatusStrip();
         this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
         this.CloseButton = new System.Windows.Forms.Button();
         this.ConnectionGroupBox = new System.Windows.Forms.GroupBox();
         this.DataReceivedValueLabel = new System.Windows.Forms.Label();
         this.DataSentValueLabel = new System.Windows.Forms.Label();
         this.DataReceivedLabel = new System.Windows.Forms.Label();
         this.DataSentLabel = new System.Windows.Forms.Label();
         this.ClearMessagesButton = new System.Windows.Forms.Button();
         this.ClientCommandGroupBox = new System.Windows.Forms.GroupBox();
         this.StatusMessageListBox = new System.Windows.Forms.ListBox();
         this.StatusStrip.SuspendLayout();
         this.ConnectionGroupBox.SuspendLayout();
         this.ClientCommandGroupBox.SuspendLayout();
         this.SuspendLayout();
         // 
         // HostAddressTextBox
         // 
         this.HostAddressTextBox.Location = new System.Drawing.Point(87, 19);
         this.HostAddressTextBox.Name = "HostAddressTextBox";
         this.HostAddressTextBox.Size = new System.Drawing.Size(193, 20);
         this.HostAddressTextBox.TabIndex = 1;
         // 
         // HostAddressLabel
         // 
         this.HostAddressLabel.AutoSize = true;
         this.HostAddressLabel.Location = new System.Drawing.Point(8, 22);
         this.HostAddressLabel.Name = "HostAddressLabel";
         this.HostAddressLabel.Size = new System.Drawing.Size(73, 13);
         this.HostAddressLabel.TabIndex = 0;
         this.HostAddressLabel.Text = "Host Address:";
         // 
         // PortTextBox
         // 
         this.PortTextBox.Location = new System.Drawing.Point(324, 19);
         this.PortTextBox.Name = "PortTextBox";
         this.PortTextBox.Size = new System.Drawing.Size(66, 20);
         this.PortTextBox.TabIndex = 3;
         this.PortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitsOnlyKeyPress);
         // 
         // PortLabel
         // 
         this.PortLabel.AutoSize = true;
         this.PortLabel.Location = new System.Drawing.Point(289, 22);
         this.PortLabel.Name = "PortLabel";
         this.PortLabel.Size = new System.Drawing.Size(29, 13);
         this.PortLabel.TabIndex = 2;
         this.PortLabel.Text = "Port:";
         // 
         // PasswordTextBox
         // 
         this.PasswordTextBox.Location = new System.Drawing.Point(87, 45);
         this.PasswordTextBox.Name = "PasswordTextBox";
         this.PasswordTextBox.Size = new System.Drawing.Size(193, 20);
         this.PasswordTextBox.TabIndex = 5;
         // 
         // PasswordLabel
         // 
         this.PasswordLabel.AutoSize = true;
         this.PasswordLabel.Location = new System.Drawing.Point(25, 49);
         this.PasswordLabel.Name = "PasswordLabel";
         this.PasswordLabel.Size = new System.Drawing.Size(56, 13);
         this.PasswordLabel.TabIndex = 4;
         this.PasswordLabel.Text = "Password:";
         // 
         // ConnectButton
         // 
         this.ConnectButton.Location = new System.Drawing.Point(12, 80);
         this.ConnectButton.Name = "ConnectButton";
         this.ConnectButton.Size = new System.Drawing.Size(119, 28);
         this.ConnectButton.TabIndex = 6;
         this.ConnectButton.Text = "Connect";
         this.ConnectButton.UseVisualStyleBackColor = true;
         this.ConnectButton.Click += new System.EventHandler(this.ConnectButtonClick);
         // 
         // MessageDisplayTextBox
         // 
         this.MessageDisplayTextBox.AcceptsReturn = true;
         this.MessageDisplayTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.MessageDisplayTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.MessageDisplayTextBox.Location = new System.Drawing.Point(12, 222);
         this.MessageDisplayTextBox.Multiline = true;
         this.MessageDisplayTextBox.Name = "MessageDisplayTextBox";
         this.MessageDisplayTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.MessageDisplayTextBox.Size = new System.Drawing.Size(675, 406);
         this.MessageDisplayTextBox.TabIndex = 2;
         this.MessageDisplayTextBox.WordWrap = false;
         // 
         // CommandTextBox
         // 
         this.CommandTextBox.Location = new System.Drawing.Point(87, 19);
         this.CommandTextBox.Name = "CommandTextBox";
         this.CommandTextBox.Size = new System.Drawing.Size(193, 20);
         this.CommandTextBox.TabIndex = 1;
         // 
         // CommandLabel
         // 
         this.CommandLabel.AutoSize = true;
         this.CommandLabel.Location = new System.Drawing.Point(24, 22);
         this.CommandLabel.Name = "CommandLabel";
         this.CommandLabel.Size = new System.Drawing.Size(57, 13);
         this.CommandLabel.TabIndex = 0;
         this.CommandLabel.Text = "Command:";
         // 
         // SendCommandButton
         // 
         this.SendCommandButton.Location = new System.Drawing.Point(292, 14);
         this.SendCommandButton.Name = "SendCommandButton";
         this.SendCommandButton.Size = new System.Drawing.Size(119, 28);
         this.SendCommandButton.TabIndex = 2;
         this.SendCommandButton.Text = "Send Command";
         this.SendCommandButton.UseVisualStyleBackColor = true;
         this.SendCommandButton.Click += new System.EventHandler(this.SendCommandButtonClick);
         // 
         // StatusStrip
         // 
         this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
         this.StatusStrip.Location = new System.Drawing.Point(0, 640);
         this.StatusStrip.Name = "StatusStrip";
         this.StatusStrip.Size = new System.Drawing.Size(699, 22);
         this.StatusStrip.TabIndex = 3;
         this.StatusStrip.Text = "statusStrip1";
         // 
         // StatusLabel
         // 
         this.StatusLabel.ForeColor = System.Drawing.Color.Blue;
         this.StatusLabel.Name = "StatusLabel";
         this.StatusLabel.Size = new System.Drawing.Size(684, 17);
         this.StatusLabel.Spring = true;
         // 
         // CloseButton
         // 
         this.CloseButton.Enabled = false;
         this.CloseButton.Location = new System.Drawing.Point(152, 80);
         this.CloseButton.Name = "CloseButton";
         this.CloseButton.Size = new System.Drawing.Size(119, 28);
         this.CloseButton.TabIndex = 7;
         this.CloseButton.Text = "Close";
         this.CloseButton.UseVisualStyleBackColor = true;
         this.CloseButton.Click += new System.EventHandler(this.CloseButtonClick);
         // 
         // ConnectionGroupBox
         // 
         this.ConnectionGroupBox.Controls.Add(this.DataReceivedValueLabel);
         this.ConnectionGroupBox.Controls.Add(this.DataSentValueLabel);
         this.ConnectionGroupBox.Controls.Add(this.DataReceivedLabel);
         this.ConnectionGroupBox.Controls.Add(this.DataSentLabel);
         this.ConnectionGroupBox.Controls.Add(this.ClearMessagesButton);
         this.ConnectionGroupBox.Controls.Add(this.ConnectButton);
         this.ConnectionGroupBox.Controls.Add(this.CloseButton);
         this.ConnectionGroupBox.Controls.Add(this.HostAddressTextBox);
         this.ConnectionGroupBox.Controls.Add(this.HostAddressLabel);
         this.ConnectionGroupBox.Controls.Add(this.PortTextBox);
         this.ConnectionGroupBox.Controls.Add(this.PortLabel);
         this.ConnectionGroupBox.Controls.Add(this.PasswordLabel);
         this.ConnectionGroupBox.Controls.Add(this.PasswordTextBox);
         this.ConnectionGroupBox.Location = new System.Drawing.Point(12, 12);
         this.ConnectionGroupBox.Name = "ConnectionGroupBox";
         this.ConnectionGroupBox.Size = new System.Drawing.Size(422, 145);
         this.ConnectionGroupBox.TabIndex = 0;
         this.ConnectionGroupBox.TabStop = false;
         this.ConnectionGroupBox.Text = "Connection";
         // 
         // DataReceivedValueLabel
         // 
         this.DataReceivedValueLabel.Location = new System.Drawing.Point(281, 120);
         this.DataReceivedValueLabel.Name = "DataReceivedValueLabel";
         this.DataReceivedValueLabel.Size = new System.Drawing.Size(112, 13);
         this.DataReceivedValueLabel.TabIndex = 12;
         this.DataReceivedValueLabel.Text = "0 KBs";
         // 
         // DataSentValueLabel
         // 
         this.DataSentValueLabel.Location = new System.Drawing.Point(62, 120);
         this.DataSentValueLabel.Name = "DataSentValueLabel";
         this.DataSentValueLabel.Size = new System.Drawing.Size(112, 13);
         this.DataSentValueLabel.TabIndex = 11;
         this.DataSentValueLabel.Text = "0 KBs";
         // 
         // DataReceivedLabel
         // 
         this.DataReceivedLabel.AutoSize = true;
         this.DataReceivedLabel.Location = new System.Drawing.Point(219, 120);
         this.DataReceivedLabel.Name = "DataReceivedLabel";
         this.DataReceivedLabel.Size = new System.Drawing.Size(56, 13);
         this.DataReceivedLabel.TabIndex = 10;
         this.DataReceivedLabel.Text = "Received:";
         // 
         // DataSentLabel
         // 
         this.DataSentLabel.AutoSize = true;
         this.DataSentLabel.Location = new System.Drawing.Point(24, 120);
         this.DataSentLabel.Name = "DataSentLabel";
         this.DataSentLabel.Size = new System.Drawing.Size(32, 13);
         this.DataSentLabel.TabIndex = 9;
         this.DataSentLabel.Text = "Sent:";
         // 
         // ClearMessagesButton
         // 
         this.ClearMessagesButton.Location = new System.Drawing.Point(292, 80);
         this.ClearMessagesButton.Name = "ClearMessagesButton";
         this.ClearMessagesButton.Size = new System.Drawing.Size(119, 28);
         this.ClearMessagesButton.TabIndex = 8;
         this.ClearMessagesButton.Text = "Clear Messages";
         this.ClearMessagesButton.UseVisualStyleBackColor = true;
         this.ClearMessagesButton.Click += new System.EventHandler(this.ClearMessagesButtonClick);
         // 
         // ClientCommandGroupBox
         // 
         this.ClientCommandGroupBox.Controls.Add(this.CommandLabel);
         this.ClientCommandGroupBox.Controls.Add(this.CommandTextBox);
         this.ClientCommandGroupBox.Controls.Add(this.SendCommandButton);
         this.ClientCommandGroupBox.Location = new System.Drawing.Point(12, 163);
         this.ClientCommandGroupBox.Name = "ClientCommandGroupBox";
         this.ClientCommandGroupBox.Size = new System.Drawing.Size(422, 53);
         this.ClientCommandGroupBox.TabIndex = 1;
         this.ClientCommandGroupBox.TabStop = false;
         this.ClientCommandGroupBox.Text = "Client Command";
         // 
         // StatusMessageListBox
         // 
         this.StatusMessageListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.StatusMessageListBox.FormattingEnabled = true;
         this.StatusMessageListBox.Location = new System.Drawing.Point(440, 25);
         this.StatusMessageListBox.Name = "StatusMessageListBox";
         this.StatusMessageListBox.Size = new System.Drawing.Size(246, 184);
         this.StatusMessageListBox.TabIndex = 4;
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(699, 662);
         this.Controls.Add(this.StatusMessageListBox);
         this.Controls.Add(this.ClientCommandGroupBox);
         this.Controls.Add(this.ConnectionGroupBox);
         this.Controls.Add(this.StatusStrip);
         this.Controls.Add(this.MessageDisplayTextBox);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MinimumSize = new System.Drawing.Size(715, 500);
         this.Name = "MainForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "HFM Client Tool";
         this.StatusStrip.ResumeLayout(false);
         this.StatusStrip.PerformLayout();
         this.ConnectionGroupBox.ResumeLayout(false);
         this.ConnectionGroupBox.PerformLayout();
         this.ClientCommandGroupBox.ResumeLayout(false);
         this.ClientCommandGroupBox.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox HostAddressTextBox;
      private System.Windows.Forms.Label HostAddressLabel;
      private System.Windows.Forms.TextBox PortTextBox;
      private System.Windows.Forms.Label PortLabel;
      private System.Windows.Forms.TextBox PasswordTextBox;
      private System.Windows.Forms.Label PasswordLabel;
      private System.Windows.Forms.Button ConnectButton;
      private System.Windows.Forms.TextBox MessageDisplayTextBox;
      private System.Windows.Forms.TextBox CommandTextBox;
      private System.Windows.Forms.Label CommandLabel;
      private System.Windows.Forms.Button SendCommandButton;
      private System.Windows.Forms.StatusStrip StatusStrip;
      private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
      private System.Windows.Forms.Button CloseButton;
      private System.Windows.Forms.GroupBox ConnectionGroupBox;
      private System.Windows.Forms.GroupBox ClientCommandGroupBox;
      private System.Windows.Forms.ListBox StatusMessageListBox;
      private System.Windows.Forms.Button ClearMessagesButton;
      private System.Windows.Forms.Label DataSentValueLabel;
      private System.Windows.Forms.Label DataReceivedLabel;
      private System.Windows.Forms.Label DataSentLabel;
      private System.Windows.Forms.Label DataReceivedValueLabel;
   }
}

