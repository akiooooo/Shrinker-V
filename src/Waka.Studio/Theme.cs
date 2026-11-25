using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Waka.Studio
{
    internal static class Dk
    {
        public static readonly Color Bg0  = Color.FromArgb(15, 17, 24);
        public static readonly Color Bg1  = Color.FromArgb(23, 26, 36);
        public static readonly Color Bg2  = Color.FromArgb(30, 34, 46);
        public static readonly Color Bg3  = Color.FromArgb(36, 40, 54);
        public static readonly Color Acc  = Color.FromArgb(174, 156, 255);
        public static readonly Color AccH = Color.FromArgb(196, 181, 255);
        public static readonly Color AccD = Color.FromArgb(140, 120, 222);
        public static readonly Color Warn = Color.FromArgb(220, 160,  30);
        public static readonly Color Err  = Color.FromArgb(220,  60,  60);
        public static readonly Color Txt  = Color.FromArgb(220, 228, 240);
        public static readonly Color TxtM = Color.FromArgb(130, 145, 165);
        public static readonly Color TxtD = Color.FromArgb( 70,  82,  98);
        public static readonly Color Bdr  = Color.FromArgb( 38,  50,  65);

        public static Font FontBody(float size = 10f, FontStyle s = FontStyle.Regular)
        {
            foreach (var n in new[] { "Segoe UI Variable Text", "Segoe UI", "Arial" })
            {
                Font f = null;
                try { f = new Font(n, size, s, GraphicsUnit.Point); } catch { }
                if (f != null) return f;
            }
            return new Font(SystemFonts.MessageBoxFont.FontFamily, size, s, GraphicsUnit.Point);
        }

        public static Font FontMono(float size = 9.5f, FontStyle s = FontStyle.Regular)
        {
            foreach (var n in new[] { "Cascadia Mono", "Consolas", "Courier New" })
            {
                Font f = null;
                try { f = new Font(n, size, s, GraphicsUnit.Point); } catch { }
                if (f != null) return f;
            }
            return new Font(FontFamily.GenericMonospace, size, s, GraphicsUnit.Point);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  CUSTOM CONTROLS
    // ─────────────────────────────────────────────────────────────────────────────
    internal sealed class DarkButton : Button
    {
        private bool _hover, _down;
        public bool Accent { get; set; }
        public Color FillColor { get; set; }

        public DarkButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor  = Dk.Txt;
            Font       = Dk.FontBody(10f);
            FillColor  = Dk.Bg3;
            Cursor     = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true;  Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _down = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _down = true;  Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e)   { _down = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            // Clear corners with parent background so rounded edges look clean
            Color parentBg = Parent != null ? Parent.BackColor : Dk.Bg0;
            g.Clear(parentBg);
            var rc = new Rectangle(0, 0, Width - 1, Height - 1);
            Color bg;
            if (Accent)
                bg = _down ? Dk.AccD : (_hover ? Dk.AccH : Dk.Acc);
            else
                bg = _down ? Dk.Bg2 : (_hover ? Lighten(FillColor, 12) : FillColor);

            using (var path = RoundRect(rc, 7))
            using (var fill = new SolidBrush(bg))
                g.FillPath(fill, path);

            Color fg = Accent ? Color.FromArgb(10, 15, 10) : ForeColor;
            TextRenderer.DrawText(g, Text, Font, rc, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine);
        }

        private static Color Lighten(Color c, int a)
        {
            return Color.FromArgb(c.A, Math.Min(255, c.R + a), Math.Min(255, c.G + a), Math.Min(255, c.B + a));
        }

        private static GraphicsPath RoundRect(Rectangle r, int rad)
        {
            var p = new GraphicsPath(); int d = rad * 2;
            p.AddArc(r.X,        r.Y,        d, d, 180, 90);
            p.AddArc(r.Right-d,  r.Y,        d, d, 270, 90);
            p.AddArc(r.Right-d,  r.Bottom-d, d, d,   0, 90);
            p.AddArc(r.X,        r.Bottom-d, d, d,  90, 90);
            p.CloseFigure(); return p;
        }
    }

    internal sealed class DarkToggle : CheckBox
    {
        public DarkToggle()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            Appearance = Appearance.Button;
            FlatStyle  = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            ForeColor  = Dk.Txt;
            Font       = Dk.FontBody(9.5f);
            Cursor     = Cursors.Hand;
            TextAlign  = ContentAlignment.MiddleCenter;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Parent != null && Parent.BackColor.A != 0 ? Parent.BackColor : Dk.Bg1);
            bool segment = Text.StartsWith("×") || Text == "MAX";
            Color foreground = Enabled ? Dk.Txt : Dk.TxtD;
            if (segment) {
                var rect = new Rectangle(0,0,Width-1,Height-1);
                using(var path = Pill(rect,8)) {
                    using(var fill = new SolidBrush(Checked && Enabled ? Color.FromArgb(65,55,94) : Dk.Bg3)) g.FillPath(fill,path);
                    using(var border = new Pen(Checked && Enabled ? Dk.Acc : Dk.Bdr,1)) g.DrawPath(border,path);
                }
                TextRenderer.DrawText(g,Text,Font,rect,Checked && Enabled ? Dk.AccH : foreground,TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter);
            } else {
                var track = new Rectangle(Width-42,(Height-22)/2,40,22);
                using(var path = Pill(track,11)) using(var fill = new SolidBrush(Checked && Enabled ? Dk.Acc : Dk.Bg3)) g.FillPath(fill,path);
                int x = Checked ? track.Right-19 : track.Left+3;
                using(var knob = new SolidBrush(!Enabled ? Dk.TxtD : Checked ? Dk.Bg0 : Dk.TxtM)) g.FillEllipse(knob,x,track.Top+3,16,16);
                TextRenderer.DrawText(g,Text,Font,new Rectangle(0,0,Width-54,Height),foreground,TextFormatFlags.Left|TextFormatFlags.VerticalCenter|TextFormatFlags.EndEllipsis);
            }
            if(Focused) ControlPaint.DrawFocusRectangle(g,new Rectangle(2,2,Width-5,Height-5),Dk.TxtM,BackColor);
        }
        private static GraphicsPath Pill(Rectangle r, int radius)
        {
            int d=radius*2; var p=new GraphicsPath();
            p.AddArc(r.X,r.Y,d,d,180,90); p.AddArc(r.Right-d,r.Y,d,d,270,90);
            p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90); p.AddArc(r.X,r.Bottom-d,d,d,90,90); p.CloseFigure(); return p;
        }

    }

    internal sealed class DarkTextBox : TextBox
    {
        public DarkTextBox()
        {
            BackColor   = Dk.Bg3;
            ForeColor   = Dk.Txt;
            BorderStyle = BorderStyle.None;
            Font        = Dk.FontBody(10f);
        }
    }

    internal sealed class DarkLogBox : RichTextBox
    {
        // .NET 4.0 не поддерживает tuples — используем простую пару
        private struct LogEntry { public string Msg; public Color Col; }

        // Буфер строк — накапливаем здесь, сбрасываем батчем через _timer
        private readonly Queue<LogEntry> _queue = new Queue<LogEntry>();
        private readonly System.Windows.Forms.Timer _timer;
        private const int MAX_CHARS = 300000; // Порог обрезки

        // Защита от принудительного скролла: если юзер мотнул вверх — не прыгаем вниз
        private bool _userScrolledUp = false;

        public DarkLogBox()
        {
            BackColor   = Dk.Bg0;
            ForeColor   = Dk.Acc;
            ReadOnly    = true;
            BorderStyle = BorderStyle.None;
            Font        = Dk.FontMono(9f);
            ScrollBars  = RichTextBoxScrollBars.Vertical;
            WordWrap    = false;

            _timer = new System.Windows.Forms.Timer { Interval = 80 };
            _timer.Tick += FlushQueue;
            _timer.Start();
        }

        // Вызывается из любого потока — просто кладём в очередь
        public void EnqueueLine(string msg, Color color)
        {
            lock (_queue) _queue.Enqueue(new LogEntry { Msg = msg, Col = color });
        }

        // AppendColored (old API) теперь тоже идёт через очередь
        public void AppendColored(string msg, Color color)
        {
            EnqueueLine(msg, color);
        }

        // Перехватываем скролл мышью / клавишами
        protected override void WndProc(ref Message m)
        {
            const int WM_VSCROLL   = 0x0115;
            const int WM_MOUSEWHEEL = 0x020A;
            const int SB_BOTTOM    = 7;

            base.WndProc(ref m);

            if (m.Msg == WM_MOUSEWHEEL)
            {
                // delta > 0 → скролл вверх
                int delta = (short)(m.WParam.ToInt64() >> 16);
                if (delta > 0)
                    _userScrolledUp = true;
                else
                    _userScrolledUp = !IsScrolledToBottom();
            }
            else if (m.Msg == WM_VSCROLL)
            {
                int cmd = m.WParam.ToInt32() & 0xFFFF;
                if (cmd == SB_BOTTOM)
                    _userScrolledUp = false;
                else
                    _userScrolledUp = !IsScrolledToBottom();
            }
        }

        // Вызывается таймером в UI-потоке — батч-сброс
        private void FlushQueue(object sender, EventArgs e)
        {
            LogEntry[] batch;
            lock (_queue)
            {
                if (_queue.Count == 0) return;
                batch = _queue.ToArray();
                _queue.Clear();
            }

            // Пересчитываем «снизу ли» только если юзер явно не мотал вверх
            bool wasAtBottom = !_userScrolledUp && IsScrolledToBottom();
            SuspendPaint();
            try
            {
                // Обрезаем начало если накопилось слишком много
                if (TextLength + batch.Length * 80 > MAX_CHARS)
                {
                    int trim = Math.Max(0, TextLength - MAX_CHARS / 2);
                    if (trim > 0) { Select(0, trim); SelectedText = ""; }
                }
                SelectionStart  = TextLength;
                SelectionLength = 0;
                foreach (var entry in batch)
                {
                    SelectionStart  = TextLength;
                    SelectionLength = 0;
                    SelectionColor  = entry.Col;
                    AppendText(entry.Msg + "\n");
                }
                SelectionColor = Dk.Acc;
            }
            finally
            {
                ResumePaint();
            }

            if (wasAtBottom) ScrollToBottom();
        }

        private bool IsScrolledToBottom()
        {
            try { return TextLength > 0 && GetLineFromCharIndex(TextLength) == GetLineFromCharIndex(GetCharIndexFromPosition(new Point(0, Height - 5))); }
            catch { return true; }
        }

        private void ScrollToBottom()
        {
            try { SelectionStart = TextLength; ScrollToCaret(); } catch { }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, int msg, bool wp, int lp);
        private const int WM_SETREDRAW = 0x000B;

        private void SuspendPaint()  { try { SendMessage(Handle, WM_SETREDRAW, false, 0); } catch { } }
        private void ResumePaint()   { try { SendMessage(Handle, WM_SETREDRAW, true,  0); Invalidate(); } catch { } }
    }
}
