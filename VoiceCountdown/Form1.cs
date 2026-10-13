using VoiceCountdown.Core.Countdown;
using VoiceCountdown.Properties;

namespace VoiceCountdown;

public partial class Form1 : Form
{
    private const string DefaultDeviceLabel = "既定のデバイス";

    private readonly IReadOnlyList<CountdownItem> items = CountdownConfig.DefaultItems;
    private readonly CountdownService countdown;
    private readonly AudioPlayer audio = new();

    /// <summary>選択中の出力デバイス名。null は既定のデバイス。</summary>
    private string? selectedDeviceName;

    // 一時停止機能が有効なときの連続クリック判定（3 回目のクリックでリセット）
    private readonly System.Windows.Forms.Timer resetArmTimer = new();
    private long lastClickTime;
    private int rapidClickCount;

    private ButtonIcon currentIcon = ButtonIcon.Play;

    // 残り時間ラベルのフォント（自分で作ったものだけ破棄する）
    private Font? ownedTimeFont;
    private string lastFittedText = "";

    private bool PauseEnabled => selectToolStripMenuItem.Checked;

    public Form1()
    {
        InitializeComponent();

        countdown = new CountdownService(items.Select(i => i.TriggerTime));

        InitTriggerList();
        InitDeviceMenu();
        InitCountdownEvents();

        resetArmTimer.Interval = SystemInformation.DoubleClickTime;
        resetArmTimer.Tick += ResetArmTimer_Tick;

        FormClosed += (_, _) => DisposeResources();

        ApplyState(CountdownState.Idle);
        FitTimeLabel();
    }

    // -----------------------------
    // 初期化
    // -----------------------------

    private void InitTriggerList()
    {
        checkedListBox1.Items.Clear();
        foreach (var item in items)
            checkedListBox1.Items.Add(item.Label, item.EnabledByDefault);
    }

    private void InitCountdownEvents()
    {
        countdown.TimeUpdated += remaining => SetTimeText(CountdownService.Format(remaining));

        countdown.Triggered += index =>
        {
            // 通過した項目を一覧で選択状態にする
            checkedListBox1.SelectedIndex = index;

            if (checkedListBox1.GetItemChecked(index))
                PlaySound(items[index].SoundKey);
        };

        countdown.StateChanged += ApplyState;
    }

    // -----------------------------
    // カウントダウン操作
    // -----------------------------

    private void StartCountdown()
    {
        var duration = radioButtonAlarm.Checked
            ? CountdownService.DurationUntil(dateTimePicker1.Value.TimeOfDay, DateTime.Now)
            : dateTimePicker1.Value.TimeOfDay;

        if (duration <= TimeSpan.Zero) return;

        countdown.Start(duration);
    }

    /// <summary>停止中なら開始、実行中なら一時停止、一時停止中なら再開する。</summary>
    private void TogglePause()
    {
        switch (countdown.State)
        {
            case CountdownState.Idle: StartCountdown(); break;
            case CountdownState.Running:
                audio.Stop();
                countdown.Pause();
                break;
            case CountdownState.Paused: countdown.Resume(); break;
        }
    }

    /// <summary>停止中なら開始、それ以外ならリセットする（一時停止機能なし）。</summary>
    private void ToggleStartReset()
    {
        if (countdown.State == CountdownState.Idle)
            StartCountdown();
        else
            ResetCountdown();
    }

    private void ResetCountdown()
    {
        rapidClickCount = 0;
        resetArmTimer.Stop();
        audio.Stop();
        countdown.Reset();
        SetIcon(IconFor(countdown.State));
    }

    /// <summary>
    /// 状態に合わせて、タイマー・アイコン・メニュー・入力欄をまとめて更新する。
    /// </summary>
    private void ApplyState(CountdownState state)
    {
        bool idle = state == CountdownState.Idle;

        // 音声は止めない（残り 1 秒の読み上げ直後に終了しても最後まで再生する）
        timer1.Enabled = state == CountdownState.Running;

        panel1.Enabled = idle;
        selectToolStripMenuItem.Enabled = idle;
        stopToolStripMenuItem.Enabled = !idle;

        if (PauseEnabled)
        {
            startToolStripMenuItem.Enabled = true;
            startToolStripMenuItem.Text = state switch
            {
                CountdownState.Running => "一時停止(&S)",
                CountdownState.Paused => "再開(&S)",
                _ => "開始(&S)",
            };
        }
        else
        {
            startToolStripMenuItem.Enabled = idle;
            startToolStripMenuItem.Text = "開始(&S)";
        }

        // リセット待ち（Stop アイコン表示中）はアイコンを変えない
        if (!resetArmTimer.Enabled)
            SetIcon(IconFor(state));
    }

