using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace KolmRakendust
{
    public class PildiVaatamine : Form
    {
        PictureBox pilt;
        Panel pildiAla;

        CheckBox venita, lemmik;
        Button kuva, eelmine, jargmine, lemmikudNupp, puhasta, sulge;
        Label info;

        List<string> pildid = new List<string>();
        List<string> koikPildid = new List<string>();

        HashSet<string> lemmikud = new HashSet<string>();

        int praegunePilt = 0;

        float zoom = 1.0f;

        bool lemmikuUuendamine = false;
        bool ainultLemmikud = false;

        string praeguneHash = "";

        string lemmikuteFail = Path.Combine(Application.StartupPath, "lemmikud.txt");

        public PildiVaatamine()
        {
            Text = "Pildi vaatamise programm";
            ClientSize = new Size(700, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            pildiAla = new Panel();
            pildiAla.Location = new Point(20, 20);
            pildiAla.Size = new Size(660, 360);
            pildiAla.AutoScroll = true;

            pilt = new PictureBox();
            pilt.Location = new Point(0, 0);
            pilt.Size = new Size(660, 360);
            pilt.BorderStyle = BorderStyle.FixedSingle;
            pilt.SizeMode = PictureBoxSizeMode.Zoom;

            pildiAla.Controls.Add(pilt);

            info = new Label();
            info.Text = "";
            info.Location = new Point(20, 385);
            info.Size = new Size(660, 25);
            info.TextAlign = ContentAlignment.MiddleCenter;
            info.AutoEllipsis = true;

            venita = new CheckBox();
            venita.Text = "Venita pilt";
            venita.Location = new Point(20, 410);
            venita.AutoSize = true;
            venita.CheckedChanged += Venita_CheckedChanged;

            lemmik = new CheckBox();
            lemmik.Text = "Lemmik";
            lemmik.Location = new Point(120, 410);
            lemmik.AutoSize = true;
            lemmik.CheckedChanged += Lemmik_CheckedChanged;

            kuva = new Button();
            kuva.Text = "Vali pildid";
            kuva.Location = new Point(150, 435);
            kuva.Size = new Size(100, 35);
            kuva.Click += Kuva_Click;

            eelmine = new Button();
            eelmine.Text = "<";
            eelmine.Location = new Point(260, 435);
            eelmine.Size = new Size(45, 35);
            eelmine.Click += Eelmine_Click;

            jargmine = new Button();
            jargmine.Text = ">";
            jargmine.Location = new Point(315, 435);
            jargmine.Size = new Size(45, 35);
            jargmine.Click += Jargmine_Click;

            lemmikudNupp = new Button();
            lemmikudNupp.Text = "Lemmikud";
            lemmikudNupp.Location = new Point(370, 435);
            lemmikudNupp.Size = new Size(90, 35);
            lemmikudNupp.Click += Lemmikud_Click;

            puhasta = new Button();
            puhasta.Text = "Puhasta";
            puhasta.Location = new Point(470, 435);
            puhasta.Size = new Size(90, 35);
            puhasta.Click += Puhasta_Click;

            sulge = new Button();
            sulge.Text = "Sulge";
            sulge.Location = new Point(570, 435);
            sulge.Size = new Size(90, 35);
            sulge.Click += Sulge_Click;

            Controls.Add(pildiAla);
            Controls.Add(info);
            Controls.Add(venita);
            Controls.Add(lemmik);
            Controls.Add(kuva);
            Controls.Add(eelmine);
            Controls.Add(jargmine);
            Controls.Add(lemmikudNupp);
            Controls.Add(puhasta);
            Controls.Add(sulge);

            // Drag & Drop
            AllowDrop = true;
            pildiAla.AllowDrop = true;
            pilt.AllowDrop = true;

            DragEnter += Pilt_DragEnter;
            DragDrop += Pilt_DragDrop;

            pildiAla.DragEnter += Pilt_DragEnter;
            pildiAla.DragDrop += Pilt_DragDrop;

            pilt.DragEnter += Pilt_DragEnter;
            pilt.DragDrop += Pilt_DragDrop;

            // Zoom hiirerattaga
            MouseWheel += Pilt_MouseWheel;

            pilt.MouseEnter += Pilt_MouseEnter;
            pildiAla.MouseEnter += Pilt_MouseEnter;

            LoeLemmikud();
        }

        private void Kuva_Click(object sender, EventArgs e)
        {
            OpenFileDialog aken = new OpenFileDialog();
            aken.Title = "Vali pildid";
            aken.Filter = "Pildifailid|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            aken.Multiselect = true;

            if (aken.ShowDialog() == DialogResult.OK)
            {
                koikPildid.Clear();
                pildid.Clear();

                koikPildid.AddRange(aken.FileNames);
                pildid.AddRange(aken.FileNames);

                praegunePilt = 0;
                ainultLemmikud = false;
                lemmikudNupp.Text = "Lemmikud";

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

                using (Image ajutinePilt = Image.FromFile(pildid[praegunePilt]))
                {
                    pilt.Image = new Bitmap(ajutinePilt);
                }

                zoom = 1.0f;

                pilt.Size = new Size(660, 360);
                pilt.Location = new Point(0, 0);

                pildiAla.AutoScrollPosition = new Point(0, 0);

                praeguneHash = LeiaSHA256(pildid[praegunePilt]);

                lemmikuUuendamine = true;
                lemmik.Checked = lemmikud.Contains(praeguneHash);
                lemmikuUuendamine = false;

                UuendaInfo();
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

        private void Lemmik_CheckedChanged(object sender, EventArgs e)
        {
            if (lemmikuUuendamine)
                return;

            if (pildid.Count == 0)
                return;

            if (praeguneHash == "")
                return;

            if (lemmik.Checked)
                lemmikud.Add(praeguneHash);
            else
                lemmikud.Remove(praeguneHash);

            SalvestaLemmikud();

            if (ainultLemmikud && !lemmik.Checked)
            {
                pildid.RemoveAt(praegunePilt);

                if (pildid.Count == 0)
                {
                    TuhjendaPilt();
                    return;
                }

                if (praegunePilt >= pildid.Count)
                    praegunePilt = pildid.Count - 1;

                KuvaPilt();
            }
        }

        private void Lemmikud_Click(object sender, EventArgs e)
        {
            if (koikPildid.Count == 0)
                return;

            if (!ainultLemmikud)
            {
                List<string> lemmikPildid = new List<string>();

                foreach (string fail in koikPildid)
                {
                    try
                    {
                        string hash = LeiaSHA256(fail);

                        if (lemmikud.Contains(hash))
                            lemmikPildid.Add(fail);
                    }
                    catch
                    {
                    }
                }

                if (lemmikPildid.Count == 0)
                {
                    MessageBox.Show("Lemmikpilte ei ole.", "Info");
                    return;
                }

                pildid.Clear();
                pildid.AddRange(lemmikPildid);

                ainultLemmikud = true;
                lemmikudNupp.Text = "Kõik pildid";
            }
            else
            {
                pildid.Clear();
                pildid.AddRange(koikPildid);

                ainultLemmikud = false;
                lemmikudNupp.Text = "Lemmikud";
            }

            praegunePilt = 0;
            KuvaPilt();
        }

        private string LeiaSHA256(string fail)
        {
            using (SHA256 sha256 = SHA256.Create())
            using (FileStream stream = File.OpenRead(fail))
            {
                byte[] hash = sha256.ComputeHash(stream);

                return BitConverter.ToString(hash)
                    .Replace("-", "")
                    .ToLower();
            }
        }

        private void LoeLemmikud()
        {
            if (!File.Exists(lemmikuteFail))
                return;

            string[] read = File.ReadAllLines(lemmikuteFail);

            foreach (string rida in read)
            {
                if (rida.Trim() != "")
                    lemmikud.Add(rida.Trim());
            }
        }

        private void SalvestaLemmikud()
        {
            try
            {
                File.WriteAllLines(lemmikuteFail, lemmikud);
            }
            catch
            {
                MessageBox.Show("Lemmikuid ei saanud salvestada.", "Viga");
            }
        }

        private void Pilt_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Pilt_DragDrop(object sender, DragEventArgs e)
        {
            string[] failid = (string[])e.Data.GetData(DataFormats.FileDrop);

            koikPildid.Clear();
            pildid.Clear();

            foreach (string fail in failid)
            {
                if (File.Exists(fail) && KasPildifail(fail))
                {
                    koikPildid.Add(fail);
                    pildid.Add(fail);
                }
            }

            if (pildid.Count == 0)
            {
                MessageBox.Show("Sobivaid pildifaile ei leitud.", "Viga");
                return;
            }

            ainultLemmikud = false;
            lemmikudNupp.Text = "Lemmikud";

            praegunePilt = 0;

            KuvaPilt();
        }

        private bool KasPildifail(string fail)
        {
            string laiend = Path.GetExtension(fail).ToLower();

            return laiend == ".jpg" ||
                   laiend == ".jpeg" ||
                   laiend == ".png" ||
                   laiend == ".bmp" ||
                   laiend == ".gif";
        }

        private void Pilt_MouseEnter(object sender, EventArgs e)
        {
            ActiveControl = null;
            Focus();
        }

        private void Pilt_MouseWheel(object sender, MouseEventArgs e)
        {
            if (pilt.Image == null)
                return;

            Point hiir = pildiAla.PointToClient(Cursor.Position);

            if (!pildiAla.ClientRectangle.Contains(hiir))
                return;

            if (e.Delta > 0)
                zoom += 0.1f;
            else
                zoom -= 0.1f;

            if (zoom < 0.5f)
                zoom = 0.5f;

            if (zoom > 3.0f)
                zoom = 3.0f;

            pilt.Size = new Size(
                (int)(660 * zoom),
                (int)(360 * zoom)
            );

            UuendaPildiAsukoht();
            UuendaInfo();
        }

        private void UuendaPildiAsukoht()
        {
            int x = 0;
            int y = 0;

            if (pilt.Width < pildiAla.ClientSize.Width)
                x = (pildiAla.ClientSize.Width - pilt.Width) / 2;

            if (pilt.Height < pildiAla.ClientSize.Height)
                y = (pildiAla.ClientSize.Height - pilt.Height) / 2;

            pilt.Location = new Point(x, y);
        }

        private void UuendaInfo()
        {
            if (pilt.Image == null || pildid.Count == 0)
                return;

            FileInfo failiInfo = new FileInfo(pildid[praegunePilt]);

            string failiSuurus;

            if (failiInfo.Length < 1024 * 1024)
            {
                failiSuurus =
                    Math.Round(failiInfo.Length / 1024.0, 1) + " KB";
            }
            else
            {
                failiSuurus =
                    Math.Round(failiInfo.Length / 1024.0 / 1024.0, 1) + " MB";
            }

            info.Text =
                (praegunePilt + 1) + " / " + pildid.Count +
                " | " + Path.GetFileName(pildid[praegunePilt]) +
                " | " + pilt.Image.Width + " x " + pilt.Image.Height +
                " | " + failiSuurus +
                " | " + (int)(zoom * 100) + "%";
        }

        private void TuhjendaPilt()
        {
            if (pilt.Image != null)
            {
                pilt.Image.Dispose();
                pilt.Image = null;
            }

            info.Text = "";
            praeguneHash = "";

            lemmikuUuendamine = true;
            lemmik.Checked = false;
            lemmikuUuendamine = false;
        }

        private void Puhasta_Click(object sender, EventArgs e)
        {
            TuhjendaPilt();

            pildid.Clear();
            koikPildid.Clear();

            praegunePilt = 0;
            ainultLemmikud = false;

            lemmikudNupp.Text = "Lemmikud";

            zoom = 1.0f;

            pilt.Size = new Size(660, 360);
            pilt.Location = new Point(0, 0);
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