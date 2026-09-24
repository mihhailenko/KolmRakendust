using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace KolmRakendust
{
    public class PildiVaatamine : Form
    {
        PictureBox pilt;
        Panel pildiAla;

        CheckBox venita, lemmik;
        Button kuva, eelmine, jargmine, puhasta, moveToAlbum;
        Label info, pildiLoendur;
        TreeView albumTree;
        Button uusAlbum, nimetaAlbum, kustutaAlbum;
        PhotoLibrary library;
        PhotoAlbum activeAlbum;
        bool updatingTree;
        // Fototeegi fail asub kasutaja arvutis, mitte programmi kaustas.
        readonly string libraryDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KolmRakendust", "PildiVaatamine");

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
            ClientSize = new Size(1000, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            pildiAla = new Panel();
            pildiAla.Location = new Point(260, 20);
            pildiAla.Size = new Size(700, 360);
            pildiAla.AutoScroll = true;

            pilt = new PictureBox();
            pilt.Location = new Point(0, 0);
            pilt.Size = new Size(700, 360);
            pilt.BorderStyle = BorderStyle.FixedSingle;
            pilt.SizeMode = PictureBoxSizeMode.Zoom;

            pildiAla.Controls.Add(pilt);

            info = new Label();
            info.Text = "";
            info.Location = new Point(260, 385);
            info.Size = new Size(700, 25);
            info.TextAlign = ContentAlignment.MiddleCenter;
            info.AutoEllipsis = true;

            venita = new CheckBox();
            venita.Text = "Venita pilt";
            venita.Location = new Point(555, 443);
            venita.AutoSize = true;
            venita.CheckedChanged += Venita_CheckedChanged;

            lemmik = new CheckBox();
            lemmik.Text = "Lemmik";
            lemmik.Location = new Point(455, 443);
            lemmik.AutoSize = true;
            lemmik.CheckedChanged += Lemmik_CheckedChanged;

            kuva = new Button();
            kuva.Text = "Lisa pildid";
            kuva.Location = new Point(20, 440);
            kuva.Size = new Size(220, 35);
            kuva.Click += Kuva_Click;

            eelmine = new Button();
            eelmine.Text = "<";
            eelmine.Location = new Point(260, 435);
            eelmine.Size = new Size(45, 35);
            eelmine.Click += Eelmine_Click;

            pildiLoendur = new Label();
            pildiLoendur.Text = "0 / 0";
            pildiLoendur.Location = new Point(310, 435);
            pildiLoendur.Size = new Size(75, 35);
            pildiLoendur.TextAlign = ContentAlignment.MiddleCenter;

            jargmine = new Button();
            jargmine.Text = ">";
            jargmine.Location = new Point(390, 435);
            jargmine.Size = new Size(45, 35);
            jargmine.Click += Jargmine_Click;

            puhasta = new Button();
            puhasta.Text = "Eemalda albumist";
            puhasta.Location = new Point(820, 435);
            puhasta.Size = new Size(140, 35);
            puhasta.Click += Puhasta_Click;

            moveToAlbum = new Button();
            moveToAlbum.Text = "Teise albumisse";
            moveToAlbum.Location = new Point(675, 435);
            moveToAlbum.Size = new Size(130, 35);
            moveToAlbum.Click += MoveToAlbum_Click;

            Controls.Add(pildiAla);
            Controls.Add(info);
            Controls.Add(venita);
            Controls.Add(lemmik);
            Controls.Add(kuva);
            Controls.Add(eelmine);
            Controls.Add(pildiLoendur);
            Controls.Add(jargmine);
            Controls.Add(puhasta);
            Controls.Add(moveToAlbum);

            // Vasakul on albumite puu ja selle all albumite nupud.
            albumTree = new TreeView();
            albumTree.Location = new Point(20, 20);
            albumTree.Size = new Size(220, 340);
            albumTree.HideSelection = false;
            albumTree.AfterSelect += AlbumTree_AfterSelect;
            albumTree.AllowDrop = true;
            albumTree.DragEnter += Pilt_DragEnter;
            albumTree.DragDrop += AlbumTree_DragDrop;
            Controls.Add(albumTree);

            uusAlbum = new Button();
            uusAlbum.Text = "Uus album";
            uusAlbum.Location = new Point(20, 370);
            uusAlbum.Size = new Size(105, 30);

            nimetaAlbum = new Button();
            nimetaAlbum.Text = "Nimeta ümber";
            nimetaAlbum.Location = new Point(130, 370);
            nimetaAlbum.Size = new Size(110, 30);

            kustutaAlbum = new Button();
            kustutaAlbum.Text = "Kustuta album";
            kustutaAlbum.Location = new Point(20, 405);
            kustutaAlbum.Size = new Size(220, 30);
            uusAlbum.Click += (s, e) => CreateAlbum();
            nimetaAlbum.Click += (s, e) => RenameAlbum();
            kustutaAlbum.Click += (s, e) => DeleteAlbum();
            Controls.Add(uusAlbum);
            Controls.Add(nimetaAlbum);
            Controls.Add(kustutaAlbum);

            // Pildifaile saab ka aknasse hiirega lohistada.
            AllowDrop = true;
            pildiAla.AllowDrop = true;
            pilt.AllowDrop = true;

            DragEnter += Pilt_DragEnter;
            DragDrop += Pilt_DragDrop;

            pildiAla.DragEnter += Pilt_DragEnter;
            pildiAla.DragDrop += Pilt_DragDrop;

            pilt.DragEnter += Pilt_DragEnter;
            pilt.DragDrop += Pilt_DragDrop;

            // Hiireratas muudab pildi suurust.
            MouseWheel += Pilt_MouseWheel;

            pilt.MouseEnter += Pilt_MouseEnter;
            pildiAla.MouseEnter += Pilt_MouseEnter;

            // Avamisel loeme varem salvestatud albumid sisse.
            LoadLibrary();
            RefreshTree(null);
        }

        private void Kuva_Click(object sender, EventArgs e)
        {
            OpenFileDialog aken = new OpenFileDialog();
            aken.Title = "Vali pildid";
            aken.Filter = "Pildifailid|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            aken.Multiselect = true;

            if (aken.ShowDialog() == DialogResult.OK)
                ImportPhotos(aken.FileNames, activeAlbum);
        }

        private string LibraryFile { get { return Path.Combine(libraryDirectory, "library.xml"); } }

        private void LoadLibrary()
        {
            // Kui XML-fail on olemas, loeme sellest albumid ja piltide asukohad.
            try
            {
                if (File.Exists(LibraryFile))
                {
                    using (FileStream stream = File.OpenRead(LibraryFile))
                    {
                        library = (PhotoLibrary)new XmlSerializer(typeof(PhotoLibrary)).Deserialize(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fototeeki ei saanud avada: " + ex.Message, "Viga");
            }
            // Esimesel käivitamisel ei ole veel midagi salvestatud.
            if (library == null)
                library = new PhotoLibrary();
            if (library.Albums == null)
                library.Albums = new List<PhotoAlbum>();
            if (library.Favorites == null)
                library.Favorites = new List<string>();
            foreach (PhotoAlbum album in library.Albums)
                if (album.Photos == null)
                    album.Photos = new List<string>();
            foreach (string hash in library.Favorites)
                if (!string.IsNullOrWhiteSpace(hash))
                    lemmikud.Add(hash.Trim());

            // Vana versiooni lemmikud lisame uude fototeeki.
            if (!File.Exists(LibraryFile)) LoeLemmikud();
            if (library.Albums.Count == 0)
            {
                library.Albums.Add(new PhotoAlbum { Name = "Minu pildid" });
                SaveLibrary();
            }
        }

        private void SaveLibrary()
        {
            // Kõigepealt kirjutame ajutisse faili. Nii ei kao vana fail poole kirjutamise pealt.
            string temporary = LibraryFile + ".tmp";
            try
            {
                Directory.CreateDirectory(libraryDirectory);
                library.Favorites = new List<string>(lemmikud);
                using (FileStream stream = File.Create(temporary))
                {
                    new XmlSerializer(typeof(PhotoLibrary)).Serialize(stream, library);
                }
                if (File.Exists(LibraryFile))
                    File.Replace(temporary, LibraryFile, null);
                else
                    File.Move(temporary, LibraryFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fototeeki ei saanud salvestada: " + ex.Message, "Viga");
            }
            finally
            {
                try
                {
                    if (File.Exists(temporary))
                        File.Delete(temporary);
                }
                catch (IOException) { }
            }
        }

        private void RefreshTree(PhotoAlbum selected)
        {
            // Ehitame vasakpoolse albumite puu uuesti üles.
            updatingTree = true;
            albumTree.BeginUpdate();
            albumTree.Nodes.Clear();
            albumTree.Nodes.Add(new TreeNode("Kõik pildid") { Tag = "all" });
            albumTree.Nodes.Add(new TreeNode("Lemmikud") { Tag = "favorites" });
            TreeNode albums = new TreeNode("Albumid");
            foreach (PhotoAlbum album in library.Albums)
                albums.Nodes.Add(new TreeNode(album.Name) { Tag = album });
            albumTree.Nodes.Add(albums);
            albums.Expand();
            albumTree.SelectedNode = albumTree.Nodes[0];
            if (selected != null)
            {
                foreach (TreeNode node in albums.Nodes)
                {
                    if (node.Tag == selected)
                        albumTree.SelectedNode = node;
                }
            }
            albumTree.EndUpdate();
            updatingTree = false;
            SelectNode(albumTree.SelectedNode);
        }

        private void AlbumTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!updatingTree) SelectNode(e.Node);
        }

        private void SelectNode(TreeNode node)
        {
            if (node == null || node.Tag == null) return;
            activeAlbum = node.Tag as PhotoAlbum;
            koikPildid.Clear();

            // Albumi puhul näitame selle pilte. Üldvaates kogume pildid kõigist albumitest.
            if (activeAlbum != null)
            {
                foreach (string path in activeAlbum.Photos)
                    if (!string.IsNullOrWhiteSpace(path)) koikPildid.Add(path);
            }
            else
            {
                foreach (PhotoAlbum album in library.Albums)
                {
                    if (album.Photos == null) continue;
                    foreach (string path in album.Photos)
                    {
                        if (string.IsNullOrWhiteSpace(path))
                            continue;
                        bool alreadyAdded = false;
                        foreach (string existing in koikPildid)
                            if (string.Equals(existing, path, StringComparison.OrdinalIgnoreCase))
                                alreadyAdded = true;
                        if (!alreadyAdded)
                            koikPildid.Add(path);
                    }
                }
            }
            pildid.Clear();
            pildid.AddRange(koikPildid);
            ainultLemmikud = (node.Tag as string) == "favorites";
            if (ainultLemmikud)
                FilterFavorites();
            praegunePilt = 0;
            if (pildid.Count == 0) TuhjendaPilt();
            else KuvaPilt();
            puhasta.Enabled = activeAlbum != null && pildid.Count > 0;
            moveToAlbum.Enabled = activeAlbum != null && pildid.Count > 0 && library.Albums.Count > 1;
            nimetaAlbum.Enabled = kustutaAlbum.Enabled = activeAlbum != null;
        }

        private void FilterFavorites()
        {
            // Lemmikute vaates jätame alles ainult lemmikuks märgitud pildid.
            pildid.Clear();
            foreach (string path in koikPildid)
            {
                try
                {
                    if (lemmikud.Contains(LeiaSHA256(path)))
                        pildid.Add(path);
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        private static string AskAlbumName(string title, string current)
        {
            // Väike aken, kus saab albumile nime kirjutada.
            using (Form dialog = new Form())
            {
                dialog.Text = title;
                dialog.ClientSize = new Size(350, 110);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;

                TextBox input = new TextBox();
                input.Location = new Point(12, 12);
                input.Size = new Size(326, 25);
                input.Text = current;

                Button ok = new Button();
                ok.Text = "OK";
                ok.Location = new Point(170, 55);
                ok.Size = new Size(80, 30);
                ok.DialogResult = DialogResult.OK;

                Button cancel = new Button();
                cancel.Text = "Loobu";
                cancel.Location = new Point(258, 55);
                cancel.Size = new Size(80, 30);
                cancel.DialogResult = DialogResult.Cancel;
                dialog.Controls.Add(input);
                dialog.Controls.Add(ok);
                dialog.Controls.Add(cancel);
                dialog.AcceptButton = ok;
                dialog.CancelButton = cancel;
                if (dialog.ShowDialog() == DialogResult.OK)
                    return input.Text.Trim();
                return null;
            }
        }

        private bool ValidAlbumName(string name, PhotoAlbum current = null)
        {
            if (name.Length > 0)
            {
                bool nameTaken = false;
                foreach (PhotoAlbum album in library.Albums)
                    if (album != current && string.Equals(album.Name, name,
                        StringComparison.CurrentCultureIgnoreCase)) nameTaken = true;
                if (!nameTaken) return true;
            }
            MessageBox.Show("Sisesta kordumatu albumi nimi.", "Album");
            return false;
        }

        private void CreateAlbum()
        {
            // Lisame uue albumi nimekirja ja salvestame muudatuse.
            string name = AskAlbumName("Uus album", "");
            if (name == null || !ValidAlbumName(name)) return;
            PhotoAlbum album = new PhotoAlbum { Name = name };
            library.Albums.Add(album);
            SaveLibrary();
            RefreshTree(album);
        }

        private void RenameAlbum()
        {
            if (activeAlbum == null) return;
            string name = AskAlbumName("Nimeta album ümber", activeAlbum.Name);
            if (name == null || !ValidAlbumName(name, activeAlbum)) return;
            activeAlbum.Name = name;
            SaveLibrary();
            RefreshTree(activeAlbum);
        }

        private void DeleteAlbum()
        {
            // Kustutame ainult albumi nimekirjast, mitte arvutis olevaid pildifaile.
            if (activeAlbum == null) return;
            if (MessageBox.Show("Kustuta album \"" + activeAlbum.Name + "\"? Pildifaile ei kustutata.",
                "Kustuta album", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            library.Albums.Remove(activeAlbum);
            if (library.Albums.Count == 0)
                library.Albums.Add(new PhotoAlbum { Name = "Minu pildid" });
            SaveLibrary();
            RefreshTree(null);
        }

        private void ImportPhotos(IEnumerable<string> paths, PhotoAlbum album)
        {
            // Kui albumit pole valitud, lähevad pildid esimesse albumisse.
            if (album == null) album = library.Albums[0];
            List<string> files = new List<string>();
            foreach (string path in paths)
                if (File.Exists(path) && KasPildifail(path)) files.Add(path);
            if (files.Count == 0)
            {
                MessageBox.Show("Sobivaid pildifaile ei leitud.", "Info");
                return;
            }
            // Kasutaja valib, kas teha pildist koopia või jätta see algsesse kausta.
            DialogResult mode = MessageBox.Show(
                "Kas kopeerida pildid rakenduse fototeeki?\n\nJah: pildid säilivad ka siis, kui originaalid teisaldatakse.\nEi: kasutatakse algsete failide asukohti.",
                "Piltide lisamine", MessageBoxButtons.YesNoCancel);
            if (mode == DialogResult.Cancel) return;
            int failed = 0;
            string first = null;
            foreach (string path in files)
            {
                try
                {
                    bool alreadyAdded = false;
                    foreach (string existing in album.Photos)
                        if (string.Equals(existing, path, StringComparison.OrdinalIgnoreCase)) alreadyAdded = true;
                    if (mode == DialogResult.No && alreadyAdded)
                        continue;
                    string saved = path;
                    if (mode == DialogResult.Yes)
                    {
                        string directory = Path.Combine(libraryDirectory, "Files");
                        Directory.CreateDirectory(directory);
                        saved = Path.Combine(directory, Guid.NewGuid().ToString("N") +
                            Path.GetExtension(path).ToLowerInvariant());
                        File.Copy(path, saved);
                    }
                    album.Photos.Add(saved);
                    if (first == null) first = saved;
                }
                catch (IOException)
                {
                    failed++;
                }
                catch (UnauthorizedAccessException)
                {
                    failed++;
                }
            }
            if (first != null)
            {
                SaveLibrary();
                RefreshTree(album);
                praegunePilt = 0;
                for (int i = 0; i < pildid.Count; i++)
                    if (string.Equals(pildid[i], first, StringComparison.OrdinalIgnoreCase)) praegunePilt = i;
                KuvaPilt();
            }
            if (failed > 0) MessageBox.Show(failed + " faili ei saanud lisada.", "Piltide lisamine");
        }

        private void AlbumTree_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null) return;
            TreeNode target = albumTree.GetNodeAt(albumTree.PointToClient(new Point(e.X, e.Y)));
            ImportPhotos(files, target == null ? null : target.Tag as PhotoAlbum);
        }

        private void MoveToAlbum_Click(object sender, EventArgs e)
        {
            // Valime teise albumi ja tõstame käesoleva pildi sinna.
            if (activeAlbum == null || pildid.Count == 0) return;
            PhotoAlbum source = activeAlbum;
            string path = pildid[praegunePilt];
            using (Form dialog = new Form())
            {
                dialog.Text = "Vali album";
                dialog.ClientSize = new Size(300, 110);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                ComboBox albums = new ComboBox();
                albums.Location = new Point(12, 12);
                albums.Width = 276;
                albums.DropDownStyle = ComboBoxStyle.DropDownList;
                foreach (PhotoAlbum album in library.Albums)
                    if (album != source) albums.Items.Add(album.Name);
                albums.SelectedIndex = 0;
                Button ok = new Button();
                ok.Text = "OK";
                ok.Location = new Point(120, 55);
                ok.DialogResult = DialogResult.OK;

                Button cancel = new Button();
                cancel.Text = "Loobu";
                cancel.Location = new Point(205, 55);
                cancel.DialogResult = DialogResult.Cancel;
                dialog.Controls.Add(albums);
                dialog.Controls.Add(ok);
                dialog.Controls.Add(cancel);
                dialog.AcceptButton = ok;
                dialog.CancelButton = cancel;
                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                foreach (PhotoAlbum album in library.Albums)
                {
                    if (album.Name != (string)albums.SelectedItem) continue;
                    bool alreadyThere = false;
                    foreach (string existing in album.Photos)
                        if (string.Equals(existing, path, StringComparison.OrdinalIgnoreCase)) alreadyThere = true;
                    if (!alreadyThere)
                        album.Photos.Add(path);
                    source.Photos.Remove(path);
                    SaveLibrary();
                    SelectNode(albumTree.SelectedNode);
                    return;
                }
            }
        }

        private void KuvaPilt()
        {
            // Avame faili ja näitame seda PictureBoxis.
            if (pildid.Count == 0)
                return;

            pildiLoendur.Text = (praegunePilt + 1) + " / " + pildid.Count;

            try
            {
                if (pilt.Image != null)
                    pilt.Image.Dispose();

                using (Image ajutinePilt = Image.FromFile(pildid[praegunePilt]))
                {
                    pilt.Image = new Bitmap(ajutinePilt);
                }

                zoom = 1.0f;

                pilt.Size = new Size(700, 360);
                pilt.Location = new Point(0, 0);

                pildiAla.AutoScrollPosition = new Point(0, 0);

                // Räsi järgi saame aru, kas see pilt on lemmik.
                praeguneHash = LeiaSHA256(pildid[praegunePilt]);

                lemmikuUuendamine = true;
                lemmik.Checked = lemmikud.Contains(praeguneHash);
                lemmikuUuendamine = false;

                UuendaInfo();
            }
            catch
            {
                pilt.Image = null;
                praeguneHash = "";
                lemmikuUuendamine = true;
                lemmik.Checked = false;
                lemmikuUuendamine = false;
                info.Text = "Pilti ei saa avada või faili ei leitud: " + pildid[praegunePilt];
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
            // Märkeruudu muutmisel lisame või eemaldame pildi lemmikutest.
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

            SaveLibrary();

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

        private string LeiaSHA256(string fail)
        {
            // Samal pildil on sama räsi ka siis, kui failinimi muutub.
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
            try
            {
                string[] read = File.ReadAllLines(lemmikuteFail);
                foreach (string rida in read)
                    if (rida.Trim() != "") lemmikud.Add(rida.Trim());
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        private void Pilt_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Pilt_DragDrop(object sender, DragEventArgs e)
        {
            // Lohistatud failid lisame valitud albumisse.
            string[] failid = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (failid != null)
                ImportPhotos(failid, activeAlbum);
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
            // Hiirerattaga saab pilti suurendada või vähendada.
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
                (int)(700 * zoom),
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
                Path.GetFileName(pildid[praegunePilt]) +
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
            pildiLoendur.Text = "0 / 0";
            praeguneHash = "";

            lemmikuUuendamine = true;
            lemmik.Checked = false;
            lemmikuUuendamine = false;
        }

        private void Puhasta_Click(object sender, EventArgs e)
        {
            // Eemaldame pildi albumist, kuid faili arvutist ei kustuta.
            if (activeAlbum == null || pildid.Count == 0) return;
            activeAlbum.Photos.Remove(pildid[praegunePilt]);
            SaveLibrary();
            SelectNode(albumTree.SelectedNode);
        }

        protected override void OnFormClosed(FormClosedEventArgs e) 
        {
            if (pilt.Image != null)
                pilt.Image.Dispose();

            base.OnFormClosed(e);
        }
    }
}
