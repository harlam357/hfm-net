namespace HFM.Forms.Views
{
   partial class ProteinLoadResultsDialog
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
            this.changesListBox = new System.Windows.Forms.ListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ProteinListBox
            // 
            this.changesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.changesListBox.FormattingEnabled = true;
            this.changesListBox.Location = new System.Drawing.Point(12, 12);
            this.changesListBox.Name = "changesListBox";
            this.changesListBox.Size = new System.Drawing.Size(260, 225);
            this.changesListBox.TabIndex = 2;
            // 
            // DialogOkButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.okButton.Location = new System.Drawing.Point(97, 249);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(91, 29);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // ProteinLoadResultsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 287);
            this.Controls.Add(this.changesListBox);
            this.Controls.Add(this.okButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 325);
            this.Name = "ProteinLoadResultsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Load Results";
            this.Load += new System.EventHandler(this.ProteinLoadResultsDialog_Load);
            this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Button okButton;
      private System.Windows.Forms.ListBox changesListBox;
   }
}
