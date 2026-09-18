using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class PiltideMang : Form
    {
        Random random = new Random();
        TableLayoutPanel laud;
        Label esimeneValik, teineValik;
        Timer taimer;
        Button uusMang;
        List<string> margid;

        public PiltideMang()
        {

            Text = "Sarnaste piltide mäng";
            ClientSize = new Size(520, 590);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Label pealkiri = new Label();
            pealkiri.Text = "Leia kõik paarid";
            pealkiri.Font = new Font("Arial", 18, FontStyle.Bold);
            pealkiri.Location = new Point(160, 20);
            pealkiri.AutoSize = true;

            laud = new TableLayoutPanel();
            laud.Location = new Point(30, 70);
            laud.Size = new Size(460, 460);
            laud.RowCount = 4;
            laud.ColumnCount = 4;
            laud.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

            for (int i = 0; i < 4; i++)
            {
                laud.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
                laud.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            }

            for (int i = 0; i < 16; i++)
            {
                Label silt = new Label();
                silt.Dock = DockStyle.Fill;
                silt.Font = new Font("Segoe UI Symbol", 30, FontStyle.Bold);
                silt.TextAlign = ContentAlignment.MiddleCenter;
                silt.BackColor = Color.WhiteSmoke;
                silt.ForeColor = silt.BackColor;
                silt.Margin = new Padding(3);
                silt.Click += Silt_Click;
                laud.Controls.Add(silt);
            }

            uusMang = new Button();
            uusMang.Text = "Uus mäng";
            uusMang.Location = new Point(200, 545);
            uusMang.Size = new Size(120, 35);
            uusMang.Click += UusMang_Click;

            taimer = new Timer();
            taimer.Interval = 750;
            taimer.Tick += Taimer_Tick;

            Controls.Add(pealkiri);
            Controls.Add(laud);
            Controls.Add(uusMang);

            BackColor = Color.LightGray;

            AlustaMang();
        }

        private void AlustaMang()
        {
            taimer.Stop();
            esimeneValik = null;
            teineValik = null;

            // on vaja et oleks täpselt 16 märge
            margid = new List<string>()
            {
                "★", "★", "●", "❤️", 
                "😀", "©", "♥", "♥", 
                "♦", "♦", "♫", "!", 
                "@", "1", "2", "3"
            };

            foreach (Control element in laud.Controls)
            {
                Label silt = element as Label;
                if (silt != null)
                {
                    int number = random.Next(margid.Count);
                    silt.Text = margid[number];
                    margid.RemoveAt(number);
                    silt.ForeColor = silt.BackColor;
                }
            }
        }

        private void Silt_Click(object sender, EventArgs e)
        {
            if (taimer.Enabled) return;

            Label silt = sender as Label;
            if (silt == null) return;
            if (silt == esimeneValik) return;
            if (silt.ForeColor == Color.Black) return;

            if (esimeneValik == null)
            {
                esimeneValik = silt;
                esimeneValik.ForeColor = Color.Black;
                return;
            }

            teineValik = silt;
            teineValik.ForeColor = Color.Black;

            if (esimeneValik.Text == teineValik.Text)
            {
                esimeneValik = null;
                teineValik = null;
                KontrolliVoitu();
            }
            else
                taimer.Start();
        }

        private void Taimer_Tick(object sender, EventArgs e)
        {
            taimer.Stop();

            esimeneValik.ForeColor = esimeneValik.BackColor;
            teineValik.ForeColor = teineValik.BackColor;
            esimeneValik = null;
            teineValik = null;
        }

        private void KontrolliVoitu()
        {
            foreach (Control element in laud.Controls)
            {
                Label silt = element as Label;
                if (silt != null && silt.ForeColor == silt.BackColor)
                    return;
            }

            MessageBox.Show("Leidsid kõik paarid!", "Võit!");
        }

        private void UusMang_Click(object sender, EventArgs e)
        {
            AlustaMang();
        }


    }
}
