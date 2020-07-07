namespace WRKANTIN
{
    partial class excelsenkronizasyon
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
            this.buttonExcelGuncelle = new System.Windows.Forms.Button();
            this.buttonExcelGoster = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonUrunExcelGuncelle = new System.Windows.Forms.Button();
            this.buttonUrunExcelGoruntule = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonExcelGuncelle
            // 
            this.buttonExcelGuncelle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buttonExcelGuncelle.Location = new System.Drawing.Point(14, 88);
            this.buttonExcelGuncelle.Name = "buttonExcelGuncelle";
            this.buttonExcelGuncelle.Size = new System.Drawing.Size(89, 61);
            this.buttonExcelGuncelle.TabIndex = 13;
            this.buttonExcelGuncelle.Text = "Ogrenci Güncelle";
            this.buttonExcelGuncelle.UseVisualStyleBackColor = true;
            this.buttonExcelGuncelle.Visible = false;
            this.buttonExcelGuncelle.Click += new System.EventHandler(this.buttonExcelGuncelle_Click);
            // 
            // buttonExcelGoster
            // 
            this.buttonExcelGoster.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buttonExcelGoster.Location = new System.Drawing.Point(14, 17);
            this.buttonExcelGoster.Name = "buttonExcelGoster";
            this.buttonExcelGoster.Size = new System.Drawing.Size(89, 61);
            this.buttonExcelGoster.TabIndex = 12;
            this.buttonExcelGoster.Text = "Ogrenci Görüntüle";
            this.buttonExcelGoster.UseVisualStyleBackColor = true;
            this.buttonExcelGoster.Click += new System.EventHandler(this.buttonExcelGoster_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(109, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1229, 650);
            this.dataGridView1.TabIndex = 11;
            // 
            // buttonUrunExcelGuncelle
            // 
            this.buttonUrunExcelGuncelle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buttonUrunExcelGuncelle.Location = new System.Drawing.Point(14, 226);
            this.buttonUrunExcelGuncelle.Name = "buttonUrunExcelGuncelle";
            this.buttonUrunExcelGuncelle.Size = new System.Drawing.Size(89, 61);
            this.buttonUrunExcelGuncelle.TabIndex = 15;
            this.buttonUrunExcelGuncelle.Text = "Ürünleri Güncelle";
            this.buttonUrunExcelGuncelle.UseVisualStyleBackColor = true;
            this.buttonUrunExcelGuncelle.Visible = false;
            this.buttonUrunExcelGuncelle.Click += new System.EventHandler(this.buttonUrunExcelGuncelle_Click);
            // 
            // buttonUrunExcelGoruntule
            // 
            this.buttonUrunExcelGoruntule.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buttonUrunExcelGoruntule.Location = new System.Drawing.Point(14, 155);
            this.buttonUrunExcelGoruntule.Name = "buttonUrunExcelGoruntule";
            this.buttonUrunExcelGoruntule.Size = new System.Drawing.Size(89, 61);
            this.buttonUrunExcelGoruntule.TabIndex = 14;
            this.buttonUrunExcelGoruntule.Text = "Ürünleri Görüntüle";
            this.buttonUrunExcelGoruntule.UseVisualStyleBackColor = true;
            this.buttonUrunExcelGoruntule.Click += new System.EventHandler(this.buttonUrunExcelGoruntule_Click);
            // 
            // excelsenkronizasyon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 681);
            this.Controls.Add(this.buttonUrunExcelGuncelle);
            this.Controls.Add(this.buttonUrunExcelGoruntule);
            this.Controls.Add(this.buttonExcelGuncelle);
            this.Controls.Add(this.buttonExcelGoster);
            this.Controls.Add(this.dataGridView1);
            this.Name = "excelsenkronizasyon";
            this.Text = "excelsenkronizasyon";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.excelsenkronizasyon_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonExcelGuncelle;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonUrunExcelGuncelle;
        public System.Windows.Forms.Button buttonExcelGoster;
        public System.Windows.Forms.Button buttonUrunExcelGoruntule;
    }
}