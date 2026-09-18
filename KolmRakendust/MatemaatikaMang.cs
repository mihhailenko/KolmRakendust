using System;
using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class MatemaatikaMang : Form
    {
        Random random = new Random();
        Label aeg;
        Label liitmineVasak, liitmineParem, lahutamineVasak, lahutamineParem;
        Label korrutamineVasak, korrutamineParem, jagamineVasak, jagamineParem;
        NumericUpDown summa, vahe, korrutis, jagatis;
        Label summaTulemus, vaheTulemus, korrutisTulemus, jagatisTulemus;
        Button alusta;
        Timer taimer;

        int liidetav1, liidetav2, vahendatav, lahutatav;
        int tegur1, tegur2, jagatav, jagaja;
        int aegaAlles;

        public MatemaatikaMang()
        {
            Text = "Matemaatiline äraarvamismäng";
            ClientSize = new Size(610, 361);
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

            liitmineVasak = LooSilt("?", 50, 60);
            LooSilt("+", 115, 60);
            liitmineParem = LooSilt("?", 180, 60);
            LooSilt("=", 245, 60);
            summa = LooVastus(70, 1);

            lahutamineVasak = LooSilt("?", 50, 110);
            LooSilt("−", 115, 110);
            lahutamineParem = LooSilt("?", 180, 110);
            LooSilt("=", 245, 110);
            vahe = LooVastus(120, 2);

            korrutamineVasak = LooSilt("?", 50, 160);
            LooSilt("×", 115, 160);
            korrutamineParem = LooSilt("?", 180, 160);
            LooSilt("=", 245, 160);
            korrutis = LooVastus(170, 3);

            jagamineVasak = LooSilt("?", 50, 210);
            LooSilt("÷", 115, 210);
            jagamineParem = LooSilt("?", 180, 210);
            LooSilt("=", 245, 210);
            jagatis = LooVastus(220, 4);

            summaTulemus = LooTulemus(60);
            vaheTulemus = LooTulemus(110);
            korrutisTulemus = LooTulemus(160);
            jagatisTulemus = LooTulemus(210);

            alusta = new Button();
            alusta.Text = "Alusta mängu";
            alusta.Font = new Font("Arial", 14);
            alusta.Location = new Point(170, 290);
            alusta.Size = new Size(130, 35);
            alusta.TabIndex = 0;
            alusta.Click += Alusta_Click;

            taimer = new Timer();
            taimer.Interval = 1000;
            taimer.Tick += Taimer_Tick;

            Controls.Add(ajaPealkiri);
            Controls.Add(aeg);
            Controls.Add(alusta);
        }

        private Label LooSilt(string tekst, int x, int y)
        {
            Label silt = new Label();
            silt.Text = tekst;
            silt.Font = new Font("Arial", 18);
            silt.Location = new Point(x, y);
            silt.Size = new Size(60, 50);
            silt.TextAlign = ContentAlignment.MiddleCenter;
            Controls.Add(silt);
            return silt;
        }

        private NumericUpDown LooVastus(int y, int jarjekord)
        {
            NumericUpDown vastus = new NumericUpDown();
            vastus.Font = new Font("Arial", 18);
            vastus.Location = new Point(320, y);
            vastus.Size = new Size(100, 35);
            vastus.Maximum = 100;
            vastus.TabIndex = jarjekord;
            vastus.Enabled = false;
            vastus.Enter += Vastus_Enter;
            Controls.Add(vastus);
            return vastus;
        }

        private void Alusta_Click(object sender, EventArgs e)
        {
            if (taimer.Enabled)
            {
                KontrolliVastuseid();
                return;
            }

            liidetav1 = random.Next(51);
            liidetav2 = random.Next(51);
            liitmineVasak.Text = liidetav1.ToString();
            liitmineParem.Text = liidetav2.ToString();

            // Lahutamise vastus ei tohi olla negatiivne.
            vahendatav = random.Next(1, 101);
            lahutatav = random.Next(1, vahendatav + 1);
            lahutamineVasak.Text = vahendatav.ToString();
            lahutamineParem.Text = lahutatav.ToString();

            tegur1 = random.Next(2, 11);
            tegur2 = random.Next(2, 11);
            korrutamineVasak.Text = tegur1.ToString();
            korrutamineParem.Text = tegur2.ToString();

            // Nii tuleb jagamise vastuseks alati täisarv.
            jagaja = random.Next(2, 11);
            jagatav = jagaja * random.Next(2, 11);
            jagamineVasak.Text = jagatav.ToString();
            jagamineParem.Text = jagaja.ToString();

            summa.Value = 0;
            vahe.Value = 0;
            korrutis.Value = 0;
            jagatis.Value = 0;
            summaTulemus.Text = "";
            vaheTulemus.Text = "";
            korrutisTulemus.Text = "";
            jagatisTulemus.Text = "";
            summa.BackColor = SystemColors.Window;
            vahe.BackColor = SystemColors.Window;
            korrutis.BackColor = SystemColors.Window;
            jagatis.BackColor = SystemColors.Window;
            LubaVastused(true);

            aegaAlles = 30;
            aeg.Text = aegaAlles + " sekundit";
            alusta.Text = "Kontrolli";
            taimer.Start();
            summa.Focus();
        }

        private Label LooTulemus(int y)
        {
            Label tulemus = LooSilt("", 430, y);
            tulemus.Size = new Size(170, 50);
            tulemus.Font = new Font("Arial", 14);
            tulemus.TextAlign = ContentAlignment.MiddleLeft;
            return tulemus;
        }

        private void KuvaTulemus(NumericUpDown vastus, Label tulemus, int oigeVastus)
        {
            tulemus.Text = "Õige vastus: " + oigeVastus;

            if (vastus.Value == oigeVastus)
            {
                vastus.BackColor = Color.LightGreen;
                tulemus.ForeColor = Color.Green;
            }
            else
            {
                vastus.BackColor = Color.LightPink;
                tulemus.ForeColor = Color.Red;
            }
        }

        private void KontrolliVastuseid()
        {
            taimer.Stop();
            KuvaTulemus(summa, summaTulemus, liidetav1 + liidetav2);
            KuvaTulemus(vahe, vaheTulemus, vahendatav - lahutatav);
            KuvaTulemus(korrutis, korrutisTulemus, tegur1 * tegur2);
            KuvaTulemus(jagatis, jagatisTulemus, jagatav / jagaja);
            LubaVastused(false);
            alusta.Text = "Alusta mängu";
            alusta.Focus();
        }

        private void Taimer_Tick(object sender, EventArgs e)
        {
            aegaAlles--;
            aeg.Text = aegaAlles + " sekundit";

            if (aegaAlles == 0)
            {
                aeg.Text = "Aeg on läbi!";
                KontrolliVastuseid();
            }
        }

        private void LubaVastused(bool lubatud)
        {
            summa.Enabled = lubatud;
            vahe.Enabled = lubatud;
            korrutis.Enabled = lubatud;
            jagatis.Enabled = lubatud;
        }

        private void Vastus_Enter(object sender, EventArgs e)
        {
            NumericUpDown vastus = sender as NumericUpDown;
            if (vastus != null)
                vastus.Select(0, vastus.Text.Length);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            taimer.Stop();
            taimer.Dispose();
            base.OnFormClosed(e);
        }
    }
}
