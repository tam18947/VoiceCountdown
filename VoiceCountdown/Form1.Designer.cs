namespace VoiceCountdown
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            checkedListBox1 = new CheckedListBox();
            label1 = new Label();
            button1 = new Button();
            panel1 = new Panel();
            dateTimePicker1 = new DateTimePicker();
            radioButtonAlarm = new RadioButton();
            radioButtonDuration = new RadioButton();
            label3 = new Label();
            label2 = new Label();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            startToolStripMenuItem = new ToolStripMenuItem();
            stopToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            selectToolStripMenuItem = new ToolStripMenuItem();
            fontToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            timer1 = new System.Windows.Forms.Timer(components);
            fontDialog1 = new FontDialog();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            panel1.SuspendLayout();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(12, 27);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = Color.White;
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.SizeChanged += SplitContainer1_Panel2_SizeChanged;
            splitContainer1.Size = new Size(560, 429);
            splitContainer1.SplitterDistance = 100;
            splitContainer1.SplitterWidth = 8;
            splitContainer1.TabIndex = 1;
            splitContainer1.TabStop = false;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel2;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(checkedListBox1);
            splitContainer2.Panel1.Controls.Add(label1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(button1);
            splitContainer2.Panel2.Controls.Add(panel1);
            splitContainer2.Size = new Size(100, 429);
            splitContainer2.SplitterDistance = 256;
            splitContainer2.TabIndex = 0;
            splitContainer2.TabStop = false;
            // 
            // checkedListBox1
            // 
            checkedListBox1.CheckOnClick = true;
            checkedListBox1.Dock = DockStyle.Fill;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Items.AddRange(new object[] { "10分前", "5分前", "4分前", "3分前", "2分前", "1分前", "30秒前", "10秒前", "5", "4", "3", "2", "1" });
            checkedListBox1.Location = new Point(0, 15);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(100, 241);
            checkedListBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Top;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(100, 15);
            label1.TabIndex = 0;
            label1.Text = "読み上げる時間";
            // 
            // button1
            // 
            button1.BackColor = Color.Transparent;
            button1.BackgroundImageLayout = ImageLayout.Zoom;
            button1.Dock = DockStyle.Fill;
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = SystemColors.Control;
            button1.Location = new Point(0, 76);
            button1.Name = "button1";
            button1.Size = new Size(100, 93);
            button1.TabIndex = 3;
            button1.UseVisualStyleBackColor = false;
            button1.Click += Button1_Click;
            button1.MouseEnter += Button1_MouseEnter;
            button1.MouseLeave += Button1_MouseLeave;
            button1.Resize += Button1_Resize;
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.Controls.Add(dateTimePicker1);
            panel1.Controls.Add(radioButtonAlarm);
            panel1.Controls.Add(radioButtonDuration);
            panel1.Controls.Add(label3);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(100, 76);
            panel1.TabIndex = 4;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Dock = DockStyle.Top;
            dateTimePicker1.Format = DateTimePickerFormat.Time;
            dateTimePicker1.Location = new Point(0, 53);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Size = new Size(100, 23);
            dateTimePicker1.TabIndex = 5;
            dateTimePicker1.Value = new DateTime(2000, 1, 1, 0, 5, 0, 0);
            // 
            // radioButtonAlarm
            // 
            radioButtonAlarm.AutoSize = true;
            radioButtonAlarm.Dock = DockStyle.Top;
            radioButtonAlarm.Location = new Point(0, 34);
            radioButtonAlarm.Name = "radioButtonAlarm";
            radioButtonAlarm.Size = new Size(100, 19);
            radioButtonAlarm.TabIndex = 3;
            radioButtonAlarm.Text = "時刻で設定";
            toolTip1.SetToolTip(radioButtonAlarm, "ユーザーが指定した時刻（例：15時など）を設定できる");
            radioButtonAlarm.UseVisualStyleBackColor = true;
            // 
            // radioButtonDuration
            // 
            radioButtonDuration.AutoSize = true;
            radioButtonDuration.Checked = true;
            radioButtonDuration.Dock = DockStyle.Top;
            radioButtonDuration.Location = new Point(0, 15);
            radioButtonDuration.Name = "radioButtonDuration";
            radioButtonDuration.Size = new Size(100, 19);
            radioButtonDuration.TabIndex = 2;
            radioButtonDuration.TabStop = true;
            radioButtonDuration.Text = "期間で設定";
            toolTip1.SetToolTip(radioButtonDuration, "ユーザーが指定した時間（例：5分、30秒など）を設定できる");
            radioButtonDuration.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.Dock = DockStyle.Top;
            label3.Location = new Point(0, 0);
            label3.Name = "label3";
            label3.Size = new Size(100, 15);
            label3.TabIndex = 4;
            label3.Text = "タイマーの時間";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(34, 15);
            label2.TabIndex = 0;
            label2.Text = "00:00";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            label2.Layout += Label2_Layout;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(584, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { startToolStripMenuItem, stopToolStripMenuItem, toolStripSeparator2, selectToolStripMenuItem, fontToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(67, 20);
            fileToolStripMenuItem.Text = "ファイル(&F)";
            // 
            // startToolStripMenuItem
            // 
            startToolStripMenuItem.Name = "startToolStripMenuItem";
            startToolStripMenuItem.Size = new Size(146, 22);
            startToolStripMenuItem.Text = "開始(&S)";
            startToolStripMenuItem.Click += Button1_Click;
            // 
            // stopToolStripMenuItem
            // 
            stopToolStripMenuItem.Enabled = false;
            stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            stopToolStripMenuItem.Size = new Size(146, 22);
            stopToolStripMenuItem.Text = "リセット(&R)";
            stopToolStripMenuItem.Click += Reset_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(143, 6);
            // 
            // selectToolStripMenuItem
            // 
            selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            selectToolStripMenuItem.Size = new Size(146, 22);
            selectToolStripMenuItem.Text = "一時停止機能";
            selectToolStripMenuItem.Click += SelectToolStripMenuItem_Click;
            // 
            // fontToolStripMenuItem
            // 
            fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            fontToolStripMenuItem.Size = new Size(146, 22);
            fontToolStripMenuItem.Text = "フォント(&F)...";
            fontToolStripMenuItem.Click += FontToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(143, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(146, 22);
            exitToolStripMenuItem.Text = "終了(&X)";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(65, 20);
            helpToolStripMenuItem.Text = "ヘルプ(&H)";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(158, 22);
            aboutToolStripMenuItem.Text = "バージョン情報(&A)";
            aboutToolStripMenuItem.Click += AboutToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripDropDownButton1, toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 459);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(584, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(81, 20);
            toolStripDropDownButton1.Tag = "";
            toolStripDropDownButton1.Text = "出力デバイス";
            toolStripDropDownButton1.Click += ToolStripDropDownButton1_Click;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(78, 17);
            toolStripStatusLabel1.Text = "既定のデバイス";
            // 
            // timer1
            // 
            timer1.Tick += Timer1_Tick;
            // 
            // fontDialog1
            // 
            fontDialog1.ShowEffects = false;
            // 
            // Form1
            // 
            AcceptButton = button1;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 481);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "カウントダウン読み上げちゃん";
            Load += Form1_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer1;
        private CheckedListBox checkedListBox1;
        private Label label1;
        private Label label2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem startToolStripMenuItem;
        private ToolStripMenuItem stopToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer timer1;
        private Button button1;
        private ToolStripMenuItem fontToolStripMenuItem;
        private FontDialog fontDialog1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem selectToolStripMenuItem;
        private SplitContainer splitContainer2;
        private Panel panel1;
        private DateTimePicker dateTimePicker1;
        private Label label3;
        private RadioButton radioButtonAlarm;
        private RadioButton radioButtonDuration;
        private ToolTip toolTip1;
    }
}