using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ShrinkerV.Core;

namespace ShrinkerV.Studio
{
    internal sealed class MainForm : Form
    {
        private DarkTextBox  _inputBox, _outputBox;
        private DarkButton   _inputBrowse, _outputBrowse;
        private DarkToggle   _f2, _f8, _f32, _fMax;
        private DarkToggle   _grayscale, _skipWeapons;
        private DarkToggle   _clothingF2, _clothingF8, _clothingF32, _clothingFMax, _clothingGrayscale;
        private DarkToggle   _pedsF2, _pedsF8, _pedsF32, _pedsFMax, _pedsGrayscale;
        private DarkToggle   _removeMap;
        private DarkToggle _protectCharacters, _keepAudio, _keepVideo, _keepEffects, _verbose;
        private FlowLayoutPanel _settings;
        private DarkButton _analyzeBtn;
        private Label _summary;
        private NumericUpDown _threads, _minSize;
        private readonly Stopwatch _elapsed = new Stopwatch();
        private readonly System.Windows.Forms.Timer _clock = new System.Windows.Forms.Timer();
        private DarkButton   _startBtn;
        private Label        _statusLabel;
        private Panel        _progressTrack, _progressFill;
        private DarkLogBox   _log;
        private CancellationTokenSource _cts;
        private bool         _busy;
        private int _totalArchives;

        // Stats panel labels (6 cells in 2×3 grid)
        private Label _stRpf, _stTexShrunken, _stTexSkipped;
        private Label _stErrors, _stSize, _stSaved;

        public MainForm()
        {
            Text          = "Shrinker V by akiooo";
            Icon = Icon.ExtractAssociatedIcon(typeof(MainForm).Assembly.Location);
            MinimumSize   = new Size(1020, 720);
            Size          = new Size(1320, 920);
            BackColor     = Dk.Bg0;
            Font          = Dk.FontBody(10f);
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            _clock.Interval = 1000;
            _clock.Tick += (s, e) => { if (_busy) _summary.Text = "В работе  •  " + _elapsed.Elapsed.ToString(@"hh\:mm\:ss") + "  •  Остановка завершает текущий архив"; };
            _clock.Start();
            FormClosed += (s, e) => _clock.Dispose();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_busy)
            {
                e.Cancel = true;
                MessageBox.Show(
                    "Обработка в процессе.\nНажмите «Стоп» для остановки перед закрытием.",
                    "Shrinker V by akiooo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            base.OnFormClosing(e);
        }

        // ── UI BUILD ─────────────────────────────────────────────────────────────
        private Control _pathSettings;
        private FlowLayoutPanel _advancedSettings;
        private DarkToggle _compressPeds;
        private Label _protectionNote;

        private static Label StudioText(string text, float size, Color color, int height)
        {
            return new Label { Text = text, Font = Dk.FontBody(size), ForeColor = color,
                Dock = DockStyle.Top, Height = height, BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft, AutoEllipsis = true };
        }

        private static Panel Card(string title, string description, int height)
        {
            var card = new Panel { BackColor = Dk.Bg1, Width = 460, Height = height, Padding = new Padding(18, 12, 18, 12), Margin = new Padding(0, 0, 0, 12) };
            card.Controls.Add(StudioText(description, 9f, Dk.TxtM, 26));
            card.Controls.Add(StudioText(title, 13f, Dk.Txt, 24));
            return card;
        }

        private static FlowLayoutPanel Segments(params DarkToggle[] toggles)
        {
            var row = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, BackColor = Dk.Bg1 };
            foreach (var toggle in toggles) {
                toggle.Width = 76; toggle.Height = 34; toggle.Margin = new Padding(0, 0, 8, 0);
                toggle.AutoCheck = false;
                var selected = toggle;
                selected.Click += (s, e) => selected.Checked = true;
                selected.CheckedChanged += (s, e) => { if (selected.Checked) foreach (var other in toggles) if (other != selected) other.Checked = false; };
                row.Controls.Add(toggle);
            }
            return row;
        }

        private static void AddCardControls(Panel card, params Control[] controls)
        {
            // Dock ordering: title, description, then supplied controls.
            foreach (var c in controls) { c.Dock = DockStyle.Top; card.Controls.Add(c); c.BringToFront(); }
            card.Height = card.Padding.Vertical + 50 + controls.Sum(c => c.Height) + 4;
        }

