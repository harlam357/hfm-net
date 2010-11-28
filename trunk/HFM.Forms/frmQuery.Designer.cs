using HFM.Forms.Controls;

namespace HFM.Forms
{
   partial class frmQuery
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQuery));
         this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
         this.panel1 = new System.Windows.Forms.Panel();
         this.txtName = new TextBoxWrapper();
         this.lblName = new LabelWrapper();
         this.btnRemove = new ButtonWrapper();
         this.btnAdd = new ButtonWrapper();
         this.dataGridView1 = new System.Windows.Forms.DataGridView();
         this.panel2 = new System.Windows.Forms.Panel();
         this.btnOK = new ButtonWrapper();
         this.btnCancel = new ButtonWrapper();
         this.tableLayoutPanel1.SuspendLayout();
         this.panel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
         this.panel2.SuspendLayout();
         this.SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         this.tableLayoutPanel1.ColumnCount = 1;
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
         this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
         this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
         this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
         this.tableLayoutPanel1.Name = "tableLayoutPanel1";
         this.tableLayoutPanel1.RowCount = 3;
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
         this.tableLayoutPanel1.Size = new System.Drawing.Size(594, 303);
         this.tableLayoutPanel1.TabIndex = 0;
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.txtName);
         this.panel1.Controls.Add(this.lblName);
         this.panel1.Controls.Add(this.btnRemove);
         this.panel1.Controls.Add(this.btnAdd);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panel1.Location = new System.Drawing.Point(3, 3);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(588, 34);
         this.panel1.TabIndex = 0;
         // 
         // txtName
         // 
         this.txtName.Location = new System.Drawing.Point(53, 8);
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(348, 20);
         this.txtName.TabIndex = 5;
         // 
         // lblName
         // 
         this.lblName.AutoSize = true;
         this.lblName.Location = new System.Drawing.Point(9, 11);
         this.lblName.Name = "lblName";
         this.lblName.Size = new System.Drawing.Size(38, 13);
         this.lblName.TabIndex = 4;
         this.lblName.Text = "Name:";
         // 
         // btnRemove
         // 
         this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnRemove.Location = new System.Drawing.Point(498, 6);
         this.btnRemove.Name = "btnRemove";
         this.btnRemove.Size = new System.Drawing.Size(81, 23);
         this.btnRemove.TabIndex = 1;
         this.btnRemove.Text = "Remove Row";
         this.btnRemove.UseVisualStyleBackColor = true;
         this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
         // 
         // btnAdd
         // 
         this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnAdd.Location = new System.Drawing.Point(411, 6);
         this.btnAdd.Name = "btnAdd";
         this.btnAdd.Size = new System.Drawing.Size(81, 23);
         this.btnAdd.TabIndex = 0;
         this.btnAdd.Text = "Add Row";
         this.btnAdd.UseVisualStyleBackColor = true;
         this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
         // 
         // dataGridView1
         // 
         this.dataGridView1.AllowUserToAddRows = false;
         this.dataGridView1.AllowUserToDeleteRows = false;
         this.dataGridView1.AllowUserToResizeColumns = false;
         this.dataGridView1.AllowUserToResizeRows = false;
         this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.dataGridView1.Location = new System.Drawing.Point(3, 43);
         this.dataGridView1.MultiSelect = false;
         this.dataGridView1.Name = "dataGridView1";
         this.dataGridView1.RowHeadersVisible = false;
         this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.dataGridView1.Size = new System.Drawing.Size(588, 217);
         this.dataGridView1.TabIndex = 1;
         // 
         // panel2
         // 
         this.panel2.Controls.Add(this.btnOK);
         this.panel2.Controls.Add(this.btnCancel);
         this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panel2.Location = new System.Drawing.Point(3, 266);
         this.panel2.Name = "panel2";
         this.panel2.Size = new System.Drawing.Size(588, 34);
         this.panel2.TabIndex = 2;
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.Location = new System.Drawing.Point(421, 6);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 2;
         this.btnOK.Text = "OK";
         this.btnOK.UseVisualStyleBackColor = true;
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.Location = new System.Drawing.Point(503, 6);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 3;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // frmQuery
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(594, 303);
         this.Controls.Add(this.tableLayoutPanel1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmQuery";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "History Query";
         this.tableLayoutPanel1.ResumeLayout(false);
         this.panel1.ResumeLayout(false);
         this.panel1.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
         this.panel2.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.Panel panel1;
      private ButtonWrapper btnAdd;
      private ButtonWrapper btnOK;
      private ButtonWrapper btnRemove;
      private ButtonWrapper btnCancel;
      private System.Windows.Forms.DataGridView dataGridView1;
      private TextBoxWrapper txtName;
      private LabelWrapper lblName;
      private System.Windows.Forms.Panel panel2;
   }
}