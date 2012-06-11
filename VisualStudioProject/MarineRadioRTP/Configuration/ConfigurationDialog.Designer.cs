namespace MarineRadioRTP
{
    partial class ConfigurationDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigurationDialog));
            this.multicastNetworkTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.multiCastSubnetValue = new System.Windows.Forms.NumericUpDown();
            this.strategyDescription = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.compressDecompressCheckBox = new System.Windows.Forms.CheckBox();
            this.enableSoundEffectsCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.multiCastSubnetValue)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // multicastNetworkTextBox
            // 
            this.multicastNetworkTextBox.Location = new System.Drawing.Point(136, 10);
            this.multicastNetworkTextBox.Name = "multicastNetworkTextBox";
            this.multicastNetworkTextBox.Size = new System.Drawing.Size(144, 20);
            this.multicastNetworkTextBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Multicast Network";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Subnet (CIDR notation)";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(331, 204);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.multiCastSubnetValue);
            this.tabPage1.Controls.Add(this.strategyDescription);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.multicastNetworkTextBox);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(323, 178);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Network environment";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // multiCastSubnetValue
            // 
            this.multiCastSubnetValue.Location = new System.Drawing.Point(136, 37);
            this.multiCastSubnetValue.Maximum = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.multiCastSubnetValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.multiCastSubnetValue.Name = "multiCastSubnetValue";
            this.multiCastSubnetValue.Size = new System.Drawing.Size(51, 20);
            this.multiCastSubnetValue.TabIndex = 3;
            this.multiCastSubnetValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // strategyDescription
            // 
            this.strategyDescription.Location = new System.Drawing.Point(9, 72);
            this.strategyDescription.Multiline = true;
            this.strategyDescription.Name = "strategyDescription";
            this.strategyDescription.ReadOnly = true;
            this.strategyDescription.Size = new System.Drawing.Size(305, 100);
            this.strategyDescription.TabIndex = 2;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.compressDecompressCheckBox);
            this.tabPage2.Controls.Add(this.enableSoundEffectsCheckBox);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.listBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(323, 178);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Audio";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // compressDecompressCheckBox
            // 
            this.compressDecompressCheckBox.AutoSize = true;
            this.compressDecompressCheckBox.Location = new System.Drawing.Point(9, 136);
            this.compressDecompressCheckBox.Name = "compressDecompressCheckBox";
            this.compressDecompressCheckBox.Size = new System.Drawing.Size(202, 17);
            this.compressDecompressCheckBox.TabIndex = 3;
            this.compressDecompressCheckBox.Text = "Compress/decompress audio streams";
            this.compressDecompressCheckBox.UseVisualStyleBackColor = true;
            // 
            // enableSoundEffectsCheckBox
            // 
            this.enableSoundEffectsCheckBox.AutoSize = true;
            this.enableSoundEffectsCheckBox.Location = new System.Drawing.Point(9, 112);
            this.enableSoundEffectsCheckBox.Name = "enableSoundEffectsCheckBox";
            this.enableSoundEffectsCheckBox.Size = new System.Drawing.Size(126, 17);
            this.enableSoundEffectsCheckBox.TabIndex = 2;
            this.enableSoundEffectsCheckBox.Text = "Enable sound effects";
            this.enableSoundEffectsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Audio device:";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 37);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(308, 69);
            this.listBox1.TabIndex = 0;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(187, 222);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 5;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(268, 222);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // ConfigurationDialog
            // 
            this.AcceptButton = this.saveButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(356, 257);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConfigurationDialog";
            this.Text = "Marine Radio Simulator - Control Panel";
            this.Load += new System.EventHandler(this.ConfigurationDialog_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.multiCastSubnetValue)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox multicastNetworkTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox strategyDescription;
        private System.Windows.Forms.NumericUpDown multiCastSubnetValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox enableSoundEffectsCheckBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.CheckBox compressDecompressCheckBox;
    }
}