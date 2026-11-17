using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WFChat.UI
{
    /// <summary>
    /// Colors, fonts and control styles shared by the client and the server, in the style of The Matrix:
    /// black background, "code" green, monospace font. Layout is set in the designer; the look is applied here at runtime.
    /// </summary>
    public static class Theme
    {
        public static readonly Color Primary = Color.FromArgb(0x00, 0xFF, 0x41);        //matrix green: header text, main buttons
        public static readonly Color PrimaryFill = Color.FromArgb(0x00, 0x3B, 0x00);    //main button background
        public static readonly Color PrimaryHover = Color.FromArgb(0x00, 0x59, 0x1A);
        public static readonly Color PrimaryPressed = Color.FromArgb(0x00, 0x7A, 0x22);
        public static readonly Color Background = Color.Black;                          //window background
        public static readonly Color Surface = Color.FromArgb(0x02, 0x0A, 0x02);        //text areas, cards: black with a hint of green
        public static readonly Color Border = Color.FromArgb(0x12, 0x7A, 0x33);
        public static readonly Color SecondaryHover = Color.FromArgb(0x0A, 0x2A, 0x12);
        public static readonly Color Text = Color.FromArgb(0x33, 0xF5, 0x6A);
        public static readonly Color TextMuted = Color.FromArgb(0x1E, 0x9A, 0x46);
        public static readonly Color HeaderBack = Color.Black;
        public static readonly Color HeaderText = Primary;
        public static readonly Color HeaderTextMuted = Color.FromArgb(0x1E, 0xA8, 0x4A);
        public static readonly Color OwnName = Color.FromArgb(0xE8, 0xFF, 0xE8);        //the current user's name: almost white, like the head of a rain stream
        public static readonly Color RainGreen = Primary;
        public static readonly Color RainHead = Color.FromArgb(0xD6, 0xFF, 0xD6);

        /// <summary>The whole interface uses one monospace font, like a terminal.</summary>
        public const string MonoFamily = "Consolas";

        public static readonly Font ChatFont = new Font(MonoFamily, 10.5F);
        public static readonly Font ChatFontBold = new Font(MonoFamily, 10.5F, FontStyle.Bold);
        public static readonly Font ChatFontItalic = new Font(MonoFamily, 10.5F, FontStyle.Italic);

        static readonly Dictionary<(float, FontStyle), Font> MonoFonts = new Dictionary<(float, FontStyle), Font>();

        /// <summary>Colors and the monospace font for the form and all its controls; buttons get the secondary style.</summary>
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
                control.Font = ToMono(control.Font);
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
                    case ComboBox comboBox:
                        comboBox.BackColor = Surface;
                        comboBox.ForeColor = Text;
                        comboBox.FlatStyle = FlatStyle.Flat;
                        break;
                }
                ApplyToChildren(control);
            }
        }

        /// <summary>
        /// Same size and style in the monospace family; "Segoe UI Semibold" becomes bold.
        /// Set on each control, not on the form: changing the form's font would rescale its whole layout.
        /// </summary>
        static Font ToMono(Font font)
        {
            FontStyle style = font.FontFamily.Name.Contains("Semibold") ? font.Style | FontStyle.Bold : font.Style;
            if (!MonoFonts.TryGetValue((font.Size, style), out Font mono))
            {
                mono = new Font(MonoFamily, font.Size, style);
                MonoFonts.Add((font.Size, style), mono);
            }
            return mono;
        }

        /// <summary>The main action of a screen: dark green button with a bright green frame.</summary>
        public static void StylePrimary(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Primary;
            button.FlatAppearance.MouseOverBackColor = PrimaryHover;
            button.FlatAppearance.MouseDownBackColor = PrimaryPressed;
            button.BackColor = PrimaryFill;
            button.ForeColor = Primary;
            button.Cursor = Cursors.Hand;
        }

        /// <summary>Other actions: black button with a dim green frame.</summary>
        public static void StyleSecondary(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Border;
            button.FlatAppearance.MouseOverBackColor = SecondaryHover;
            button.FlatAppearance.MouseDownBackColor = Border;
            button.BackColor = Surface;
            button.ForeColor = Text;
            button.Cursor = Cursors.Hand;
        }

        /// <summary>Black band at the top of a window with a green title, a muted subtitle and a green line under it.</summary>
        public static void StyleHeader(Panel header, Label title, Label subtitle = null)
        {
            header.BackColor = HeaderBack;
            header.Padding = new Padding(header.Padding.Left, header.Padding.Top, header.Padding.Right, 2); //the labels must not cover the line
            title.ForeColor = HeaderText;
            title.BackColor = HeaderBack;
            if (subtitle != null)
            {
                subtitle.ForeColor = HeaderTextMuted;
                subtitle.BackColor = HeaderBack;
            }
            header.Paint += (sender, e) =>
            {
                var panel = (Panel)sender;
                using var pen = new Pen(Primary);
                e.Graphics.DrawLine(pen, 0, panel.Height - 1, panel.Width, panel.Height - 1);
            };
        }

        /// <summary>Near-black block with a green border, e.g. the login card.</summary>
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
        /// Bordered frame around a borderless multiline text box: gives the text some padding
        /// and keeps the scroll bar inside the border.
        /// </summary>
        public static void StyleTextFrame(Panel frame, TextBoxBase textBox)
        {
            StyleCard(frame);
            frame.Padding = new Padding(8, 6, 2, 6);
            textBox.BorderStyle = BorderStyle.None;
            textBox.BackColor = Surface;
        }

        /// <summary>Small dim hint text such as a character counter.</summary>
        public static void StyleMuted(Label label)
        {
            label.ForeColor = TextMuted;
        }
    }
}
