using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using WRKANTIN.Entities;

namespace WRKANTIN
{
    public partial class Form1 : Form
    {
        private KantinContext db;
        string baglanti = "", key = "";
        bool veritabaniyedeklendi = false;
        public Form1()
        {
            InitializeComponent();
            xmlOku();
            RegistryIslemleri();
            db.KullaniciSet.ToList();          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ayarDoldur();
            sepetiYenile();
            textBox2.Focus();
            barkodsuzUrunListele();
            timer1.Start();
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

        public void sepetiYenile()
        {
            var list = db.SepetSet.Select(i => new { i.SepetUrun.Barkod, i.SepetUrun.Ad, i.SepetUrun.Fiyat, i.Adet }).ToList();
            dataGridViewSepet.DataSource = list;
            decimal toplam = 0;
            foreach (var item in list)
            {
                toplam += item.Fiyat * item.Adet;
            }
            textBox1.Text = toplam.ToString();
        }
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string barkod = textBox2.Text;
                Urun urun = db.UrunSet.FirstOrDefault(i => i.Barkod == barkod);
                if (urun == null) { return; }
                Sepet sepet = null;
                sepet = db.SepetSet.FirstOrDefault(i => i.UrunId == urun.UrunId);
                if (sepet == null)
                {
                    sepet = new Sepet() { UrunId = urun.UrunId, SepetUrun = urun, Adet = 1 };
                    db.SepetSet.Add(sepet);
                }
                else
                {
                    int adet = sepet.Adet; adet++;
                    sepet.Adet = adet;
                }
                db.SaveChanges();
                var list = db.SepetSet.Select(i => new { i.SepetUrun.Barkod, i.SepetUrun.Ad, i.SepetUrun.Fiyat, i.Adet }).ToList();
                dataGridViewSepet.DataSource = list;
                decimal toplam = 0;
                foreach (var item in list)
                {
                    toplam += item.Fiyat * item.Adet;
                }
                textBox1.Text = toplam.ToString();
                textBox2.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OdemeForm odemeForm = new OdemeForm();
            odemeForm.label3.Text = textBox1.Text;
            odemeForm.ShowDialog();

        }

        private void buttonUrunEkle_Click(object sender, EventArgs e)
        {
            Urun urun = null;
            urun= db.UrunSet.FirstOrDefault(i => i.Barkod == textBoxUrunBarkod.Text);
            if (urun != null) { MessageBox.Show("Girilen Barkod Numarası Mevcut!"); return; }
            urun = new Urun()
            {
                Barkod = textBoxUrunBarkod.Text,
                Ad = textBoxUrunAd.Text,
                Barkodlu = radioButtonUrunBarkodlu.Checked,
                Fiyat =Convert.ToDecimal( textBoxUrunFiyat.Text),
                Kategori = comboBoxurunKategori.Text,
                Stok = Convert.ToInt32(textBoxUrunStok.Text)
            };
            db.UrunSet.Add(urun);
            db.SaveChanges();
            MessageBox.Show("Ürün Ekleme İşlemi Başarılı", "İşlem Başarılı");
            labelurunId.Text = "";
            textBoxUrunAd.Clear();
            textBoxUrunBarkod.Clear();
            comboBoxurunKategori.Text = "";
            textBoxUrunFiyat.Clear();
            textBoxUrunStok.Clear();
            urunleriListele();
        }

        public void urunleriListele()
        {
            var urun = db.UrunSet.ToList();
            dataGridView2.DataSource = urun;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            urunleriListele();
            urunKategoriDoldur();
            labelurunId.Text = "";
            textBoxUrunAd.Clear();
            textBoxUrunBarkod.Clear();
            comboBoxurunKategori.Text = "";
            textBoxUrunFiyat.Clear();
            textBoxUrunStok.Clear();
            textBoxBarkodAra.Clear();
            textBoxUrunAdAra.Clear();
            tabControl1.SelectedIndex = 1;
        }

        private void buttonUrunGuncelle_Click(object sender, EventArgs e)
        {
            if (labelurunId.Text != "")
            {
                if(labelurunId.Text != "id")
                {
                    int urunId = Convert.ToInt32(labelurunId.Text);
                    Urun urun = null;
                    urun = db.UrunSet.FirstOrDefault(i => i.UrunId == urunId);

                    urun.Barkod = textBoxUrunBarkod.Text;
                    urun.Ad = textBoxUrunAd.Text;
                    urun.Barkodlu = radioButtonUrunBarkodlu.Checked;
                    urun.Fiyat = Convert.ToDecimal(textBoxUrunFiyat.Text);
                    urun.Kategori = comboBoxurunKategori.Text;
                    urun.Stok = Convert.ToInt32(textBoxUrunStok.Text);
                    db.SaveChanges();
                    labelurunId.Text = "";
                    textBoxUrunAd.Clear();
                    textBoxUrunBarkod.Clear();
                    comboBoxurunKategori.Text = "";
                    textBoxUrunFiyat.Clear();
                    textBoxUrunStok.Clear();
                    urunleriListele();
                    MessageBox.Show("Ürün Ekleme İşlemi Başarılı", "İşlem Başarılı");
                }               
            }
            else
            {
                MessageBox.Show("Öncelikle Ürün Seçimi Yapılmalıdır.");
            }
            
        }

        private void dataGridView2_DoubleClick(object sender, EventArgs e)
        {
            textBoxEklenecekStok.Enabled = true;
            string barkod = dataGridView2.SelectedRows[0].Cells[1].Value.ToString();
            Urun urun = null;
            urun = db.UrunSet.FirstOrDefault(i => i.Barkod == barkod);
            if (urun == null) { return; }
            labelurunId.Text = urun.UrunId.ToString();
            textBoxUrunBarkod.Text = urun.Barkod;
            textBoxUrunAd.Text = urun.Ad;
            radioButtonUrunBarkodlu.Checked = urun.Barkodlu;
            textBoxUrunFiyat.Text = urun.Fiyat.ToString();
            comboBoxurunKategori.Text = urun.Kategori;
            textBoxUrunStok.Text = urun.Stok.ToString();           
            textBoxSuAnkiStok.Text = urun.Stok.ToString();
        }

