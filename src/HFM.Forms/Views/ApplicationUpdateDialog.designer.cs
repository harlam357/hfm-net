
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
         this.lblFirstLine = new System.Windows.Forms.Label();
         this.lblYourVersion = new System.Windows.Forms.Label();
         this.lblCurrentVersion = new System.Windows.Forms.Label();
         this.lblSelectDownload = new System.Windows.Forms.Label();
         this.cboUpdateFiles = new System.Windows.Forms.ComboBox();
         this.btnDownload = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.progressDownload = new System.Windows.Forms.ProgressBar();
         this.SuspendLayout();
         // 
         // lblFirstLine
         // 
         this.lblFirstLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.lblFirstLine.Location = new System.Drawing.Point(12, 13);
         this.lblFirstLine.Name = "lblFirstLine";
         this.lblFirstLine.Size = new System.Drawing.Size(267, 31);
         this.lblFirstLine.TabIndex = 0;
         this.lblFirstLine.Text = "A new version of {0} is available for download.";
         // 
         // lblYourVersion
         // 
         this.lblYourVersion.AutoSize = true;
         this.lblYourVersion.Location = new System.Drawing.Point(12, 54);
         this.lblYourVersion.Name = "lblYourVersion";
         this.lblYourVersion.Size = new System.Drawing.Size(72, 13);
         this.lblYourVersion.TabIndex = 1;
         this.lblYourVersion.Text = "Your version: ";
         // 
         // lblCurrentVersion
         // 
         this.lblCurrentVersion.AutoSize = true;
         this.lblCurrentVersion.Location = new System.Drawing.Point(12, 77);
         this.lblCurrentVersion.Name = "lblCurrentVersion";
         this.lblCurrentVersion.Size = new System.Drawing.Size(84, 13);
         this.lblCurrentVersion.TabIndex = 2;
         this.lblCurrentVersion.Text = "Current version: ";
         // 
         // lblSelectDownload
         // 
         this.lblSelectDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.lblSelectDownload.Location = new System.Drawing.Point(10, 113);
         this.lblSelectDownload.Name = "lblSelectDownload";
         this.lblSelectDownload.Size = new System.Drawing.Size(267, 18);
         this.lblSelectDownload.TabIndex = 3;
         this.lblSelectDownload.Text = "Please select an update to download.";
         this.lblSelectDownload.TextAlign = System.Drawing.ContentAlignment.TopCenter;
         // 
         // cboUpdateFiles
         // 
         this.cboUpdateFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboUpdateFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboUpdateFiles.FormattingEnabled = true;
         this.cboUpdateFiles.Location = new System.Drawing.Point(44, 134);
         this.cboUpdateFiles.Name = "cboUpdateFiles";
         this.cboUpdateFiles.Size = new System.Drawing.Size(199, 21);
         this.cboUpdateFiles.TabIndex = 4;
         // 
         // btnDownload
         // 
         this.btnDownload.Location = new System.Drawing.Point(63, 176);
         this.btnDownload.Name = "btnDownload";
         this.btnDownload.Size = new System.Drawing.Size(75, 23);
         this.btnDownload.TabIndex = 5;
         this.btnDownload.Text = "Download";
         this.btnDownload.UseVisualStyleBackColor = true;
         this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(149, 176);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 6;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // progressDownload
         // 
         this.progressDownload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.progressDownload.Location = new System.Drawing.Point(10, 132);
         this.progressDownload.Name = "progressDownload";
         this.progressDownload.Size = new System.Drawing.Size(267, 25);
         this.progressDownload.Step = 1;
         this.progressDownload.TabIndex = 7;
         this.progressDownload.Visible = false;
         // 
         // UpdateDialog
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(287, 211);
         this.Controls.Add(this.btnCancel);
         this.Controls.Add(this.btnDownload);
         this.Controls.Add(this.cboUpdateFiles);
         this.Controls.Add(this.lblSelectDownload);
         this.Controls.Add(this.lblCurrentVersion);
         this.Controls.Add(this.lblYourVersion);
         this.Controls.Add(this.lblFirstLine);
         this.Controls.Add(this.progressDownload);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "ApplicationUpdateDialog";
         this.ShowIcon = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Application Update";
         this.Load += new System.EventHandler(this.UpdateDialog_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label lblFirstLine;
      private System.Windows.Forms.Label lblYourVersion;
      private System.Windows.Forms.Label lblCurrentVersion;
      private System.Windows.Forms.Label lblSelectDownload;
      private System.Windows.Forms.ComboBox cboUpdateFiles;
      private System.Windows.Forms.Button btnDownload;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.ProgressBar progressDownload;
   }
}