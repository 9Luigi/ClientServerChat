using System.Drawing;
using System.Windows.Forms;

namespace WFChat.UI
{
    /// <summary>
    /// Colors and control styles shared by the client and the server: a classic corporate palette
    /// (navy, white, light gray). Layout and fonts are set in the designer; colors are applied here at runtime.
    /// </summary>
    public static class Theme
    {
        public static readonly Color Primary = Color.FromArgb(0x1E, 0x3A, 0x5F);        //navy: header, main buttons
        public static readonly Color PrimaryHover = Color.FromArgb(0x2C, 0x52, 0x82);
        public static readonly Color PrimaryPressed = Color.FromArgb(0x16, 0x2C, 0x48);
        public static readonly Color Background = Color.FromArgb(0xF3, 0xF5, 0xF8);     //window background
        public static readonly Color Surface = Color.White;                             //text areas, cards
        public static readonly Color Border = Color.FromArgb(0xCB, 0xD2, 0xDB);
        public static readonly Color SecondaryHover = Color.FromArgb(0xE8, 0xED, 0xF3);
        public static readonly Color Text = Color.FromArgb(0x1F, 0x29, 0x33);
        public static readonly Color TextMuted = Color.FromArgb(0x6B, 0x77, 0x85);
        public static readonly Color HeaderText = Color.White;
        public static readonly Color HeaderTextMuted = Color.FromArgb(0xB8, 0xC7, 0xDA);

        public static readonly Font ChatFont = new Font("Segoe UI", 10F);
        public static readonly Font ChatFontBold = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font ChatFontItalic = new Font("Segoe UI", 10F, FontStyle.Italic);

        /// <summary>Background and text colors for the form and all its controls; buttons get the secondary style.</summary>
        public static void Apply(Form form)
        {
            form.BackColor = Background;
            form.ForeColor = Text;
            ApplyToChildren(form);
        }

        static void ApplyToChildren(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                switch (control)
                {
                    case TextBoxBase textBox: //TextBox, RichTextBox, MaskedTextBox
                        textBox.BackColor = Surface;
                        textBox.ForeColor = Text;
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case Button button:
                        StyleSecondary(button);
                        break;
                }
                ApplyToChildren(control);
            }
        }

        /// <summary>The main action of a screen: filled navy button.</summary>
        public static void StylePrimary(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = PrimaryHover;
            button.FlatAppearance.MouseDownBackColor = PrimaryPressed;
            button.BackColor = Primary;
            button.ForeColor = Color.White;
            button.Cursor = Cursors.Hand;
        }

        /// <summary>Other actions: white button with a thin border and navy text.</summary>
        public static void StyleSecondary(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = SecondaryHover;
            button.FlatAppearance.MouseDownBackColor = Border;
            button.BackColor = Surface;
            button.ForeColor = Primary;
            button.Cursor = Cursors.Hand;
        }

        /// <summary>Navy band at the top of a window with a title and an optional muted subtitle.</summary>
        public static void StyleHeader(Panel header, Label title, Label subtitle = null)
        {
            header.BackColor = Primary;
            title.ForeColor = HeaderText;
            title.BackColor = Primary;
            if (subtitle != null)
            {
                subtitle.ForeColor = HeaderTextMuted;
                subtitle.BackColor = Primary;
            }
        }

        /// <summary>White block with a border, e.g. the login card.</summary>
        public static void StyleCard(Panel card)
        {
            card.BackColor = Surface;
            card.Paint += (sender, e) =>
            {
                var panel = (Panel)sender;
                using var pen = new Pen(Border);
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            };
        }

        /// <summary>
        /// White bordered frame around a borderless multiline text box: gives the text some padding
        /// and keeps the scroll bar inside the border.
        /// </summary>
        public static void StyleTextFrame(Panel frame, TextBoxBase textBox)
        {
            StyleCard(frame);
            frame.Padding = new Padding(8, 6, 2, 6);
            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Surface;
        }

        /// <summary>Small gray hint text such as a character counter.</summary>
        public static void StyleMuted(Label label)
        {
            label.ForeColor = TextMuted;
        }
    }
}