        public void urunKategoriDoldur()
        {
            List<string> liste = db.SatilanUrunSet.Select(k => k.urun.Kategori).Distinct().ToList();
            foreach (var item in liste)
            {
                comboBoxurunKategori.Items.Add(item.ToString());
            }
        }

        private void buttonOgrenciEkle_Click(object sender, EventArgs e)
        {
            Ogrenci ogrenci = null;
            ogrenci = db.OgrenciSet.FirstOrDefault(i => i.No == textBoxOgrenciNo.Text);
            if (ogrenci != null) { MessageBox.Show("Girilen Öğrenci Numarası Mevcut");return; }
            ogrenci = new Ogrenci()
            {
                No = textBoxOgrenciNo.Text,
                Ad = textBoxOgrenciAdi.Text,
                Soyad = textBoxOgrenciSoyadi.Text,
                Sinif = textBoxOgrenciSinifi.Text,
                Tel = textBoxOgrenciTelefonu.Text,
                KartNo = textBoxOgrenciKartNo.Text,
                Bakiye = Convert.ToDecimal(textBoxOgrenciBakiye.Text)
            };
            db.OgrenciSet.Add(ogrenci);
            db.SaveChanges();
            MessageBox.Show("Öğrenci Ekleme İşlemi Başarılı", "İşlem Başarılı");
            ogrencileriListele();
        }

        private void buttonOgrenciGuncelle_Click(object sender, EventArgs e)
        {
            if (labelogrenciId.Text != "")
            {
                int ogrenciid = Convert.ToInt32(labelogrenciId.Text);
                Ogrenci ogrenci = null;
                ogrenci = db.OgrenciSet.FirstOrDefault(i => i.OgrenciId == ogrenciid);
                if (ogrenci == null) { return; }
                ogrenci.No = textBoxOgrenciNo.Text;
                ogrenci.Ad = textBoxOgrenciAdi.Text;
                ogrenci.Soyad = textBoxOgrenciSoyadi.Text;
                ogrenci.Sinif = textBoxOgrenciSinifi.Text;
                ogrenci.Tel = textBoxOgrenciTelefonu.Text;
                ogrenci.KartNo = textBoxOgrenciKartNo.Text;
                ogrenci.Bakiye = Convert.ToDecimal(textBoxOgrenciBakiye.Text);
                db.SaveChanges();
                MessageBox.Show("Öğrenci Güncelleme İşlemi Başarılı", "İşlem Başarılı");
                labelogrenciId.Text = "";
                textBoxOgrenciNo.Clear();
                textBoxOgrenciAdi.Clear();
                textBoxOgrenciSoyadi.Clear();
                textBoxOgrenciSinifi.Clear();
                textBoxOgrenciTelefonu.Clear();
                textBoxOgrenciKartNo.Clear();
                textBoxOgrenciBakiye.Clear();
                textBoxBakiyeSuanki.Clear();
                ogrencileriListele();
            }
            else
            {
                MessageBox.Show("Öncelikle Öğrenci Seçimi Yapılmalıdır!");
            }
            
        }

        private void dataGridView3_DoubleClick(object sender, EventArgs e)
        {
            textBoxBakiyeEklenecek.Enabled = true;
            string no = dataGridView3.SelectedRows[0].Cells[1].Value.ToString();
            Ogrenci ogrenci = null;
            ogrenci = db.OgrenciSet.FirstOrDefault(i => i.No == no);
            labelogrenciId.Text = ogrenci.OgrenciId.ToString();
            textBoxOgrenciNo.Text = ogrenci.No;
            textBoxOgrenciAdi.Text = ogrenci.Ad;
            textBoxOgrenciSoyadi.Text = ogrenci.Soyad;
            textBoxOgrenciSinifi.Text = ogrenci.Sinif;
            textBoxOgrenciTelefonu.Text = ogrenci.Tel;
            textBoxOgrenciKartNo.Text = ogrenci.KartNo;
            textBoxOgrenciBakiye.Text = ogrenci.Bakiye.ToString();
            textBoxBakiyeSuanki.Text = ogrenci.Bakiye.ToString();
        }

        public void ogrencileriListele()
        {
            var ogrenci = db.OgrenciSet.ToList();
            dataGridView3.DataSource = ogrenci;
        }

        private void pictureBoxkayitlar_Click(object sender, EventArgs e)
        {
            labelogrenciId.Text = "";
            textBoxOgrenciNo.Clear();
            textBoxOgrenciAdi.Clear();
            textBoxOgrenciSoyadi.Clear();
            textBoxOgrenciSinifi.Clear();
            textBoxOgrenciTelefonu.Clear();
            textBoxOgrenciKartNo.Clear();
            textBoxOgrenciBakiye.Clear();
            textBoxBakiyeSuanki.Clear();
            RefreshAll();

            ogrencileriListele();
            tabControl1.SelectedIndex = 2;
        }

        public void RefreshAll()
        {
            foreach (var entity in db.ChangeTracker.Entries())
            {
                entity.Reload();
            }
        }

        private void buttonOgrenciSil_Click(object sender, EventArgs e)
        {
            int ogrenciId = Convert.ToInt32(labelogrenciId.Text);
            Ogrenci ogrenci = db.OgrenciSet.FirstOrDefault(i => i.OgrenciId == ogrenciId);
            if(ogrenci==null) { return; }
            db.OgrenciSet.Remove(ogrenci);
            db.SaveChanges();
            MessageBox.Show("Öğrenci Silme İşlemi Başarılı");
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            kategoriDoldur();
            SatislariListele();          
            comboBoxKategoriSec.Enabled = false;           
            tabControl1.SelectedIndex = 3;
        }

