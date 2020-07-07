using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using WRKANTIN.Entities;

namespace WRKANTIN
{
    public partial class Form2 : Form
    {
        public SqlConnection sqlbag, sqlbag2;
        SqlCommand k;
        SqlDataReader rd;
        StreamReader sr;
        Form1 frm;
        private KantinContext db;
        string baglanti = "", key = "";

        public Form2()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            xmlOku();
            sqlconfigoku();
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
        public void sqlconfigoku()
        {                 
            sqlbag = new SqlConnection(baglanti);
            k = new SqlCommand("select sifre,adsoyad,yetki from Kullanicis", sqlbag);
            sqlbag.Open();
            rd = k.ExecuteReader();
            rd.Read();
            sqlbag.Close();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                sifrekontrol();
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            sifrekontrol();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
                    
            Application.ExitThread();
            Application.Exit();
        }
    
        public void sifrekontrol()
        {
            try
            {
                k = new SqlCommand("select sifre,adsoyad,yetki from Kullanicis WHERE kullaniciadi='" + textBox1.Text + "'", sqlbag);
                sqlbag.Open();
                rd = k.ExecuteReader();
                rd.Read();
                string sifre = rd["sifre"].ToString();
                if (sifre == textBox2.Text)
                {
                    frm = new Form1();
                    string yetkili = rd["adsoyad"].ToString();
                    string yetki = rd["yetki"].ToString();
                    if (yetki == "0")
                    {
                        frm.pictureBoxkayitlar.Enabled = false;
                    }
                    else if (yetki == "1") { yetki = "1"; frm.tabControl1.SelectedIndex = 1; }
                    sqlbag.Close();
                    frm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Hatalı Şifre Girişi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Giriş İşleminde Hata");
            }
        }

        public Screen GetSecondaryScreen()
        {
            if (Screen.AllScreens.Length == 1)
            {
                return null;
            }
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Primary == false)
                {
                    return screen;
                }
            }
            return null;
        }

    }
}
