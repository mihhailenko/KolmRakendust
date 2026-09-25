using System.Drawing;
using System.Windows.Forms;

namespace KolmRakendust
{
    internal static class UiTheme
    {
        public static readonly Color Background = Color.FromArgb(246, 247, 249);
        public static readonly Color Surface = Color.White;
        public static readonly Color Text = Color.FromArgb(32, 36, 43);
        public static readonly Color Muted = Color.FromArgb(105, 113, 125);
        public static readonly Color Accent = Color.FromArgb(70, 119, 201);
        public static readonly Color AccentHover = Color.FromArgb(54, 99, 174);
        public static readonly Color Border = Color.FromArgb(223, 228, 234);
        public static readonly Color Green = Color.FromArgb(221, 244, 231);
        public static readonly Color Red = Color.FromArgb(255, 234, 232);

        public static void Form(Form form)
        {
            form.BackColor = Background;
            form.ForeColor = Text;
            form.Font = new Font("Segoe UI", 10f);
        }

        public static void Button(Button button, bool primary = false)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = primary ? 0 : 1;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = primary ? AccentHover : Background;
            button.FlatAppearance.MouseDownBackColor = primary ? AccentHover : Border;
            button.BackColor = primary ? Accent : Surface;
            button.ForeColor = primary ? Surface : Text;
            button.Font = new Font("Segoe UI", 9.5f, primary ? FontStyle.Bold : FontStyle.Regular);
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;
        }
    }
}
