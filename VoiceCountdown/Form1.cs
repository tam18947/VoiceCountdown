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
        private readonly string[] wavFiles = new string[]
        {
            "timer_10punmae_01.wav",
            "timer_5funmae_01.wav",
            "timer_4punmae_01.wav",
            "timer_3punmae_01.wav",
            "timer_2funmae_01.wav",
            "timer_1punmae_01.wav",
            "30byoumae.wav",
            "10byoumae.wav",
            "num005_01.wav",
            "num004_01.wav",
            "num003_01.wav",
            "num002_01.wav",
            "num001_01.wav",
        };
        /// <summary>
        /// 音声の時間をTimeSpanで用意する
        /// </summary>
        private readonly TimeSpan[] timeSpans = new TimeSpan[]
        {
            new TimeSpan(0, 10, 0),
            new TimeSpan(0, 5, 0),
            new TimeSpan(0, 4, 0),
            new TimeSpan(0, 3, 0),
            new TimeSpan(0, 2, 0),
            new TimeSpan(0, 1, 0),
            new TimeSpan(0, 0, 30),
            new TimeSpan(0, 0, 10),
            new TimeSpan(0, 0, 5),
            new TimeSpan(0, 0, 4),
            new TimeSpan(0, 0, 3),
            new TimeSpan(0, 0, 2),
            new TimeSpan(0, 0, 1),
        };
        /// <summary>
        /// Stopwatchオブジェクトを作成する
        /// </summary>
        private readonly Stopwatch sw = new();
        /// <summary>
        /// チェックされた出力デバイスの番号を保持する変数
        /// </summary>
        int checkedDeviceIndex = -1;
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
                label2.Text = ts.ToString(@"mm\:ss");
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
                            int ind = 0;
                            if (checkedDeviceIndex == -1)
                            {
                                ind = -1;
                            }
                            else
                            {
                                var devices = AudioPlayer.GetDevices();
                                for (int i = 0; i < devices.Count; i++)
                                {
                                    if (devices[i] == toolStripStatusLabel1.Text)
                                    { break; }
                                    else
                                    { ind++; }
                                }
                                if (devices.Count == ind)
                                {
                                    ind = checkedDeviceIndex = -1;
                                    toolStripStatusLabel1.Text = "既定のデバイス";
                                }
                            }
                            audioPlayer = new AudioPlayer(@"wav\" + wavFiles[j], ind);
                            audioPlayer.Play();
                            break;
                        }
                    }
                    else { break; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
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
        /// labelのコントロールサイズを基にしてフォントサイズを自動調節する
        /// </summary>
        /// <param name="controlP">親コントロール</param>
        /// <param name="controlC">子コントロール</param>
        private void ControlSizeChange(Control controlP, Control controlC)
        {
            // コントロールが初期化済みなら
            if (!initialized) { return; }
            int val = 0;
            Point p = GetControlLocation(controlP, controlC);
            while (controlC.Font.Size - 1 > 0 && (p.X != 0 || p.Y != 0))
            {
                if (p.X < 0 || p.Y < 0)
                {
                    // 文字を小さくする
                    p = ResizeFont(controlP, controlC, -1f);
                    if (val == 1)
                    {
                        break;
                    }
                    val = -1;
                }
                else
                {
                    // 文字を大きくする
                    p = ResizeFont(controlP, controlC, 1f);
                    if (val == -1)
                    {
                        // 文字を小さくする
                        p = ResizeFont(controlP, controlC, -1f);
                        break;
                    }
                    val = 1;
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
        private void Start_Click(object sender, EventArgs e)
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
                            button1.BackgroundImage = Resources.Pause;
                        }
                        else
                        {
                            // タイマーが停止していたら開始ボタンにする
                            button1.BackgroundImage = Resources.Play;
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
                    button1.BackgroundImage = Resources.Play;
                    // オーディオを停止する
                    audioPlayer?.Stop();
                    // タイマーを停止する
                    timer1.Stop();
                    // ストップウォッチをリセットする
                    sw.Reset();
                    label2.Text = "00:00";
                    dateTimePicker1.Enabled = true;
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
                        button1.BackgroundImage = Resources.Play;
                        startToolStripMenuItem.Enabled = true;
                        startToolStripMenuItem.Text = "再開";
                    }
                    else
                    {
                        current = 0;
                        timeSpan = dateTimePicker1.Value - baseDate;
                        // タイマーを開始する
                        timer1.Start();
                        // ストップウォッチを開始する
                        sw.Start();
                        button1.BackgroundImage = Resources.Pause;
                        dateTimePicker1.Enabled = false;
                        stopToolStripMenuItem.Enabled = true;
                        startToolStripMenuItem.Text = "一時停止";
                        selectToolStripMenuItem.Enabled = false;
                    }
                    if (clickCount == 2)
                    {
                        button1.BackgroundImage = Resources.Stop;
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
                button1.BackgroundImage = Resources.Play;
                // オーディオを停止する
                audioPlayer?.Stop();
                // タイマーを停止する
                timer1.Stop();
                // ストップウォッチをリセットする
                sw.Reset();
                label2.Text = "00:00";
                dateTimePicker1.Enabled = true;
                startToolStripMenuItem.Enabled = true;
                stopToolStripMenuItem.Enabled = false;
                selectToolStripMenuItem.Enabled = true;
            }
            else
            {
                current = 0;
                timeSpan = dateTimePicker1.Value - baseDate;
                // タイマーを開始する
                timer1.Start();
                // ストップウォッチを開始する
                sw.Start();
                button1.BackgroundImage = Resources.Stop;
                dateTimePicker1.Enabled = false;
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
                    b = b && devices.Contains(item.Text);
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
            MessageBox.Show("VoiceCountdown\r\n\r\nVersion 20230603\r\nあみたろの声素材工房(https://amitaro.net/)の音声を使用しました");
        }

        private void Button1_MouseEnter(object sender, EventArgs e) => Cursor = Cursors.Hand;

        private void Button1_MouseLeave(object sender, EventArgs e) => Cursor = Cursors.Default;
    }
}