    private ButtonIcon IconFor(CountdownState state) => state switch
    {
        CountdownState.Running => PauseEnabled ? ButtonIcon.Pause : ButtonIcon.Stop,
        _ => ButtonIcon.Play,
    };

    // -----------------------------
    // ボタン・メニュー
    // -----------------------------

    private void Button1_Click(object sender, EventArgs e)
    {
        if (!PauseEnabled)
        {
            ToggleStartReset();
            return;
        }

        // メニューからの操作は連続クリック判定の対象にしない
        if (sender != button1)
        {
            TogglePause();
            return;
        }

        long now = Environment.TickCount64;
        rapidClickCount = now - lastClickTime <= SystemInformation.DoubleClickTime ? rapidClickCount + 1 : 1;
        lastClickTime = now;

        // 3 回目の連続クリックでリセット
        if (rapidClickCount >= 3)
        {
            ResetCountdown();
            return;
        }

        TogglePause();

        // 2 回目の連続クリックで、しばらく Stop アイコンを表示してリセットを待ち受ける
        if (rapidClickCount == 2)
        {
            SetIcon(ButtonIcon.Stop);
            resetArmTimer.Stop();
            resetArmTimer.Start();
        }
    }

    private void ResetArmTimer_Tick(object? sender, EventArgs e)
    {
        resetArmTimer.Stop();
        rapidClickCount = 0;
        SetIcon(IconFor(countdown.State));
    }

    private void Reset_Click(object? sender, EventArgs e) => ResetCountdown();

    private void Timer1_Tick(object sender, EventArgs e) => countdown.Tick();

    private void SelectToolStripMenuItem_Click(object sender, EventArgs e)
    {
        selectToolStripMenuItem.Checked = !selectToolStripMenuItem.Checked;
        ApplyState(countdown.State);
    }

    private void FontToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (fontDialog1.ShowDialog() != DialogResult.OK) return;

