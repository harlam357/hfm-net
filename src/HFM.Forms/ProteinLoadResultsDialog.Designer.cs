namespace HFM.Forms
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProteinLoadResultsDialog));
         this.ProteinListBox = new System.Windows.Forms.ListBox();
         this.DialogOkButton = new HFM.Forms.Controls.ButtonWrapper();
         this.SuspendLayout();
         // 
         // ProteinListBox
         // 
         this.ProteinListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.ProteinListBox.FormattingEnabled = true;
         this.ProteinListBox.Location = new System.Drawing.Point(12, 12);
         this.ProteinListBox.Name = "ProteinListBox";
         this.ProteinListBox.Size = new System.Drawing.Size(260, 225);
         this.ProteinListBox.TabIndex = 2;
         // 
         // DialogOkButton
         // 
         this.DialogOkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
         this.DialogOkButton.Location = new System.Drawing.Point(97, 249);
         this.DialogOkButton.Name = "DialogOkButton";
         this.DialogOkButton.Size = new System.Drawing.Size(91, 29);
         this.DialogOkButton.TabIndex = 0;
         this.DialogOkButton.Text = "OK";
         this.DialogOkButton.UseVisualStyleBackColor = true;
         this.DialogOkButton.Click += new System.EventHandler(this.DialogOkButtonClick);
         // 
         // ProteinLoadResultsDialog
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(284, 287);
         this.Controls.Add(this.ProteinListBox);
         this.Controls.Add(this.DialogOkButton);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.MinimumSize = new System.Drawing.Size(300, 325);
         this.Name = "ProteinLoadResultsDialog";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Project Load Results";
         this.ResumeLayout(false);

      }

      #endregion

      private Controls.ButtonWrapper DialogOkButton;
      private System.Windows.Forms.ListBox ProteinListBox;
   }
}