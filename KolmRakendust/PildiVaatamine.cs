using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace KolmRakendust
{
    public class PildiVaatamine : Form
    {
        PictureBox pilt;
        CheckBox venita;
        Button kuva, eelmine, jargmine, puhasta, sulge;
        Label info;

        List<string> pildid = new List<string>();
        int praegunePilt = 0;

        public PildiVaatamine()
        {
            Text = "Pildi vaatamise programm";
            ClientSize = new Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            pilt = new PictureBox();
            pilt.Location = new Point(20, 20);
            pilt.Size = new Size(660, 360);
            pilt.BorderStyle = BorderStyle.FixedSingle;
            pilt.SizeMode = PictureBoxSizeMode.Zoom;

            info = new Label();
            info.Text = "";
            info.Location = new Point(310, 390);
            info.Size = new Size(100, 25);
            info.TextAlign = ContentAlignment.MiddleCenter;

            venita = new CheckBox();
            venita.Text = "Venita pilt";
            venita.Location = new Point(20, 435);
            venita.AutoSize = true;
            venita.CheckedChanged += Venita_CheckedChanged;

            kuva = new Button();
            kuva.Text = "Vali pildid";
            kuva.Location = new Point(160, 430);
            kuva.Size = new Size(100, 35);
            kuva.Click += Kuva_Click;

            eelmine = new Button();
            eelmine.Text = "<";
            eelmine.Location = new Point(275, 430);
            eelmine.Size = new Size(50, 35);
            eelmine.Click += Eelmine_Click;

            jargmine = new Button();
            jargmine.Text = ">";
            jargmine.Location = new Point(335, 430);
            jargmine.Size = new Size(50, 35);
            jargmine.Click += Jargmine_Click;

            puhasta = new Button();
            puhasta.Text = "Puhasta";
            puhasta.Location = new Point(445, 430);
            puhasta.Size = new Size(100, 35);
            puhasta.Click += Puhasta_Click;

            sulge = new Button();
            sulge.Text = "Sulge";
            sulge.Location = new Point(560, 430);
            sulge.Size = new Size(100, 35);
            sulge.Click += Sulge_Click;

            Controls.Add(pilt);
            Controls.Add(info);
            Controls.Add(venita);
            Controls.Add(kuva);
            Controls.Add(eelmine);
            Controls.Add(jargmine);
            Controls.Add(puhasta);
            Controls.Add(sulge);
        }

        private void Kuva_Click(object sender, EventArgs e)
        {
            OpenFileDialog aken = new OpenFileDialog();
            aken.Title = "Vali pildid";
            aken.Filter = "Pildifailid|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            aken.Multiselect = true;

            if (aken.ShowDialog() == DialogResult.OK)
            {
                pildid.Clear();
                pildid.AddRange(aken.FileNames);
                praegunePilt = 0;

                KuvaPilt();
            }
        }

        private void KuvaPilt()
        {
            if (pildid.Count == 0)
                return;

            try
            {
                if (pilt.Image != null)
                    pilt.Image.Dispose();

                pilt.Image = Image.FromFile(pildid[praegunePilt]);
                info.Text = (praegunePilt + 1) + " / " + pildid.Count;
            }
            catch
            {
                pilt.Image = null;
                MessageBox.Show("Seda faili ei saa pildina avada.", "Viga");
            }
        }

        private void Eelmine_Click(object sender, EventArgs e)
        {
            if (pildid.Count == 0)
                return;

            praegunePilt--;

            if (praegunePilt < 0)
                praegunePilt = pildid.Count - 1;

            KuvaPilt();
        }

        private void Jargmine_Click(object sender, EventArgs e)
        {
            if (pildid.Count == 0)
                return;

            praegunePilt++;

            if (praegunePilt >= pildid.Count)
                praegunePilt = 0;

            KuvaPilt();
        }

        private void Venita_CheckedChanged(object sender, EventArgs e)
        {
            if (venita.Checked)
                pilt.SizeMode = PictureBoxSizeMode.StretchImage;
            else
                pilt.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void Puhasta_Click(object sender, EventArgs e)
        {
            if (pilt.Image != null)
            {
                pilt.Image.Dispose();
                pilt.Image = null;
            }

            pildid.Clear();
            praegunePilt = 0;
            info.Text = "";
        }

        private void Sulge_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (pilt.Image != null)
                pilt.Image.Dispose();

            base.OnFormClosed(e);
        }
    }
}