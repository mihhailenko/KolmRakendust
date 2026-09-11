using System;
using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class PildiVaatamine : Form
    {
        PictureBox pilt;
        CheckBox venita;
        Button kuva, puhasta, sulge;

        public PildiVaatamine()
        {
            Text = "Pildi vaatamise programm";
            ClientSize = new Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            pilt = new PictureBox();
            pilt.Location = new Point(20, 20);
            pilt.Size = new Size(660, 380);
            pilt.BorderStyle = BorderStyle.FixedSingle;
            pilt.SizeMode = PictureBoxSizeMode.Zoom;

            venita = new CheckBox();
            venita.Text = "Venita pilt";
            venita.Location = new Point(20, 425);
            venita.AutoSize = true;
            venita.CheckedChanged += Venita_CheckedChanged;

            kuva = new Button();
            kuva.Text = "Vali pilt";
            kuva.Location = new Point(330, 420);
            kuva.Size = new Size(100, 35);
            kuva.Click += Kuva_Click;

            puhasta = new Button();
            puhasta.Text = "Puhasta";
            puhasta.Location = new Point(445, 420);
            puhasta.Size = new Size(100, 35);
            puhasta.Click += Puhasta_Click;

            sulge = new Button();
            sulge.Text = "Sulge";
            sulge.Location = new Point(560, 420);
            sulge.Size = new Size(100, 35);
            sulge.Click += Sulge_Click;

            Controls.Add(pilt);
            Controls.Add(venita);
            Controls.Add(kuva);
            Controls.Add(puhasta);
            Controls.Add(sulge);
        }

        private void Kuva_Click(object sender, EventArgs e)
        {
            OpenFileDialog aken = new OpenFileDialog();
            aken.Title = "Vali pilt";
            aken.Filter = "Pildifailid|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Kõik failid|*.*";

            if (aken.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (pilt.Image != null) pilt.Image.Dispose();
                    pilt.Image = Image.FromFile(aken.FileName);
                }
                catch
                {
                    pilt.Image = null;
                    MessageBox.Show("Seda faili ei saa pildina avada.", "Viga");
                }
            }
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
        }

        private void Sulge_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (pilt.Image != null) pilt.Image.Dispose();
            base.OnFormClosed(e);
        }


    }
}
