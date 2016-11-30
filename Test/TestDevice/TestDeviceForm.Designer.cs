namespace TestDevice
{
    partial class TestDeviceForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btCom = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbbBaud = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbbCom = new System.Windows.Forms.ComboBox();
            this.btNet = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbbPort = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbbIP = new System.Windows.Forms.ComboBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btSendFile = new System.Windows.Forms.Button();
            this.btSelect = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // btCom
            // 
            this.btCom.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btCom.Location = new System.Drawing.Point(367, 36);
            this.btCom.Name = "btCom";
            this.btCom.Size = new System.Drawing.Size(106, 26);
            this.btCom.TabIndex = 33;
            this.btCom.Text = "打开串口";
            this.btCom.UseVisualStyleBackColor = true;
            this.btCom.Click += new System.EventHandler(this.btCom_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(231, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 32;
            this.label2.Text = "Com Baud";
            // 
            // cbbBaud
            // 
            this.cbbBaud.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbBaud.FormattingEnabled = true;
            this.cbbBaud.Items.AddRange(new object[] {
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600"});
            this.cbbBaud.Location = new System.Drawing.Point(306, 36);
            this.cbbBaud.Name = "cbbBaud";
            this.cbbBaud.Size = new System.Drawing.Size(55, 24);
            this.cbbBaud.TabIndex = 31;
            this.cbbBaud.Text = "9600";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(8, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 30;
            this.label1.Text = "Com Port";
            // 
            // cbbCom
            // 
            this.cbbCom.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbCom.FormattingEnabled = true;
            this.cbbCom.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12"});
            this.cbbCom.Location = new System.Drawing.Point(102, 36);
            this.cbbCom.Name = "cbbCom";
            this.cbbCom.Size = new System.Drawing.Size(99, 24);
            this.cbbCom.TabIndex = 29;
            this.cbbCom.Text = "COM1";
            // 
            // btNet
            // 
            this.btNet.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btNet.Location = new System.Drawing.Point(367, 68);
            this.btNet.Name = "btNet";
            this.btNet.Size = new System.Drawing.Size(50, 26);
            this.btNet.TabIndex = 38;
            this.btNet.Text = "连接网络";
            this.btNet.UseVisualStyleBackColor = true;
            this.btNet.Click += new System.EventHandler(this.btNet_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(207, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 16);
            this.label3.TabIndex = 37;
            this.label3.Text = "Remote Port";
            // 
            // cbbPort
            // 
            this.cbbPort.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbPort.FormattingEnabled = true;
            this.cbbPort.Items.AddRange(new object[] {
            "6688"});
            this.cbbPort.Location = new System.Drawing.Point(306, 68);
            this.cbbPort.Name = "cbbPort";
            this.cbbPort.Size = new System.Drawing.Size(55, 24);
            this.cbbPort.TabIndex = 36;
            this.cbbPort.Text = "6688";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(8, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 16);
            this.label4.TabIndex = 35;
            this.label4.Text = "Remote IP";
            // 
            // cbbIP
            // 
            this.cbbIP.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbbIP.FormattingEnabled = true;
            this.cbbIP.Items.AddRange(new object[] {
            "127.0.0.1"});
            this.cbbIP.Location = new System.Drawing.Point(102, 68);
            this.cbbIP.Name = "cbbIP";
            this.cbbIP.Size = new System.Drawing.Size(99, 24);
            this.cbbIP.TabIndex = 34;
            this.cbbIP.Text = "127.0.0.1";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listBox1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 19;
            this.listBox1.Location = new System.Drawing.Point(0, 135);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(477, 270);
            this.listBox1.TabIndex = 39;
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(8, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 16);
            this.label5.TabIndex = 41;
            this.label5.Text = "DeviceCode";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown1.Location = new System.Drawing.Point(102, 6);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(99, 26);
            this.numericUpDown1.TabIndex = 42;
            // 
            // txtFilePath
            // 
            this.txtFilePath.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtFilePath.Location = new System.Drawing.Point(12, 99);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(291, 26);
            this.txtFilePath.TabIndex = 44;
            // 
            // btSendFile
            // 
            this.btSendFile.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btSendFile.Location = new System.Drawing.Point(367, 99);
            this.btSendFile.Name = "btSendFile";
            this.btSendFile.Size = new System.Drawing.Size(106, 26);
            this.btSendFile.TabIndex = 45;
            this.btSendFile.Text = "发送文件";
            this.btSendFile.UseVisualStyleBackColor = true;
            this.btSendFile.Click += new System.EventHandler(this.btSendFile_Click);
            // 
            // btSelect
            // 
            this.btSelect.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btSelect.Location = new System.Drawing.Point(306, 98);
            this.btSelect.Name = "btSelect";
            this.btSelect.Size = new System.Drawing.Size(55, 27);
            this.btSelect.TabIndex = 46;
            this.btSelect.Text = "...";
            this.btSelect.UseVisualStyleBackColor = true;
            this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(423, 68);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 26);
            this.button1.TabIndex = 47;
            this.button1.Text = "发送";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TestDeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 405);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btSelect);
            this.Controls.Add(this.btSendFile);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btNet);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbbPort);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbbIP);
            this.Controls.Add(this.btCom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbbBaud);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbbCom);
            this.MaximizeBox = false;
            this.Name = "TestDeviceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "模拟终端设备";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btCom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbbBaud;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbbCom;
        private System.Windows.Forms.Button btNet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbbPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbbIP;
        private System.Windows.Forms.ListBox listBox1;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btSendFile;
        private System.Windows.Forms.Button btSelect;
        private System.Windows.Forms.Button button1;
    }
}

