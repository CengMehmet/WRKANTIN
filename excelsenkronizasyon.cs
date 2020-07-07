using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace WRKANTIN
{
    public partial class excelsenkronizasyon : Form
    {
        SqlCommand k;
        SqlDataReader rd;
        private KantinContext db;
        string baglanti = "", key = "";


        public excelsenkronizasyon()
        {
            InitializeComponent();
            xmlOku(); ;
        }

        public string xmlOku()
        {
            baglanti = ""; key = "";
            XmlTextReader oku = new XmlTextReader("config.xml");
            while (oku.Read())
            {
                if (oku.NodeType == XmlNodeType.Element)
                {
                    switch (oku.Name)
                    {
                        case "SqlBaglanti":
                            baglanti = oku.ReadString();
                            db = new KantinContext(baglanti);
                            break;
                    }
                }                
            }
            oku.Close();
            return baglanti;
        }

        private void buttonExcelGoster_Click(object sender, EventArgs e)
        {
            buttonExcelGuncelle.Visible = true;
            buttonUrunExcelGuncelle.Visible = false;
            excelgoster();
        }

        private void excelgoster()
        {
            string dosya_yolu = "ogrenci.xls";

            OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + dosya_yolu + "; Extended Properties = Excel 12.0");
            baglanti.Open();
            string sorgu = "select * from [Sayfa1$]";
            OleDbDataAdapter data_adaptor = new OleDbDataAdapter(sorgu, baglanti);
            baglanti.Close();

            System.Data.DataTable dt = new System.Data.DataTable();
            data_adaptor.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        public void exceldenguncelle()
        {
            for (int l = 0; l < dataGridView1.Rows.Count - 1; l++)
            {
                int i = 0;
                using (SqlConnection conn = new SqlConnection(xmlOku()))
                {
                    conn.Open();
                    k = new SqlCommand("Select No, Ad,Soyad FROM Ogrencis WHERE No='" + dataGridView1.Rows[l].Cells[0].Value.ToString() + "' AND No!='" + "" + "'", conn);
                    rd = k.ExecuteReader();

                    while (rd.Read())
                    {
                        i++;
                    }
                }

                if (i == 0)
                {
                    using (SqlConnection conn = new SqlConnection(xmlOku()))
                    {
                        conn.Open();
                         k = new SqlCommand("INSERT INTO Ogrencis (No,Ad,Soyad,KartNo,Sinif,Tel,Bakiye)VALUES('" +
                         dataGridView1.Rows[l].Cells[0].Value.ToString() + "', '" +
                         dataGridView1.Rows[l].Cells[1].Value.ToString() + "', '" +
                         dataGridView1.Rows[l].Cells[2].Value.ToString() + "', '" +
                         dataGridView1.Rows[l].Cells[3].Value.ToString() + "', '" +
                         dataGridView1.Rows[l].Cells[4].Value.ToString() + "', '" +
                         dataGridView1.Rows[l].Cells[5].Value.ToString() + "', '" +
                         dataGridView1.Rows[l].Cells[6].Value.ToString() +
                         "')",conn);
                        k.ExecuteNonQuery();
                    }
                }
                else if (i == 1)
                {
                    using (SqlConnection conn = new SqlConnection(xmlOku()))
                    {
                        conn.Open();
                        k = new SqlCommand("UPDATE Ogrencis SET No='" + dataGridView1.Rows[l].Cells[0].Value.ToString() + "', " +
                        "Ad='" + dataGridView1.Rows[l].Cells[1].Value.ToString() + "', " +
                        "Soyad='" + dataGridView1.Rows[l].Cells[2].Value.ToString() + "', " +
                        "KartNo='" + dataGridView1.Rows[l].Cells[3].Value.ToString() + "', " +
                        "Sinif='" + dataGridView1.Rows[l].Cells[4].Value.ToString() + "', " +
                        "Tel='" + dataGridView1.Rows[l].Cells[5].Value.ToString() + "', " +
                        "Bakiye='" + dataGridView1.Rows[l].Cells[6].Value.ToString() +
                        "' WHERE No='" + dataGridView1.Rows[l].Cells[0].Value.ToString() + "'", conn);
                        k.ExecuteNonQuery();
                    }
                }

            }
            string[] sutunAdi = { "Ad", "Soyad", "KartNo", "Sinif", "Tel", "Bakiye" };
            for (int m = 0; m < sutunAdi.Length; m++)
            {
                if (sutunAdi[m] == "Bakiye")
                {
                    using (SqlConnection conn = new SqlConnection(xmlOku()))
                    {
                        conn.Open();
                        k = new SqlCommand("UPDATE Ogrencis SET " + sutunAdi[m] + "='" + "0" + "' WHERE (" + sutunAdi[m] + " IS NULL)", conn);
                        k.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(xmlOku()))
                    {
                        conn.Open();
                        k = new SqlCommand("UPDATE Ogrencis SET " + sutunAdi[m] + "=''" + "" + "WHERE " + sutunAdi[m] + " IS NULL", conn);
                        k.ExecuteNonQuery();
                    }
                }
            }
            excelgoster();
            MessageBox.Show("Tablo Doldurma İşlemi Tamamlanmıştır.", "Tablo Doldurma");

        }

        private void buttonExcelGuncelle_Click(object sender, EventArgs e)
        {
            exceldenguncelle();
        }

        private void buttonUrunExcelGoruntule_Click(object sender, EventArgs e)
        {
            buttonUrunExcelGuncelle.Visible = true;
            buttonExcelGuncelle.Visible = false;
            urunexcelgoster();
        }

        private void urunexcelgoster()
        {
            string dosya_yolu = "urunler.xls";

            OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + dosya_yolu + "; Extended Properties = Excel 12.0");
            baglanti.Open();
            string sorgu = "select * from [Sayfa1$]";
            OleDbDataAdapter data_adaptor = new OleDbDataAdapter(sorgu, baglanti);
            baglanti.Close();

            System.Data.DataTable dt = new System.Data.DataTable();
            data_adaptor.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        public void urunexcelguncelle()
        {
            bool barkod = true;
            for (int l = 0; l < dataGridView1.Rows.Count - 1; l++)
            {
                int i = 0;
                using (SqlConnection conn = new SqlConnection(xmlOku()))
                {
                    conn.Open();
                    k = new SqlCommand("Select Barkod, Ad,Kategori,Barkodlu,Fiyat,Stok FROM Uruns WHERE Barkod='" + dataGridView1.Rows[l].Cells[0].Value.ToString() + "'", conn);
                    rd = k.ExecuteReader();

                    while (rd.Read())
                    {
                        i++;
                    }
                }

                if (i == 0)
                {
                    if (dataGridView1.Rows[l].Cells[3].Value.ToString() == "0") { barkod = false; }
                    else if (dataGridView1.Rows[l].Cells[3].Value.ToString() == "1") { barkod = true; }
                    using (SqlConnection conn = new SqlConnection(xmlOku()))
                    {
                        conn.Open();
                        k = new SqlCommand("INSERT INTO Uruns (Barkod,Ad,Kategori,Barkodlu,Fiyat,Stok)VALUES('" +
                        dataGridView1.Rows[l].Cells[0].Value.ToString() + "', '" +
                        dataGridView1.Rows[l].Cells[1].Value.ToString() + "', '" +
                        dataGridView1.Rows[l].Cells[2].Value.ToString() + "', '" +
                        barkod + "', '" +
                        dataGridView1.Rows[l].Cells[4].Value.ToString() + "', '" +
                        dataGridView1.Rows[l].Cells[5].Value.ToString() +
                        "')", conn);
                        k.ExecuteNonQuery();
                    }
                }
                else if (i == 1)
                {
                    if (dataGridView1.Rows[l].Cells[3].Value.ToString() == "0") { barkod = false; }
                    else if (dataGridView1.Rows[l].Cells[3].Value.ToString() == "1") { barkod = true; }
                    using (SqlConnection conn = new SqlConnection(xmlOku()))
                    {
                        conn.Open();
                        k = new SqlCommand("UPDATE Uruns SET Barkod='" + dataGridView1.Rows[l].Cells[0].Value.ToString() + "', " +
                        "Ad='" + dataGridView1.Rows[l].Cells[1].Value.ToString() + "', " +
                        "Kategori='" + dataGridView1.Rows[l].Cells[2].Value.ToString() + "', " +
                        "Barkodlu='" + barkod + "', " +
                        "Fiyat='" + dataGridView1.Rows[l].Cells[4].Value.ToString() + "', " +
                        "Stok='" + dataGridView1.Rows[l].Cells[5].Value.ToString() + 
                        "' WHERE Barkod='" + dataGridView1.Rows[l].Cells[0].Value.ToString() + "'", conn);
                        k.ExecuteNonQuery();
                    }
                }

            }
            string[] sutunAdi = { "Ad", "Kategori", "Fiyat", "Stok" };
            for (int m = 0; m < sutunAdi.Length; m++)
            {
                if (sutunAdi[m] == "Fiyat" || sutunAdi[m] == "Stok")
                {
                    using (SqlConnection conn = new SqlConnection(xmlOku()))
                    {
                        conn.Open();
                        k = new SqlCommand("UPDATE Uruns SET " + sutunAdi[m] + "='" + "0" + "' WHERE (" + sutunAdi[m] + " IS NULL)", conn);
                        k.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(xmlOku()))
                    {
                        conn.Open();
                        k = new SqlCommand("UPDATE Uruns SET " + sutunAdi[m] + "=''" + "" + "WHERE " + sutunAdi[m] + " IS NULL", conn);
                        k.ExecuteNonQuery();
                    }
                }
            }
            urunexcelgoster();
            MessageBox.Show("Tablo Doldurma İşlemi Tamamlanmıştır.", "Tablo Doldurma");

        }

        private void buttonUrunExcelGuncelle_Click(object sender, EventArgs e)
        {
            urunexcelguncelle();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void excelsenkronizasyon_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Equals("EXCEL"))
                {
                    clsProcess.Kill();
                    break;
                }
            }
        }
    }
}
