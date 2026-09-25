using System;
using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class Avavorm : Form
    {
        public Avavorm()
        {
            Text = "Kolm rakendust";
            ClientSize = new Size(540, 426);
            MinimumSize = new Size(540, 426);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            UiTheme.Form(this);

            Label pealkiri = new Label();
            pealkiri.Text = "Kolm rakendust";
            pealkiri.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            pealkiri.Location = new Point(36, 28);
            pealkiri.Size = new Size(470, 42);
            Controls.Add(pealkiri);

            Label kirjeldus = new Label();
            kirjeldus.Text = "Vali, millega soovid alustada";
            kirjeldus.ForeColor = UiTheme.Muted;
            kirjeldus.Location = new Point(38, 76);
            kirjeldus.Size = new Size(440, 28);
            Controls.Add(kirjeldus);

            LisaValik("Pildi vaatamine", "Albumid, lemmikud ja fotod", 122,
                (s, e) => { using (PildiVaatamine vorm = new PildiVaatamine()) vorm.ShowDialog(this); });
            LisaValik("Matemaatikamäng", "Neli ülesannet ja 30 sekundit", 210,
                (s, e) => { using (MatemaatikaMang vorm = new MatemaatikaMang()) vorm.ShowDialog(this); });
            LisaValik("Piltide paarid", "Leia kokku sobivad pildid", 298,
                (s, e) => { using (PiltideMang vorm = new PiltideMang()) vorm.ShowDialog(this); });
        }

        private void LisaValik(string nimi, string kirjeldus, int y, EventHandler avamine)
        {
            Button nupp = new Button();
            nupp.Text = nimi + "\n" + kirjeldus + "  →";
            nupp.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            nupp.TextAlign = ContentAlignment.MiddleLeft;
            nupp.Padding = new Padding(20, 0, 10, 0);
            nupp.Location = new Point(36, y);
            nupp.Size = new Size(468, 72);
            UiTheme.Button(nupp);
            nupp.Font = new Font("Segoe UI", 11);
            nupp.Click += avamine;
            Controls.Add(nupp);
        }
    }
}
