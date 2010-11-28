using HFM.Forms.Controls;

namespace HFM.Forms
{
   partial class frmMessages
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMessages));
         this.txtMessages = new TextBoxWrapper();
         this.SuspendLayout();
         // 
         // txtMessages
         // 
         this.txtMessages.BackColor = System.Drawing.SystemColors.Window;
         this.txtMessages.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtMessages.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.txtMessages.Location = new System.Drawing.Point(0, 0);
         this.txtMessages.Multiline = true;
         this.txtMessages.Name = "txtMessages";
         this.txtMessages.ReadOnly = true;
         this.txtMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
         this.txtMessages.Size = new System.Drawing.Size(701, 455);
         this.txtMessages.TabIndex = 0;
         this.txtMessages.WordWrap = false;
         this.txtMessages.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessages_KeyDown);
         // 
         // frmMessages
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(701, 455);
         this.Controls.Add(this.txtMessages);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmMessages";
         this.Text = "Messages";
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private TextBoxWrapper txtMessages;
   }
}
