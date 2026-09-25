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
        ComboBox teemad, reziim;
        Label olek, esimeseMangijaSilt, teiseMangijaSilt;
        Button uusMang;
        Button esimeneValik, teineValik;
        Timer taimer;
        List<Image> pildid = new List<Image>();
        int paarid, katsed;
        readonly string[] mangijad = new string[2];
        readonly int[] punktid = new int[2];
        int aktiivneMangija, alustaja;

        bool Kahekesi { get { return reziim.SelectedIndex == 1; } }

        public PiltideMang()
        {
            Text = "Sarnaste piltide mäng";
            ClientSize = new Size(560, 720);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            UiTheme.Form(this);

            Label pealkiri = new Label();
            pealkiri.Text = "Leia kõik paarid";
            pealkiri.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            pealkiri.Location = new Point(24, 10);
            pealkiri.AutoSize = true;

            Label teemaPealkiri = new Label();
            teemaPealkiri.Text = "Teema";
            teemaPealkiri.Location = new Point(24, 65);
            teemaPealkiri.AutoSize = true;

            teemad = new ComboBox();
            teemad.Location = new Point(85, 59);
            teemad.Size = new Size(305, 30);
            teemad.Font = new Font("Segoe UI", 10);
            teemad.DropDownStyle = ComboBoxStyle.DropDownList;
            teemad.Items.Add("Automargid");
            teemad.Items.Add("Minecraft plokid");
            teemad.Items.Add("Minecraft loomad");
            teemad.Items.Add("Minecraft tööriistad");

            uusMang = new Button();
            uusMang.Text = "Uus mäng";
            uusMang.Location = new Point(402, 58);
            uusMang.Size = new Size(134, 32);
            UiTheme.Button(uusMang, true);
            uusMang.Click += UusMang_Click;

            Label reziimiSilt = new Label();
            reziimiSilt.Text = "Mäng";
            reziimiSilt.Location = new Point(24, 102);
            reziimiSilt.AutoSize = true;
            reziim = new ComboBox();
            reziim.Location = new Point(85, 97);
            reziim.Size = new Size(305, 30);
            reziim.DropDownStyle = ComboBoxStyle.DropDownList;
            reziim.Items.Add("Üks mängija");
            reziim.Items.Add("Kaks mängijat");

            laud = new TableLayoutPanel();
            laud.Location = new Point(24, 176);
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
                kaart.Font = new Font("Segoe UI", 25, FontStyle.Bold);
                kaart.BackColor = Color.FromArgb(231, 237, 247);
                kaart.BackgroundImageLayout = ImageLayout.Zoom;
                kaart.FlatStyle = FlatStyle.Flat;
                kaart.FlatAppearance.BorderSize = 0;
                kaart.Cursor = Cursors.Hand;
                kaart.Click += Kaart_Click;
                laud.Controls.Add(kaart);
            }

            olek = new Label();
            olek.Location = new Point(24, 130);
            olek.Size = new Size(512, 30);
            olek.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            olek.ForeColor = UiTheme.Muted;

            esimeseMangijaSilt = LooMangijaSilt(24);
            teiseMangijaSilt = LooMangijaSilt(294);

            taimer = new Timer();
            taimer.Interval = 750;
            taimer.Tick += Taimer_Tick;

            Controls.Add(pealkiri);
            Controls.Add(teemaPealkiri);
            Controls.Add(teemad);
            Controls.Add(uusMang);
            Controls.Add(reziimiSilt);
            Controls.Add(reziim);
            Controls.Add(laud);
            Controls.Add(olek);
            Controls.Add(esimeseMangijaSilt);
            Controls.Add(teiseMangijaSilt);

            teemad.SelectedIndex = 0;
            reziim.SelectedIndex = 0;
            teemad.SelectedIndexChanged += Teemad_SelectedIndexChanged;
            reziim.SelectedIndexChanged += Reziim_SelectedIndexChanged;
            AlustaMang();
        }

        private Label LooMangijaSilt(int x)
        {
            Label silt = new Label();
            silt.Location = new Point(x, 159);
            silt.Size = new Size(242, 32);
            silt.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            silt.TextAlign = ContentAlignment.MiddleCenter;
            silt.AutoEllipsis = true;
            return silt;
        }

        private void AlustaMang()
        {
            taimer.Stop();
            esimeneValik = null;
            teineValik = null;
            paarid = 0;
            katsed = 0;
            punktid[0] = punktid[1] = 0;
            aktiivneMangija = alustaja;
            laud.Location = new Point(24, Kahekesi ? 196 : 176);
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
                esimeneValik.BackColor = UiTheme.Green;
                teineValik.BackColor = UiTheme.Green;
                esimeneValik = null;
                teineValik = null;
                paarid++;
                if (Kahekesi)
                    punktid[aktiivneMangija]++;
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
            if (Kahekesi)
                aktiivneMangija = 1 - aktiivneMangija;
            UuendaTulemust();
        }

        private void PeidaKaart(Button kaart)
        {
            kaart.BackgroundImage = null;
            kaart.Text = "?";
            kaart.BackColor = Color.FromArgb(231, 237, 247);
        }

        private void UuendaTulemust()
        {
            olek.Text = "Paarid: " + paarid + "/8    Katsed: " + katsed;
            olek.ForeColor = UiTheme.Muted;
            esimeseMangijaSilt.Visible = teiseMangijaSilt.Visible = Kahekesi;
            if (!Kahekesi) return;

            Label[] sildid = { esimeseMangijaSilt, teiseMangijaSilt };
            for (int i = 0; i < 2; i++)
            {
                sildid[i].Text = mangijad[i] + ": " + punktid[i] + " paari";
                sildid[i].BackColor = i == aktiivneMangija ? UiTheme.Accent : UiTheme.Surface;
                sildid[i].ForeColor = i == aktiivneMangija ? UiTheme.Surface : UiTheme.Text;
            }
        }

        private void KontrolliVoitu()
        {
            if (paarid == 8)
            {
                olek.Text = "Kõik paarid leitud!  Katseid: " + katsed;
                olek.ForeColor = Color.FromArgb(29, 122, 72);
                string tulemus;
                if (Kahekesi)
                {
                    string voitja = punktid[0] == punktid[1] ? "Viik!" :
                        "Võitja: " + mangijad[punktid[0] > punktid[1] ? 0 : 1];
                    tulemus = voitja + "\n" + mangijad[0] + ": " + punktid[0] +
                        " paari\n" + mangijad[1] + ": " + punktid[1] + " paari";
                }
                else
                    tulemus = "Leidsid kõik paarid! Katseid: " + katsed;

                DialogResult revanš = MessageBox.Show(this, tulemus + "\n\nKas soovite revanši?",
                    "Mäng läbi", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (revanš == DialogResult.Yes)
                {
                    if (Kahekesi) alustaja = 1 - alustaja;
                    AlustaMang();
                }
                else
                    Close();
            }
        }

        private void UusMang_Click(object sender, EventArgs e)
        {
            AlustaMang();
        }

        private void Teemad_SelectedIndexChanged(object sender, EventArgs e)
        {
            AlustaMang();
        }

        private void Reziim_SelectedIndexChanged(object sender, EventArgs e)
        {
            taimer.Stop();
            laud.Enabled = false;
            if (Kahekesi && !KysiMangijad())
            {
                // Loobumisel jääb eelmise mängu asemel alles üksikmäng.
                reziim.SelectedIndex = 0;
                return;
            }
            alustaja = 0;
            AlustaMang();
        }

        private bool KysiMangijad()
        {
            using (Form aken = new Form())
            {
                aken.Text = "Mängijate nimed";
                aken.ClientSize = new Size(370, 220);
                aken.StartPosition = FormStartPosition.CenterParent;
                aken.FormBorderStyle = FormBorderStyle.FixedDialog;
                aken.MaximizeBox = false;
                aken.MinimizeBox = false;
                UiTheme.Form(aken);

                Label juhis = new Label();
                juhis.Text = "Sisestage mõlema mängija nimi";
                juhis.Location = new Point(20, 14);
                juhis.Size = new Size(330, 26);
                juhis.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                aken.Controls.Add(juhis);

                TextBox[] nimed = new TextBox[2];
                for (int i = 0; i < 2; i++)
                {
                    Label silt = new Label();
                    silt.Text = (i + 1) + ". mängija";
                    silt.Location = new Point(20, 48 + i * 55);
                    silt.Size = new Size(110, 26);
                    aken.Controls.Add(silt);
                    nimed[i] = new TextBox();
                    nimed[i].Location = new Point(135, 46 + i * 55);
                    nimed[i].Size = new Size(212, 27);
                    nimed[i].MaxLength = 18;
                    aken.Controls.Add(nimed[i]);
                }

                Button alustaNupp = new Button();
                alustaNupp.Text = "Alusta";
                alustaNupp.Location = new Point(172, 161);
                alustaNupp.Size = new Size(84, 36);
                UiTheme.Button(alustaNupp, true);
                alustaNupp.Click += (s, e) =>
                {
                    string esimene = nimed[0].Text.Trim();
                    string teine = nimed[1].Text.Trim();
                    if (esimene.Length == 0 || teine.Length == 0 ||
                        string.Equals(esimene, teine, StringComparison.CurrentCultureIgnoreCase))
                    {
                        MessageBox.Show(aken, "Sisestage kaks erinevat nime.", "Mängijate nimed");
                        return;
                    }
                    mangijad[0] = esimene;
                    mangijad[1] = teine;
                    aken.DialogResult = DialogResult.OK;
                };
                Button loobu = new Button();
                loobu.Text = "Loobu";
                loobu.Location = new Point(264, 161);
                loobu.Size = new Size(84, 36);
                UiTheme.Button(loobu);
                loobu.DialogResult = DialogResult.Cancel;
                aken.Controls.Add(alustaNupp);
                aken.Controls.Add(loobu);
                aken.AcceptButton = alustaNupp;
                aken.CancelButton = loobu;
                return aken.ShowDialog(this) == DialogResult.OK;
            }
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
