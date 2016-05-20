namespace UnityWorkerManager
{
    partial class ManagerForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.常用命令ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBuildAndroid = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBuildIOS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBuildWeb = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPackageAndroid = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPackageIOS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPackageWeb = new System.Windows.Forms.ToolStripMenuItem();
            this.同步BackgroundWorkerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLinkAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLinkClear = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLinkOverrite = new System.Windows.Forms.ToolStripMenuItem();
            this.常用ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnBuildAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDoPostBuildCmd = new System.Windows.Forms.ToolStripMenuItem();
            this.cbRunPostCmdAfterBuild = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrCheckWorkDone = new System.Windows.Forms.Timer(this.components);
            this.btnCommonCmd = new System.Windows.Forms.Button();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCmd = new System.Windows.Forms.Button();
            this.tbInput = new System.Windows.Forms.TextBox();
            this.tbOutput = new System.Windows.Forms.TextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnBuildSWB = new System.Windows.Forms.ToolStripMenuItem();
            this.btnBuildTianshen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.常用ToolStripMenuItem,
            this.同步BackgroundWorkerToolStripMenuItem,
            this.常用命令ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(344, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 常用命令ToolStripMenuItem
            // 
            this.常用命令ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuBuildAndroid,
            this.menuBuildIOS,
            this.menuBuildWeb,
            this.menuPackageAndroid,
            this.menuPackageIOS,
            this.menuPackageWeb});
            this.常用命令ToolStripMenuItem.Name = "常用命令ToolStripMenuItem";
            this.常用命令ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.常用命令ToolStripMenuItem.Text = "工具";
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
            this.menuPackageIOS.Text = "Package_IOS";
            this.menuPackageIOS.Click += new System.EventHandler(this.menuPackageIOS_Click);
            // 
            // menuPackageWeb
            // 
            this.menuPackageWeb.Name = "menuPackageWeb";
            this.menuPackageWeb.Size = new System.Drawing.Size(177, 22);
            this.menuPackageWeb.Text = "Package_Web";
            this.menuPackageWeb.Click += new System.EventHandler(this.menuPackageWeb_Click);
            // 
            // 同步BackgroundWorkerToolStripMenuItem
            // 
            this.同步BackgroundWorkerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLinkAll,
            this.btnLinkClear,
            this.btnLinkOverrite});
            this.同步BackgroundWorkerToolStripMenuItem.Name = "同步BackgroundWorkerToolStripMenuItem";
            this.同步BackgroundWorkerToolStripMenuItem.Size = new System.Drawing.Size(56, 21);
            this.同步BackgroundWorkerToolStripMenuItem.Text = "硬链接";
            // 
            // btnLinkAll
            // 
            this.btnLinkAll.Name = "btnLinkAll";
            this.btnLinkAll.Size = new System.Drawing.Size(152, 22);
            this.btnLinkAll.Text = "全部建立链接";
            this.btnLinkAll.Click += new System.EventHandler(this.btnLinkAll_Click);
            // 
            // btnLinkClear
            // 
            this.btnLinkClear.Name = "btnLinkClear";
            this.btnLinkClear.Size = new System.Drawing.Size(152, 22);
            this.btnLinkClear.Text = "全部清空链接";
            this.btnLinkClear.Click += new System.EventHandler(this.btnLinkClear_Click);
            // 
            // btnLinkOverrite
            // 
            this.btnLinkOverrite.Name = "btnLinkOverrite";
            this.btnLinkOverrite.Size = new System.Drawing.Size(152, 22);
            this.btnLinkOverrite.Text = "全部覆盖链接";
            this.btnLinkOverrite.Click += new System.EventHandler(this.btnLinkOverrite_Click);
            // 
            // 常用ToolStripMenuItem
            // 
            this.常用ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBuildSWB,
            this.btnBuildTianshen,
            this.toolStripSeparator1,
            this.btnBuildAll,
            this.btnDoPostBuildCmd,
            this.toolStripSeparator2,
            this.cbRunPostCmdAfterBuild});
            this.常用ToolStripMenuItem.Name = "常用ToolStripMenuItem";
            this.常用ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.常用ToolStripMenuItem.Text = "工程";
            // 
            // btnBuildAll
            // 
            this.btnBuildAll.Name = "btnBuildAll";
            this.btnBuildAll.Size = new System.Drawing.Size(172, 22);
            this.btnBuildAll.Text = "全部发布";
            this.btnBuildAll.Click += new System.EventHandler(this.btnBuildAll_Click);
            // 
            // btnDoPostBuildCmd
            // 
            this.btnDoPostBuildCmd.Name = "btnDoPostBuildCmd";
            this.btnDoPostBuildCmd.Size = new System.Drawing.Size(172, 22);
            this.btnDoPostBuildCmd.Text = "直接执行后处理";
            this.btnDoPostBuildCmd.Click += new System.EventHandler(this.btnDoPostBuildCmd_Click);
            // 
            // cbRunPostCmdAfterBuild
            // 
            this.cbRunPostCmdAfterBuild.Checked = true;
            this.cbRunPostCmdAfterBuild.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRunPostCmdAfterBuild.Name = "cbRunPostCmdAfterBuild";
            this.cbRunPostCmdAfterBuild.Size = new System.Drawing.Size(172, 22);
            this.cbRunPostCmdAfterBuild.Text = "发布后执行后处理";
            this.cbRunPostCmdAfterBuild.Click += new System.EventHandler(this.cbRunPostCmdAfterBuild_Click);
            // 
            // tmrCheckWorkDone
            // 
            this.tmrCheckWorkDone.Tick += new System.EventHandler(this.tmrCheckWorkDone_Tick);
            // 
            // btnCommonCmd
            // 
            this.btnCommonCmd.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCommonCmd.Location = new System.Drawing.Point(0, 364);
            this.btnCommonCmd.Name = "btnCommonCmd";
            this.btnCommonCmd.Size = new System.Drawing.Size(344, 34);
            this.btnCommonCmd.TabIndex = 5;
            this.btnCommonCmd.Text = "执行Unity命令";
            this.btnCommonCmd.UseVisualStyleBackColor = true;
            this.btnCommonCmd.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(169, 6);
            // 
            // btnCmd
            // 
            this.btnCmd.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCmd.Location = new System.Drawing.Point(0, 330);
            this.btnCmd.Name = "btnCmd";
            this.btnCmd.Size = new System.Drawing.Size(344, 34);
            this.btnCmd.TabIndex = 8;
            this.btnCmd.Text = "执行标准命令";
            this.btnCmd.UseVisualStyleBackColor = true;
            this.btnCmd.Visible = false;
            // 
            // tbInput
            // 
            this.tbInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbInput.Location = new System.Drawing.Point(0, 296);
            this.tbInput.Multiline = true;
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(344, 34);
            this.tbInput.TabIndex = 9;
            this.tbInput.Visible = false;
            // 
            // tbOutput
            // 
            this.tbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbOutput.Location = new System.Drawing.Point(0, 25);
            this.tbOutput.Multiline = true;
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutput.Size = new System.Drawing.Size(344, 271);
            this.tbOutput.TabIndex = 10;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(169, 6);
            // 
            // btnBuildSWB
            // 
            this.btnBuildSWB.Name = "btnBuildSWB";
            this.btnBuildSWB.Size = new System.Drawing.Size(172, 22);
            this.btnBuildSWB.Text = "发布 SWBuilder";
            this.btnBuildSWB.Click += new System.EventHandler(this.btnBuildSWB_Click);
            // 
            // btnBuildTianshen
            // 
            this.btnBuildTianshen.Name = "btnBuildTianshen";
            this.btnBuildTianshen.Size = new System.Drawing.Size(172, 22);
            this.btnBuildTianshen.Text = "发布 Tianshen";
            this.btnBuildTianshen.Click += new System.EventHandler(this.btnBuildTianshen_Click);
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 398);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.btnCmd);
            this.Controls.Add(this.btnCommonCmd);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ManagerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UnityWorker管理器";
            this.Load += new System.EventHandler(this.ManagerForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 常用命令ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuBuildAndroid;
        private System.Windows.Forms.ToolStripMenuItem menuBuildIOS;
        private System.Windows.Forms.ToolStripMenuItem menuBuildWeb;
        private System.Windows.Forms.ToolStripMenuItem menuPackageAndroid;
        private System.Windows.Forms.ToolStripMenuItem menuPackageIOS;
        private System.Windows.Forms.ToolStripMenuItem menuPackageWeb;
        private System.Windows.Forms.ToolStripMenuItem 同步BackgroundWorkerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnLinkAll;
        private System.Windows.Forms.ToolStripMenuItem btnLinkClear;
        private System.Windows.Forms.ToolStripMenuItem btnLinkOverrite;
        private System.Windows.Forms.ToolStripMenuItem 常用ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnBuildAll;
        private System.Windows.Forms.ToolStripMenuItem btnDoPostBuildCmd;
        private System.Windows.Forms.Timer tmrCheckWorkDone;
        private System.Windows.Forms.Button btnCommonCmd;
        private System.Windows.Forms.ToolStripMenuItem cbRunPostCmdAfterBuild;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button btnCmd;
        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.TextBox tbOutput;
        private System.Windows.Forms.ToolStripMenuItem btnBuildSWB;
        private System.Windows.Forms.ToolStripMenuItem btnBuildTianshen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}