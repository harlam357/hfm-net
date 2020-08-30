
namespace HFM.Forms.Views
{
   partial class AboutDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            this.versionLabel = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBoxWrapper1 = new System.Windows.Forms.TextBox();
            this.googleCodeLinkLabel = new System.Windows.Forms.LinkLabel();
            this.googleGroupLinkLabel = new System.Windows.Forms.LinkLabel();
            this.lblDate = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblVersion
            // 
            this.versionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel.ForeColor = System.Drawing.Color.White;
            this.versionLabel.Location = new System.Drawing.Point(298, 16);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(269, 28);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "[Version]";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(30, 30);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(74, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::HFM.Forms.Properties.Resources.hfm_logo_large;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(12, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(280, 181);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // textBoxWrapper1
            // 
            this.textBoxWrapper1.BackColor = System.Drawing.Color.Black;
            this.textBoxWrapper1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxWrapper1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxWrapper1.ForeColor = System.Drawing.Color.LightGray;
            this.textBoxWrapper1.Location = new System.Drawing.Point(12, 199);
            this.textBoxWrapper1.Multiline = true;
            this.textBoxWrapper1.Name = "textBoxWrapper1";
            this.textBoxWrapper1.ReadOnly = true;
            this.textBoxWrapper1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxWrapper1.Size = new System.Drawing.Size(555, 169);
            this.textBoxWrapper1.TabIndex = 4;
            this.textBoxWrapper1.Text = resources.GetString("textBoxWrapper1.Text");
            this.textBoxWrapper1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lnkHfmGoogleCode
            // 
            this.googleCodeLinkLabel.AutoSize = true;
            this.googleCodeLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.googleCodeLinkLabel.LinkColor = System.Drawing.Color.Goldenrod;
            this.googleCodeLinkLabel.Location = new System.Drawing.Point(313, 90);
            this.googleCodeLinkLabel.Name = "googleCodeLinkLabel";
            this.googleCodeLinkLabel.Size = new System.Drawing.Size(0, 16);
            this.googleCodeLinkLabel.TabIndex = 2;
            this.googleCodeLinkLabel.TabStop = true;
            this.googleCodeLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ProjectSiteLink_LinkClicked);
            // 
            // lnkHfmGoogleGroup
            // 
            this.googleGroupLinkLabel.AutoSize = true;
            this.googleGroupLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.googleGroupLinkLabel.LinkColor = System.Drawing.Color.Goldenrod;
            this.googleGroupLinkLabel.Location = new System.Drawing.Point(313, 117);
            this.googleGroupLinkLabel.Name = "googleGroupLinkLabel";
            this.googleGroupLinkLabel.Size = new System.Drawing.Size(0, 16);
            this.googleGroupLinkLabel.TabIndex = 3;
            this.googleGroupLinkLabel.TabStop = true;
            this.googleGroupLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SupportForumLink_LinkClicked);
            // 
            // lblDate
            // 
            this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblDate.Location = new System.Drawing.Point(298, 49);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(269, 28);
            this.lblDate.TabIndex = 6;
            this.lblDate.Text = "[BuildDate]";
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // AboutDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(579, 381);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.googleGroupLinkLabel);
            this.Controls.Add(this.googleCodeLinkLabel);
            this.Controls.Add(this.textBoxWrapper1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About HFM.NET";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion
      private System.Windows.Forms.Label versionLabel;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.Button btnClose;
      private System.Windows.Forms.TextBox textBoxWrapper1;
      private System.Windows.Forms.LinkLabel googleCodeLinkLabel;
      private System.Windows.Forms.LinkLabel googleGroupLinkLabel;
      private System.Windows.Forms.Label lblDate;

   }
}
