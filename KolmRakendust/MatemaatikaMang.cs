using System;
using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class MatemaatikaMang : Form
    {
        // Iga massiivi koht vastab ühele ülesandereale.
        Random random = new Random();
        Label aeg, tulemuseSilt;
        Label[] vasakud = new Label[4];
        Label[] tehted = new Label[4];
        Label[] paremad = new Label[4];
        Label[] tulemused = new Label[4];
        Label[] tagasiside = new Label[4];
        NumericUpDown[] vastused = new NumericUpDown[4];
        CheckBox[] valikud = new CheckBox[4];
        CheckBox[] puuduvadKohad = new CheckBox[3];
        int[] oigedVastused = new int[4];
        Button alusta, tyhjenda;
        Timer taimer;
        int aegaAlles;
        bool mangKaib;

        public MatemaatikaMang()
        {
            Text = "Matemaatiline äraarvamismäng";
            ClientSize = new Size(630, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            UiTheme.Form(this);

            Label pealkiri = new Label();
            pealkiri.Text = "Arvuta kiiresti";
            pealkiri.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            pealkiri.Location = new Point(26, 19);
            pealkiri.Size = new Size(330, 38);
            Controls.Add(pealkiri);
            Label juhis = new Label();
            juhis.Text = "Leia neli puuduvat arvu 30 sekundiga";
            juhis.ForeColor = UiTheme.Muted;
            juhis.Location = new Point(28, 59);
            juhis.Size = new Size(370, 28);
            Controls.Add(juhis);

            Label ajaPealkiri = new Label();
            ajaPealkiri.Text = "AEGA ALLES";
            ajaPealkiri.ForeColor = UiTheme.Muted;
            ajaPealkiri.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            ajaPealkiri.Location = new Point(443, 23);
            ajaPealkiri.Size = new Size(150, 22);
            Controls.Add(ajaPealkiri);
            aeg = new Label();
            aeg.Text = "30 sekundit";
            aeg.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            aeg.Location = new Point(442, 47);
            aeg.Size = new Size(158, 32);
            aeg.TextAlign = ContentAlignment.MiddleLeft;
            Controls.Add(aeg);

            for (int i = 0; i < 4; i++)
            {
                int y = 102 + i * 60;
                Panel rida = new Panel();
                rida.Location = new Point(18, y - 2);
                rida.Size = new Size(594, 56);
                rida.BackColor = UiTheme.Surface;
                Controls.Add(rida);

                // Kõik rea elemendid peavad olema rea paneeli sees.
                // Muidu katab paneel vormil asuvaid arve.
                vasakud[i] = LooSilt(rida, "?", 22, 2, 80);
                tehted[i] = LooSilt(rida, "+", 115, 2, 35);
                paremad[i] = LooSilt(rida, "?", 167, 2, 80);
                LooSilt(rida, "=", 262, 2, 45);
                tulemused[i] = LooSilt(rida, "?", 322, 2, 80);

                vastused[i] = new NumericUpDown();
                vastused[i].Font = new Font("Segoe UI", 17);
                vastused[i].ForeColor = UiTheme.Text;
                vastused[i].Location = new Point(322, 10);
                vastused[i].Size = new Size(80, 35);
                vastused[i].Maximum = 100;
                vastused[i].TabIndex = i + 1;
                vastused[i].Enabled = false;
                vastused[i].Enter += Vastus_Enter;
                vastused[i].KeyDown += Vastus_KeyDown;
                rida.Controls.Add(vastused[i]);

                tagasiside[i] = LooSilt(rida, "", 412, 2, 170);
                tagasiside[i].Font = new Font("Segoe UI", 9.5f);
                tagasiside[i].TextAlign = ContentAlignment.MiddleLeft;
            }

            Label valikuPealkiri = new Label();
            valikuPealkiri.Text = "HARJUTA TEHTEID";
            valikuPealkiri.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            valikuPealkiri.ForeColor = UiTheme.Muted;
            valikuPealkiri.Location = new Point(38, 358);
            valikuPealkiri.AutoSize = true;
            Controls.Add(valikuPealkiri);
            string[] nimed = { "Liitmine", "Lahutamine", "Korrutamine", "Jagamine" };
            for (int i = 0; i < 4; i++)
            {
                valikud[i] = new CheckBox();
                valikud[i].Text = nimed[i];
                valikud[i].Checked = true;
                valikud[i].Location = new Point(38, 382 + i * 27);
                valikud[i].Size = new Size(180, 25);
                Controls.Add(valikud[i]);
            }

            Label kohaPealkiri = new Label();
            kohaPealkiri.Text = "PUUDUV ARV";
            kohaPealkiri.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            kohaPealkiri.ForeColor = UiTheme.Muted;
            kohaPealkiri.Location = new Point(328, 358);
            kohaPealkiri.AutoSize = true;
            Controls.Add(kohaPealkiri);
            string[] kohad = { "1. arv", "2. arv", "3. arv (vastus)" };
            for (int i = 0; i < 3; i++)
            {
                puuduvadKohad[i] = new CheckBox();
                puuduvadKohad[i].Text = kohad[i];
                puuduvadKohad[i].Checked = i == 2;
                puuduvadKohad[i].Location = new Point(328, 382 + i * 27);
                puuduvadKohad[i].Size = new Size(180, 25);
                Controls.Add(puuduvadKohad[i]);
            }

            alusta = new Button();
            alusta.Text = "Alusta mängu";
            alusta.Location = new Point(160, 506);
            alusta.Size = new Size(145, 40);
            alusta.TabIndex = 0;
            UiTheme.Button(alusta, true);
            alusta.Click += Alusta_Click;
            tyhjenda = new Button();
            tyhjenda.Text = "Tühjenda vastused";
            tyhjenda.Location = new Point(319, 506);
            tyhjenda.Size = new Size(168, 40);
            tyhjenda.Enabled = false;
            UiTheme.Button(tyhjenda);
            tyhjenda.Click += Tyhjenda_Click;
            Controls.Add(alusta);
            Controls.Add(tyhjenda);

            tulemuseSilt = new Label();
            tulemuseSilt.Location = new Point(30, 552);
            tulemuseSilt.Size = new Size(570, 32);
            tulemuseSilt.TextAlign = ContentAlignment.MiddleCenter;
            tulemuseSilt.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            Controls.Add(tulemuseSilt);

            taimer = new Timer();
            taimer.Interval = 1000;
            taimer.Tick += Taimer_Tick;
        }

        private Label LooSilt(Control parent, string tekst, int x, int y, int laius)
        {
            Label silt = new Label();
            silt.Text = tekst;
            silt.Font = new Font("Segoe UI", 17);
            silt.ForeColor = UiTheme.Text;
            silt.Location = new Point(x, y);
            silt.Size = new Size(laius, 50);
            silt.TextAlign = ContentAlignment.MiddleCenter;
            parent.Controls.Add(silt);
            return silt;
        }

        private void Alusta_Click(object sender, EventArgs e)
        {
            // Sama nupp alustab uut vooru või kontrollib käimasolevat vooru.
            if (mangKaib)
            {
                KontrolliVastuseid();
                return;
            }

            // Kogume kokku märgitud tehted.
            int[] valitudTehted = new int[4];
            int valitudArv = 0;
            for (int i = 0; i < 4; i++)
            {
                if (valikud[i].Checked)
                {
                    valitudTehted[valitudArv] = i;
                    valitudArv++;
                }
            }

            if (valitudArv == 0)
            {
                MessageBox.Show("Vali vähemalt üks tehe.");
                return;
            }

            // Kogume kokku kohad, kus vastusekast võib olla.
            int[] valitudKohad = new int[3];
            int kohtadeArv = 0;
            for (int i = 0; i < 3; i++)
            {
                if (puuduvadKohad[i].Checked)
                {
                    valitudKohad[kohtadeArv] = i;
                    kohtadeArv++;
                }
            }

            if (kohtadeArv == 0)
            {
                MessageBox.Show("Vali vähemalt üks puuduv arv.");
                return;
            }

            // Teeme alati neli ülesannet. Vajadusel kordame valitud tehteid.
            for (int i = 0; i < 4; i++)
            {
                LooUlesanne(i, valitudTehted[i % valitudArv], valitudKohad, kohtadeArv);
            }

            AlustaSamaUlesannet();
        }

        private void Tyhjenda_Click(object sender, EventArgs e)
        {
            // Samad ülesanded jäävad alles, ainult vastused ja aeg lähevad algusesse.
            AlustaSamaUlesannet();
        }

        private void AlustaSamaUlesannet()
        {
            // Kõigepealt peatame taimeri, et vastuste kustutamine seda uuesti ei käivitaks.
            taimer.Stop();
            mangKaib = false;
            for (int i = 0; i < 4; i++)
            {
                vastused[i].Value = 0;
                vastused[i].BackColor = SystemColors.Window;
                vastused[i].Enabled = true;
                vastused[i].BringToFront(); // Vastusekast peab olema siltidest eespool.
                tagasiside[i].Text = "";
            }

            for (int i = 0; i < 4; i++)
            {
                valikud[i].Enabled = false;
                if (i < 3)
                    puuduvadKohad[i].Enabled = false;
            }

            // Loendus algab kohe, kui ülesanded ilmuvad ja vastuseväljad on aktiivsed.
            aegaAlles = 30;
            aeg.Text = "30 sekundit";
            alusta.Text = "Kontrolli";
            tyhjenda.Enabled = true;
            mangKaib = true;
            tulemuseSilt.Text = "";
            aeg.ForeColor = UiTheme.Text;
            vastused[0].Focus();
            taimer.Start();
        }

        private void LooUlesanne(int rida, int tehe, int[] kohad, int kohtadeArv)
        {
            // Loome arvud valitud tehte jaoks.
            int esimene = 0;
            int teine = 0;
            int tulemus = 0;

            if (tehe == 0)
            {
                esimene = random.Next(51);
                teine = random.Next(51);
                tulemus = esimene + teine;
                tehted[rida].Text = "+";
            }
            else if (tehe == 1)
            {
                // Lahutamise vastus ei tohi olla negatiivne.
                esimene = random.Next(1, 101);
                teine = random.Next(1, esimene + 1);
                tulemus = esimene - teine;
                tehted[rida].Text = "−";
            }
            else if (tehe == 2)
            {
                esimene = random.Next(2, 11);
                teine = random.Next(2, 11);
                tulemus = esimene * teine;
                tehted[rida].Text = "×";
            }
            else
            {
                // Jagamise tulemuseks tuleb alati täisarv.
                teine = random.Next(2, 11);
                tulemus = random.Next(2, 11);
                esimene = teine * tulemus;
                tehted[rida].Text = "÷";
            }

            // Näitame arvud kõigepealt õiges järjekorras.
            vasakud[rida].Text = esimene.ToString();
            paremad[rida].Text = teine.ToString();
            tulemused[rida].Text = tulemus.ToString();

            // Puuduva arvu koht valitakse ainult märgitud kohtade hulgast.
            int puuduvKoht = kohad[random.Next(kohtadeArv)];
            if (puuduvKoht == 0)
            {
                oigedVastused[rida] = esimene;
                vasakud[rida].Text = "";
                vastused[rida].Location = new Point(22, 10);
            }
            else if (puuduvKoht == 1)
            {
                oigedVastused[rida] = teine;
                paremad[rida].Text = "";
                vastused[rida].Location = new Point(167, 10);
            }
            else
            {
                oigedVastused[rida] = tulemus;
                tulemused[rida].Text = "";
                vastused[rida].Location = new Point(322, 10);
            }
        }

        private void KontrolliVastuseid()
        {
            // Võrdleme kõiki nelja vastust ja loeme õiged kokku.
            taimer.Stop();
            mangKaib = false;
            alusta.Focus(); // Kinnitame ka viimati kirjutatud arvu enne kontrollimist.
            int oigeid = 0;
            for (int i = 0; i < 4; i++)
            {
                tagasiside[i].Text = "Õige vastus: " + oigedVastused[i];
                if (vastused[i].Value == oigedVastused[i])
                {
                    oigeid++;
                    vastused[i].BackColor = UiTheme.Green;
                    tagasiside[i].ForeColor = Color.FromArgb(29, 122, 72);
                }
                else
                {
                    vastused[i].BackColor = UiTheme.Red;
                    tagasiside[i].ForeColor = Color.FromArgb(187, 62, 59);
                }
                vastused[i].Enabled = false;
                valikud[i].Enabled = true;
                if (i < 3)
                    puuduvadKohad[i].Enabled = true;
            }
            alusta.Text = "Alusta mängu";

            // Tulemus jääb aknasse nähtavale kuni järgmise vooruni.
            tulemuseSilt.Text = (aegaAlles == 0 ? "Aeg on läbi!  " : "") +
                "Õigeid vastuseid: " + oigeid + " / 4";
            tulemuseSilt.ForeColor = oigeid == 4 ? Color.FromArgb(29, 122, 72) : UiTheme.Text;
        }

        private void Taimer_Tick(object sender, EventArgs e)
        {
            // Iga sekundi järel jääb üks sekund vähemaks.
            aegaAlles--;
            aeg.Text = aegaAlles + " sekundit";

            if (aegaAlles == 0)
            {
                aeg.Text = "Aeg on läbi!";
                KontrolliVastuseid();
            }
        }

        private void Vastus_Enter(object sender, EventArgs e)
        {
            NumericUpDown vastus = sender as NumericUpDown;
            if (vastus != null)
                vastus.Select(0, vastus.Text.Length);
        }

        private void Vastus_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            // Enter viib järgmise vastuseni, viimases reas lõpetab vooru.
            e.SuppressKeyPress = true;
            int rida = Array.IndexOf(vastused, sender);
            if (rida < 3)
                vastused[rida + 1].Focus();
            else
                KontrolliVastuseid();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            taimer.Stop();
            taimer.Dispose();
            base.OnFormClosed(e);
        }
    }
}
