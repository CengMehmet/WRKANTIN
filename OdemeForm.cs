using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WRKANTIN.Entities;

namespace WRKANTIN
{
    public partial class OdemeForm : Form
    {
        private KantinContext db;
        string baglanti = "", key = "";
        Form1 anaform;
        List<SatilanUrun> yazdirilacaksepet = new List<SatilanUrun>();
        Ogrenci yazdirilacakOgrenci = new Ogrenci();
        Ogrenci bakiyeYetersizOgrenci = new Ogrenci();
        
        public OdemeForm()
        {
            InitializeComponent();
            anaform = ((Form1)Application.OpenForms["Form1"]);
            xmlOku();
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
        Ogrenci ogrenci;
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Enter)
            {
                string KartNo = textBox1.Text;
                ogrenci = db.OgrenciSet.FirstOrDefault(i => i.KartNo == KartNo);
                if (ogrenci != null)
                {
                    label2.Text = ogrenci.Bakiye.ToString();
                }
                else
                {
                    MessageBox.Show("Tanımsız Karta Satış Yapılamaz!Lütfen Tanımlı Bir Kart Okutunuz!");
                    textBox1.Clear();
                }
                
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (label2.Text != "bakiye")
            {
                if (MessageBox.Show("Ödemeyi Onaylıyor musunuz", "Ödeme", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    decimal bakiye = ogrenci.Bakiye;
                    decimal tutar = Convert.ToDecimal(label3.Text);
                    if (bakiye < tutar) 
                    {
                        DialogResult res = MessageBox.Show("Bakiye Yetersiz.Ayrıntıları yazdırmak istiyor musunuz?", "Yetersiz Bakiye", MessageBoxButtons.YesNo);
                        if (res == DialogResult.Yes)
                        {
                            bakiyeYetersizOgrenci = ogrenci;
                            var sepetlist2 = db.SepetSet.Select(i => new { i.Adet, i.SepetUrun, i.SepetId }).ToList();
                            foreach (var item in sepetlist2)
                            {
                                SatilanUrun satilanlar = new SatilanUrun() { UrunId = item.SepetUrun.UrunId, Adet = item.Adet };
                                satilanlar.urun = db.UrunSet.Where(i => i.UrunId == item.SepetUrun.UrunId).FirstOrDefault();
                                yazdirilacaksepet.Add(satilanlar);
                            }
                            yetersizfisyazdir();
                        }
                        return;
                    }
                    bakiye -= tutar;
                    ogrenci.Bakiye = bakiye;
                    var sepetlist = db.SepetSet.Select(i => new { i.Adet, i.SepetUrun, i.SepetId }).ToList();
                    yazdirilacakOgrenci = new Ogrenci();
                    yazdirilacaksepet.Clear();
                    foreach (var item in sepetlist)
                    {
                        SatilanUrun satilanlar = new SatilanUrun() { UrunId = item.SepetUrun.UrunId, Adet = item.Adet };
                        db.SatilanUrunSet.Add(satilanlar);
                        db.SepetSet.Remove(db.SepetSet.FirstOrDefault(i => i.SepetId == item.SepetId));
                        item.SepetUrun.Stok -= satilanlar.Adet;
                        yazdirilacaksepet.Add(satilanlar);
                    }
                    Satilan satilan = new Satilan() { ToplamTutar = tutar, Tarih = DateTime.Now, OgrenciId = ogrenci.OgrenciId, Alici = ogrenci };
                    yazdirilacakOgrenci = ogrenci;
                    
                    db.SatilanSet.Add(satilan);
                    db.SaveChanges();
                    if (checkBoxMakbuzYazdir.Checked)
                    {
                        fisyazdir();
                    }
                    ((Form1)Application.OpenForms["Form1"]).sepetiYenile();
                    this.Close();
                    MessageBox.Show("İşlem Başarılı"); return;
                }
            }           
        }

        private void yetersizfisyazdir()
        {
            PrintDocument pdPrint = new PrintDocument();
            pdPrint.PrintPage += new PrintPageEventHandler(pdYetersiz_PrintPage);
            pdPrint.PrinterSettings.PrinterName = anaform.comboBoxYazici.Text;
            pdPrint.Print();
        }

        private void pdYetersiz_PrintPage(object sender, PrintPageEventArgs e)
        {
            float x, y, lineOffset;
            double toplam = 0;
            // Font seçimi
            Font printFont = new Font("Lucida Console", (float)7, FontStyle.Regular, GraphicsUnit.Point); // Substituted to FontA Font

            e.Graphics.PageUnit = GraphicsUnit.Point;

            // Resim çizdir
            x = 79;
            y = 0;

            // Fişi hazırla
            lineOffset = 7 + printFont.GetHeight(e.Graphics) - (float)3.5;
            x = 0;
            y = lineOffset;
            e.Graphics.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), printFont, Brushes.Black, Convert.ToInt32(80 - (DateTime.Now.ToString("yyyy-MM-dd HH:mm").ToString()).Length), y);
            y += lineOffset;
            e.Graphics.DrawString(anaform.labelyetkili.Text, printFont, Brushes.Black, Convert.ToInt32(62 - ((anaform.labelyetkili.Text.Length * 11 / 2) / 4)), y);
            y += lineOffset;
            e.Graphics.DrawString(anaform.textBoxFirmaBaslik.Text, printFont, Brushes.Black, Convert.ToInt32(62 - ((anaform.textBoxFirmaBaslik.Text.Length * 11 / 2) / 4)), y);
            y += lineOffset;
            e.Graphics.DrawString(anaform.textBoxFisBaslik.Text, printFont, Brushes.Black, Convert.ToInt32(62 - ((anaform.textBoxFisBaslik.Text.Length * 11 / 2) / 4)), y);
            y += lineOffset;

