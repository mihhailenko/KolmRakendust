using System;
using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    public class Avavorm : Form
    {
        Button pildiNupp, matemaatikaNupp, piltideMangNupp;

        public Avavorm()
        {
            Text = "Kolm rakendust";
            ClientSize = new Size(500, 330);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            Label pealkiri = new Label();
            pealkiri.Text = "Vali rakendus";
            pealkiri.Font = new Font("Arial", 18, FontStyle.Bold);
            pealkiri.Location = new Point(150, 30);
            pealkiri.AutoSize = true;

            pildiNupp = new Button();
            pildiNupp.Text = "Pildi vaatamise programm";
            pildiNupp.Location = new Point(100, 90);
            pildiNupp.Size = new Size(300, 45);
            pildiNupp.Click += PildiNupp_Click;

            matemaatikaNupp = new Button();
            matemaatikaNupp.Text = "Matemaatiline äraarvamismäng";
            matemaatikaNupp.Location = new Point(100, 155);
            matemaatikaNupp.Size = new Size(300, 45);
            matemaatikaNupp.Click += MatemaatikaNupp_Click;

            piltideMangNupp = new Button();
            piltideMangNupp.Text = "Sarnaste piltide mäng";
            piltideMangNupp.Location = new Point(100, 220);
            piltideMangNupp.Size = new Size(300, 45);
            piltideMangNupp.Click += PiltideMangNupp_Click;

            Controls.Add(pealkiri);
            Controls.Add(pildiNupp);
            Controls.Add(matemaatikaNupp);
            Controls.Add(piltideMangNupp);
        }

        private void PildiNupp_Click(object sender, EventArgs e)
        {
            PildiVaatamine vorm = new PildiVaatamine();
            vorm.ShowDialog(this);
        }

        private void MatemaatikaNupp_Click(object sender, EventArgs e)
        {
            MatemaatikaMang vorm = new MatemaatikaMang();
            vorm.ShowDialog(this);
        }

        private void PiltideMangNupp_Click(object sender, EventArgs e)
        {
            PiltideMang vorm = new PiltideMang();
            vorm.ShowDialog(this);
        }
    }
}
