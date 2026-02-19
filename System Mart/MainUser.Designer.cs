namespace System_Mart
{
    partial class MainUser
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlMessage = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAccount = new System.Windows.Forms.Button();
            this.btnPay = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAddBuy = new System.Windows.Forms.Button();
            this.txtInputBarcode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblbarCode = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblNameProduct = new System.Windows.Forms.Label();
            this.dgvProductOrder = new System.Windows.Forms.DataGridView();
            this.lblPrice = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.picbProduct = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.pnlMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductOrder)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbProduct)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.MenuBar;
            this.panel1.Controls.Add(this.pnlMessage);
            this.panel1.Controls.Add(this.lblName);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnAccount);
            this.panel1.Controls.Add(this.btnPay);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnAddBuy);
            this.panel1.Controls.Add(this.txtInputBarcode);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(340, 454);
            this.panel1.TabIndex = 0;
            // 
            // pnlMessage
            // 
            this.pnlMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.pnlMessage.Controls.Add(this.lblMessage);
            this.pnlMessage.Location = new System.Drawing.Point(17, 38);
            this.pnlMessage.Name = "pnlMessage";
            this.pnlMessage.Size = new System.Drawing.Size(316, 23);
            this.pnlMessage.TabIndex = 9;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(1, 3);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(64, 16);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Message";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(94, 19);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(0, 16);
            this.lblName.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Username :";
            // 
            // btnAccount
            // 
            this.btnAccount.BackColor = System.Drawing.Color.White;
            this.btnAccount.Image = global::System_Mart.Properties.Resources.settings__1_;
            this.btnAccount.Location = new System.Drawing.Point(15, 389);
            this.btnAccount.Name = "btnAccount";
            this.btnAccount.Size = new System.Drawing.Size(70, 49);
            this.btnAccount.TabIndex = 6;
            this.btnAccount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAccount.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAccount.UseVisualStyleBackColor = false;
            this.btnAccount.Click += new System.EventHandler(this.btnAccount_Click);
            // 
            // btnPay
            // 
            this.btnPay.BackColor = System.Drawing.Color.White;
            this.btnPay.Image = global::System_Mart.Properties.Resources.coin__1_;
            this.btnPay.Location = new System.Drawing.Point(199, 389);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(128, 49);
            this.btnPay.TabIndex = 5;
            this.btnPay.Text = "Pay";
            this.btnPay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPay.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPay.UseVisualStyleBackColor = false;
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.Image = global::System_Mart.Properties.Resources.cancel__2___1_;
            this.btnCancel.Location = new System.Drawing.Point(149, 117);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(128, 49);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel Buy";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAddBuy
            // 
            this.btnAddBuy.BackColor = System.Drawing.Color.White;
            this.btnAddBuy.Image = global::System_Mart.Properties.Resources.shopping_cart__1_;
            this.btnAddBuy.Location = new System.Drawing.Point(15, 117);
            this.btnAddBuy.Name = "btnAddBuy";
            this.btnAddBuy.Size = new System.Drawing.Size(128, 49);
            this.btnAddBuy.TabIndex = 3;
            this.btnAddBuy.Text = "Add Buy";
            this.btnAddBuy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddBuy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddBuy.UseVisualStyleBackColor = false;
            this.btnAddBuy.Click += new System.EventHandler(this.btnAddBuy_Click);
            // 
            // txtInputBarcode
            // 
            this.txtInputBarcode.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtInputBarcode.Location = new System.Drawing.Point(15, 84);
            this.txtInputBarcode.Name = "txtInputBarcode";
            this.txtInputBarcode.Size = new System.Drawing.Size(262, 22);
            this.txtInputBarcode.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input BarCode :";
            // 
            // lblbarCode
            // 
            this.lblbarCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblbarCode.Location = new System.Drawing.Point(598, 41);
            this.lblbarCode.Name = "lblbarCode";
            this.lblbarCode.Size = new System.Drawing.Size(371, 23);
            this.lblbarCode.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(595, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "Product ID :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(595, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 18);
            this.label4.TabIndex = 4;
            this.label4.Text = "Product Name  :";
            // 
            // lblNameProduct
            // 
            this.lblNameProduct.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNameProduct.Location = new System.Drawing.Point(598, 98);
            this.lblNameProduct.Name = "lblNameProduct";
            this.lblNameProduct.Size = new System.Drawing.Size(371, 23);
            this.lblNameProduct.TabIndex = 5;
            // 
            // dgvProductOrder
            // 
            this.dgvProductOrder.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductOrder.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dgvProductOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductOrder.Location = new System.Drawing.Point(363, 220);
            this.dgvProductOrder.Name = "dgvProductOrder";
            this.dgvProductOrder.RowHeadersWidth = 51;
            this.dgvProductOrder.RowTemplate.Height = 24;
            this.dgvProductOrder.Size = new System.Drawing.Size(695, 218);
            this.dgvProductOrder.TabIndex = 6;
            this.dgvProductOrder.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.SelectData_Click);
            // 
            // lblPrice
            // 
            this.lblPrice.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPrice.Location = new System.Drawing.Point(598, 154);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(371, 23);
            this.lblPrice.TabIndex = 8;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(595, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 18);
            this.label7.TabIndex = 7;
            this.label7.Text = "Price of Product :";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panel2.Controls.Add(this.label8);
            this.panel2.Location = new System.Drawing.Point(363, 197);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(695, 22);
            this.panel2.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(5, 2);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 18);
            this.label8.TabIndex = 0;
            this.label8.Text = "Product Oder";
            // 
            // picbProduct
            // 
            this.picbProduct.Location = new System.Drawing.Point(363, 3);
            this.picbProduct.Name = "picbProduct";
            this.picbProduct.Size = new System.Drawing.Size(185, 181);
            this.picbProduct.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picbProduct.TabIndex = 1;
            this.picbProduct.TabStop = false;
            this.picbProduct.Click += new System.EventHandler(this.picbProduct_Click);
            // 
            // MainUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1085, 454);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dgvProductOrder);
            this.Controls.Add(this.lblNameProduct);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblbarCode);
            this.Controls.Add(this.picbProduct);
            this.Controls.Add(this.panel1);
            this.Name = "MainUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cashier";
            this.Load += new System.EventHandler(this.MainUser_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlMessage.ResumeLayout(false);
            this.pnlMessage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductOrder)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbProduct)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtInputBarcode;
        private System.Windows.Forms.PictureBox picbProduct;
        private System.Windows.Forms.Label lblbarCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblNameProduct;
        private System.Windows.Forms.DataGridView dgvProductOrder;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnPay;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAddBuy;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnAccount;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlMessage;
        private System.Windows.Forms.Label lblMessage;
    }
}