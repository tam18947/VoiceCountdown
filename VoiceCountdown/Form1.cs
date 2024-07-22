using VoiceCountdown.Properties;
using System.Diagnostics;

namespace VoiceCountdown
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < checkedListBox1.Items.Count - 2; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
            ToolStripDropDownButton1_Click();
            initialized = true;
            ControlSizeChange(splitContainer1.Panel2, label2);
        }

        /// <summary>
        /// あみたろの声素材工房(https://amitaro.net/)の音声を使用しました
        /// </summary>
        private readonly UnmanagedMemoryStream[] wavStream =
        [
            Resources.timer_10punmae_01,
            Resources.timer_5funmae_01,
            Resources.timer_4punmae_01,
            Resources.timer_3punmae_01,
            Resources.timer_2funmae_01,
            Resources.timer_1punmae_01,
            Resources._30byoumae,
            Resources._10byoumae,
            Resources.num005_01,
            Resources.num004_01,
            Resources.num003_01,
            Resources.num002_01,
            Resources.num001_01,
        ];

        /// <summary>
        /// 音声の時間をTimeSpanで用意する
        /// </summary>
        private readonly TimeSpan[] timeSpans =
        [
            new(0, 10, 0),
            new(0, 5, 0),
            new(0, 4, 0),
            new(0, 3, 0),
            new(0, 2, 0),
            new(0, 1, 0),
            new(0, 0, 30),
            new(0, 0, 10),
            new(0, 0, 5),
            new(0, 0, 4),
            new(0, 0, 3),
            new(0, 0, 2),
            new(0, 0, 1),
        ];
        /// <summary>
        /// Stopwatchオブジェクトを作成する
        /// </summary>
        private readonly Stopwatch sw = new();
        /// <summary>
        /// チェックされた出力デバイスの番号を保持する変数
        /// </summary>
        private int checkedDeviceIndex = -1;
        /// <summary>
        /// AudioPlayerの変数
        /// </summary>
        private AudioPlayer? audioPlayer = null;
        /// <summary>
        /// ベースの時間の設定
        /// </summary>
        private readonly DateTime baseDate = new(2000, 1, 1, 0, 0, 0);
        /// <summary>
        /// 開始する時間を保持する変数
        /// </summary>
        private TimeSpan timeSpan = TimeSpan.Zero;
        /// <summary>
        /// 現在の timeSpans の配列番号
        /// </summary>
        private int current = 0;
        /// <summary>
        /// 初期化が終了したかどうか
        /// </summary>
        private readonly bool initialized = false;
        /// <summary>
        /// ボタンのクリックカウント
        /// </summary>
        private int clickCount = 0;
        /// <summary>
        /// ボタンがダブルクリックされてからトリプルクリックされるまでの計測時間
        /// </summary>
        private int clickInterval = 0;
        /// <summary>
        /// 
        /// </summary>
        private Stream? stream1 = null;

        /// <summary>
        /// タイマーで時間が来たら音声を再生させるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                var h = sw.Elapsed.Hours;
                var m = sw.Elapsed.Minutes;
                var s = sw.Elapsed.Seconds;
                var ts = timeSpan - new TimeSpan(h, m, s);
                if (ts.TotalSeconds <= 0)
                {
                    Reset_Click();
                }
                label2.Text = ts.Hours > 0 ? ts.ToString(@"h\:mm\:ss") : ts.ToString(@"mm\:ss");
                for (int j = current; j < checkedListBox1.Items.Count; j++)
                {
                    if (timeSpans[j] > ts)
                    {
                        current++;
                    }
                    else if (timeSpans[j] == ts)
                    {
                        checkedListBox1.SelectedIndex = current++;
                        if (checkedListBox1.GetItemChecked(j))
                        {
                            audioPlayer?.Stop();
                            wavStream[j].Seek(0, SeekOrigin.Begin);
                            audioPlayer = new AudioPlayer(wavStream[j], GetDeviceNumber());
                            audioPlayer.Play();
                            return;
                        }
                    }
                    else { return; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
        }
        /// <summary>
        /// 音声デバイス番号を取得する
        /// </summary>
        /// <returns></returns>
        private int GetDeviceNumber()
        {
            if (checkedDeviceIndex == -1) { return -1; }

            var devices = AudioPlayer.GetDevices();
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i] == toolStripStatusLabel1.Text)
                {
                    return i;
                }
            }
            toolStripStatusLabel1.Text = "既定のデバイス";
            checkedDeviceIndex = -1;
            return -1;
        }

        /// <summary>
        /// 出力デバイス名の ToolStripMenuItem がクリックされたときに実行されるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            // クリックされたアイテムを取得する
            ToolStripMenuItem? clickedItem = (ToolStripMenuItem?)sender;
            if (clickedItem is null) { return; }
            // すべてのアイテムからチェックを外す
            foreach (ToolStripMenuItem item in toolStripDropDownButton1.DropDownItems)
            { item.Checked = false; }
            // クリックされたアイテムにチェックをつける
            clickedItem.Checked = true;
            // チェックされたアイテムのインデックスを取得する
            checkedDeviceIndex = toolStripDropDownButton1.DropDownItems.IndexOf(clickedItem);
            // チェックされたアイテムの名前をラベルに表示する
            toolStripStatusLabel1.Text = toolStripDropDownButton1.DropDownItems[checkedDeviceIndex].ToString();
        }

        /// <summary>
        /// サイズ変更で時間表示の文字サイズを変更するイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SplitContainer1_Panel2_SizeChanged(object sender, EventArgs e) => ControlSizeChange((SplitterPanel)sender, label2);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label2_TextChanged(object sender, EventArgs e) => ControlSizeChange(splitContainer1.Panel2, label2);
        /// <summary>
        /// labelのコントロールサイズを基にしてフォントサイズを自動調節する
        /// </summary>
        /// <param name="controlP">親コントロール</param>
        /// <param name="controlC">子コントロール</param>
        private void ControlSizeChange(Control controlP, Control controlC)
        {
            // コントロールが初期化済みなら
            if (!initialized) { return; }
            // コントロールの中心座標
            Point p = GetControlLocation(controlP, controlC);
            bool flag = p.X > 0 && p.Y > 0;
            while (true)
            {
                if (p.X < 0 || p.Y < 0)
                {
                    if (controlC.Font.Size <= 1)
                    {
                        break;
                    }
                    // 文字を小さくする
                    p = ResizeFont(controlP, controlC, -1f);
                    if (flag)
                    {
                        break;
                    }
                }
                else
                {
                    // 文字を大きくする
                    p = ResizeFont(controlP, controlC, 1f);
                    if (!flag)
                    {
                        // 文字を小さくする
                        p = ResizeFont(controlP, controlC, -1f);
                        break;
                    }
                }
            }
            controlC.Location = p + new Size(Margin.Left, Margin.Top);
        }
        private Point ResizeFont(Control controlP, Control controlC, float emSize)
        {
            controlC.Font = new Font(controlC.Font.FontFamily, (float)(controlC.Font.Size + emSize), controlC.Font.Style);
            return GetControlLocation(controlP, controlC);
        }
        private Point GetControlLocation(Control controlP, Control controlC)
        {
            Size s = controlP.ClientSize - controlC.ClientSize;
            return new Point(
                (s.Width - Margin.Left - Margin.Right) / 2,
                (s.Height - Margin.Top - Margin.Bottom) / 2);
        }

        /// <summary>
        /// カウントダウンを開始するイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            if (selectToolStripMenuItem.Checked)
            {
                ButtonWithPause_Click();
            }
            else
            {
                ButtonWithoutPause_Click();
            }
        }

        /// <summary>
        /// カウントダウンをリセットするイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Click(object? sender = null, EventArgs? e = null)
        {
            if (selectToolStripMenuItem.Checked)
            {
                clickCount = 2;
                ButtonWithPause_Click();
            }
            else
            {
                ButtonWithoutPause_Click();
            }
        }

        /// <summary>
        /// タイマーの開始，リセットを変更する実処理（一時停止機能あり）
        /// </summary>
        private void ButtonWithPause_Click()
        {
            // クリック数カウント
            clickCount++;
            if (clickCount == 1)
            {
                clickInterval = 0;
                System.Windows.Forms.Timer t = new()
                {
                    Interval = SystemInformation.DoubleClickTime
                };
                t.Start();
                t.Tick += (s, args) =>
                {
                    if (clickCount != 2)
                    {
                        clickCount = 0;
                    }
                    t.Stop();
                };
            }
            // ダブルクリックだったらトリプルクリックの計測開始
            else if (clickCount == 2)
            {
                System.Windows.Forms.Timer t = new()
                {
                    Interval = 100
                };
                t.Start();
                t.Tick += (s, args) =>
                {
                    // 時間計測
                    clickInterval += t.Interval;
                    // ダブルクリック間隔の時間を超えたらリセット
                    if (SystemInformation.DoubleClickTime < clickInterval)
                    {
                        t.Stop();
                        clickInterval = 0;
                        clickCount = 0;
                        if (timer1.Enabled)
                        {
                            // タイマーが開始していたら一時停止ボタンにする
                            stream1 = GetType().Assembly.GetManifestResourceStream("VoiceCountdown.Resources.Pause.svg");
                            button1.BackgroundImage = Svg.GetImage(button1.Size, stream1);
                        }
                        else
                        {
                            // タイマーが停止していたら開始ボタンにする
                            stream1 = GetType().Assembly.GetManifestResourceStream("VoiceCountdown.Resources.Play.svg");
                            button1.BackgroundImage = Svg.GetImage(button1.Size, stream1);
                        }
                    }
                };
            }
            // ダブルクリック間隔の時間を超えるまで処理
            if (clickInterval < SystemInformation.DoubleClickTime)
            {
                if (clickCount == 3)
                {
                    clickCount = 0;
                    // トリプルクリックされたときの処理を記述
                    stream1 = GetType().Assembly.GetManifestResourceStream("VoiceCountdown.Resources.Play.svg");
                    button1.BackgroundImage = Svg.GetImage(button1.Size, stream1);
                    // オーディオを停止する
                    audioPlayer?.Stop();
                    // タイマーを停止する
                    timer1.Stop();
                    // ストップウォッチをリセットする
                    sw.Reset();
                    label2.Text = "00:00";
                    dateTimePicker1.Enabled = true;
                    checkBox1.Enabled = true;
                    stopToolStripMenuItem.Enabled = false;
                    startToolStripMenuItem.Text = "開始";
                    selectToolStripMenuItem.Enabled = true;
                }
                else
                {
                    if (timer1.Enabled)
                    {
                        // オーディオを停止する
                        audioPlayer?.Stop();
                        // タイマーを停止する
                        timer1.Stop();
                        // ストップウォッチを止める
                        sw.Stop();
                        stream1 = GetType().Assembly.GetManifestResourceStream("VoiceCountdown.Resources.Play.svg");
                        button1.BackgroundImage = Svg.GetImage(button1.Size, stream1);
                        startToolStripMenuItem.Enabled = true;
                        startToolStripMenuItem.Text = "再開";
                    }
                    else
                    {
                        current = 0;
                        timeSpan = dateTimePicker1.Value - baseDate;
                        // 時刻で指定
                        if (checkBox1.Checked)
                        {
                            timeSpan -= DateTime.Now - DateTime.Today;
                            timeSpan = timeSpan > TimeSpan.Zero ? timeSpan : timeSpan + TimeSpan.FromDays(1);
                        }
                        // タイマーを開始する
                        timer1.Start();
                        // ストップウォッチを開始する
                        sw.Start();
                        stream1 = GetType().Assembly.GetManifestResourceStream("VoiceCountdown.Resources.Pause.svg");
                        button1.BackgroundImage = Svg.GetImage(button1.Size, stream1);
                        dateTimePicker1.Enabled = false;
                        checkBox1.Enabled = false;
                        stopToolStripMenuItem.Enabled = true;
                        startToolStripMenuItem.Text = "一時停止";
                        selectToolStripMenuItem.Enabled = false;
                    }
                    if (clickCount == 2)
                    {
                        stream1 = GetType().Assembly.GetManifestResourceStream("VoiceCountdown.Resources.Stop.svg");
                        button1.BackgroundImage = Svg.GetImage(button1.Size, stream1);
                    }
                }
            }
        }
        /// <summary>
        /// タイマーの開始，リセットを変更する実処理（一時停止機能なし）
        /// </summary>
        private void ButtonWithoutPause_Click()
        {
            if (timer1.Enabled)
            {
                stream1 = GetType().Assembly.GetManifestResourceStream("VoiceCountdown.Resources.Play.svg");
                button1.BackgroundImage = Svg.GetImage(button1.Size, stream1);
                // オーディオを停止する
                audioPlayer?.Stop();
                // タイマーを停止する
                timer1.Stop();
                // ストップウォッチをリセットする
                sw.Reset();
                label2.Text = "00:00";
                dateTimePicker1.Enabled = true;
                checkBox1.Enabled = true;
                startToolStripMenuItem.Enabled = true;
                stopToolStripMenuItem.Enabled = false;
                selectToolStripMenuItem.Enabled = true;
            }
            else
            {
                current = 0;
                timeSpan = dateTimePicker1.Value - baseDate;
                // 時刻で指定
                if (checkBox1.Checked)
                {
                    timeSpan -= DateTime.Now - DateTime.Today;
                    timeSpan = timeSpan > TimeSpan.Zero ? timeSpan : timeSpan + TimeSpan.FromDays(1);
                }
                // タイマーを開始する
                timer1.Start();
                // ストップウォッチを開始する
                sw.Start();
                stream1 = GetType().Assembly.GetManifestResourceStream("VoiceCountdown.Resources.Stop.svg");
                button1.BackgroundImage = Svg.GetImage(button1.Size, stream1);
                dateTimePicker1.Enabled = false;
                checkBox1.Enabled = false;
                startToolStripMenuItem.Enabled = false;
                stopToolStripMenuItem.Enabled = true;
                selectToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// 出力デバイスを選択するイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolStripDropDownButton1_Click(object? sender = null, EventArgs? e = null)
        {
            bool b = true;
            var devices = AudioPlayer.GetDevices();
            if (devices.Count == toolStripDropDownButton1.DropDownItems.Count)
            {
                foreach (ToolStripMenuItem item in toolStripDropDownButton1.DropDownItems)
                {
                    var txt = item.Text is null ? "" : item.Text;
                    b = b && devices.Contains(txt);
                }
            }
            else
            {
                b = false;
            }
            if (!b)
            {
                checkedDeviceIndex = -1;
                toolStripDropDownButton1.DropDownItems.Clear();
                for (int i = 0; i < devices.Count; i++)
                {
                    // ドロップダウンリストに表示するアイテムを作成する
                    ToolStripMenuItem item = new(devices[i]);
                    // アイテムがクリックされたときのイベントハンドラを設定する
                    item.Click += new EventHandler(ToolStripMenuItem_Click);
                    toolStripDropDownButton1.DropDownItems.Add(item);
                    if (devices[i] == toolStripStatusLabel1.Text)
                    {
                        checkedDeviceIndex = i;
                        item.Checked = true;
                    }
                }
                if (checkedDeviceIndex == -1)
                {
                    toolStripStatusLabel1.Text = "既定のデバイス";
                }
            }
        }

        /// <summary>
        /// 一時停止機能を使用するかを選択するイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectToolStripMenuItem.Checked = !selectToolStripMenuItem.Checked;
        }

        /// <summary>
        /// フォントダイアログを表示するイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                label2.Font = new Font(fontDialog1.Font.FontFamily, label2.Font.Size);
                ControlSizeChange(splitContainer1.Panel2, label2);
            }
        }

        /// <summary>
        /// 終了イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            audioPlayer?.Stop();
            Close();
        }

        /// <summary>
        /// バージョン情報を表示するイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string version = "Version 20240608";
            MessageBox.Show("Voice Countdown -" + Text + "-\r\n" + version
                + "\r\n\r\n\r\nクレジット情報：\r\nあみたろの声素材工房(https://amitaro.net/)の音声を使用しました", "Voice Countdown のバージョン情報");
        }

        private void Button1_MouseEnter(object sender, EventArgs e) => Cursor = Cursors.Hand;

        private void Button1_MouseLeave(object sender, EventArgs e) => Cursor = Cursors.Default;

        private void Button1_Resize(object sender, EventArgs e)
        {
            if (stream1 is not null)
            {
                stream1.Seek(0, SeekOrigin.Begin);
                button1.BackgroundImage = Svg.GetImage(button1.Size, stream1);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            stream1 = GetType().Assembly.GetManifestResourceStream("VoiceCountdown.Resources.Play.svg");
        }
    }
}
