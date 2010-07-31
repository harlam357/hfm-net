namespace HFM.Forms
{
   partial class frmProjects
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
            _timer.Dispose();
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProjects));
         this.progressBar1 = new System.Windows.Forms.ProgressBar();
         this.lblProject = new HFM.Classes.LabelWrapper();
         this.SuspendLayout();
         // 
         // progressBar1
         // 
         this.progressBar1.Location = new System.Drawing.Point(12, 12);
         this.progressBar1.Name = "progressBar1";
         this.progressBar1.Size = new System.Drawing.Size(306, 24);
         this.progressBar1.TabIndex = 0;
         // 
         // lblProject
         // 
         this.lblProject.Location = new System.Drawing.Point(12, 44);
         this.lblProject.Name = "lblProject";
         this.lblProject.Size = new System.Drawing.Size(306, 13);
         this.lblProject.TabIndex = 1;
         this.lblProject.Text = "lblProject";
         this.lblProject.TextAlign = System.Drawing.ContentAlignment.TopCenter;
         // 
         // frmProjects
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(330, 68);
         this.Controls.Add(this.lblProject);
         this.Controls.Add(this.progressBar1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmProjects";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Project Download";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProjects_FormClosing);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.ProgressBar progressBar1;
      private HFM.Classes.LabelWrapper lblProject;
   }
}