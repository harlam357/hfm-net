/*
 * HFM.NET - About Form
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using HFM.Forms.Controls;

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
         this.lblProduct = new LabelWrapper();
         this.lblVersion = new LabelWrapper();
         this.btnClose = new ButtonWrapper();
         this.pictureBox1 = new System.Windows.Forms.PictureBox();
         this.textBoxWrapper1 = new TextBoxWrapper();
         this.lnkHfmGoogleCode = new System.Windows.Forms.LinkLabel();
         this.lnkHfmGoogleGroup = new System.Windows.Forms.LinkLabel();
         this.lblDate = new LabelWrapper();
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
         this.SuspendLayout();
         // 
         // lblProduct
         // 
         this.lblProduct.BackColor = System.Drawing.Color.Gainsboro;
         this.lblProduct.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.lblProduct.Font = new System.Drawing.Font("Times New Roman", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblProduct.Location = new System.Drawing.Point(298, 11);
         this.lblProduct.Name = "lblProduct";
         this.lblProduct.Size = new System.Drawing.Size(269, 42);
         this.lblProduct.TabIndex = 0;
         this.lblProduct.Text = "HFM.NET";
         this.lblProduct.TextAlign = System.Drawing.ContentAlignment.TopCenter;
         // 
         // lblVersion
         // 
         this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblVersion.ForeColor = System.Drawing.Color.White;
         this.lblVersion.Location = new System.Drawing.Point(298, 59);
         this.lblVersion.Name = "lblVersion";
         this.lblVersion.Size = new System.Drawing.Size(269, 28);
         this.lblVersion.TabIndex = 1;
         this.lblVersion.Text = "[Version]";
         this.lblVersion.TextAlign = System.Drawing.ContentAlignment.TopCenter;
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
         this.lnkHfmGoogleCode.AutoSize = true;
         this.lnkHfmGoogleCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lnkHfmGoogleCode.LinkColor = System.Drawing.Color.Goldenrod;
         this.lnkHfmGoogleCode.Location = new System.Drawing.Point(313, 121);
         this.lnkHfmGoogleCode.Name = "lnkHfmGoogleCode";
         this.lnkHfmGoogleCode.Size = new System.Drawing.Size(209, 16);
         this.lnkHfmGoogleCode.TabIndex = 2;
         this.lnkHfmGoogleCode.TabStop = true;
         this.lnkHfmGoogleCode.Text = "http://code.google.com/p/hfm-net/";
         this.lnkHfmGoogleCode.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHfmGoogleCode_LinkClicked);
         // 
         // lnkHfmGoogleGroup
         // 
         this.lnkHfmGoogleGroup.AutoSize = true;
         this.lnkHfmGoogleGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lnkHfmGoogleGroup.LinkColor = System.Drawing.Color.Goldenrod;
         this.lnkHfmGoogleGroup.Location = new System.Drawing.Point(313, 148);
         this.lnkHfmGoogleGroup.Name = "lnkHfmGoogleGroup";
         this.lnkHfmGoogleGroup.Size = new System.Drawing.Size(247, 16);
         this.lnkHfmGoogleGroup.TabIndex = 3;
         this.lnkHfmGoogleGroup.TabStop = true;
         this.lnkHfmGoogleGroup.Text = "http://groups.google.com/group/hfm-net/";
         this.lnkHfmGoogleGroup.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHfmGoogleGroup_LinkClicked);
         // 
         // lblDate
         // 
         this.lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.lblDate.ForeColor = System.Drawing.Color.Gainsboro;
         this.lblDate.Location = new System.Drawing.Point(298, 89);
         this.lblDate.Name = "lblDate";
         this.lblDate.Size = new System.Drawing.Size(269, 28);
         this.lblDate.TabIndex = 6;
         this.lblDate.Text = "[BuildDate]";
         this.lblDate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
         // 
         // frmAbout
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.Color.Black;
         this.CancelButton = this.btnClose;
         this.ClientSize = new System.Drawing.Size(579, 381);
         this.Controls.Add(this.lblDate);
         this.Controls.Add(this.lnkHfmGoogleGroup);
         this.Controls.Add(this.lnkHfmGoogleCode);
         this.Controls.Add(this.textBoxWrapper1);
         this.Controls.Add(this.lblProduct);
         this.Controls.Add(this.pictureBox1);
         this.Controls.Add(this.lblVersion);
         this.Controls.Add(this.btnClose);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmAbout";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "About HFM.NET";
         ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private LabelWrapper lblProduct;
      private LabelWrapper lblVersion;
      private System.Windows.Forms.PictureBox pictureBox1;
      private ButtonWrapper btnClose;
      private TextBoxWrapper textBoxWrapper1;
      private System.Windows.Forms.LinkLabel lnkHfmGoogleCode;
      private System.Windows.Forms.LinkLabel lnkHfmGoogleGroup;
      private LabelWrapper lblDate;

   }
}