        private void barkodsuzUrunListele()
        {
            var urunList = db.UrunSet.Select(i => new { i.Barkod, i.Ad, i.Kategori, i.Barkodlu, i.Fiyat, i.Stok }).Where(i=> i.Barkodlu==false).ToList();
            dataGridViewBarkodsuzUrunler.DataSource = urunList;
        }
    

        public void SatislariListele()
        {
            if (radioButton1.Checked)
            {
                comboBoxKategoriSec.SelectedIndex = 0;
                comboBoxKategoriSec.Enabled = false;
                var list = db.SatilanSet.Select(i => new { i.SatilanId, i.ToplamTutar, Ogrenci = i.Alici.Ad + " " + i.Alici.Soyad, i.Tarih }).Distinct().ToList();
                dataGridView4.DataSource = list;
                dataGridView4.Columns[0].HeaderText = "Sipariş Numarası";
            }
            else if(radioButton2.Checked)
            {
                comboBoxKategoriSec.Enabled = true;
                var list = db.SatilanUrunSet.Select(i => new { i.SatilanId, i.urun.Barkod, i.urun.Kategori, i.urun.Ad, i.urun.Fiyat, i.Adet }).ToList();
                dataGridView4.DataSource = list;
                dataGridView4.Columns[0].HeaderText = "Sipariş Numarası";
            }
            else if (radioButton3.Checked)
            {
                var list = db.BakiyeHareketSet.Select(i => new { i.ogrenci.No, i.ogrenci.Ad,i.ogrenci.Soyad, i.yuklenenBakiye,i.yuklemeTarihi }).ToList();
                dataGridView4.DataSource = list;
            }
            else if (radioButton4.Checked)
            {
                groupBox7.Visible = true;
                groupBox5.Visible = false;
                double yuklubakiye = 0;
                var yuklubakiyelist = db.OgrenciSet.Select(i => new { i.No,i.Ad,i.Soyad,i.KartNo, i.Bakiye }).ToList();
                dataGridView4.DataSource = yuklubakiyelist;
                for (int j = 0; j < dataGridView4.Rows.Count; j++)
                {
                    yuklubakiye += Convert.ToDouble(dataGridView4.Rows[j].Cells[4].Value.ToString());
                }
                labelYukluBakiye.Text = yuklubakiye.ToString() + " TL";
            }
        }
        public void SatislariListele(DateTime baslangic,DateTime bitis)
        {
            if (radioButton1.Checked)
            {
                groupBox7.Visible = false;
                groupBox5.Visible = true;
                bitis = bitis.AddDays(1);
                if (comboBoxKategoriSec.SelectedItem.ToString() == "Tümü")
                {
                    var list = db.SatilanSet.Select(i => new { i.SatilanId, i.ToplamTutar, Ogrenci = i.Alici.Ad + " " + i.Alici.Soyad, i.Tarih }).Where(k => k.Tarih > baslangic && k.Tarih < bitis).Distinct().ToList();
                    dataGridView4.DataSource = list;
                    dataGridView4.Columns[0].HeaderText = "Sipariş Numarası";
                }             
            }
            else if(radioButton2.Checked)
            {
                groupBox7.Visible = false;
                groupBox5.Visible = true;
                int satilanUrunCesit = 0;
                double hasilat = 0;
                if (comboBoxKategoriSec.SelectedItem.ToString() == "" || comboBoxKategoriSec.SelectedItem.ToString() == "Tümü")
                {                  
                    bitis = bitis.AddDays(1);
                    var list = db.SatilanUrunSet.Select(i => new { i.SatilanId, i.urun.Barkod, i.urun.Kategori, i.urun.Ad, i.urun.Fiyat, i.Adet, i.Siparis.Tarih }).Where(j => j.Tarih < bitis && j.Tarih > baslangic).ToList();
                    dataGridView4.DataSource = list;

                    labelUrunCesit.Text = db.SatilanUrunSet.Select(j => j.urun.Barkod).Distinct().Count().ToString() + " Çeşit";
                    for (int i = 0; i < dataGridView4.Rows.Count; i++)
                    {
                        satilanUrunCesit += Convert.ToInt32(dataGridView4.Rows[i].Cells[5].Value);
                        hasilat += Convert.ToDouble(dataGridView4.Rows[i].Cells[4].Value);
                    }
                    labelSatilanAdet.Text = satilanUrunCesit.ToString() + " ADET";
                    labelHasilat.Text = hasilat.ToString() + " TL";
                    dataGridView4.Columns[0].HeaderText = "Sipariş Numarası";
                }
                else
                {
                    bitis = bitis.AddDays(1);
                    string kategori = comboBoxKategoriSec.SelectedItem.ToString();
                    var list = db.SatilanUrunSet.Select(i => new { i.SatilanId,i.urun.Barkod, i.urun.Kategori, i.urun.Ad, i.urun.Fiyat, i.Adet, i.Siparis.Tarih }).Where(j => j.Tarih < bitis && j.Tarih > baslangic && j.Kategori == kategori).ToList();
                    dataGridView4.DataSource = list;
                    labelUrunCesit.Text = db.SatilanUrunSet.Select(j => j.urun.Barkod).Distinct().Count().ToString() + " Çeşit";
                    for (int i = 0; i < dataGridView4.Rows.Count; i++)
                    {
                        satilanUrunCesit += Convert.ToInt32(dataGridView4.Rows[i].Cells[5].Value);
                        hasilat += Convert.ToDouble(dataGridView4.Rows[i].Cells[4].Value);
                    }
                    labelSatilanAdet.Text = satilanUrunCesit.ToString() + " ADET";
                    labelHasilat.Text = hasilat.ToString() + " TL";
                    dataGridView4.Columns[0].HeaderText = "Sipariş Numarası";
                }               
            }
            else if (radioButton3.Checked)
            {
                groupBox7.Visible = true;
                groupBox5.Visible = false;
                label34.Visible = true; label35.Visible = true;
                labelislemsayisi.Visible = true; labelbakiyehasilat.Visible = true;
                bitis = bitis.AddHours(23).AddMinutes(59);
                var list = db.BakiyeHareketSet.Select(i => new { i.ogrenci.No, i.ogrenci.Ad, i.ogrenci.Soyad, i.yuklenenBakiye, i.yuklemeTarihi }).Where(j=>j.yuklemeTarihi<=bitis && j.yuklemeTarihi>=baslangic).ToList();
                dataGridView4.DataSource = list;
                double hasilat = 0;
                for(int i = 0; i < dataGridView4.Rows.Count; i++)
                {
                    hasilat += Convert.ToDouble(dataGridView4.Rows[i].Cells[3].Value.ToString());
                }
                label23.Text = "Toplam İşlem Sayısı";
                labelislemsayisi.Text = dataGridView4.Rows.Count.ToString();
                labelbakiyehasilat.Text = hasilat.ToString() + " TL";
            }
            else if (radioButton4.Checked)
            {
                groupBox7.Visible = true;
                groupBox5.Visible = false;
                label34.Visible = false;label35.Visible = false;
                labelislemsayisi.Visible = false;labelbakiyehasilat.Visible = false;
                double yuklubakiye = 0;
                var yuklubakiyelist = db.OgrenciSet.Select(i => new { i.No, i.Ad, i.Soyad, i.KartNo, i.Bakiye }).ToList();
                dataGridView4.DataSource = yuklubakiyelist;
                for (int j = 0; j < dataGridView4.Rows.Count; j++)
                {
                    yuklubakiye += Convert.ToDouble(dataGridView4.Rows[j].Cells[4].Value.ToString());
                }
                labelYukluBakiye.Text = yuklubakiye.ToString() + " TL";
            }
        }

