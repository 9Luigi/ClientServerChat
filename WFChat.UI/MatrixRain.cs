using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace WFChat.UI
{
    /// <summary>
    /// The "digital rain" from The Matrix: columns of mirrored green glyphs falling down, each leaving a fading trail.
    /// Can be used as a background: child controls placed on it are drawn on top.
    /// </summary>
    /// <remarks>
    /// Frames are drawn into an off-screen bitmap that is kept between ticks. Every tick the bitmap is darkened a little
    /// (that is what makes the trails fade) and one new glyph is drawn at the head of each column.
    /// The animation runs only while the control is visible.
    /// </remarks>
    public class MatrixRain : Control
    {
        //half-width katakana, digits and a few symbols, as in the film; latin fallback when no Japanese font is installed
        const string KatakanaGlyphs = "ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝ0123456789:.=*+-<>|";
        const string FallbackGlyphs = "0123456789ABCDEFZ:.=*+-<>|";

        readonly Timer _timer = new Timer();
        readonly Random _random = new Random();
        readonly SolidBrush _headBrush = new SolidBrush(Theme.RainHead);
        readonly SolidBrush _trailBrush = new SolidBrush(Theme.RainGreen);
        SolidBrush _fadeBrush;
        string _glyphs;
        Font _glyphFont;
        Bitmap _canvas;
        int _cell;      //cell size in pixels on this screen
        int[] _heads;   //row of each column's head; negative = still waiting above the top edge
        int _trailFade;

        public MatrixRain()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            BackColor = Color.Black;
            Interval = 50;
            TrailFade = 26;
            _timer.Tick += (sender, e) => Step();
        }

        /// <summary>Milliseconds between frames: smaller is faster.</summary>
        [DefaultValue(50)]
        public int Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        /// <summary>How much the picture darkens per frame, 1–255: larger makes shorter trails.</summary>
        [DefaultValue(26)]
        public int TrailFade
        {
            get => _trailFade;
            set
            {
                _trailFade = Math.Clamp(value, 1, 255);
                _fadeBrush?.Dispose();
                _fadeBrush = new SolidBrush(Color.FromArgb(_trailFade, Color.Black));
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _cell = LogicalToDeviceUnits(16);
            _glyphs = PickGlyphs(out string family);
            _glyphFont?.Dispose();
            _glyphFont = new Font(family, _cell * 0.8f, FontStyle.Regular, GraphicsUnit.Pixel);
            ResetCanvas();
            UpdateTimer();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _timer.Stop();
            base.OnHandleDestroyed(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            UpdateTimer();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ResetCanvas();
        }

        void UpdateTimer()
        {
            _timer.Enabled = IsHandleCreated && Visible && !DesignMode;
        }

        /// <summary>A new black bitmap of the control's size; every column starts at a random height above the top.</summary>
        void ResetCanvas()
        {
            if (_cell == 0 || Width <= 0 || Height <= 0)
            {
                return;
            }
            _canvas?.Dispose();
            _canvas = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(_canvas))
            {
                g.Clear(Color.Black);
            }
            int rows = Height / _cell + 1;
            _heads = Enumerable.Range(0, Width / _cell + 1).Select(_ => -_random.Next(0, rows)).ToArray();
        }

        /// <summary>One frame: darken everything, turn the old heads green, draw new heads one row lower.</summary>
        void Step()
        {
            if (_canvas == null)
            {
                return;
            }
            int rows = _canvas.Height / _cell + 1;
            using (Graphics g = Graphics.FromImage(_canvas))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.FillRectangle(_fadeBrush, 0, 0, _canvas.Width, _canvas.Height);
                for (int column = 0; column < _heads.Length; column++)
                {
                    int row = _heads[column];
                    if (row > 0)
                    {
                        DrawGlyph(g, column, row - 1, _trailBrush);
                    }
                    if (row >= 0)
                    {
                        DrawGlyph(g, column, row, _headBrush);
                    }
                    //past the bottom: start again from the top now and then, so columns don't fall in step
                    _heads[column] = row > rows && _random.Next(100) < 4 ? -_random.Next(0, rows / 2) : row + 1;
                }
            }
            Invalidate();
        }

        void DrawGlyph(Graphics g, int column, int row, Brush brush)
        {
            int x = column * _cell, y = row * _cell;
            g.FillRectangle(Brushes.Black, x, y, _cell, _cell); //clear the cell, so the old glyph doesn't show through
            string glyph = _glyphs[_random.Next(_glyphs.Length)].ToString();
            g.TranslateTransform(x + _cell, y); //the film shows the glyphs mirrored
            g.ScaleTransform(-1, 1);
            g.DrawString(glyph, _glyphFont, brush, 0, 0);
            g.ResetTransform();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_canvas != null)
            {
                e.Graphics.DrawImageUnscaled(_canvas, 0, 0);
            }
            base.OnPaint(e); //raises Paint, so an overlay (the splash text) is drawn on top of the rain
        }

        /// <summary>Katakana need a Japanese font; Windows usually has one of these.</summary>
        static string PickGlyphs(out string family)
        {
            using var installed = new InstalledFontCollection();
            string japanese = new[] { "MS Gothic", "Yu Gothic", "Meiryo" }
                .FirstOrDefault(name => installed.Families.Any(f => f.Name == name));
            family = japanese ?? Theme.MonoFamily;
            return japanese != null ? KatakanaGlyphs : FallbackGlyphs;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
                _canvas?.Dispose();
                _glyphFont?.Dispose();
                _fadeBrush?.Dispose();
                _headBrush.Dispose();
                _trailBrush.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
