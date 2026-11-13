using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace WFChat.UI
{
    /// <summary>
    /// Loading screen in the style of The Matrix: digital rain with a terminal box on top, where the lines are typed
    /// one character at a time and a progress bar fills up. Closes by itself, or at once on a click or any key.
    /// </summary>
    /// <example>
    /// <code>
    /// using (var splash = new SplashForm("WFCHAT // CLIENT", "Wake up, Neo...", "The Matrix has you..."))
    ///     splash.ShowDialog();
    /// Application.Run(new MainForm());
    /// </code>
    /// </example>
    public sealed class SplashForm : Form
    {
        static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(4.5);
        const double TypingShare = 0.7; //the text is fully typed after 70% of the time, then it stays on screen

        readonly MatrixRain _rain = new MatrixRain { Dock = DockStyle.Fill };
        readonly string _title;
        readonly string[] _lines;
        readonly int _totalChars;
        readonly TimeSpan _duration;
        readonly Stopwatch _clock = new Stopwatch();
        readonly Timer _timer = new Timer { Interval = 30 };
        readonly Font _titleFont = new Font(Theme.MonoFamily, 10F, FontStyle.Bold);
        readonly Font _lineFont = new Font(Theme.MonoFamily, 15F);
        readonly Font _smallFont = new Font(Theme.MonoFamily, 9.5F);
        int _typedChars;
        bool _fading;

        public SplashForm(string title, params string[] lines) : this(title, DefaultDuration, lines)
        {
        }

        public SplashForm(string title, TimeSpan duration, params string[] lines)
        {
            _title = title;
            _lines = lines;
            _totalChars = lines.Sum(line => line.Length);
            _duration = duration;

            Text = title;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.Black;
            KeyPreview = true;
            ClientSize = LogicalToDeviceUnits(new Size(760, 440));

            _rain.Paint += PaintTerminal;
            _rain.Click += (sender, e) => Finish();
            KeyDown += (sender, e) => Finish();
            Controls.Add(_rain);
            _timer.Tick += Tick;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _clock.Start();
            _timer.Start();
        }

        /// <summary>Types the text in step with the clock; at the end fades the window out and closes it.</summary>
        void Tick(object sender, EventArgs e)
        {
            if (_fading)
            {
                Opacity -= 0.12;
                if (Opacity <= 0.05)
                {
                    _timer.Stop();
                    Close();
                }
                return;
            }
            double progress = Progress;
            _typedChars = (int)Math.Min(_totalChars, _totalChars * progress / TypingShare);
            if (progress >= 1)
            {
                Finish();
            }
            //no Invalidate here: the rain repaints itself every frame and the terminal box is drawn with it
        }

        double Progress => Math.Min(1, _clock.Elapsed.TotalMilliseconds / _duration.TotalMilliseconds);

        /// <summary>Starts the fade-out; a second call (another click) does nothing.</summary>
        void Finish()
        {
            if (_fading)
            {
                return;
            }
            _typedChars = _totalChars;
            _fading = true;
        }

        /// <summary>The terminal box over the rain: title, typed lines with a blinking cursor, progress bar.</summary>
        void PaintTerminal(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            int lineHeight = (int)(_lineFont.GetHeight(g) * 1.5);
            int width = (int)(ClientSize.Width * 0.72);
            int height = lineHeight * (_lines.Length + 3);
            var box = new Rectangle((ClientSize.Width - width) / 2, (ClientSize.Height - height) / 2, width, height);
            int pad = LogicalToDeviceUnits(18);

            using (var shade = new SolidBrush(Color.FromArgb(225, Color.Black)))
            using (var frame = new Pen(Theme.Primary))
            using (var text = new SolidBrush(Theme.Primary))
            using (var muted = new SolidBrush(Theme.TextMuted))
            {
                g.FillRectangle(shade, box);
                g.DrawRectangle(frame, box);
                g.DrawString(_title, _titleFont, muted, box.X + pad, box.Y + pad / 2);
                using (var right = new StringFormat { Alignment = StringAlignment.Far })
                {
                    g.DrawString("click or press any key to skip", _smallFont, muted, box.Right - pad, box.Y + pad / 2, right);
                }

                //lines typed so far; the cursor blinks after the last typed character
                int left = _typedChars;
                float y = box.Y + lineHeight;
                PointF cursor = new PointF(box.X + pad, y);
                foreach (string line in _lines)
                {
                    if (left <= 0)
                    {
                        break;
                    }
                    string typed = line.Substring(0, Math.Min(line.Length, left));
                    left -= typed.Length;
                    g.DrawString(typed, _lineFont, text, box.X + pad, y);
                    cursor = new PointF(box.X + pad + g.MeasureString(typed, _lineFont, PointF.Empty, StringFormat.GenericTypographic).Width, y);
                    y += lineHeight;
                }
                if (_clock.ElapsedMilliseconds / 400 % 2 == 0)
                {
                    g.FillRectangle(text, cursor.X + 2, cursor.Y + 3, _lineFont.Size * 0.6f, _lineFont.GetHeight(g) - 4);
                }

                //progress bar: a frame that fills with green, the percentage beside it
                int barY = box.Bottom - pad - LogicalToDeviceUnits(10);
                int barWidth = box.Width - 2 * pad - LogicalToDeviceUnits(56);
                var bar = new Rectangle(box.X + pad, barY, barWidth, LogicalToDeviceUnits(10));
                g.DrawRectangle(frame, bar);
                g.FillRectangle(text, bar.X + 2, bar.Y + 2, (int)((bar.Width - 3) * Progress), bar.Height - 3);
                g.DrawString($"{Progress * 100:0}%", _smallFont, text, bar.Right + LogicalToDeviceUnits(10), bar.Y - LogicalToDeviceUnits(4));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
                _titleFont.Dispose();
                _lineFont.Dispose();
                _smallFont.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