            e.Graphics.DrawString("______________________________________", printFont, Brushes.Black, x, y);
            y += lineOffset;
            //if (anaform.radioButton1.Checked)
            {
                e.Graphics.DrawString("Yetkili: " + anaform.labelyetkili.Text, printFont, Brushes.Black, x, y);

                y += lineOffset;
                e.Graphics.DrawString("Müşteri: " + bakiyeYetersizOgrenci.Ad + " " + bakiyeYetersizOgrenci.Soyad, printFont, Brushes.Black, x, y);
                y += lineOffset;
                e.Graphics.DrawString("Bakiye: " + bakiyeYetersizOgrenci.Bakiye.ToString(), printFont, Brushes.Black, x, y);
                y += lineOffset;
                e.Graphics.DrawString("______________________________________", printFont, Brushes.Black, x, y);
                y += lineOffset;
                e.Graphics.DrawString("Ürün Listesi", printFont, Brushes.Black, (62 - (("Ürün Listesi".Length * 11 / 2) / 4)), y);
                y += lineOffset;
            }
            foreach (var item in yazdirilacaksepet)
            {
                e.Graphics.DrawString(item.Adet + " * " + item.urun.Ad, printFont, Brushes.Black, x, y);
                e.Graphics.DrawString((item.urun.Fiyat * item.Adet).ToString() + " TL", printFont, Brushes.Black, Convert.ToInt32(110 - ((item.urun.Fiyat * item.Adet).ToString() + " TL").Length), y);
                toplam += Convert.ToDouble(item.urun.Fiyat * item.Adet);
                y += lineOffset;
            }

            e.Graphics.DrawString("______________________________________", printFont, Brushes.Black, x, y);

            Font printFontBold = new Font("MingLiU", 10, FontStyle.Regular, GraphicsUnit.Point);
            lineOffset = printFontBold.GetHeight(e.Graphics) - 4;
            y += lineOffset;            
            e.Graphics.DrawString("   ******Yetersiz Bakiye******   ", printFont, Brushes.Black, Convert.ToInt32(62 - (("   ******Yetersiz Bakiye******   ".Length * 11 / 2) / 4)), y);
            y += 40;
            
            e.HasMorePages = false;     //yazdırma işlemi tamamlandı
        }

        private void fisyazdir()
        {
            PrintDocument pdPrint = new PrintDocument();
            pdPrint.PrintPage += new PrintPageEventHandler(pdPrint_PrintPage);
            pdPrint.PrinterSettings.PrinterName = anaform.comboBoxYazici.Text;
            pdPrint.Print();
        }