        SetTimeFont(new Font(fontDialog1.Font.FontFamily, label2.Font.Size, fontDialog1.Font.Style));
        FitTimeLabel();
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        string version = "Version 20261013";
        MessageBox.Show("Voice Countdown -" + Text + "-\r\n" + version
            + "\r\n\r\n\r\nクレジット情報：\r\nあみたろの声素材工房(https://amitaro.net/)の音声を使用しました", "Voice Countdown のバージョン情報");
    }

    private void Form1_Load(object sender, EventArgs e) => SetIcon(currentIcon, force: true);

    private void Button1_MouseEnter(object sender, EventArgs e) => Cursor = Cursors.Hand;

    private void Button1_MouseLeave(object sender, EventArgs e) => Cursor = Cursors.Default;

    // -----------------------------
    // 音声・出力デバイス
    // -----------------------------

    private void PlaySound(string soundKey)
    {
        var stream = Resources.ResourceManager.GetStream(soundKey);
        if (stream is null) return;

        try
        {
            audio.Play(stream, GetDeviceNumber());
        }
        catch (Exception ex)
        {
            // 出力デバイスが外された場合など。カウントダウンは止めない
            audio.Stop();
            MessageBox.Show(ex.Message, "音声を再生できませんでした");
        }
    }

    /// <summary>
    /// 選択中のデバイス名から、現在のデバイス番号を求める。
    /// デバイスが見つからなければ既定のデバイスに戻す。
    /// </summary>
    private int GetDeviceNumber()
    {
        if (selectedDeviceName is null) return AudioPlayer.DefaultDevice;

        var names = AudioPlayer.GetDeviceNames();
        for (int i = 0; i < names.Count; i++)
        {
            if (names[i] == selectedDeviceName) return i;
        }

        SelectDevice(null);
        return AudioPlayer.DefaultDevice;
    }

    /// <summary>デバイス一覧を作り直し、選択中のデバイスにチェックを付ける。</summary>
    private void InitDeviceMenu()
    {
        var names = AudioPlayer.GetDeviceNames();

        if (selectedDeviceName is not null && !names.Contains(selectedDeviceName))
            selectedDeviceName = null;

        var oldItems = toolStripDropDownButton1.DropDownItems.Cast<ToolStripItem>().ToArray();
        toolStripDropDownButton1.DropDownItems.Clear();
        foreach (var old in oldItems)
            old.Dispose();

        var defaultItem = new ToolStripMenuItem(DefaultDeviceLabel) { Checked = selectedDeviceName is null };
        defaultItem.Click += (_, _) => SelectDevice(null);
        toolStripDropDownButton1.DropDownItems.Add(defaultItem);
        toolStripDropDownButton1.DropDownItems.Add(new ToolStripSeparator());

        foreach (var name in names)
        {
            var item = new ToolStripMenuItem(name) { Checked = name == selectedDeviceName };
            item.Click += (_, _) => SelectDevice(name);
            toolStripDropDownButton1.DropDownItems.Add(item);
        }

        toolStripStatusLabel1.Text = selectedDeviceName ?? DefaultDeviceLabel;
    }

    private void SelectDevice(string? name)
    {
        selectedDeviceName = name;
        toolStripStatusLabel1.Text = name ?? DefaultDeviceLabel;

        foreach (ToolStripItem item in toolStripDropDownButton1.DropDownItems)
        {
            if (item is ToolStripMenuItem menuItem)
                menuItem.Checked = menuItem.Text == toolStripStatusLabel1.Text;
        }
    }

    private void ToolStripDropDownButton1_DropDownOpening(object? sender, EventArgs e) => InitDeviceMenu();

    // -----------------------------
    // ボタンのアイコン
    // -----------------------------

    private void SetIcon(ButtonIcon icon, bool force = false)
    {
        if (!force && icon == currentIcon && button1.BackgroundImage is not null) return;

        currentIcon = icon;
        RenderIcon();
    }

    private void RenderIcon()
    {
        var old = button1.BackgroundImage;
        button1.BackgroundImage = SvgIcons.Render(currentIcon, button1.ClientSize);
        old?.Dispose();
    }

    private void Button1_Resize(object sender, EventArgs e) => RenderIcon();

    // -----------------------------
    // 残り時間の表示（パネルいっぱいに収まるようフォントサイズを調整）
    // -----------------------------

    private void SetTimeText(string text)
    {
        if (label2.Text == text) return;

        label2.Text = text;
        // "09:59" → "1:00:00" のように文字数が変わったときだけ再計算する
        if (text.Length != lastFittedText.Length)
            FitTimeLabel();
    }

    private void SplitContainer1_Panel2_SizeChanged(object sender, EventArgs e) => FitTimeLabel();

    private void FitTimeLabel()
    {
        var panel = splitContainer1.Panel2;
        var area = new Size(
            panel.ClientSize.Width - Margin.Horizontal,
            panel.ClientSize.Height - Margin.Vertical);
        if (area.Width <= 0 || area.Height <= 0) return;

        // 大きめのフォントで一度だけ測り、比率からサイズを決める
        const float probeSize = 100f;
        var family = label2.Font.FontFamily;
        var style = label2.Font.Style;
        Size measured;
        using (var probe = new Font(family, probeSize, style))
            measured = TextRenderer.MeasureText(label2.Text, probe);

        if (measured.Width > 0 && measured.Height > 0)
        {
            float scale = Math.Min((float)area.Width / measured.Width, (float)area.Height / measured.Height);
            float emSize = Math.Max(1f, MathF.Floor(probeSize * scale));

            if (Math.Abs(label2.Font.Size - emSize) >= 1f)
                SetTimeFont(new Font(family, emSize, style));
        }

        label2.Location = new Point(
            (panel.ClientSize.Width - label2.Width) / 2,
            (panel.ClientSize.Height - label2.Height) / 2);

        lastFittedText = label2.Text;
    }

    private void SetTimeFont(Font font)
    {
        var old = ownedTimeFont;
        label2.Font = font;
        ownedTimeFont = font;
        old?.Dispose();
    }

    // -----------------------------
    // 終了処理
    // -----------------------------

    private void DisposeResources()
    {
        timer1.Stop();
        resetArmTimer.Dispose();
        audio.Dispose();
        button1.BackgroundImage?.Dispose();
        button1.BackgroundImage = null;
    }
}
