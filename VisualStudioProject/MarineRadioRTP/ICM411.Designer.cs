namespace MarineRadioRTP
{
    partial class ICM411
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ICM411));
            this.lcdPanel = new System.Windows.Forms.Panel();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showAISRadarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.talkButton = new MarineRadioRTP.MarineRadioButton();
            this.rightTopLabel = new MarineRadioRTP.LcdLabel();
            this.dscIndicatorLabel = new MarineRadioRTP.IndicatorLabel();
            this.txIndicatorLabel = new MarineRadioRTP.IndicatorLabel();
            this.busyIndicatorLabel = new MarineRadioRTP.IndicatorLabel();
            this.callIndicatorLabel = new MarineRadioRTP.IndicatorLabel();
            this.usaIndicatorLabel = new MarineRadioRTP.IndicatorLabel();
            this.intIndicatorLabel = new MarineRadioRTP.IndicatorLabel();
            this.posReplyIndicatorLabel = new MarineRadioRTP.IndicatorLabel();
            this.tagIndicatorLabel = new MarineRadioRTP.IndicatorLabel();
            this.gpsIndicatorLabel = new MarineRadioRTP.IndicatorLabel();
            this.channelLabel = new MarineRadioRTP.LcdLabel();
            this.clearButton = new MarineRadioRTP.MarineRadioButton();
            this.handsetChanDown = new MarineRadioRTP.MarineRadioButton();
            this.handsetChanUp = new MarineRadioRTP.MarineRadioButton();
            this.marineRadioButton1 = new MarineRadioRTP.MarineRadioButton();
            this.dualChButton = new MarineRadioRTP.MarineRadioButton();
            this.scanButton = new MarineRadioRTP.MarineRadioButton();
            this.sixteenButton = new MarineRadioRTP.MarineRadioButton();
            this.entButton = new MarineRadioRTP.MarineRadioButton();
            this.menuButton = new MarineRadioRTP.CyclingMarineRadioButton();
            this.pictureBox2 = new MarineRadioRTP.ICM411Knob();
            this.pictureBox1 = new MarineRadioRTP.ICM411Knob();
            this.distressButton = new MarineRadioRTP.MarineRadioButton();
            this.chanDownButton = new MarineRadioRTP.MarineRadioButton();
            this.chanUpButton = new MarineRadioRTP.MarineRadioButton();
            this.lcdPanel.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lcdPanel
            // 
            this.lcdPanel.BackColor = System.Drawing.Color.DimGray;
            this.lcdPanel.Controls.Add(this.rightTopLabel);
            this.lcdPanel.Controls.Add(this.dscIndicatorLabel);
            this.lcdPanel.Controls.Add(this.txIndicatorLabel);
            this.lcdPanel.Controls.Add(this.busyIndicatorLabel);
            this.lcdPanel.Controls.Add(this.callIndicatorLabel);
            this.lcdPanel.Controls.Add(this.usaIndicatorLabel);
            this.lcdPanel.Controls.Add(this.intIndicatorLabel);
            this.lcdPanel.Controls.Add(this.posReplyIndicatorLabel);
            this.lcdPanel.Controls.Add(this.tagIndicatorLabel);
            this.lcdPanel.Controls.Add(this.gpsIndicatorLabel);
            this.lcdPanel.Controls.Add(this.channelLabel);
            this.lcdPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcdPanel.Location = new System.Drawing.Point(376, 367);
            this.lcdPanel.Name = "lcdPanel";
            this.lcdPanel.Size = new System.Drawing.Size(308, 121);
            this.lcdPanel.TabIndex = 22;
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Marine Radio Simulator";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAISRadarToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(157, 76);
            // 
            // showAISRadarToolStripMenuItem
            // 
            this.showAISRadarToolStripMenuItem.Name = "showAISRadarToolStripMenuItem";
            this.showAISRadarToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.showAISRadarToolStripMenuItem.Text = "Show AIS Radar";
            this.showAISRadarToolStripMenuItem.Click += new System.EventHandler(this.showAISRadarToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(153, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(878, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 24;
            this.button1.Text = "Options";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(878, 42);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 23);
            this.button2.TabIndex = 25;
            this.button2.Text = "Show AIS Radar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.showAISRadarToolStripMenuItem_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(878, 72);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(115, 23);
            this.button3.TabIndex = 26;
            this.button3.Text = "Exit";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // talkButton
            // 
            this.talkButton.BackColor = System.Drawing.Color.Transparent;
            this.talkButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.talkButton.IsHeld = false;
            this.talkButton.IsHoldable = true;
            this.talkButton.Location = new System.Drawing.Point(266, 167);
            this.talkButton.Name = "talkButton";
            this.talkButton.Size = new System.Drawing.Size(101, 92);
            this.talkButton.TabIndex = 23;
            this.talkButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.marineRadioButton4_MouseClick);
            // 
            // rightTopLabel
            // 
            this.rightTopLabel.AutoSize = true;
            this.rightTopLabel.BackColor = System.Drawing.Color.Transparent;
            this.rightTopLabel.Blinking = false;
            this.rightTopLabel.BlinkInterval = 250;
            this.rightTopLabel.Font = new System.Drawing.Font("Digital-7", 27.75F);
            this.rightTopLabel.Location = new System.Drawing.Point(99, 0);
            this.rightTopLabel.MaxWidth = 215;
            this.rightTopLabel.Name = "rightTopLabel";
            this.rightTopLabel.ScrollInterval = 400;
            this.rightTopLabel.Size = new System.Drawing.Size(0, 38);
            this.rightTopLabel.TabIndex = 21;
            // 
            // dscIndicatorLabel
            // 
            this.dscIndicatorLabel.AutoSize = true;
            this.dscIndicatorLabel.BackColor = System.Drawing.Color.Black;
            this.dscIndicatorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dscIndicatorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(140)))), ((int)(((byte)(35)))));
            this.dscIndicatorLabel.Location = new System.Drawing.Point(218, 81);
            this.dscIndicatorLabel.Name = "dscIndicatorLabel";
            this.dscIndicatorLabel.Size = new System.Drawing.Size(32, 13);
            this.dscIndicatorLabel.TabIndex = 23;
            this.dscIndicatorLabel.Text = "DSC";
            this.dscIndicatorLabel.Visible = false;
            // 
            // txIndicatorLabel
            // 
            this.txIndicatorLabel.AutoSize = true;
            this.txIndicatorLabel.BackColor = System.Drawing.Color.Black;
            this.txIndicatorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txIndicatorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(140)))), ((int)(((byte)(35)))));
            this.txIndicatorLabel.Location = new System.Drawing.Point(9, 14);
            this.txIndicatorLabel.Name = "txIndicatorLabel";
            this.txIndicatorLabel.Size = new System.Drawing.Size(23, 13);
            this.txIndicatorLabel.TabIndex = 22;
            this.txIndicatorLabel.Text = "TX";
            this.txIndicatorLabel.Visible = false;
            // 
            // busyIndicatorLabel
            // 
            this.busyIndicatorLabel.AutoSize = true;
            this.busyIndicatorLabel.BackColor = System.Drawing.Color.Black;
            this.busyIndicatorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.busyIndicatorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(140)))), ((int)(((byte)(35)))));
            this.busyIndicatorLabel.Location = new System.Drawing.Point(9, 32);
            this.busyIndicatorLabel.Name = "busyIndicatorLabel";
            this.busyIndicatorLabel.Size = new System.Drawing.Size(40, 13);
            this.busyIndicatorLabel.TabIndex = 22;
            this.busyIndicatorLabel.Text = "BUSY";
            this.busyIndicatorLabel.Visible = false;
            // 
            // callIndicatorLabel
            // 
            this.callIndicatorLabel.AutoSize = true;
            this.callIndicatorLabel.BackColor = System.Drawing.Color.Black;
            this.callIndicatorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.callIndicatorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(140)))), ((int)(((byte)(35)))));
            this.callIndicatorLabel.Location = new System.Drawing.Point(9, 52);
            this.callIndicatorLabel.Name = "callIndicatorLabel";
            this.callIndicatorLabel.Size = new System.Drawing.Size(37, 13);
            this.callIndicatorLabel.TabIndex = 22;
            this.callIndicatorLabel.Text = "CALL";
            this.callIndicatorLabel.Visible = false;
            // 
            // usaIndicatorLabel
            // 
            this.usaIndicatorLabel.AutoSize = true;
            this.usaIndicatorLabel.BackColor = System.Drawing.Color.Black;
            this.usaIndicatorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usaIndicatorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(140)))), ((int)(((byte)(35)))));
            this.usaIndicatorLabel.Location = new System.Drawing.Point(9, 71);
            this.usaIndicatorLabel.Name = "usaIndicatorLabel";
            this.usaIndicatorLabel.Size = new System.Drawing.Size(32, 13);
            this.usaIndicatorLabel.TabIndex = 22;
            this.usaIndicatorLabel.Text = "USA";
            this.usaIndicatorLabel.Visible = false;
            // 
            // intIndicatorLabel
            // 
            this.intIndicatorLabel.AutoSize = true;
            this.intIndicatorLabel.BackColor = System.Drawing.Color.Black;
            this.intIndicatorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intIndicatorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(140)))), ((int)(((byte)(35)))));
            this.intIndicatorLabel.Location = new System.Drawing.Point(9, 90);
            this.intIndicatorLabel.Name = "intIndicatorLabel";
            this.intIndicatorLabel.Size = new System.Drawing.Size(28, 13);
            this.intIndicatorLabel.TabIndex = 22;
            this.intIndicatorLabel.Text = "INT";
            this.intIndicatorLabel.Visible = false;
            // 
            // posReplyIndicatorLabel
            // 
            this.posReplyIndicatorLabel.AutoSize = true;
            this.posReplyIndicatorLabel.BackColor = System.Drawing.Color.Black;
            this.posReplyIndicatorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posReplyIndicatorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(140)))), ((int)(((byte)(35)))));
            this.posReplyIndicatorLabel.Location = new System.Drawing.Point(177, 100);
            this.posReplyIndicatorLabel.Name = "posReplyIndicatorLabel";
            this.posReplyIndicatorLabel.Size = new System.Drawing.Size(76, 13);
            this.posReplyIndicatorLabel.TabIndex = 22;
            this.posReplyIndicatorLabel.Text = "POS REPLY";
            this.posReplyIndicatorLabel.Visible = false;
            // 
            // tagIndicatorLabel
            // 
            this.tagIndicatorLabel.AutoSize = true;
            this.tagIndicatorLabel.BackColor = System.Drawing.Color.Black;
            this.tagIndicatorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tagIndicatorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(140)))), ((int)(((byte)(35)))));
            this.tagIndicatorLabel.Location = new System.Drawing.Point(160, 38);
            this.tagIndicatorLabel.Name = "tagIndicatorLabel";
            this.tagIndicatorLabel.Size = new System.Drawing.Size(32, 13);
            this.tagIndicatorLabel.TabIndex = 22;
            this.tagIndicatorLabel.Text = "TAG";
            this.tagIndicatorLabel.Visible = false;
            // 
            // gpsIndicatorLabel
            // 
            this.gpsIndicatorLabel.AutoSize = true;
            this.gpsIndicatorLabel.BackColor = System.Drawing.Color.Black;
            this.gpsIndicatorLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gpsIndicatorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(140)))), ((int)(((byte)(35)))));
            this.gpsIndicatorLabel.Location = new System.Drawing.Point(177, 81);
            this.gpsIndicatorLabel.Name = "gpsIndicatorLabel";
            this.gpsIndicatorLabel.Size = new System.Drawing.Size(32, 13);
            this.gpsIndicatorLabel.TabIndex = 22;
            this.gpsIndicatorLabel.Text = "GPS";
            this.gpsIndicatorLabel.Visible = false;
            // 
            // channelLabel
            // 
            this.channelLabel.AutoSize = true;
            this.channelLabel.Blinking = false;
            this.channelLabel.BlinkInterval = 250;
            this.channelLabel.Font = new System.Drawing.Font("Digital-7", 84.74999F);
            this.channelLabel.Location = new System.Drawing.Point(30, 21);
            this.channelLabel.MaxWidth = -1;
            this.channelLabel.Name = "channelLabel";
            this.channelLabel.ScrollInterval = 1000;
            this.channelLabel.Size = new System.Drawing.Size(0, 113);
            this.channelLabel.TabIndex = 24;
            // 
            // clearButton
            // 
            this.clearButton.BackColor = System.Drawing.Color.Transparent;
            this.clearButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.clearButton.IsHeld = false;
            this.clearButton.IsHoldable = false;
            this.clearButton.Location = new System.Drawing.Point(489, 592);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(71, 46);
            this.clearButton.TabIndex = 20;
            // 
            // handsetChanDown
            // 
            this.handsetChanDown.BackColor = System.Drawing.Color.Transparent;
            this.handsetChanDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.handsetChanDown.IsHeld = false;
            this.handsetChanDown.IsHoldable = true;
            this.handsetChanDown.Location = new System.Drawing.Point(610, 118);
            this.handsetChanDown.Name = "handsetChanDown";
            this.handsetChanDown.Size = new System.Drawing.Size(38, 35);
            this.handsetChanDown.TabIndex = 19;
            // 
            // handsetChanUp
            // 
            this.handsetChanUp.BackColor = System.Drawing.Color.Transparent;
            this.handsetChanUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.handsetChanUp.IsHeld = false;
            this.handsetChanUp.IsHoldable = true;
            this.handsetChanUp.Location = new System.Drawing.Point(566, 113);
            this.handsetChanUp.Name = "handsetChanUp";
            this.handsetChanUp.Size = new System.Drawing.Size(37, 40);
            this.handsetChanUp.TabIndex = 18;
            // 
            // marineRadioButton1
            // 
            this.marineRadioButton1.BackColor = System.Drawing.Color.Transparent;
            this.marineRadioButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.marineRadioButton1.IsHeld = false;
            this.marineRadioButton1.IsHoldable = false;
            this.marineRadioButton1.Location = new System.Drawing.Point(578, 61);
            this.marineRadioButton1.Name = "marineRadioButton1";
            this.marineRadioButton1.Size = new System.Drawing.Size(45, 45);
            this.marineRadioButton1.TabIndex = 17;
            // 
            // dualChButton
            // 
            this.dualChButton.BackColor = System.Drawing.Color.Transparent;
            this.dualChButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.dualChButton.IsHeld = false;
            this.dualChButton.IsHoldable = false;
            this.dualChButton.Location = new System.Drawing.Point(488, 531);
            this.dualChButton.Name = "dualChButton";
            this.dualChButton.Size = new System.Drawing.Size(72, 48);
            this.dualChButton.TabIndex = 16;
            // 
            // scanButton
            // 
            this.scanButton.BackColor = System.Drawing.Color.Transparent;
            this.scanButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.scanButton.IsHeld = false;
            this.scanButton.IsHoldable = false;
            this.scanButton.Location = new System.Drawing.Point(405, 592);
            this.scanButton.Name = "scanButton";
            this.scanButton.Size = new System.Drawing.Size(77, 46);
            this.scanButton.TabIndex = 16;
            // 
            // sixteenButton
            // 
            this.sixteenButton.BackColor = System.Drawing.Color.Transparent;
            this.sixteenButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sixteenButton.IsHeld = false;
            this.sixteenButton.IsHoldable = true;
            this.sixteenButton.Location = new System.Drawing.Point(405, 533);
            this.sixteenButton.Name = "sixteenButton";
            this.sixteenButton.Size = new System.Drawing.Size(77, 45);
            this.sixteenButton.TabIndex = 15;
            // 
            // entButton
            // 
            this.entButton.BackColor = System.Drawing.Color.Transparent;
            this.entButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.entButton.IsHeld = false;
            this.entButton.IsHoldable = false;
            this.entButton.Location = new System.Drawing.Point(566, 592);
            this.entButton.Name = "entButton";
            this.entButton.Size = new System.Drawing.Size(76, 51);
            this.entButton.TabIndex = 14;
            // 
            // menuButton
            // 
            this.menuButton.BackColor = System.Drawing.Color.Transparent;
            this.menuButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.menuButton.IsHeld = false;
            this.menuButton.IsHoldable = true;
            this.menuButton.Location = new System.Drawing.Point(566, 533);
            this.menuButton.Name = "menuButton";
            this.menuButton.SelfCycling = false;
            this.menuButton.Size = new System.Drawing.Size(76, 43);
            this.menuButton.TabIndex = 13;
            this.menuButton.MenuCycled += new System.EventHandler<MarineRadioRTP.CyclingMenuItem>(this.MenuCycled);
            this.menuButton.CyclingMenuClick += new System.EventHandler(this.menuButton_CyclingMenuClick);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.BackgroundImage")));
            this.pictureBox2.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.pictureBox2.Location = new System.Drawing.Point(750, 529);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(122, 123);
            this.pictureBox2.TabIndex = 12;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.pictureBox1.Location = new System.Drawing.Point(750, 318);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(122, 123);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.KnobChange += new System.EventHandler<MarineRadioRTP.KnobChangeEventArgs>(this.pictureBox1_KnobChange);
            // 
            // distressButton
            // 
            this.distressButton.BackColor = System.Drawing.Color.Transparent;
            this.distressButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("distressButton.BackgroundImage")));
            this.distressButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.distressButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.distressButton.IsHeld = false;
            this.distressButton.IsHoldable = false;
            this.distressButton.Location = new System.Drawing.Point(651, 538);
            this.distressButton.Name = "distressButton";
            this.distressButton.Size = new System.Drawing.Size(84, 84);
            this.distressButton.TabIndex = 11;
            this.distressButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.distressPanel_MouseClick);
            this.distressButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.distressButton_MouseDown);
            this.distressButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.distressButton_MouseUp);
            // 
            // chanDownButton
            // 
            this.chanDownButton.BackColor = System.Drawing.Color.Transparent;
            this.chanDownButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chanDownButton.IsHeld = false;
            this.chanDownButton.IsHoldable = true;
            this.chanDownButton.Location = new System.Drawing.Point(326, 592);
            this.chanDownButton.Name = "chanDownButton";
            this.chanDownButton.Size = new System.Drawing.Size(72, 46);
            this.chanDownButton.TabIndex = 10;
            // 
            // chanUpButton
            // 
            this.chanUpButton.BackColor = System.Drawing.Color.Transparent;
            this.chanUpButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chanUpButton.IsHeld = false;
            this.chanUpButton.IsHoldable = true;
            this.chanUpButton.Location = new System.Drawing.Point(329, 532);
            this.chanUpButton.Name = "chanUpButton";
            this.chanUpButton.Size = new System.Drawing.Size(69, 48);
            this.chanUpButton.TabIndex = 9;
            // 
            // ICM411
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1005, 694);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.talkButton);
            this.Controls.Add(this.lcdPanel);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.handsetChanDown);
            this.Controls.Add(this.handsetChanUp);
            this.Controls.Add(this.marineRadioButton1);
            this.Controls.Add(this.dualChButton);
            this.Controls.Add(this.scanButton);
            this.Controls.Add(this.sixteenButton);
            this.Controls.Add(this.entButton);
            this.Controls.Add(this.menuButton);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.distressButton);
            this.Controls.Add(this.chanDownButton);
            this.Controls.Add(this.chanUpButton);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ICM411";
            this.Text = "IC-M411 Marine Radio Simulator";
            this.Load += new System.EventHandler(this.ICM411_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ICM411_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ICM411_KeyDown);
            this.lcdPanel.ResumeLayout(false);
            this.lcdPanel.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MarineRadioButton distressButton;
        private MarineRadioRTP.ICM411Knob pictureBox1;
        public MarineRadioButton entButton;
        public MarineRadioButton chanUpButton;
        public MarineRadioButton chanDownButton;
        public CyclingMarineRadioButton menuButton;
        public MarineRadioButton sixteenButton;
        public MarineRadioButton scanButton;
        public MarineRadioButton dualChButton;
        private MarineRadioButton marineRadioButton1;
        private MarineRadioButton handsetChanUp;
        private MarineRadioButton handsetChanDown;
        private MarineRadioButton clearButton;
        private System.Windows.Forms.Panel lcdPanel;
        private MarineRadioRTP.LcdLabel rightTopLabel;
        private MarineRadioRTP.ICM411Knob pictureBox2;
        private IndicatorLabel gpsIndicatorLabel;
        private IndicatorLabel dscIndicatorLabel;
        private IndicatorLabel intIndicatorLabel;
        private IndicatorLabel txIndicatorLabel;
        private IndicatorLabel busyIndicatorLabel;
        private IndicatorLabel callIndicatorLabel;
        private IndicatorLabel usaIndicatorLabel;
        private IndicatorLabel posReplyIndicatorLabel;
        private LcdLabel channelLabel;
        private MarineRadioButton talkButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showAISRadarToolStripMenuItem;
        private IndicatorLabel tagIndicatorLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}