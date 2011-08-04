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
         this.StatusStrip.SuspendLayout();
         this.SuspendLayout();
         // 
         // HostAddressTextBox
         // 
         this.HostAddressTextBox.Location = new System.Drawing.Point(99, 12);
         this.HostAddressTextBox.Name = "HostAddressTextBox";
         this.HostAddressTextBox.Size = new System.Drawing.Size(193, 20);
         this.HostAddressTextBox.TabIndex = 0;
         // 
         // HostAddressLabel
         // 
         this.HostAddressLabel.AutoSize = true;
         this.HostAddressLabel.Location = new System.Drawing.Point(20, 15);
         this.HostAddressLabel.Name = "HostAddressLabel";
         this.HostAddressLabel.Size = new System.Drawing.Size(73, 13);
         this.HostAddressLabel.TabIndex = 1;
         this.HostAddressLabel.Text = "Host Address:";
         // 
         // PortTextBox
         // 
         this.PortTextBox.Location = new System.Drawing.Point(343, 12);
         this.PortTextBox.Name = "PortTextBox";
         this.PortTextBox.Size = new System.Drawing.Size(66, 20);
         this.PortTextBox.TabIndex = 2;
         this.PortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitsOnlyKeyPress);
         // 
         // PortLabel
         // 
         this.PortLabel.AutoSize = true;
         this.PortLabel.Location = new System.Drawing.Point(308, 15);
         this.PortLabel.Name = "PortLabel";
         this.PortLabel.Size = new System.Drawing.Size(29, 13);
         this.PortLabel.TabIndex = 3;
         this.PortLabel.Text = "Port:";
         // 
         // PasswordTextBox
         // 
         this.PasswordTextBox.Location = new System.Drawing.Point(99, 36);
         this.PasswordTextBox.Name = "PasswordTextBox";
         this.PasswordTextBox.Size = new System.Drawing.Size(193, 20);
         this.PasswordTextBox.TabIndex = 4;
         // 
         // PasswordLabel
         // 
         this.PasswordLabel.AutoSize = true;
         this.PasswordLabel.Location = new System.Drawing.Point(37, 40);
         this.PasswordLabel.Name = "PasswordLabel";
         this.PasswordLabel.Size = new System.Drawing.Size(56, 13);
         this.PasswordLabel.TabIndex = 5;
         this.PasswordLabel.Text = "Password:";
         // 
         // ConnectButton
         // 
         this.ConnectButton.Location = new System.Drawing.Point(420, 15);
         this.ConnectButton.Name = "ConnectButton";
         this.ConnectButton.Size = new System.Drawing.Size(135, 41);
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
         this.MessageDisplayTextBox.Location = new System.Drawing.Point(12, 167);
         this.MessageDisplayTextBox.Multiline = true;
         this.MessageDisplayTextBox.Name = "MessageDisplayTextBox";
         this.MessageDisplayTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.MessageDisplayTextBox.Size = new System.Drawing.Size(546, 410);
         this.MessageDisplayTextBox.TabIndex = 7;
         this.MessageDisplayTextBox.WordWrap = false;
         // 
         // CommandTextBox
         // 
         this.CommandTextBox.Location = new System.Drawing.Point(99, 131);
         this.CommandTextBox.Name = "CommandTextBox";
         this.CommandTextBox.Size = new System.Drawing.Size(193, 20);
         this.CommandTextBox.TabIndex = 8;
         // 
         // CommandLabel
         // 
         this.CommandLabel.AutoSize = true;
         this.CommandLabel.Location = new System.Drawing.Point(36, 134);
         this.CommandLabel.Name = "CommandLabel";
         this.CommandLabel.Size = new System.Drawing.Size(57, 13);
         this.CommandLabel.TabIndex = 9;
         this.CommandLabel.Text = "Command:";
         // 
         // SendCommandButton
         // 
         this.SendCommandButton.Location = new System.Drawing.Point(420, 120);
         this.SendCommandButton.Name = "SendCommandButton";
         this.SendCommandButton.Size = new System.Drawing.Size(135, 41);
         this.SendCommandButton.TabIndex = 10;
         this.SendCommandButton.Text = "Send Command";
         this.SendCommandButton.UseVisualStyleBackColor = true;
         this.SendCommandButton.Click += new System.EventHandler(this.SendCommandButtonClick);
         // 
         // StatusStrip
         // 
         this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
         this.StatusStrip.Location = new System.Drawing.Point(0, 589);
         this.StatusStrip.Name = "StatusStrip";
         this.StatusStrip.Size = new System.Drawing.Size(570, 22);
         this.StatusStrip.TabIndex = 11;
         this.StatusStrip.Text = "statusStrip1";
         // 
         // StatusLabel
         // 
         this.StatusLabel.ForeColor = System.Drawing.Color.Blue;
         this.StatusLabel.Name = "StatusLabel";
         this.StatusLabel.Size = new System.Drawing.Size(555, 17);
         this.StatusLabel.Spring = true;
         // 
         // CloseButton
         // 
         this.CloseButton.Location = new System.Drawing.Point(420, 62);
         this.CloseButton.Name = "CloseButton";
         this.CloseButton.Size = new System.Drawing.Size(135, 41);
         this.CloseButton.TabIndex = 12;
         this.CloseButton.Text = "Close";
         this.CloseButton.UseVisualStyleBackColor = true;
         this.CloseButton.Click += new System.EventHandler(this.CloseButtonClick);
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(570, 611);
         this.Controls.Add(this.CloseButton);
         this.Controls.Add(this.StatusStrip);
         this.Controls.Add(this.SendCommandButton);
         this.Controls.Add(this.CommandLabel);
         this.Controls.Add(this.CommandTextBox);
         this.Controls.Add(this.MessageDisplayTextBox);
         this.Controls.Add(this.ConnectButton);
         this.Controls.Add(this.PasswordLabel);
         this.Controls.Add(this.PasswordTextBox);
         this.Controls.Add(this.PortLabel);
         this.Controls.Add(this.PortTextBox);
         this.Controls.Add(this.HostAddressLabel);
         this.Controls.Add(this.HostAddressTextBox);
         this.Name = "MainForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "HFM Client Tool";
         this.StatusStrip.ResumeLayout(false);
         this.StatusStrip.PerformLayout();
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
   }
}

