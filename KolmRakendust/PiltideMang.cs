using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class PiltideMang : Form
    {
        Random random = new Random();
        TableLayoutPanel laud;
        ComboBox teemad;
        Label olek;
        Button uusMang;
        Button esimeneValik, teineValik;
        Timer taimer;
        List<Image> pildid = new List<Image>();
        int paarid, katsed;

        public PiltideMang()
        {
            Text = "Sarnaste piltide mäng";
            ClientSize = new Size(560, 665);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.WhiteSmoke;

            Label pealkiri = new Label();
            pealkiri.Text = "Leia kõik paarid";
            pealkiri.Font = new Font("Arial", 20, FontStyle.Bold);
            pealkiri.Location = new Point(24, 16);
            pealkiri.AutoSize = true;

            Label teemaPealkiri = new Label();
            teemaPealkiri.Text = "Teema";
            teemaPealkiri.Location = new Point(24, 73);
            teemaPealkiri.AutoSize = true;

            teemad = new ComboBox();
            teemad.Location = new Point(85, 68);
            teemad.Size = new Size(305, 30);
            teemad.DropDownStyle = ComboBoxStyle.DropDownList;
            teemad.Items.Add("Automargid");
            teemad.Items.Add("Minecraft plokid");
            teemad.Items.Add("Minecraft loomad");
            teemad.Items.Add("Minecraft tööriistad");

            uusMang = new Button();
            uusMang.Text = "Uus mäng";
            uusMang.Location = new Point(402, 67);
            uusMang.Size = new Size(134, 32);
            uusMang.Click += UusMang_Click;

            laud = new TableLayoutPanel();
            laud.Location = new Point(24, 113);
            laud.Size = new Size(512, 512);
            laud.RowCount = 4;
            laud.ColumnCount = 4;

            for (int i = 0; i < 4; i++)
            {
                laud.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                laud.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            }

            for (int i = 0; i < 16; i++)
            {
                Button kaart = new Button();
                kaart.Dock = DockStyle.Fill;
                kaart.Margin = new Padding(4);
                kaart.Text = "?";
                kaart.Font = new Font("Arial", 25, FontStyle.Bold);
                kaart.BackColor = Color.LightBlue;
                kaart.BackgroundImageLayout = ImageLayout.Zoom;
                kaart.FlatStyle = FlatStyle.Flat;
                kaart.FlatAppearance.BorderSize = 0;
                kaart.Click += Kaart_Click;
                laud.Controls.Add(kaart);
            }

            olek = new Label();
            olek.Location = new Point(24, 635);
            olek.Size = new Size(512, 25);

            taimer = new Timer();
            taimer.Interval = 750;
            taimer.Tick += Taimer_Tick;

            Controls.Add(pealkiri);
            Controls.Add(teemaPealkiri);
            Controls.Add(teemad);
            Controls.Add(uusMang);
            Controls.Add(laud);
            Controls.Add(olek);

            teemad.SelectedIndex = 0;
            teemad.SelectedIndexChanged += Teemad_SelectedIndexChanged;
            AlustaMang();
        }

        private void AlustaMang()
        {
            taimer.Stop();
            esimeneValik = null;
            teineValik = null;
            paarid = 0;
            katsed = 0;
            laud.Enabled = false;
            PuhastaPildid();
            UuendaTulemust();

            string kaust = "car-brands";
            if (teemad.SelectedIndex == 1)
                kaust = "minecraft-blocks";
            else if (teemad.SelectedIndex == 2)
                kaust = "minecraft-animals";
            else if (teemad.SelectedIndex == 3)
                kaust = "minecraft-tools";

            string asukoht = Path.Combine(Application.StartupPath, "GameAssets", kaust);

            try
            {
                List<string> failid = new List<string>(Directory.GetFiles(asukoht, "*.png"));
                if (failid.Count < 8)
                {
                    olek.Text = "Teemas peab olema vähemalt 8 pilti.";
                    MessageBox.Show("Lisa kausta vähemalt 8 PNG-pilti:\n" + asukoht, "Pilte pole piisavalt");
                    return;
                }

                // Valime kaustast 8 erinevat pilti
                for (int i = 0; i < 8; i++)
                {
                    int number = random.Next(failid.Count);
                    pildid.Add(new Bitmap(failid[number]));
                    failid.RemoveAt(number);
                }

                // Iga pildi number lisatakse kaks korda, et tekiks paar
                List<int> numbrid = new List<int>();
                for (int i = 0; i < 8; i++)
                {
                    numbrid.Add(i);
                    numbrid.Add(i);
                }

                foreach (Control element in laud.Controls)
                {
                    Button kaart = element as Button;
                    int number = random.Next(numbrid.Count);
                    kaart.Tag = numbrid[number];
                    numbrid.RemoveAt(number);
                }

                laud.Enabled = true;
            }
            catch (Exception)
            {
                PuhastaPildid();
                olek.Text = "Piltide laadimine ebaõnnestus.";
                MessageBox.Show("Kontrolli, kas kaust on olemas ja PNG-pildid on korras:\n" + asukoht, "Viga");
            }
        }

        private void Kaart_Click(object sender, EventArgs e)
        {
            if (taimer.Enabled) return;

            Button kaart = sender as Button;
            if (kaart == null) return;
            if (kaart.Tag == null) return;
            if (kaart.BackgroundImage != null) return;

            // Tag hoiab pildi numbrit pildid-listis.
            int number = (int)kaart.Tag;
            kaart.BackgroundImage = pildid[number];
            kaart.Text = "";
            kaart.BackColor = Color.White;

            if (esimeneValik == null)
            {
                esimeneValik = kaart;
                return;
            }

            teineValik = kaart;
            katsed++;

            if ((int)esimeneValik.Tag == (int)teineValik.Tag)
            {
                esimeneValik.BackColor = Color.LightGreen;
                teineValik.BackColor = Color.LightGreen;
                esimeneValik = null;
                teineValik = null;
                paarid++;
                UuendaTulemust();
                KontrolliVoitu();
            }
            else
            {
                UuendaTulemust();
                taimer.Start();
            }
        }

        private void Taimer_Tick(object sender, EventArgs e)
        {
            taimer.Stop();
            PeidaKaart(esimeneValik);
            PeidaKaart(teineValik);
            esimeneValik = null;
            teineValik = null;
        }

        private void PeidaKaart(Button kaart)
        {
            kaart.BackgroundImage = null;
            kaart.Text = "?";
            kaart.BackColor = Color.LightBlue;
        }

        private void UuendaTulemust()
        {
            olek.Text = "Paarid: " + paarid + "/8    Katsed: " + katsed;
        }

        private void KontrolliVoitu()
        {
            if (paarid == 8)
                MessageBox.Show("Leidsid kõik paarid! Katseid: " + katsed, "Palju õnne!");
        }

        private void UusMang_Click(object sender, EventArgs e)
        {
            AlustaMang();
        }

        private void Teemad_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlustaMang();
        }

        private void PuhastaPildid()
        {
            foreach (Control element in laud.Controls)
            {
                Button kaart = element as Button;
                PeidaKaart(kaart);
                kaart.Tag = null;
            }

            // Vabastame eelmise mängu pildid mälust.
            foreach (Image pilt in pildid)
                pilt.Dispose();

            pildid.Clear();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            taimer.Stop();
            taimer.Dispose();
            PuhastaPildid();
            base.OnFormClosed(e);
        }
    }
}