        public void kategoriDoldur()
        {
            comboBoxKategoriSec.Items.Clear();
            comboBoxKategoriSec.Items.Add("Tümü");
            List<string> liste = db.SatilanUrunSet.Select(k => k.urun.Kategori).Distinct().ToList();
            foreach (var item in liste)
            {
                comboBoxKategoriSec.Items.Add(item.ToString());
            }
            comboBoxKategoriSec.SelectedIndex = 0;
        }

        private void pictureBoxcikis_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
            try
            {
                Application.Exit();
                Environment.Exit(1);
            }
            catch (Exception)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        private void textBoxEklenecekStok_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }
            if (textBoxSuAnkiStok.Text == "") { textBoxSuAnkiStok.Text = "0"; }          
        }

        private void textBoxEklenecekStok_TextChanged(object sender, EventArgs e)
        {
            if (textBoxEklenecekStok.Text != "")
            {
                if (textBoxEklenecekStok.Text != "-") { textBoxYeniStok.Text = 
                        (Convert.ToInt32(textBoxSuAnkiStok.Text) +
                        Convert.ToInt32(textBoxEklenecekStok.Text)).ToString(); }               
            }
            else
            {
                textBoxYeniStok.Text = textBoxSuAnkiStok.Text;
            }
        }

        private void buttonStokGuncelle_Click(object sender, EventArgs e)
        {
            int urunId = Convert.ToInt32(labelurunId.Text);
            Urun urun = null;
            urun = db.UrunSet.FirstOrDefault(i => i.UrunId == urunId);
            DialogResult res = MessageBox.Show(urun.Ad + 
                " isimli ürünün stoğu " + textBoxEklenecekStok.Text +
                " adet artırılıp " + textBoxYeniStok.Text +
                " olarak güncellenecektir.Onaylıyor Musunuz?","Ürün Stok Güncelleme" 
                , MessageBoxButtons.YesNo
                ,MessageBoxIcon.Information);
            if (res == DialogResult.Yes) 
            {
                urun.Stok = Convert.ToInt32(textBoxYeniStok.Text);
                db.SaveChanges();
                urunleriListele();
                textBoxSuAnkiStok.Clear();
                textBoxYeniStok.Clear();
                textBoxEklenecekStok.Enabled = false;
                textBoxEklenecekStok.Clear();
                MessageBox.Show("Stok Güncelleme İşlemi Başarılı", "İşlem Başarılı");
            }         
        }

        private void textBoxBakiyeEklenecek_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '-') && (e.KeyChar != ','))
            {
                e.Handled = true;
            }
            if (textBoxBakiyeSuanki.Text == "") { textBoxBakiyeSuanki.Text = "0"; }
        }

        private void textBoxBakiyeEklenecek_TextChanged(object sender, EventArgs e)
        {
            if (textBoxBakiyeEklenecek.Text != "")
            {
                if (textBoxBakiyeEklenecek.Text != "-")
                {
                    textBoxBakiyeGuncel.Text =
                    (Convert.ToDouble(textBoxBakiyeSuanki.Text) +
                     Convert.ToDouble(textBoxBakiyeEklenecek.Text)).ToString();
                }
            }
            else
            {
                textBoxBakiyeGuncel.Text = textBoxBakiyeSuanki.Text;
            }
        }

        private void buttonBakiyeGuncelle_Click(object sender, EventArgs e)
        {
            int ogrenciid = Convert.ToInt32(labelogrenciId.Text);
            Ogrenci ogrenci = null;
            ogrenci = db.OgrenciSet.FirstOrDefault(i => i.OgrenciId == ogrenciid);
            if (ogrenci == null) { return; }
            DialogResult res = MessageBox.Show(ogrenci.Ad + " " + ogrenci.Soyad +
                " isimli öğrencinin bakiyesi " + textBoxBakiyeEklenecek.Text +
                " TL eklenip " + textBoxBakiyeGuncel.Text +
                " TL olarak güncellenecektir.Onaylıyor Musunuz?", "Öğrenci Bakiye Güncelleme"
                , MessageBoxButtons.YesNo
                , MessageBoxIcon.Information);
            if (res == DialogResult.Yes)
            {
                ogrenci.Bakiye = Convert.ToDecimal(textBoxBakiyeGuncel.Text);
                BakiyeHareket bakiyeekleme = new BakiyeHareket()
                {
                    ogrenci = ogrenci,
                    yuklenenBakiye = Convert.ToDecimal(textBoxBakiyeEklenecek.Text),
                    yuklemeTarihi = DateTime.Now
                };
                db.BakiyeHareketSet.Add(bakiyeekleme);
                db.SaveChanges();
                MessageBox.Show("Bakiye Güncelleme İşlemi Başarılı", "İşlem Başarılı");
                textBoxBakiyeSuanki.Clear();
                textBoxBakiyeEklenecek.Clear();
                textBoxBakiyeGuncel.Clear();
                textBoxBakiyeEklenecek.Enabled = false;
                ogrencileriListele();
            }           
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            sepetiYenile();
            textBox2.Focus();
            barkodsuzUrunListele();
        }

       


        private void dataGridViewBarkodsuzUrunler_DoubleClick(object sender, EventArgs e)
        {
            string barkod = dataGridViewBarkodsuzUrunler.SelectedRows[0].Cells[0].Value.ToString();
            Urun urun = db.UrunSet.FirstOrDefault(i => i.Barkod == barkod);
            if (urun == null) { return; }
            Sepet sepet = null;
            sepet = db.SepetSet.FirstOrDefault(i => i.UrunId == urun.UrunId);
            if (sepet == null)
            {
                sepet = new Sepet() { UrunId = urun.UrunId, SepetUrun = urun, Adet = 1 };
                db.SepetSet.Add(sepet);
            }
            else
            {
                int adet = sepet.Adet; adet++;
                sepet.Adet = adet;
            }
            db.SaveChanges();
            var list = db.SepetSet.Select(i => new { i.SepetUrun.Barkod, i.SepetUrun.Ad, i.SepetUrun.Fiyat, i.Adet }).ToList();
            dataGridViewSepet.DataSource = list;
            decimal toplam = 0;
            foreach (var item in list)
            {
                toplam += item.Fiyat * item.Adet;
            }
            textBox1.Text = toplam.ToString();
        }

        private void textBoxBarkodsuzUrunAra_TextChanged(object sender, EventArgs e)
        {
            var urunList = db.UrunSet.Select(i => new { i.Barkod, i.Ad, i.Kategori, i.Barkodlu, i.Fiyat, i.Stok }).Where(i => i.Barkodlu == false && i.Ad.Contains(textBoxBarkodsuzUrunAra.Text)).ToList();
            dataGridViewBarkodsuzUrunler.DataSource = urunList;
        }

        private void dataGridViewSepet_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewSepet.Rows.Count > 0)
            {
                string barkod = dataGridViewSepet.SelectedRows[0].Cells[0].Value.ToString();
                Urun urun = db.UrunSet.FirstOrDefault(i => i.Barkod == barkod);

                if (urun == null) { return; }
                Sepet sepet = null;
                sepet = db.SepetSet.FirstOrDefault(i => i.UrunId == urun.UrunId);
                if (sepet == null)
                {
                    return;
                }
                else
                {
                    int adet = sepet.Adet; adet--;
                    sepet.Adet = adet;
                    if (sepet.Adet == 0) { db.SepetSet.Remove(sepet); }                   
                }
                db.SaveChanges();
                var list = db.SepetSet.Select(i => new { i.SepetUrun.Barkod, i.SepetUrun.Ad, i.SepetUrun.Fiyat, i.Adet }).ToList();
                dataGridViewSepet.DataSource = list;
                decimal toplam = 0;
                foreach (var item in list)
                {
                    toplam += item.Fiyat * item.Adet;
                }
                textBox1.Text = toplam.ToString();
            }
            
        }

        private void dataGridView4_DoubleClick(object sender, EventArgs e)
        {
            int siparisno = Convert.ToInt32(dataGridView4.SelectedRows[0].Cells[0].Value.ToString());
            var satilanList = db.SatilanUrunSet.Select(i => new { i.SatilanId,i.urun.Barkod,i.urun.Kategori, i.urun.Ad, i.urun.Fiyat,i.Adet }).Where(j=> j.SatilanId == siparisno).ToList();
            //var satilanList = db.SatilanSet.Where(j => j.SatilanId == siparisno).Select(i => new { i.satilanlar }).ToList();
            
            if (satilanList == null){return;}
            SiparisAyrinti siparisAyrinti = new SiparisAyrinti();

            siparisAyrinti.dataGridViewSatisAyrinti.DataSource = satilanList;
            siparisAyrinti.label1.Text += dataGridView4.SelectedRows[0].Cells[0].Value.ToString();
            siparisAyrinti.label2.Text += dataGridView4.SelectedRows[0].Cells[2].Value.ToString();
            siparisAyrinti.label3.Text += dataGridView4.SelectedRows[0].Cells[3].Value.ToString();
            siparisAyrinti.ShowDialog();
            siparisAyrinti.dataGridViewSatisAyrinti.Columns[0].HeaderText = "Sipariş No";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {           
            if (radioButton1.Checked == false)
            {
                if (dataGridView4.Rows.Count > 0)
                {
                    dataGridView4.DataSource = null;
                }               
            }
            else
            {
                groupBox7.Visible = false;
                groupBox5.Visible = true;
                SatislariListele(Convert.ToDateTime(dateTimePickerBaslangic.Text), Convert.ToDateTime(dateTimePickerBitis.Text));
            }
        }

        private void pictureBoxSiparisFiltrele_Click(object sender, EventArgs e)
        {
            SatislariListele(Convert.ToDateTime(dateTimePickerBaslangic.Text), Convert.ToDateTime(dateTimePickerBitis.Text));
        }

        private void pictureBoxurunExcelSenk_Click(object sender, EventArgs e)
        {
            excelsenkronizasyon excel = new excelsenkronizasyon();
            excel.buttonExcelGoster.Visible = false;
            excel.ShowDialog();
        }

        private void pictureBoxOgrenciExcelSenk_Click(object sender, EventArgs e)
        {
            excelsenkronizasyon excel = new excelsenkronizasyon();
            excel.buttonUrunExcelGoruntule.Visible = false;
            excel.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelsaat.Text = DateTime.Now.ToString("HH:mm");
            labeltarih.Text = DateTime.Now.ToString("dd-MM-yyyy");
            if(labelsaat.Text == "17:30" && veritabaniyedeklendi==false)
            {
                if (!File.Exists("C:\\VeritabaniYedekler\\KantinBackup" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".bak"))
                {
                    using (SqlConnection conn = new SqlConnection("Data Source=.; User Id=sa; Password=Recep123"))
                    {
                        conn.Open();
                        SqlCommand command = new SqlCommand("BACKUP DATABASE [KantinContext] TO DISK = N'C:\\VeritabaniYedekler\\KantinBackup" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".bak' WITH INIT", conn);
                        command.ExecuteNonQuery();
                    }
                }
                veritabaniyedeklendi = true;
            }
            if(labelsaat.Text == "17:35")
            {
                DirectoryInfo d = new DirectoryInfo(@"C:\\VeritabaniYedekler");//Assuming Test is your Folder
                FileInfo[] Files = d.GetFiles("*.bak"); //Getting Text files
                string str = "";
                foreach (FileInfo file in Files)
                {
                    if (file.CreationTime < DateTime.Now.AddDays(-7))
                    {
                        file.Delete();
                    }
                }
                veritabaniyedeklendi = false;
            }
        }

        private void pictureBoxayarlar_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
            ayarDoldur();
        }

        public void ayarDoldur()
        {
            Ayarlar ayar = new Ayarlar();
            ayar = db.AyarSet.FirstOrDefault();
            YaziciDoldur();
            if (ayar != null)
            {
                textBoxFirmaBaslik.Text = ayar.FisFirma;
                textBoxFisBaslik.Text = ayar.FisBaslik;
                textBoxsonSatir.Text = ayar.SonSatir;
                comboBoxYazici.Text = ayar.YaziciAdi;
            }
            else
            {
                ayar = new Ayarlar();
                ayar.FisFirma = "";
                ayar.FisBaslik = "";
                ayar.YaziciAdi = "";
                db.AyarSet.Add(ayar);
                db.SaveChanges();
            }
        }

        public void YaziciDoldur()
        {
            comboBoxYazici.Items.Clear();
            foreach(String yazici in PrinterSettings.InstalledPrinters)
            {
                comboBoxYazici.Items.Add(yazici);
            }
        }

        private void buttonYaziciAyarGuncelle_Click(object sender, EventArgs e)
        {
            Ayarlar ayar = new Ayarlar();
            ayar = db.AyarSet.FirstOrDefault();
            ayar.FisFirma = textBoxFirmaBaslik.Text;
            ayar.FisBaslik = textBoxFisBaslik.Text;
            ayar.SonSatir = textBoxsonSatir.Text;
            ayar.YaziciAdi = comboBoxYazici.SelectedItem.ToString();
            db.SaveChanges();
        }

        private void buttonSepetiBosalt_Click(object sender, EventArgs e)
        {
            if (dataGridViewSepet.Rows.Count > 0)
            {
                for(int j = 0; j < dataGridViewSepet.Rows.Count; j++)
                {
                    string barkod = dataGridViewSepet.Rows[j].Cells[0].Value.ToString();
                    Urun urun = db.UrunSet.FirstOrDefault(i => i.Barkod == barkod);

                    if (urun == null) { return; }
                    Sepet sepet = null;
                    sepet = db.SepetSet.FirstOrDefault(i => i.UrunId == urun.UrunId);
                    if (sepet == null)
                    {
                        return;
                    }
                    db.SepetSet.Remove(sepet);
                    db.SaveChanges();                    
                }
                var list = db.SepetSet.Select(i => new { i.SepetUrun.Barkod, i.SepetUrun.Ad, i.SepetUrun.Fiyat, i.Adet }).ToList();
                dataGridViewSepet.DataSource = list;
                decimal toplam = 0;               
                textBox1.Text = toplam.ToString();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == false)
            {
                dataGridView4.DataSource = null;       
            }
            else
            {
                groupBox7.Visible = true;
                groupBox5.Visible = false;
                label34.Visible = true; label35.Visible = true;
                label31.Visible = false;labelYukluBakiye.Visible = false;
                labelislemsayisi.Visible = true; labelbakiyehasilat.Visible = true;
                labelislemsayisi.Text = ""; labelbakiyehasilat.Text = "";
                SatislariListele(Convert.ToDateTime(dateTimePickerBaslangic.Text), Convert.ToDateTime(dateTimePickerBitis.Text));
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == false)
            {
                if (dataGridView4.Rows.Count > 0)
                {
                    dataGridView4.DataSource = null;
                }
            }
            else
            {
                groupBox7.Visible = false;
                groupBox5.Visible = true;
                SatislariListele(Convert.ToDateTime(dateTimePickerBaslangic.Text), Convert.ToDateTime(dateTimePickerBitis.Text));
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked == false)
            {
                if (dataGridView4.Rows.Count > 0)
                {
                    dataGridView4.DataSource = null;
                }
            }
            else
            {
                groupBox7.Visible = true;
                groupBox5.Visible = false;
                label34.Visible = false; label35.Visible = false;
                label31.Visible = true; labelYukluBakiye.Visible = true;
                labelYukluBakiye.Text = "";
                labelislemsayisi.Visible = false; labelbakiyehasilat.Visible = false;
                SatislariListele();
            }
        }

        private void textBoxOgrenciNoAra_TextChanged(object sender, EventArgs e)
        {
            if (textBoxOgrenciNoAra.Text != "")
            {
                var ogrenciList = db.OgrenciSet.Where(i => i.No.Contains(textBoxOgrenciNoAra.Text)).ToList();
                dataGridView3.DataSource = ogrenciList;
            }
            else
            {
                var ogrenciList = db.OgrenciSet.ToList();
                dataGridView3.DataSource = ogrenciList;
            }
            
        }

        private void textBoxOgrenciAdAra_TextChanged(object sender, EventArgs e)
        {
            if (textBoxOgrenciAdAra.Text != "")
            {
                var ogrenciList = db.OgrenciSet.Where(i => i.Ad.Contains(textBoxOgrenciAdAra.Text)).ToList();
                dataGridView3.DataSource = ogrenciList;
            }
            else
            {
                var ogrenciList = db.OgrenciSet.ToList();
                dataGridView3.DataSource = ogrenciList;
            }
        }

        private void textBoxBarkodAra_TextChanged(object sender, EventArgs e)
        {
            if (textBoxBarkodAra.Text != "")
            {
                var urunList = db.UrunSet.Where(i => i.Barkod.Contains(textBoxBarkodAra.Text)).ToList();
                dataGridView2.DataSource = urunList;
            }
            else
            {
                var urunList = db.UrunSet.ToList();
                dataGridView2.DataSource = urunList;
            }
            
        }

        private void textBoxUrunAdAra_TextChanged(object sender, EventArgs e)
        {
            if (textBoxUrunAdAra.Text != "")
            {
                var urunList = db.UrunSet.Where(i => i.Ad.Contains(textBoxUrunAdAra.Text)).ToList();
                dataGridView2.DataSource = urunList;
            }
            else
            {
                var urunList = db.UrunSet.ToList();
                dataGridView2.DataSource = urunList;
            }
        }

        private void pictureBoxHesapHareketYazdir_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(dateTimePickerBaslangic.Text + " tarihi ile " + dateTimePickerBitis.Text 
                + " tarihi arasındaki toplam satışlar yazdırılacaktır.Onaylıyor Musunuz?",
                "Hesap Hareket Yazdırma", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = printDocument1;
                MyPrintPreviewDialog.ShowDialog();
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            string baslik = dateTimePickerBaslangic.Text + " İLE " + dateTimePickerBitis.Text + " ARASI SATIŞ KAYITLARI";
            PrintDialog MyPrintDialog = new PrintDialog();
            MyPrintDialog.AllowCurrentPage = false;
            MyPrintDialog.AllowPrintToFile = true;
            MyPrintDialog.AllowSelection = true;
            MyPrintDialog.AllowSomePages = false;
            MyPrintDialog.PrintToFile = true;
            MyPrintDialog.ShowHelp = false;
            MyPrintDialog.ShowNetwork = false;
            MyPrintDialog.PrinterSettings.PrinterName = "Microsoft Print to PDF";
            PaperSize ps = new PaperSize();
            ps.RawKind = (int)PaperKind.A4;
            MyPrintDialog.PrinterSettings.DefaultPageSettings.PaperSize = ps;

            printDocument1.DocumentName = "SATIS KAYITLARI";
            printDocument1.PrinterSettings = MyPrintDialog.PrinterSettings;
            printDocument1.DefaultPageSettings = MyPrintDialog.PrinterSettings.DefaultPageSettings;
            printDocument1.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(40, 40, 80, 40);
            Font baslikFont = new Font("Calibri", 12, FontStyle.Bold);
            Font metinFont = new Font("Calibri", 10, FontStyle.Bold);
            Font griFont = new Font("Calibri", 10, FontStyle.Regular);
            Font kucukFont = new Font("Calibri", 7, FontStyle.Regular);
            SolidBrush sbrush = new SolidBrush(Color.Black);
            SolidBrush gbrush = new SolidBrush(Color.DarkGray);
            Pen pen = new Pen(Color.DarkGray);
            Point point = new Point(80, 50);
            e.Graphics.DrawString(baslik, baslikFont, sbrush, point.X+160,point.Y);
            point.Y += 20;
            e.Graphics.DrawLine(pen, point.X, point.Y, point.X + 667, point.Y);
            DateTime baslangic = Convert.ToDateTime(dateTimePickerBaslangic.Text);
            DateTime bitis = Convert.ToDateTime(dateTimePickerBitis.Text);
            double gunsayi = (bitis - baslangic).TotalDays;
            int ustnokta = point.Y;
            e.Graphics.DrawString("TARİH", metinFont, sbrush, point.X + 80, point.Y + 10);
            e.Graphics.DrawString("İŞLEM SAYISI", metinFont, sbrush, point.X + 200, point.Y + 10);
            e.Graphics.DrawString("HASILAT", metinFont, sbrush, point.X + 430, point.Y + 10);
            e.Graphics.DrawString("YÜKLENEN BAKİYE", metinFont, sbrush, point.X + 540, point.Y + 10);
            point.Y += 20;
            e.Graphics.DrawLine(pen, point.X, point.Y+20, point.X + 667, point.Y+20);

            decimal toplamHasilat = 0;
            decimal toplamBakiye = 0;

            for (int k = 0; k <= gunsayi; k++)
            {
                baslangic = baslangic.AddDays(k);
                bitis = baslangic.AddDays(k).AddHours(23).AddMinutes(59);
               
                decimal hasilat = 0;
                decimal yuklenenBakiye = 0;
                var list = db.SatilanSet.Select(i => new { i.ToplamTutar, i.Tarih })
                    .Where(j => j.Tarih > baslangic && j.Tarih < bitis).Distinct().ToList();
                var bakiyelist = db.BakiyeHareketSet.Select(i => new { i.ogrenci.No, i.ogrenci.Ad, 
                    i.ogrenci.Soyad, i.yuklenenBakiye, i.yuklemeTarihi })
                    .Where(j => j.yuklemeTarihi <= bitis && j.yuklemeTarihi >= baslangic).ToList();

                e.Graphics.DrawString(baslangic.AddDays(k).ToString("dd.MM.yyyy"), metinFont, sbrush, point.X + 70, point.Y + 20);
                e.Graphics.DrawString(list.Count.ToString() + " Satış " + bakiyelist.Count.ToString() + " Bakiye", metinFont, sbrush, point.X + 210, point.Y + 20);
                foreach(var item in list)
                {
                    hasilat += item.ToplamTutar;
                }
                e.Graphics.DrawString(hasilat.ToString() + " TL", metinFont, sbrush, point.X +390, point.Y + 20);
                toplamHasilat += hasilat;
                foreach(var item in bakiyelist)
                {
                    yuklenenBakiye += item.yuklenenBakiye;
                }
                e.Graphics.DrawString(yuklenenBakiye.ToString() + " TL", metinFont, sbrush, point.X + 550, point.Y + 20);
                toplamBakiye += yuklenenBakiye;
                e.Graphics.DrawLine(pen, point.X, point.Y + 40, point.X + 667, point.Y + 40);
                point.Y += 20;
            }
            e.Graphics.DrawString("TÜM İŞLEM ÖZETLERİ", metinFont, sbrush, point.X + 20, point.Y + 20);
            e.Graphics.DrawString("Toplam: " + toplamHasilat.ToString() + " TL", metinFont, sbrush, point.X + 380, point.Y + 20);
            e.Graphics.DrawString("Toplam: " + toplamBakiye.ToString() + " TL", metinFont, sbrush, point.X + 540, point.Y + 20);
            e.Graphics.DrawLine(pen, point.X, ustnokta, point.X, point.Y + 40);
            e.Graphics.DrawLine(pen, point.X+195, ustnokta, point.X+195, point.Y + 40);
            e.Graphics.DrawLine(pen, point.X+375, ustnokta, point.X+375, point.Y + 40);
            e.Graphics.DrawLine(pen, point.X+535, ustnokta, point.X+535, point.Y + 40);
            e.Graphics.DrawLine(pen, point.X + 667, ustnokta, point.X + 667, point.Y + 40);
            e.Graphics.DrawLine(pen, point.X, point.Y+40, point.X + 667, point.Y + 40);
        }

        private void buttonVeritabaniYedekle_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Veritabanının yedeği alınacaktır.Onaylıyor Musunuz?", "Veritabanı Yedekleme", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                try
                {
                    if (!Directory.Exists("C:\\VeritabaniYedekler"))
                    {
                        Directory.CreateDirectory("C:\\VeritabaniYedekler");
                    }
                    if (!File.Exists("C:\\VeritabaniYedekler\\KantinBackup" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".bak"))
                    {
                        using (SqlConnection conn = new SqlConnection("Data Source=.; User Id=sa; Password=Recep123"))
                        {
                            conn.Open();
                            SqlCommand command = new SqlCommand("BACKUP DATABASE [KantinContext] TO DISK = N'C:\\VeritabaniYedekler\\KantinBackup" + DateTime.Now.Date.ToString("yyyy-MM-dd") + ".bak' WITH INIT", conn);
                            command.ExecuteNonQuery();
                        }
                        MessageBox.Show("Veritabanı Yedekleme İşlemi Tamamlandı.");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Veritabanı Yedeği Alınamadı.Lütfen Tekrar Deneyiniz.","Hata!",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                
            }                      
        }


        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                if (radioButton1.Checked)
                {
                    var ogrenci = db.OgrenciSet.Where(j => j.No == textBox3.Text).FirstOrDefault();
                    if (ogrenci != null)
                    {
                        var list = db.SatilanSet.Where(i => i.OgrenciId == ogrenci.OgrenciId).Select(i => new { i.SatilanId, i.ToplamTutar, Ogrenci = i.Alici.Ad + " " + i.Alici.Soyad, i.Tarih }).Distinct().ToList();
                        dataGridView4.DataSource = list;
                        dataGridView4.Columns[0].HeaderText = "Sipariş Numarası";
                    }
                    else
                    {
                        MessageBox.Show("Öğrenci Numarası Yanlış.Lütfen Numarasını Kontrol Edip Tekrar Deneyiniz.");
                    }
                }
                else if (radioButton3.Checked)
                {
                    var list = db.BakiyeHareketSet.Select(i => new { i.ogrenci.No, i.ogrenci.Ad, i.ogrenci.Soyad, i.yuklenenBakiye, i.yuklemeTarihi }).Where(j => j.No==textBox3.Text).ToList();
                    dataGridView4.DataSource = list;
                } 
                else if (radioButton4.Checked)
                {
                    var yuklubakiyelist = db.OgrenciSet.Where(i=>i.No==textBox3.Text).Select(i => new { i.No, i.Ad, i.Soyad, i.KartNo, i.Bakiye }).ToList();
                    dataGridView4.DataSource = yuklubakiyelist;
                }
            }
            else
            {
                kategoriDoldur();
                SatislariListele();
                comboBoxKategoriSec.Enabled = false;
            }           
        }

        private void RegistryIslemleri()
        {
            string sistemkey = license.CPUSeriNo() + license.HDDserino();
            Registry.CurrentUser.CreateSubKey("PWR");
            RegistryKey PtsReg = Registry.CurrentUser.OpenSubKey("PWR", true);
            try { key = PtsReg.GetValue("key").ToString(); } catch (Exception e) { }
            if (string.IsNullOrEmpty(key))
            {
                try { PtsReg.SetValue("key", "1"); } catch (Exception e) { }
                DialogResult dr = MessageBox.Show("Lisans Hatası", "Lisans", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (dr == DialogResult.OK) Environment.Exit(0);
            }
            else
            {
                if (sistemkey != key)
                {
                    DialogResult dr = MessageBox.Show("Lisans Hatası", "Lisans", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (dr == DialogResult.OK) Environment.Exit(0);
                }
            }
        }
    }
}