        private static void ResponsiveCards(FlowLayoutPanel panel)
        {
            panel.SizeChanged += (s, e) => {
                int width = Math.Max(280, panel.ClientSize.Width - panel.Padding.Horizontal - 20);
                foreach (Control card in panel.Controls) card.Width = width;
            };
        }

        private void BuildUI()
        {
            SuspendLayout();
            var shell = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Dk.Bg0 };
            shell.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
            shell.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var top = new Panel { Dock = DockStyle.Fill, BackColor = Dk.Bg1, Padding = new Padding(24,12,24,12), Margin = Padding.Empty };
            var brand = new Label { Text = "Shrinker V by akiooo", Dock = DockStyle.Left, Width = 224,
                ForeColor = Dk.Txt, TextAlign = ContentAlignment.MiddleLeft, Font = Dk.FontBody(15,FontStyle.Bold) };
            var logo = new PictureBox { Dock = DockStyle.Left, Width = 48, SizeMode = PictureBoxSizeMode.Zoom, Margin = Padding.Empty };
            using (var stream = typeof(MainForm).Assembly.GetManifestResourceStream("ShrinkerV.Logo.png"))
            using (var image = Image.FromStream(stream)) logo.Image = new Bitmap(image);
            logo.Disposed += (s,e) => logo.Image.Dispose();
            var nav = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 438, WrapContents = false };
            var navMain = new DarkButton { Text = "Сжатие", Width = 132, Height = 40, Accent = true };
            var navAdvanced = new DarkButton { Text = "Параметры", Width = 140, Height = 40 };
            var navLog = new DarkButton { Text = "Журнал", Width = 132, Height = 40 };
            nav.Controls.AddRange(new Control[] { navMain, navAdvanced, navLog });
            top.Controls.Add(nav); top.Controls.Add(brand); top.Controls.Add(logo);
            shell.Controls.Add(top,0,0);
            var workspace = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(24, 8, 24, 12), Margin = new Padding(0), BackColor = Dk.Bg0 };
            workspace.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));
            workspace.RowStyles.Add(new RowStyle(SizeType.Absolute, 138));
            workspace.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            workspace.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
            var header = new Panel { Dock = DockStyle.Fill };
            var headline = StudioText("Сжатие текстур GTA V", 22, Dk.Txt, 42); headline.Font = Dk.FontBody(22, FontStyle.Bold);
            header.Controls.Add(StudioText("Перетащите папки в поля ниже или выберите их через обзор.", 10, Dk.TxtM, 30));
            header.Controls.Add(headline); workspace.Controls.Add(header,0,0);

            var paths = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = Dk.Bg1, Padding = new Padding(18,12,18,12), ColumnCount = 2, RowCount = 2, Margin = new Padding(0,0,0,14) };
            paths.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50)); paths.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));
            paths.RowStyles.Add(new RowStyle(SizeType.Absolute,28)); paths.RowStyles.Add(new RowStyle(SizeType.Percent,100));
            paths.Controls.Add(StudioText("Исходная папка GTA V или dlcpacks", 9, Dk.TxtM,28),0,0);
            paths.Controls.Add(StudioText("Папка результата",9,Dk.TxtM,28),1,0);
            paths.Controls.Add(StudioPath(ref _inputBox,ref _inputBrowse),0,1);
            paths.Controls.Add(StudioPath(ref _outputBox,ref _outputBrowse),1,1);
            _inputBrowse.Click += (s,e)=>Browse(_inputBox); _outputBrowse.Click += (s,e)=>Browse(_outputBox);
            _pathSettings = paths; workspace.Controls.Add(paths,0,1);

            var content = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0) };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,55)); content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,45));
            var settingsHost = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0,0,18,0) };
            _settings = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, BackColor = Dk.Bg0, Margin = new Padding(0) };
            _advancedSettings = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, BackColor = Dk.Bg0, Visible = false };
            ResponsiveCards(_settings); ResponsiveCards(_advancedSettings);

            var quality = Card("Окружение", "Разрешение текстур мира. ×8 уменьшает обе стороны в 8 раз.", 206);
            var presets = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 34 };
            foreach (string name in new[] { "Бережный", "Баланс", "Сильный" }) {
                var b = new DarkButton { Text = name, Width = 108, Height = 30, Font = Dk.FontBody(9), Margin = new Padding(0,0,8,0) };
                string choice = name; b.Click += (s,e)=>ApplyPreset(choice); presets.Controls.Add(b);
            }
            _f2 = Toggle("×2",76,34); _f8 = Toggle("×8",76,34); _f32 = Toggle("×32",76,34); _fMax = Toggle("MAX",76,34); _f8.Checked = true;
            _grayscale = Toggle("Чёрно-белые текстуры мира",380,30);
            AddCardControls(quality,presets,Segments(_f2,_f8,_f32,_fMax),_grayscale);
            _settings.Controls.Add(quality);

            var player = Card("Ваш персонаж", "GTA Online + Майкл, Франклин и Тревор", 150);
            _protectCharacters = Toggle("Сохранять игрока без изменений",380,32); _protectCharacters.Checked = true;
            _protectionNote = StudioText("Кожа, одежда, тату, волосы и аксессуары защищены.",9,Dk.Acc,26);
            AddCardControls(player,_protectCharacters,_protectionNote); _settings.Controls.Add(player);

            var npc = Card("Педы и NPC", "Прохожие, сюжетные NPC и отдельные модели педов.", 194);
            _compressPeds = Toggle("Сжимать NPC отдельно от игрока",380,32); _compressPeds.Checked = true;
            _pedsF2 = Toggle("×2",76,34); _pedsF8 = Toggle("×8",76,34); _pedsF32 = Toggle("×32",76,34); _pedsFMax = Toggle("MAX",76,34); _pedsF8.Checked = true;
            _pedsGrayscale = Toggle("Чёрно-белые текстуры NPC",380,30);
            AddCardControls(npc,_compressPeds,Segments(_pedsF2,_pedsF8,_pedsF32,_pedsFMax),_pedsGrayscale); _settings.Controls.Add(npc);

            var clothing = Card("Игрок / ручная настройка", "Доступно, только если защита игрока выключена.", 156);
            _clothingF2 = Toggle("×2",76,34); _clothingF8 = Toggle("×8",76,34); _clothingF32 = Toggle("×32",76,34); _clothingFMax = Toggle("MAX",76,34); _clothingF8.Checked = true;
            _clothingGrayscale = Toggle("Чёрно-белые текстуры игрока",380,34);
            AddCardControls(clothing,Segments(_clothingF2,_clothingF8,_clothingF32,_clothingFMax),_clothingGrayscale); _advancedSettings.Controls.Add(clothing);
            var media = Card("Содержимое игры", "Включено — исходные ресурсы сохраняются.",216);
            _keepAudio = Toggle("Сохранить звуки / AWC",380,32); _keepAudio.Checked = true;
            _keepVideo = Toggle("Сохранить видео / BIK",380,32); _keepVideo.Checked = true;
            _keepEffects = Toggle("Сохранить эффекты / YPT",380,32); _keepEffects.Checked = true;
            _skipWeapons = Toggle("Сохранить текстуры оружия",380,32); _skipWeapons.Checked = true;
            AddCardControls(media,_keepAudio,_keepVideo,_keepEffects,_skipWeapons); _advancedSettings.Controls.Add(media);
            var performance = Card("Производительность", "Потоки CPU                             Минимум текстуры, px",170);
            var limits = new FlowLayoutPanel { Height = 36, Dock = DockStyle.Top };
            _threads = new NumericUpDown { Minimum = 1, Maximum = Math.Max(1,Environment.ProcessorCount), Value = Math.Max(1,Environment.ProcessorCount/2), Width = 155, BackColor = Dk.Bg3, ForeColor = Dk.Txt, BorderStyle = BorderStyle.FixedSingle };
            _minSize = new NumericUpDown { Minimum = 4, Maximum = 256, Increment = 4, Value = 32, Width = 155, BackColor = Dk.Bg3, ForeColor = Dk.Txt, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(16,3,0,0) };
            limits.Controls.AddRange(new Control[]{_threads,_minSize});
            _verbose = Toggle("Подробный журнал текстур",380,34);
            AddCardControls(performance,limits,_verbose); _advancedSettings.Controls.Add(performance);
            var experimental = Card("Удаление карты", "Заменяет ресурсы мира заглушками. Меняет вид игры.",132);
            _removeMap = Toggle("Вырезать карту",380,34); AddCardControls(experimental,_removeMap); _advancedSettings.Controls.Add(experimental);
            _protectCharacters.CheckedChanged += (s,e)=>UpdateProtectionControls();
            _compressPeds.CheckedChanged += (s,e)=>UpdateProtectionControls(); UpdateProtectionControls();
            settingsHost.Controls.Add(_advancedSettings); settingsHost.Controls.Add(_settings); content.Controls.Add(settingsHost,0,0);

            var monitor = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Margin = new Padding(0) };
            monitor.RowStyles.Add(new RowStyle(SizeType.Absolute,34)); monitor.RowStyles.Add(new RowStyle(SizeType.Absolute,42));
            monitor.RowStyles.Add(new RowStyle(SizeType.Absolute,198)); monitor.RowStyles.Add(new RowStyle(SizeType.Absolute,42)); monitor.RowStyles.Add(new RowStyle(SizeType.Percent,100));
            _statusLabel = StudioText("Готов к работе",14,Dk.Txt,34); monitor.Controls.Add(_statusLabel,0,0);
            var progressArea = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0,15,0,20) };
            _progressTrack = new Panel { Dock = DockStyle.Fill, BackColor = Dk.Bg3 }; _progressFill = new Panel { BackColor = Dk.Acc, Width = 0, Dock = DockStyle.Left };
            _progressTrack.Controls.Add(_progressFill); progressArea.Controls.Add(_progressTrack); monitor.Controls.Add(progressArea,0,1);
            var stats = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Margin = new Padding(0) };
            stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50)); stats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,50));
            for(int i=0;i<3;i++) stats.RowStyles.Add(new RowStyle(SizeType.Percent,33.333f));
            _stRpf=StatTile("АРХИВЫ", "0 / 0"); _stTexShrunken=StatTile("ТЕКСТУР СЖАТО","0"); _stTexSkipped=StatTile("ПРОПУЩЕНО ТЕКСТУР","0"); _stErrors=StatTile("ОШИБКИ","0"); _stSize=StatTile("ДО → ПОСЛЕ","—"); _stSaved=StatTile("ОСВОБОЖДЕНО","—");
            var values = new[] {_stRpf,_stTexShrunken,_stTexSkipped,_stErrors,_stSize,_stSaved};
            for(int i=0;i<values.Length;i++) stats.Controls.Add(StudioStat(values[i]),i%2,i/2);
            monitor.Controls.Add(stats,0,2);
            var logBar = new Panel { Dock = DockStyle.Fill };
            var query = new DarkTextBox { Dock = DockStyle.Fill, AccessibleName = "Найти в журнале" };
            var find = new DarkButton { Text = "Найти", Width = 76, Dock = DockStyle.Right };
            Action search = () => {
                int index = NextMatch(_log.Text, query.Text, _log.SelectionStart + _log.SelectionLength);
                if (index >= 0) { _log.Select(index, query.Text.Length); _log.ScrollToCaret(); _log.Focus(); }
                else if (query.Text.Length > 0) _summary.Text = "В журнале совпадений нет";
            };
            find.Click += (s,e) => search();
            query.KeyDown += (s,e) => { if(e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; search(); } };
            logBar.Controls.Add(query); logBar.Controls.Add(find);
            var export = new DarkButton { Text = "Экспорт", Width = 88, Dock = DockStyle.Right, FillColor = Dk.Bg0, Font = Dk.FontBody(9) };
            export.Click += (s,e)=> { using(var d = new SaveFileDialog { Filter="Текстовый журнал|*.txt",FileName="Shrinker V by akiooo - session.txt" }) if(d.ShowDialog(this)==DialogResult.OK) try { File.WriteAllText(d.FileName,_log.Text,Encoding.UTF8); } catch(Exception ex) { MessageBox.Show(this,ex.Message); } };
            logBar.Controls.Add(export); export.SendToBack(); monitor.Controls.Add(logBar,0,3);
            _log = new DarkLogBox { Dock = DockStyle.Fill, BackColor = Dk.Bg1, BorderStyle = BorderStyle.None, Font = Dk.FontMono(9), WordWrap = true }; monitor.Controls.Add(_log,0,4);
            content.Controls.Add(monitor,1,0); workspace.Controls.Add(content,0,2);

            Action<int> navigate = page => {
                _settings.Visible = page == 0; _advancedSettings.Visible = page == 1; settingsHost.Visible = page != 2;
                content.ColumnStyles[0].Width = page == 2 ? 0 : 55; content.ColumnStyles[1].Width = page == 2 ? 100 : 45;
                navMain.Accent=page==0; navAdvanced.Accent=page==1; navLog.Accent=page==2;
                navMain.Invalidate(); navAdvanced.Invalidate(); navLog.Invalidate();
            };
            navMain.Click += (s,e)=>navigate(0); navAdvanced.Click += (s,e)=>navigate(1); navLog.Click += (s,e)=>navigate(2);
            var footer = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2, Margin = new Padding(0), Padding = new Padding(0,10,0,0) };
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100)); footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,156)); footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,186));
            footer.RowStyles.Add(new RowStyle(SizeType.Percent,100)); footer.RowStyles.Add(new RowStyle(SizeType.Absolute,22));
            _analyzeBtn = new DarkButton { Text="Проверить папку", Dock=DockStyle.Fill, Font=Dk.FontBody(9) }; _analyzeBtn.Click += AnalyzeClick;
            _startBtn = new DarkButton { Text="Начать сжатие  →", Accent=true, Dock=DockStyle.Fill, Font=Dk.FontBody(11,FontStyle.Bold) }; _startBtn.Click += StartClick;
            _summary=StudioText("Игрок защищён  ·  NPC сжимаются отдельно",9,Dk.TxtM,32); _summary.Dock=DockStyle.Fill;
            footer.Controls.Add(_summary,0,0); footer.Controls.Add(_analyzeBtn,1,0); footer.Controls.Add(_startBtn,2,0);
            var openOutput = new LinkLabel { Text="Открыть результат", Dock=DockStyle.Fill, LinkColor=Dk.Acc, ActiveLinkColor=Dk.AccH, VisitedLinkColor=Dk.Acc, Font=Dk.FontBody(8.5f), TextAlign=ContentAlignment.MiddleLeft };
            openOutput.LinkClicked += (s,e)=> { string p=_outputBox.Text.Trim(); if(Directory.Exists(p)) Process.Start("explorer.exe","\""+p+"\""); else Log("Укажите существующую папку результата.",LogLevel.Warn); };
            footer.Controls.Add(openOutput,0,1); workspace.Controls.Add(footer,0,3);
            shell.Controls.Add(workspace,0,1); Controls.Add(shell);
            _log.AppendColored("  Shrinker V by akiooo\n\n  Выберите источник и папку результата.\n  NPC можно сжимать независимо от игрока.\n\n  MP и три героя одиночной игры защищены.\n  Выход содержит RPF, а не полную копию игры.",Dk.TxtM);
            ResumeLayout(true);
        }

        private static Panel StudioPath(ref DarkTextBox box, ref DarkButton button)
        {
            var panel = new Panel { Dock=DockStyle.Fill, BackColor=Dk.Bg3, Margin=new Padding(0,5,12,5), Padding=new Padding(12,12,7,8) };
            button = new DarkButton { Text="···", Width=36, Dock=DockStyle.Right, FillColor=Dk.Bg3, Font=Dk.FontBody(15) };
            box = new DarkTextBox { Dock=DockStyle.Fill, Font=Dk.FontBody(10) };
            var field = box;
            field.AllowDrop = true;
            field.DragEnter += (s,e) => e.Effect = GetDroppedFolder(e.Data) == null ? DragDropEffects.None : DragDropEffects.Copy;
            field.DragDrop += (s,e) => { string folder = GetDroppedFolder(e.Data); if (folder != null) field.Text = folder; };
            panel.Controls.Add(box); panel.Controls.Add(button); return panel;
        }

        internal static string GetDroppedFolder(IDataObject data)
        {
            var paths = data == null ? null : data.GetData(DataFormats.FileDrop) as string[];
            return paths != null && paths.Length == 1 && Directory.Exists(paths[0]) ? paths[0] : null;
        }

        internal static int NextMatch(string text, string query, int start)
        {
            if (String.IsNullOrEmpty(query)) return -1;
            int index = text.IndexOf(query, Math.Min(text.Length, Math.Max(0,start)), StringComparison.OrdinalIgnoreCase);
            return index >= 0 ? index : text.IndexOf(query, StringComparison.OrdinalIgnoreCase);
        }

        private static Panel StudioStat(Label value)
        {
            var panel = new Panel { Dock=DockStyle.Fill, BackColor=Dk.Bg1, Margin=new Padding(0,0,8,8), Padding=new Padding(14,8,8,8) };
            value.Font=Dk.FontBody(15,FontStyle.Bold); value.TextAlign=ContentAlignment.MiddleLeft; value.Dock=DockStyle.Fill;
            var caption=StudioText(value.Tag as string,7.5f,Dk.TxtM,18);
            panel.Controls.Add(value); panel.Controls.Add(caption); return panel;
        }

        private void UpdateProtectionControls()
        {
            foreach (Control c in new Control[] { _clothingF2, _clothingF8, _clothingF32, _clothingFMax, _clothingGrayscale }) c.Enabled = !_protectCharacters.Checked;
            foreach (Control c in new Control[] { _pedsF2, _pedsF8, _pedsF32, _pedsFMax, _pedsGrayscale }) c.Enabled = _compressPeds.Checked;
            if (_protectionNote != null) _protectionNote.Text = _protectCharacters.Checked ? "Кожа, одежда, тату, волосы и аксессуары защищены." : "Защита выключена. Настройки игрока — в «Параметрах».";
        }
        private void ApplyPreset(string name)
        {
            _f2.Checked = name == "Бережный"; _f8.Checked = name == "Баланс"; _f32.Checked = name == "Сильный"; _fMax.Checked = false;
            _pedsF2.Checked = name == "Бережный"; _pedsF8.Checked = name == "Баланс"; _pedsF32.Checked = name == "Сильный"; _pedsFMax.Checked = false;
            _protectCharacters.Checked = true; _keepAudio.Checked = _keepVideo.Checked = _keepEffects.Checked = true;
            _grayscale.Checked = _removeMap.Checked = false; _skipWeapons.Checked = true;
            _compressPeds.Checked = true;
            _summary.Text = "Профиль: " + name + " • Игрок защищён, NPC сжимаются";
        }
        private async void AnalyzeClick(object sender, EventArgs e)
        {
            string path = _inputBox.Text.Trim();
            if (!Directory.Exists(path)) { Log("Укажите существующую входную папку.", LogLevel.Warn); return; }
            _analyzeBtn.Enabled = _startBtn.Enabled = false;
            try {
                var sizes = await Task.Run(() => Directory.EnumerateFiles(path, "*.rpf", SearchOption.AllDirectories).Select(p => new FileInfo(p).Length).ToArray());
                _summary.Text = "Найдено: " + sizes.Length + " RPF • " + FmtBytes(sizes.Sum()) + " • Проверена доступность файлов";
                Log(_summary.Text + ". Размер после сжатия станет известен после обработки.");
            } catch (Exception ex) { Log("Проверка: " + ex.Message, LogLevel.Error); }
            finally { _analyzeBtn.Enabled = _startBtn.Enabled = true; }
        }
        // ── UI HELPERS ────────────────────────────────────────────────────────────
        private static DarkToggle Toggle(string text, int w, int h)
        {
            return new DarkToggle { Text = text, Width = w, Height = h, Margin = new Padding(0, 0, 4, 2) };
        }

        // ── FOLDER PICKER (Vista+ проводник-стиль через SHBrowseForFolder) ──────────
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct BROWSEINFO
        {
            public IntPtr hwndOwner;
            public IntPtr pidlRoot;
            public string pszDisplayName;
            public string lpszTitle;
            public uint   ulFlags;
            public IntPtr lpfn;
            public IntPtr lParam;
            public int    iImage;
        }

        [DllImport("shell32.dll")]
        private static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern bool SHGetPathFromIDListW(IntPtr pidl, StringBuilder pszPath);

        [DllImport("ole32.dll")]
        private static extern void CoTaskMemFree(IntPtr pv);

        private void Browse(TextBox t)
        {
            const uint BIF_RETURNONLYFSDIRS = 0x0001;
            const uint BIF_NEWDIALOGSTYLE   = 0x0040; // Vista+ resizable Explorer picker

            var bi = new BROWSEINFO
            {
                hwndOwner    = Handle,
                lpszTitle    = "Выберите папку",
                ulFlags      = BIF_RETURNONLYFSDIRS | BIF_NEWDIALOGSTYLE,
            };

            IntPtr pidl = SHBrowseForFolder(ref bi);
            if (pidl == IntPtr.Zero) return;

            try
            {
                var sb = new StringBuilder(260);
                if (SHGetPathFromIDListW(pidl, sb))
                    t.Text = sb.ToString();
            }
            finally
            {
                CoTaskMemFree(pidl);
            }
        }

        private int Factor()
        {
            if (_f2.Checked)   return 2;
            if (_f32.Checked)  return 32;
            if (_fMax.Checked) return 256;
            return 8;
        }

        private int ClothingFactor()
        {
            if (_clothingF2.Checked)   return 2;
            if (_clothingF32.Checked)  return 32;
            if (_clothingFMax.Checked) return 256;
            return 8;
        }

        private int PedsFactor()
        {
            if (_pedsF2.Checked)   return 2;
            if (_pedsF32.Checked)  return 32;
            if (_pedsFMax.Checked) return 256;
            return 8;
        }

        // ── STATE ─────────────────────────────────────────────────────────────────
        private void SetBusy(bool busy)
        {
            if (InvokeRequired) { BeginInvoke(new Action<bool>(SetBusy), busy); return; }
            _busy = busy;
            _settings.Enabled = !busy;
            _advancedSettings.Enabled = _pathSettings.Enabled = _analyzeBtn.Enabled = !busy;
            if (busy) _elapsed.Restart(); else { _elapsed.Stop(); _summary.Text = "Время сеанса: " + _elapsed.Elapsed.ToString(@"hh\:mm\:ss"); }
            _startBtn.Text    = busy ? "Остановить" : "Начать сжатие  →";
            _startBtn.Accent  = !busy;
            _startBtn.FillColor = busy ? Color.FromArgb(160, 40, 40) : Dk.Bg3;
            _inputBrowse.Enabled = _outputBrowse.Enabled = !busy;
        }

        private void SetStatus(string text, Color? col = null)
        {
            if (InvokeRequired) { BeginInvoke(new Action<string, Color?>(SetStatus), text, col); return; }
            _statusLabel.Text      = text;
            _statusLabel.ForeColor = col ?? Dk.Acc;
        }

        private void SetProgress(int val, int max)
        {
            if (InvokeRequired) { BeginInvoke(new Action<int, int>(SetProgress), val, max); return; }
            if (max <= 0 || _progressTrack.Width <= 0) return;
            _progressFill.Width = Math.Max(0,
                Math.Min((int)((double)val / max * _progressTrack.Width), _progressTrack.Width));
        }

        private void SetStats(ProcessingStats st)
        {
            if (InvokeRequired) { BeginInvoke(new Action<ProcessingStats>(SetStats), st); return; }
            _stRpf.Text         = st.ArchivesProcessed + " / " + _totalArchives
                                  + (st.ArchivesModified > 0 ? "  (" + st.ArchivesModified + " изм.)" : "");
            _stTexShrunken.Text = st.TexturesShrunken.ToString("N0");
            _stTexSkipped.Text  = st.TexturesSkipped.ToString("N0");
            _stErrors.Text      = st.ErrorsEncountered == 0
                                  ? "—"
                                  : st.ErrorsEncountered.ToString();
            _stSize.Text        = FmtBytes(st.BytesBefore) + "  →  " + FmtBytes(st.BytesAfter);
            if (st.BytesBefore > 0 && st.BytesAfter < st.BytesBefore)
            {
                long saved = st.BytesBefore - st.BytesAfter;
                int pct    = (int)Math.Round((double)saved / st.BytesBefore * 100);
                _stSaved.Text = FmtBytes(saved) + "  (−" + pct + "%)";
            }
            else _stSaved.Text = "—";
        }

        private static string FmtBytes(long b)
        {
            if (b >= 1L << 30) return string.Format("{0:F1} GB", b / (double)(1L << 30));
            if (b >= 1L << 20) return string.Format("{0:F0} MB", b / (double)(1L << 20));
            if (b >= 1L << 10) return (b / (1L << 10)).ToString() + " KB";
            return b.ToString() + " B";
        }

        // Creates a label used as a stat tile VALUE cell
        private static Label StatTile(string caption, string value)
        {
            var lbl = new Label
            {
                Text      = value,
                ForeColor = Dk.Txt,
                BackColor = Color.Transparent,
                Font      = Dk.FontBody(11f, FontStyle.Bold),
                AutoSize  = false,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Tag       = caption
            };
            return lbl;
        }


        public void Log(string msg, LogLevel lvl = LogLevel.Info)
        {
            if (InvokeRequired) { BeginInvoke(new Action<string, LogLevel>(Log), msg, lvl); return; }
            Color col = lvl == LogLevel.Error ? Dk.Err
                      : lvl == LogLevel.Warn  ? Dk.Warn
                      : lvl == LogLevel.Ok    ? Dk.Acc
                      : Dk.Txt;
            string prefix = lvl == LogLevel.Error ? "✖ "
                          : lvl == LogLevel.Warn  ? "⚠ "
                          : lvl == LogLevel.Ok    ? "✔ "
                          : "  ";
            string line = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + prefix + msg;
            _log.AppendColored(line, col);
            StudioLog.Write(line);
        }

        // ── START / STOP ──────────────────────────────────────────────────────────
        private void StartClick(object sender, EventArgs e)
        {
            if (_busy)
            {
                try { if (_cts != null) _cts.Cancel(); } catch { }
                SetStatus("Останавливаюсь…", Dk.Warn);
                return;
            }

            string inp = _inputBox.Text.Trim();
            string out_ = _outputBox.Text.Trim();

            if (string.IsNullOrEmpty(inp) || !Directory.Exists(inp))
            {
                MessageBox.Show(this, "Укажите существующую входную папку.",
                    "Shrinker V by akiooo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try {
                inp = Path.GetFullPath(inp).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                if (!string.IsNullOrEmpty(out_)) {
                    out_ = Path.GetFullPath(out_).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                    if (out_.StartsWith(inp, StringComparison.OrdinalIgnoreCase) || inp.StartsWith(out_, StringComparison.OrdinalIgnoreCase)) {
                        MessageBox.Show(this, "Входная и выходная папки не должны совпадать или находиться друг внутри друга."); return;
                    }
                }
            } catch (Exception ex) { MessageBox.Show(this, ex.Message); return; }
            bool inPlace = string.IsNullOrEmpty(out_);
            if (inPlace)
            {
                if (MessageBox.Show(this,
                    "Выходная папка не указана — файлы будут изменены НА МЕСТЕ!\n" +
                    "Это необратимо. Продолжить?",
                    "Shrinker V by akiooo — Предупреждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }
            else
            {
                try { Directory.CreateDirectory(out_); }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Не удалось создать выходную папку: " + ex.Message,
                        "Shrinker V by akiooo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            _log.Clear();
            StudioLog.Init(inPlace ? inp : out_);
            if (_cts != null) _cts.Dispose();
            _cts = new CancellationTokenSource();
            _totalArchives = 0;

            var opts = new ProcessingOptions
            {
                InputFolder       = inp,
                OutputFolder      = inPlace ? inp : out_,
                ProcessInPlace    = inPlace,
                CompressionFactor = Factor(),
                UseGrayscale      = _grayscale.Checked,
                ClothingCompressionFactor = ClothingFactor(),
                ClothingUseGrayscale      = _clothingGrayscale.Checked,
                PedsCompressionFactor     = PedsFactor(),
                PedsUseGrayscale          = _pedsGrayscale.Checked,
                SkipWeapons       = _skipWeapons.Checked,
                RemoveMap         = _removeMap.Checked,
                MinTextureSize    = (int)_minSize.Value,
                MaxThreads = (int)_threads.Value,
                VerboseLog = _verbose.Checked,
                ProtectCharacters = _protectCharacters.Checked,
                SkipPeds = !_compressPeds.Checked,
                KeepAudio = _keepAudio.Checked, KeepVideo = _keepVideo.Checked, KeepEffects = _keepEffects.Checked,
                Cancel = _cts.Token,
            };

            SetBusy(true);
            SetStatus("Работаю…");
            SetStats(new ProcessingStats()); SetProgress(0, 1);

            var t = new Thread(() =>
            {
                ProcessingStats stats = null;
                try
                {
                    stats = Shrinker.RunWithProgress(opts, (msg, lvl, current, processed, total) =>
                    {
                        _totalArchives = total;
                        Log(msg, lvl);
                        SetStats(current);
                        SetProgress(processed, total);
                    });
                    _cts.Token.ThrowIfCancellationRequested();
                    SetStats(stats);
                    SetStatus(string.Format(
                        "Готово · RPF: {0} · Текстур: {1} · Ошибок: {2}",
                        stats.ArchivesProcessed, stats.TexturesShrunken, stats.ErrorsEncountered),
                        stats.ErrorsEncountered > 0 ? Dk.Warn : Dk.Acc);
                }
                catch (OperationCanceledException)
                {
                    SetStatus("Остановлено.", Dk.Warn);
                    Log("=== Отменено пользователем ===", LogLevel.Warn);
                }
                catch (Exception ex)
                {
                    SetStatus("Ошибка: " + ex.Message, Dk.Err);
                    Log("FATAL: " + ex.ToString(), LogLevel.Error);
                }
                finally
                {
                    BeginInvoke(new Action(StudioLog.Close));
                    SetBusy(false);
                }
            });
            t.IsBackground = true;
            t.Name = "ShrinkerVWorker";
            t.Start();
        }
    }
}
