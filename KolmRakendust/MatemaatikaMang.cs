using System;
using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class MatemaatikaMang : Form
    {
        // Iga massiivi koht vastab ühele ülesandereale.
        Random random = new Random();
        Label aeg;
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
            // Mängu akna suurus ja pealkiri.
            Text = "Matemaatiline äraarvamismäng";
            ClientSize = new Size(610, 485);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;

            Label ajaPealkiri = new Label();
            ajaPealkiri.Text = "Aega alles";
            ajaPealkiri.Font = new Font("Arial", 15.75f);
            ajaPealkiri.Location = new Point(155, 15);
            ajaPealkiri.AutoSize = true;

            aeg = new Label();
            aeg.Font = new Font("Arial", 15.75f);
            aeg.Location = new Point(270, 10);
            aeg.Size = new Size(200, 30);
            aeg.BorderStyle = BorderStyle.FixedSingle;
            aeg.TextAlign = ContentAlignment.MiddleLeft;

            // Loome neli rida. Igas reas on kaks arvu, tehtemärk ja vastuse koht.
            for (int i = 0; i < 4; i++)
            {
                int y = 60 + i * 50;
                vasakud[i] = LooSilt("?", 30, y, 85);
                tehted[i] = LooSilt("+", 125, y, 45);
                paremad[i] = LooSilt("?", 185, y, 85);
                LooSilt("=", 280, y, 45);
                tulemused[i] = LooSilt("?", 330, y, 85);

                vastused[i] = new NumericUpDown();
                vastused[i].Font = new Font("Arial", 18);
                vastused[i].Location = new Point(330, y + 10);
                vastused[i].Size = new Size(85, 35);
                vastused[i].Maximum = 100;
                vastused[i].TabIndex = i + 1;
                vastused[i].Enabled = false;
                vastused[i].Enter += Vastus_Enter;
                vastused[i].KeyDown += Vastus_KeyDown;
                vastused[i].ValueChanged += Vastus_ValueChanged;
                Controls.Add(vastused[i]);

                tagasiside[i] = LooSilt("", 425, y, 180);
                tagasiside[i].Font = new Font("Arial", 12);
                tagasiside[i].TextAlign = ContentAlignment.MiddleLeft;
            }

            Label valikuPealkiri = new Label();
            valikuPealkiri.Text = "Harjuta tehteid:";
            valikuPealkiri.Location = new Point(50, 275);
            valikuPealkiri.AutoSize = true;
            Controls.Add(valikuPealkiri);

            // Siit saab valida, milliseid tehteid järgmises voorus harjutada.
            string[] nimed = { "Liitmine", "Lahutamine", "Korrutamine", "Jagamine" };
            for (int i = 0; i < 4; i++)
            {
                valikud[i] = new CheckBox();
                valikud[i].Text = nimed[i];
                valikud[i].Checked = true;
                valikud[i].Location = new Point(50, 300 + i * 28);
                valikud[i].Size = new Size(130, 25);
                Controls.Add(valikud[i]);
            }

            Label kohaPealkiri = new Label();
            kohaPealkiri.Text = "Milline arv võib puuduma:";
            kohaPealkiri.Location = new Point(310, 275);
            kohaPealkiri.AutoSize = true;
            Controls.Add(kohaPealkiri);

            // Vaikimisi tuleb leida tehte vastus ehk kolmas arv.
            string[] kohad = { "1. arv", "2. arv", "3. arv (vastus)" };
            for (int i = 0; i < 3; i++)
            {
                puuduvadKohad[i] = new CheckBox();
                puuduvadKohad[i].Text = kohad[i];
                puuduvadKohad[i].Checked = i == 2;
                puuduvadKohad[i].Location = new Point(310, 300 + i * 28);
                puuduvadKohad[i].Size = new Size(160, 25);
                Controls.Add(puuduvadKohad[i]);
            }

            alusta = new Button();
            alusta.Text = "Alusta mängu";
            alusta.Font = new Font("Arial", 14);
            alusta.Location = new Point(170, 435);
            alusta.Size = new Size(130, 35);
            alusta.TabIndex = 0;
            alusta.Click += Alusta_Click;

            tyhjenda = new Button();
            tyhjenda.Text = "Tühjenda vastused";
            tyhjenda.Font = new Font("Arial", 11);
            tyhjenda.Location = new Point(320, 435);
            tyhjenda.Size = new Size(170, 35);
            tyhjenda.Enabled = false;
            tyhjenda.Click += Tyhjenda_Click;

            // Taimer hakkab käima alles siis, kui mängija esimest korda vastab.
            taimer = new Timer();
            taimer.Interval = 1000;
            taimer.Tick += Taimer_Tick;

            Controls.Add(ajaPealkiri);
            Controls.Add(aeg);
            Controls.Add(alusta);
            Controls.Add(tyhjenda);
        }

        private Label LooSilt(string tekst, int x, int y, int laius)
        {
            Label silt = new Label();
            silt.Text = tekst;
            silt.Font = new Font("Arial", 18);
            silt.Location = new Point(x, y);
            silt.Size = new Size(laius, 50);
            silt.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(silt);
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

            // Aeg hakkab vähenema alles esimese sisestuse juures.
            aegaAlles = 30;
            aeg.Text = "30 sekundit";
            alusta.Text = "Kontrolli";
            tyhjenda.Enabled = true;
            mangKaib = true;
            vastused[0].Focus();
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
                vastused[rida].Location = new Point(30, 70 + rida * 50);
            }
            else if (puuduvKoht == 1)
            {
                oigedVastused[rida] = teine;
                paremad[rida].Text = "";
                vastused[rida].Location = new Point(185, 70 + rida * 50);
            }
            else
            {
                oigedVastused[rida] = tulemus;
                tulemused[rida].Text = "";
                vastused[rida].Location = new Point(330, 70 + rida * 50);
            }
        }

        private void KontrolliVastuseid()
        {
            // Võrdleme kõiki nelja vastust ja loeme õiged kokku.
            taimer.Stop();
            mangKaib = false;
            int oigeid = 0;
            for (int i = 0; i < 4; i++)
            {
                tagasiside[i].Text = "Õige vastus: " + oigedVastused[i];
                if (vastused[i].Value == oigedVastused[i])
                {
                    oigeid++;
                    vastused[i].BackColor = Color.LightGreen;
                    tagasiside[i].ForeColor = Color.Green;
                }
                else
                {
                    vastused[i].BackColor = Color.LightPink;
                    tagasiside[i].ForeColor = Color.Red;
                }
                vastused[i].Enabled = false;
                valikud[i].Enabled = true;
                if (i < 3)
                    puuduvadKohad[i].Enabled = true;
            }
            alusta.Text = "Alusta mängu";
            alusta.Focus();

            // Vooru lõpus näeb mängija oma tulemust eraldi aknas.
            if (oigeid == 4)
                MessageBox.Show(this, "Väga tubli! Kõik 4 vastust on õiged. Palju õnne võidu puhul!", "Mängu tulemus");
            else
                MessageBox.Show(this, "Õigeid vastuseid: " + oigeid + "/4. Proovi veel kord!", "Mängu tulemus");
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
            // Esimene number või nooleklahv käivitab taimeri.
            if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) ||
                (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) ||
                e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                AlustaTaimer();

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

        private void Vastus_ValueChanged(object sender, EventArgs e)
        {
            // See käivitab taimeri ka siis, kui arvu muudetakse hiirega.
            AlustaTaimer();
        }

        private void AlustaTaimer()
        {
            if (mangKaib && !taimer.Enabled)
                taimer.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            taimer.Stop();
            taimer.Dispose();
            base.OnFormClosed(e);
        }
    }
}
