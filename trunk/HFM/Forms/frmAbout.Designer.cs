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
         this.lblProduct = new HFM.Classes.LabelWrapper();
         this.lblVersion = new HFM.Classes.LabelWrapper();
         this.lblCopyrights = new HFM.Classes.LabelWrapper();
         this.lblLinkOrig = new System.Windows.Forms.LinkLabel();
         this.btnClose = new HFM.Classes.ButtonWrapper();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.lblGPL = new HFM.Classes.LabelWrapper();
         this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
         this.labelWrapper1 = new HFM.Classes.LabelWrapper();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.tableLayoutPanel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // lblProduct
         // 
         this.lblProduct.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblProduct.Location = new System.Drawing.Point(3, 4);
         this.lblProduct.Name = "lblProduct";
         this.lblProduct.Size = new System.Drawing.Size(115, 32);
         this.lblProduct.TabIndex = 1;
         this.lblProduct.Text = "HFM.NET";
         // 
         // lblVersion
         // 
         this.lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblVersion.Location = new System.Drawing.Point(128, 6);
         this.lblVersion.Name = "lblVersion";
         this.lblVersion.Size = new System.Drawing.Size(245, 28);
         this.lblVersion.TabIndex = 2;
         this.lblVersion.Text = "[Version]";
         // 
         // lblCopyrights
         // 
         this.lblCopyrights.AutoSize = true;
         this.tableLayoutPanel1.SetColumnSpan(this.lblCopyrights, 4);
         this.lblCopyrights.Location = new System.Drawing.Point(3, 110);
         this.lblCopyrights.Name = "lblCopyrights";
         this.lblCopyrights.Size = new System.Drawing.Size(447, 78);
         this.lblCopyrights.TabIndex = 3;
         this.lblCopyrights.Text = resources.GetString("lblCopyrights.Text");
         // 
         // lblLinkOrig
         // 
         this.lblLinkOrig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.tableLayoutPanel1.SetColumnSpan(this.lblLinkOrig, 4);
         this.lblLinkOrig.LinkArea = new System.Windows.Forms.LinkArea(29, 15);
         this.lblLinkOrig.Location = new System.Drawing.Point(3, 202);
         this.lblLinkOrig.Name = "lblLinkOrig";
         this.lblLinkOrig.Size = new System.Drawing.Size(497, 20);
         this.lblLinkOrig.TabIndex = 5;
         this.lblLinkOrig.TabStop = true;
         this.lblLinkOrig.Text = "HFM.NET was derived from the FAHLogStats.NET code base.  The UI is inspired by Fa" +
             "hMon.";
         this.lblLinkOrig.UseCompatibleTextRendering = true;
         this.lblLinkOrig.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
         // 
         // btnClose
         // 
         this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.btnClose.Location = new System.Drawing.Point(481, 226);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(74, 23);
         this.btnClose.TabIndex = 0;
         this.btnClose.Text = "Close";
         this.btnClose.UseVisualStyleBackColor = false;
         // 
         // pictureBox1
         // 
         this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.tableLayoutPanel1.SetColumnSpan(this.pictureBox1, 2);
         this.pictureBox1.Image = global::HFM.Properties.Resources.aboutBox;
         this.pictureBox1.InitialImage = null;
         this.pictureBox1.Location = new System.Drawing.Point(381, 3);
         this.pictureBox1.Name = "pictureBox1";
         this.tableLayoutPanel1.SetRowSpan(this.pictureBox1, 2);
         this.pictureBox1.Size = new System.Drawing.Size(174, 104);
         this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.pictureBox1.TabIndex = 4;
         this.pictureBox1.TabStop = false;
         // 
         // lblGPL
         // 
         this.lblGPL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.tableLayoutPanel1.SetColumnSpan(this.lblGPL, 3);
         this.lblGPL.Location = new System.Drawing.Point(3, 225);
         this.lblGPL.Name = "lblGPL";
         this.lblGPL.Size = new System.Drawing.Size(452, 27);
         this.lblGPL.TabIndex = 6;
         this.lblGPL.Text = "This program is free software; you can redistribute it and/or modify it under the" +
             " terms of the\r\nGNU General Public License, version 2, as published by the Free S" +
             "oftware Foundation.";
         // 
         // tableLayoutPanel1
         // 
         this.tableLayoutPanel1.ColumnCount = 4;
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
         this.tableLayoutPanel1.Controls.Add(this.lblProduct, 0, 0);
         this.tableLayoutPanel1.Controls.Add(this.lblGPL, 0, 4);
         this.tableLayoutPanel1.Controls.Add(this.btnClose, 3, 4);
         this.tableLayoutPanel1.Controls.Add(this.lblLinkOrig, 0, 3);
         this.tableLayoutPanel1.Controls.Add(this.lblVersion, 1, 0);
         this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 2, 0);
         this.tableLayoutPanel1.Controls.Add(this.labelWrapper1, 0, 1);
         this.tableLayoutPanel1.Controls.Add(this.lblCopyrights, 0, 2);
         this.tableLayoutPanel1.Location = new System.Drawing.Point(7, 7);
         this.tableLayoutPanel1.Name = "tableLayoutPanel1";
         this.tableLayoutPanel1.RowCount = 5;
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
         this.tableLayoutPanel1.Size = new System.Drawing.Size(558, 252);
         this.tableLayoutPanel1.TabIndex = 7;
         // 
         // labelWrapper1
         // 
         this.labelWrapper1.AutoSize = true;
         this.tableLayoutPanel1.SetColumnSpan(this.labelWrapper1, 2);
         this.labelWrapper1.Location = new System.Drawing.Point(3, 40);
         this.labelWrapper1.Name = "labelWrapper1";
         this.labelWrapper1.Size = new System.Drawing.Size(257, 52);
         this.labelWrapper1.TabIndex = 5;
         this.labelWrapper1.Text = "Copyright (c) Ryan Harlamert (harlam357) 2009-2010.\r\nCopyright (c) David Rawling " +
             "2006-2007.\r\n\r\nAll Rights Reserved.";
         // 
         // frmAbout
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(571, 265);
         this.Controls.Add(this.tableLayoutPanel1);
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
         this.tableLayoutPanel1.ResumeLayout(false);
         this.tableLayoutPanel1.PerformLayout();
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
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private HFM.Classes.LabelWrapper labelWrapper1;

   }
}