        private void pdPrint_PrintPage(object sender, PrintPageEventArgs e)
        {
            float x, y, lineOffset;
            double toplam = 0;
            // Font seçimi
            Font printFont = new Font("Lucida Console", (float)7, FontStyle.Regular, GraphicsUnit.Point); // Substituted to FontA Font

            e.Graphics.PageUnit = GraphicsUnit.Point;

            // Resim çizdir
            x = 79;
            y = 0;
            int a = Convert.ToInt32(e.PageSettings.PrintableArea.Width - ("Toplam     " + toplam + " TL").Length);
            // Fişi hazırla
            lineOffset = 7 + printFont.GetHeight(e.Graphics) - (float)3.5;
            x = 0;
            y = lineOffset;
            e.Graphics.DrawString(DateTime.Now.ToString("yyyy-MM-dd HH:mm"), printFont, Brushes.Black, Convert.ToInt32(80 - (DateTime.Now.ToString("yyyy-MM-dd HH:mm").ToString()).Length), y);
            y += lineOffset*2;
            e.Graphics.DrawString(anaform.labelyetkili.Text, printFont, Brushes.Black, Convert.ToInt32(62 - ((anaform.labelyetkili.Text.Length * 11 / 2) / 4)), y);
            y += lineOffset;
            e.Graphics.DrawString(anaform.textBoxFirmaBaslik.Text, printFont, Brushes.Black, Convert.ToInt32(62 - ((anaform.textBoxFirmaBaslik.Text.Length * 11 / 2) / 4)), y);
            y += lineOffset;
            e.Graphics.DrawString(anaform.textBoxFisBaslik.Text, printFont, Brushes.Black, Convert.ToInt32(62 - ((anaform.textBoxFisBaslik.Text.Length * 11 / 2) / 4)), y);
            y += lineOffset;
            
            e.Graphics.DrawString("______________________________________", printFont, Brushes.Black, x, y);
            y += lineOffset;
            //if (anaform.radioButton1.Checked)
            {
                e.Graphics.DrawString("Müşteri : " + yazdirilacakOgrenci.KartNo + "-" + yazdirilacakOgrenci.Ad + " " + yazdirilacakOgrenci.Soyad, printFont, Brushes.Black, x, y);
                y += lineOffset;
                e.Graphics.DrawString("______________________________________", printFont, Brushes.Black, x, y);
                y += lineOffset;
                e.Graphics.DrawString("Ürün Listesi", printFont, Brushes.Black, (62 - (("Ürün Listesi".Length * 11 / 2) / 4)), y);
                y += lineOffset;
            }
            foreach(var item in yazdirilacaksepet)
            {
                e.Graphics.DrawString(item.Adet + " * " + item.urun.Ad, printFont, Brushes.Black, x, y);
                e.Graphics.DrawString((item.urun.Fiyat*item.Adet).ToString() + " TL", printFont, Brushes.Black, Convert.ToInt32(110 - ((item.urun.Fiyat * item.Adet).ToString() + " TL").Length), y);
                toplam += Convert.ToDouble(item.urun.Fiyat*item.Adet);
                y += lineOffset;
            }
            e.Graphics.DrawString("______________________________________", printFont, Brushes.Black, x, y);

            Font printFontBold = new Font("MingLiU", 10, FontStyle.Regular, GraphicsUnit.Point);
            lineOffset = printFontBold.GetHeight(e.Graphics) - 4;
            y += lineOffset;
            
            e.Graphics.DrawString("Toplam :   " + toplam + " TL", printFontBold, Brushes.Black, Convert.ToInt32(82 - ("Toplam :   " + toplam + " TL").Length), y);
            y += lineOffset*2;
            e.Graphics.DrawString("Kalan Bakiye : " + yazdirilacakOgrenci.Bakiye + " TL", printFont, Brushes.Black, Convert.ToInt32(54-("Kalan Bakiye :" + yazdirilacakOgrenci.Bakiye + " TL").Length), y);
            y += lineOffset;
            e.Graphics.DrawString("______________________________________", printFont, Brushes.Black, x, y);
            y += lineOffset * 2;
            e.Graphics.DrawString("---" + anaform.textBoxsonSatir.Text + "---", printFontBold, Brushes.Black, Convert.ToInt32(62 - ((("---" + anaform.textBoxsonSatir.Text + "---").Length * 11 / 2) / 4)), y);           
            y += lineOffset*4;
            e.Graphics.DrawString("______________________________________", printFontBold, Brushes.Black, x, y);
            y += lineOffset * 4;
            e.HasMorePages = false;     //yazdırma işlemi tamamlandı
        }
    }
}
