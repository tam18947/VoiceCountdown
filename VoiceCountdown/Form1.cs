using VoiceCountdown.Properties;
using System.Diagnostics;

namespace VoiceCountdown
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, true);
            }
            var devices = AudioPlayer.GetDevices();
            for (int i = 0; i < devices.Count; i++)
            {
                // ドロップダウンリストに表示するアイテムを作成する
                ToolStripMenuItem item = new(devices[i]);
                // アイテムがクリックされたときのイベントハンドラを設定する
                item.Click += new EventHandler(ToolStripMenuItem_Click);
                toolStripDropDownButton1.DropDownItems.Add(item);
                if (i == 0)
                {
                    checkedDeviceIndex = i;
                    toolStripStatusLabel1.Text = devices[i];
                    item.Checked = true;
                }
            }
            TimeLabel_Resize();
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
            "3,2,1.wav"
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
            new TimeSpan(0, 0, 3)
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
                if (ts.TotalSeconds < 0)
                {
                    Button_Click();
                    return;
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
                            var devices = AudioPlayer.GetDevices();
                            for (int i = 0; i < devices.Count; i++)
                            {
                                if (devices[i] == toolStripDropDownButton1.DropDownItems[checkedDeviceIndex].ToString())
                                { break; }
                                else
                                { ind++; }
                            }
                            if (devices.Count == ind)
                            { ind = -1; }
                            audioPlayer = new AudioPlayer(@"wav\" + wavFiles[j], ind);
                            audioPlayer.Play();
                            break;
                        }
                    }
                    else { break; }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unplayable File");
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
        private void SplitContainer2_Panel1_SizeChanged(object sender, EventArgs e) => TimeLabel_Resize();
        /// <summary>
        /// サイズ変更で時間表示の文字サイズを変更する実処理
        /// </summary>
        private void TimeLabel_Resize()
        {
            Font labelFont = label2.Font;
            Size labelSize = label2.Size;
            int fontSize = (int)(Math.Min(labelSize.Width, labelSize.Height) * 0.55);
            labelFont = new Font(labelFont.FontFamily, fontSize);
            label2.Font = labelFont;
        }

        /// <summary>
        /// タイマーの開始，停止を変更するイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e) => Button_Click();
        /// <summary>
        /// タイマーの開始，停止を変更する実処理
        /// </summary>
        private void Button_Click()
        {
            if (timer1.Enabled)
            {
                audioPlayer?.Stop();
                button1.BackgroundImage = Resources.Play;
                // タイマーを停止する
                timer1.Stop();
                // ストップウォッチを止める
                sw.Stop();
                sw.Reset();
                label2.Text = "00:00";
                dateTimePicker1.Enabled = true;
                startToolStripMenuItem.Enabled = true;
                stopToolStripMenuItem.Enabled = false;
            }
            else
            {
                current = 0;
                button1.BackgroundImage = Resources.Stop;
                timeSpan = dateTimePicker1.Value - baseDate;
                // タイマーを開始する
                timer1.Start();
                // ストップウォッチを開始する
                sw.Start();
                dateTimePicker1.Enabled = false;
                startToolStripMenuItem.Enabled = false;
                stopToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// バージョン情報を表示するイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) =>
            MessageBox.Show("VoiceCountdown\r\n\r\nVersion 20230428\r\nあみたろの声素材工房(https://amitaro.net/)の音声を使用しました");

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
    }
}
