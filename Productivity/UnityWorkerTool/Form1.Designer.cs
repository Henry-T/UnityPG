namespace UnityWorkerTool
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.miConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBuildAndroid = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBuildIOS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBuildWeb = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPackageAndroid = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPackageIOS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPackageWeb = new System.Windows.Forms.ToolStripMenuItem();
            this.tbCommand = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.listWorkQueue = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbInfoSlaveName = new System.Windows.Forms.Label();
            this.lbInfoCmd = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbInfoUnityCmd = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSendFullHour = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miConnect});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(852, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // miConnect
            // 
            this.miConnect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBuildAndroid,
            this.menuBuildIOS,
            this.menuBuildWeb,
            this.menuPackageAndroid,
            this.menuPackageIOS,
            this.menuPackageWeb});
            this.miConnect.Name = "miConnect";
            this.miConnect.Size = new System.Drawing.Size(68, 21);
            this.miConnect.Text = "命令示例";
            // 
            // menuBuildAndroid
            // 
            this.menuBuildAndroid.Name = "menuBuildAndroid";
            this.menuBuildAndroid.Size = new System.Drawing.Size(177, 22);
            this.menuBuildAndroid.Text = "Build_Android";
            this.menuBuildAndroid.Click += new System.EventHandler(this.menuBuildAndroid_Click);
            // 
            // menuBuildIOS
            // 
            this.menuBuildIOS.Name = "menuBuildIOS";
            this.menuBuildIOS.Size = new System.Drawing.Size(177, 22);
            this.menuBuildIOS.Text = "Build_IOS";
            this.menuBuildIOS.Click += new System.EventHandler(this.menuBuildIOS_Click);
            // 
            // menuBuildWeb
            // 
            this.menuBuildWeb.Name = "menuBuildWeb";
            this.menuBuildWeb.Size = new System.Drawing.Size(177, 22);
            this.menuBuildWeb.Text = "Build_Web";
            this.menuBuildWeb.Click += new System.EventHandler(this.menuBuildWeb_Click);
            // 
            // menuPackageAndroid
            // 
            this.menuPackageAndroid.Name = "menuPackageAndroid";
            this.menuPackageAndroid.Size = new System.Drawing.Size(177, 22);
            this.menuPackageAndroid.Text = "Package_Android";
            this.menuPackageAndroid.Click += new System.EventHandler(this.menuPackageAndroid_Click);
            // 
            // menuPackageIOS
            // 
            this.menuPackageIOS.Name = "menuPackageIOS";
            this.menuPackageIOS.Size = new System.Drawing.Size(177, 22);
            this.menuPackageIOS.Text = "Pakcage_IOS";
            this.menuPackageIOS.Click += new System.EventHandler(this.menuPackageIOS_Click);
            // 
            // menuPackageWeb
            // 
            this.menuPackageWeb.Name = "menuPackageWeb";
            this.menuPackageWeb.Size = new System.Drawing.Size(177, 22);
            this.menuPackageWeb.Text = "Package_Web";
            this.menuPackageWeb.Click += new System.EventHandler(this.menuPackageWeb_Click);
            // 
            // tbCommand
            // 
            this.tbCommand.Location = new System.Drawing.Point(14, 423);
            this.tbCommand.Name = "tbCommand";
            this.tbCommand.Size = new System.Drawing.Size(600, 21);
            this.tbCommand.TabIndex = 2;
            this.tbCommand.Text = "-slave build_web -platform web -command build";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(620, 421);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(105, 23);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // listWorkQueue
            // 
            this.listWorkQueue.FormattingEnabled = true;
            this.listWorkQueue.ItemHeight = 12;
            this.listWorkQueue.Location = new System.Drawing.Point(12, 55);
            this.listWorkQueue.Name = "listWorkQueue";
            this.listWorkQueue.Size = new System.Drawing.Size(284, 340);
            this.listWorkQueue.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "事务队列";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(305, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "事务详情";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(305, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "奴隶名称";
            // 
            // lbInfoSlaveName
            // 
            this.lbInfoSlaveName.AutoSize = true;
            this.lbInfoSlaveName.Location = new System.Drawing.Point(305, 80);
            this.lbInfoSlaveName.Name = "lbInfoSlaveName";
            this.lbInfoSlaveName.Size = new System.Drawing.Size(65, 12);
            this.lbInfoSlaveName.TabIndex = 8;
            this.lbInfoSlaveName.Text = "----------";
            // 
            // lbInfoCmd
            // 
            this.lbInfoCmd.AutoSize = true;
            this.lbInfoCmd.Location = new System.Drawing.Point(305, 139);
            this.lbInfoCmd.Name = "lbInfoCmd";
            this.lbInfoCmd.Size = new System.Drawing.Size(65, 12);
            this.lbInfoCmd.TabIndex = 10;
            this.lbInfoCmd.Text = "----------";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(305, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "工具命令";
            // 
            // lbInfoUnityCmd
            // 
            this.lbInfoUnityCmd.AutoSize = true;
            this.lbInfoUnityCmd.Location = new System.Drawing.Point(305, 216);
            this.lbInfoUnityCmd.Name = "lbInfoUnityCmd";
            this.lbInfoUnityCmd.Size = new System.Drawing.Size(77, 12);
            this.lbInfoUnityCmd.TabIndex = 12;
            this.lbInfoUnityCmd.Text = "------------";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(305, 191);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 12);
            this.label7.TabIndex = 11;
            this.label7.Text = "Unity命令";
            // 
            // tbLog
            // 
            this.tbLog.Location = new System.Drawing.Point(465, 52);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(375, 343);
            this.tbLog.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(463, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "信息";
            // 
            // btnSendFullHour
            // 
            this.btnSendFullHour.Location = new System.Drawing.Point(735, 421);
            this.btnSendFullHour.Name = "btnSendFullHour";
            this.btnSendFullHour.Size = new System.Drawing.Size(105, 23);
            this.btnSendFullHour.TabIndex = 15;
            this.btnSendFullHour.Text = "每小时发送";
            this.btnSendFullHour.UseVisualStyleBackColor = true;
            this.btnSendFullHour.Click += new System.EventHandler(this.btnSendFullHour_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 456);
            this.Controls.Add(this.btnSendFullHour);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.lbInfoUnityCmd);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lbInfoCmd);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lbInfoSlaveName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listWorkQueue);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.tbCommand);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "UnityWorker助手";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem miConnect;
        private System.Windows.Forms.TextBox tbCommand;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ToolStripMenuItem menuBuildAndroid;
        private System.Windows.Forms.ToolStripMenuItem menuBuildIOS;
        private System.Windows.Forms.ToolStripMenuItem menuBuildWeb;
        private System.Windows.Forms.ToolStripMenuItem menuPackageAndroid;
        private System.Windows.Forms.ToolStripMenuItem menuPackageIOS;
        private System.Windows.Forms.ToolStripMenuItem menuPackageWeb;
        private System.Windows.Forms.ListBox listWorkQueue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbInfoSlaveName;
        private System.Windows.Forms.Label lbInfoCmd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbInfoUnityCmd;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSendFullHour;
    }
}

