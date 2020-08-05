
namespace HFM.Forms
{
   partial class ApplicationUpdateDialog
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
            this.captionLabel = new System.Windows.Forms.Label();
            this.thisVersionLabel = new System.Windows.Forms.Label();
            this.newVersionLabel = new System.Windows.Forms.Label();
            this.downloadProgressLabel = new System.Windows.Forms.Label();
            this.updateFilesComboBox = new System.Windows.Forms.ComboBox();
            this.downloadButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.downloadProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // lblFirstLine
            // 
            this.captionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.captionLabel.Location = new System.Drawing.Point(12, 13);
            this.captionLabel.Name = "captionLabel";
            this.captionLabel.Size = new System.Drawing.Size(267, 31);
            this.captionLabel.TabIndex = 0;
            this.captionLabel.Text = "A new version of...";
            // 
            // lblYourVersion
            // 
            this.thisVersionLabel.AutoSize = true;
            this.thisVersionLabel.Location = new System.Drawing.Point(12, 54);
            this.thisVersionLabel.Name = "thisVersionLabel";
            this.thisVersionLabel.Size = new System.Drawing.Size(0, 13);
            this.thisVersionLabel.TabIndex = 1;
            this.thisVersionLabel.Text = "This version: ";
            // 
            // lblCurrentVersion
            // 
            this.newVersionLabel.AutoSize = true;
            this.newVersionLabel.Location = new System.Drawing.Point(12, 77);
            this.newVersionLabel.Name = "newVersionLabel";
            this.newVersionLabel.Size = new System.Drawing.Size(0, 13);
            this.newVersionLabel.TabIndex = 2;
            this.newVersionLabel.Text = "New version: ";
            // 
            // lblSelectDownload
            // 
            this.downloadProgressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadProgressLabel.Location = new System.Drawing.Point(10, 113);
            this.downloadProgressLabel.Name = "downloadProgressLabel";
            this.downloadProgressLabel.Size = new System.Drawing.Size(267, 18);
            this.downloadProgressLabel.TabIndex = 3;
            this.downloadProgressLabel.Text = "Please select an update to download.";
            this.downloadProgressLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cboUpdateFiles
            // 
            this.updateFilesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.updateFilesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.updateFilesComboBox.FormattingEnabled = true;
            this.updateFilesComboBox.Location = new System.Drawing.Point(44, 134);
            this.updateFilesComboBox.Name = "updateFilesComboBox";
            this.updateFilesComboBox.Size = new System.Drawing.Size(199, 21);
            this.updateFilesComboBox.TabIndex = 4;
            // 
            // DownloadButton
            // 
            this.downloadButton.Location = new System.Drawing.Point(63, 176);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(75, 23);
            this.downloadButton.TabIndex = 5;
            this.downloadButton.Text = "Download";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // btnCancel
            // 
            this.cancelButton.Location = new System.Drawing.Point(149, 176);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // progressDownload
            // 
            this.downloadProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadProgressBar.Location = new System.Drawing.Point(10, 132);
            this.downloadProgressBar.Name = "downloadProgressBar";
            this.downloadProgressBar.Size = new System.Drawing.Size(267, 25);
            this.downloadProgressBar.Step = 1;
            this.downloadProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.downloadProgressBar.TabIndex = 7;
            this.downloadProgressBar.Visible = false;
            // 
            // ApplicationUpdateDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 211);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.updateFilesComboBox);
            this.Controls.Add(this.downloadProgressLabel);
            this.Controls.Add(this.newVersionLabel);
            this.Controls.Add(this.thisVersionLabel);
            this.Controls.Add(this.captionLabel);
            this.Controls.Add(this.downloadProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ApplicationUpdateDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Application Update";
            this.Load += new System.EventHandler(this.ApplicationUpdateDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label captionLabel;
      private System.Windows.Forms.Label thisVersionLabel;
      private System.Windows.Forms.Label newVersionLabel;
      private System.Windows.Forms.Label downloadProgressLabel;
      private System.Windows.Forms.ComboBox updateFilesComboBox;
      private System.Windows.Forms.Button downloadButton;
      private System.Windows.Forms.Button cancelButton;
      private System.Windows.Forms.ProgressBar downloadProgressBar;
   }
}
