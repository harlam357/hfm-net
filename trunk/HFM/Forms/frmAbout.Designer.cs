/*
 * HFM.NET - About Form
 * Copyright (C) 2006-2007 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

namespace HFM.Forms
{
   partial class frmAbout
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
         this.lblProduct = new Classes.LabelWrapper();
         this.lblVersion = new Classes.LabelWrapper();
         this.lblCopyrights = new Classes.LabelWrapper();
         this.lblLinkOrig = new System.Windows.Forms.LinkLabel();
         this.btnClose = new Classes.ButtonWrapper();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.lblGPL = new Classes.LabelWrapper();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.SuspendLayout();
         // 
         // lblProduct
         // 
         this.lblProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblProduct.Location = new System.Drawing.Point(8, 7);
         this.lblProduct.Name = "lblProduct";
         this.lblProduct.Size = new System.Drawing.Size(115, 32);
         this.lblProduct.TabIndex = 1;
         this.lblProduct.Text = "HFM.NET";
         // 
         // lblVersion
         // 
         this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblVersion.Location = new System.Drawing.Point(129, 10);
         this.lblVersion.Name = "lblVersion";
         this.lblVersion.Size = new System.Drawing.Size(230, 28);
         this.lblVersion.TabIndex = 2;
         this.lblVersion.Text = "[Version]";
         // 
         // lblCopyrights
         // 
         this.lblCopyrights.Location = new System.Drawing.Point(10, 37);
         this.lblCopyrights.Name = "lblCopyrights";
         this.lblCopyrights.Size = new System.Drawing.Size(497, 126);
         this.lblCopyrights.TabIndex = 3;
         this.lblCopyrights.Text = resources.GetString("lblCopyrights.Text");
         // 
         // lblLinkOrig
         // 
         this.lblLinkOrig.LinkArea = new System.Windows.Forms.LinkArea(58, 15);
         this.lblLinkOrig.Location = new System.Drawing.Point(10, 169);
         this.lblLinkOrig.Name = "lblLinkOrig";
         this.lblLinkOrig.Size = new System.Drawing.Size(497, 22);
         this.lblLinkOrig.TabIndex = 5;
         this.lblLinkOrig.TabStop = true;
         this.lblLinkOrig.Text = "The HFM.NET UI was inspired by FahMon and is based on the FAHLogStats.NET code ba" +
             "se.";
         this.lblLinkOrig.UseCompatibleTextRendering = true;
         this.lblLinkOrig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnClose.Location = new System.Drawing.Point(465, 202);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(75, 23);
         this.btnClose.TabIndex = 0;
         this.btnClose.Text = "Close";
         this.btnClose.UseVisualStyleBackColor = false;
         // 
         // pictureBox1
         // 
         this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pictureBox1.Image = global::HFM.Properties.Resources.aboutBox;
         this.pictureBox1.InitialImage = null;
         this.pictureBox1.Location = new System.Drawing.Point(376, 14);
         this.pictureBox1.Name = "pictureBox1";
         this.pictureBox1.Size = new System.Drawing.Size(164, 93);
         this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.pictureBox1.TabIndex = 4;
         this.pictureBox1.TabStop = false;
         // 
         // lblGPL
         // 
         this.lblGPL.Location = new System.Drawing.Point(10, 197);
         this.lblGPL.Name = "lblGPL";
         this.lblGPL.Size = new System.Drawing.Size(497, 37);
         this.lblGPL.TabIndex = 6;
         this.lblGPL.Text = "This program is free software; you can redistribute it and/or modify it under the" +
             " terms of the\r\nGNU General Public License, version 2, as published by the Free S" +
             "oftware Foundation.";
         // 
         // frmAbout
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(552, 235);
         this.Controls.Add(this.btnClose);
         this.Controls.Add(this.lblGPL);
         this.Controls.Add(this.lblLinkOrig);
         this.Controls.Add(this.pictureBox1);
         this.Controls.Add(this.lblCopyrights);
         this.Controls.Add(this.lblVersion);
         this.Controls.Add(this.lblProduct);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmAbout";
         this.ShowInTaskbar = false;
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "About HFM.NET";
         this.TopMost = true;
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private Classes.LabelWrapper lblProduct;
      private Classes.LabelWrapper lblVersion;
      private Classes.LabelWrapper lblCopyrights;
      private System.Windows.Forms.PictureBox pictureBox1;
      private System.Windows.Forms.LinkLabel lblLinkOrig;
      private Classes.ButtonWrapper btnClose;
      private Classes.LabelWrapper lblGPL;

   }
}