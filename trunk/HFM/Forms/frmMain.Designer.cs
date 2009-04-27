namespace HFM.Forms
{
   partial class frmMain
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
         System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
         System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
         System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
         this.bgWorkTimer = new System.Windows.Forms.Timer(this.components);
         this.statusStrip = new System.Windows.Forms.StatusStrip();
         this.statusLabelLeft = new System.Windows.Forms.ToolStripStatusLabel();
         this.statusLabelHosts = new System.Windows.Forms.ToolStripStatusLabel();
         this.statusLabelPPW = new System.Windows.Forms.ToolStripStatusLabel();
         this.statusLabelRight = new System.Windows.Forms.ToolStripStatusLabel();
         this.notifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.mnuNotifyRst = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuNotifyMin = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuNotifyMax = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuNotifySep1 = new System.Windows.Forms.ToolStripSeparator();
         this.mnuNotifyQuit = new System.Windows.Forms.ToolStripMenuItem();
         this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
         this.AppMenu = new System.Windows.Forms.MenuStrip();
         this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuFileSaveas = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
         this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuEditPreferences = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuClients = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuClientsAdd = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuClientsSep1 = new System.Windows.Forms.ToolStripSeparator();
         this.mnuClientsEdit = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuClientsDelete = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuClientsSep2 = new System.Windows.Forms.ToolStripSeparator();
         this.mnuClientsViewCachedLog = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuClientsViewClientFiles = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuClientsSep3 = new System.Windows.Forms.ToolStripSeparator();
         this.mnuClientsRefreshSelected = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuClientsRefreshAll = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuViewShowHideLog = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuViewSep1 = new System.Windows.Forms.ToolStripSeparator();
         this.mnuViewToggleDateTime = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuToolsMessages = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuToolsDownloadProjects = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuHelpContents = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuHelpIndex = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuHelpSep1 = new System.Windows.Forms.ToolStripSeparator();
         this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
         this.openConfigDialog = new System.Windows.Forms.OpenFileDialog();
         this.saveConfigDialog = new System.Windows.Forms.SaveFileDialog();
         this.txtLogFile = new System.Windows.Forms.TextBox();
         this.gridContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.mnuContextClientsRefreshSelected = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuContextClientsSep1 = new System.Windows.Forms.ToolStripSeparator();
         this.mnuContextClientsAdd = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuContextClientsEdit = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuContextClientsDelete = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuContextClientsSep2 = new System.Windows.Forms.ToolStripSeparator();
         this.mnuContextClientsViewCachedLog = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuContextClientsViewClientFiles = new System.Windows.Forms.ToolStripMenuItem();
         this.webGenTimer = new System.Windows.Forms.Timer(this.components);
         this.splitContainer1 = new System.Windows.Forms.SplitContainer();
         this.dataGridView1 = new System.Windows.Forms.DataGridView();
         this.statusStrip.SuspendLayout();
         this.notifyMenu.SuspendLayout();
         this.AppMenu.SuspendLayout();
         this.gridContextMenuStrip.SuspendLayout();
         this.splitContainer1.Panel1.SuspendLayout();
         this.splitContainer1.Panel2.SuspendLayout();
         this.splitContainer1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
         this.SuspendLayout();
         // 
         // bgWorkTimer
         // 
         this.bgWorkTimer.Interval = 600000;
         this.bgWorkTimer.Tick += new System.EventHandler(this.bgWorkTimer_Tick);
         // 
         // statusStrip
         // 
         this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabelLeft,
            this.statusLabelHosts,
            this.statusLabelPPW,
            this.statusLabelRight});
         this.statusStrip.Location = new System.Drawing.Point(0, 744);
         this.statusStrip.Name = "statusStrip";
         this.statusStrip.Size = new System.Drawing.Size(988, 29);
         this.statusStrip.TabIndex = 2;
         // 
         // statusLabelLeft
         // 
         this.statusLabelLeft.AutoSize = false;
         this.statusLabelLeft.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                     | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                     | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
         this.statusLabelLeft.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
         this.statusLabelLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.statusLabelLeft.Name = "statusLabelLeft";
         this.statusLabelLeft.Size = new System.Drawing.Size(839, 24);
         this.statusLabelLeft.Spring = true;
         this.statusLabelLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // statusLabelHosts
         // 
         this.statusLabelHosts.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                     | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                     | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
         this.statusLabelHosts.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
         this.statusLabelHosts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.statusLabelHosts.Name = "statusLabelHosts";
         this.statusLabelHosts.Size = new System.Drawing.Size(17, 24);
         this.statusLabelHosts.Text = "0";
         this.statusLabelHosts.ToolTipText = "Number of Hosts";
         // 
         // statusLabelPPW
         // 
         this.statusLabelPPW.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                     | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                     | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
         this.statusLabelPPW.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
         this.statusLabelPPW.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.statusLabelPPW.Name = "statusLabelPPW";
         this.statusLabelPPW.Size = new System.Drawing.Size(17, 24);
         this.statusLabelPPW.Text = "0";
         this.statusLabelPPW.ToolTipText = "Points Per Week";
         // 
         // statusLabelRight
         // 
         this.statusLabelRight.AutoSize = false;
         this.statusLabelRight.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                     | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                     | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
         this.statusLabelRight.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
         this.statusLabelRight.Name = "statusLabelRight";
         this.statusLabelRight.Size = new System.Drawing.Size(100, 24);
         this.statusLabelRight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // notifyMenu
         // 
         this.notifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNotifyRst,
            this.mnuNotifyMin,
            this.mnuNotifyMax,
            this.mnuNotifySep1,
            this.mnuNotifyQuit});
         this.notifyMenu.Name = "notifyMenu";
         this.notifyMenu.Size = new System.Drawing.Size(129, 98);
         // 
         // mnuNotifyRst
         // 
         this.mnuNotifyRst.Image = global::HFM.Properties.Resources.Restore;
         this.mnuNotifyRst.Name = "mnuNotifyRst";
         this.mnuNotifyRst.Size = new System.Drawing.Size(128, 22);
         this.mnuNotifyRst.Text = "&Restore";
         this.mnuNotifyRst.Click += new System.EventHandler(this.mnuNotifyRst_Click);
         // 
         // mnuNotifyMin
         // 
         this.mnuNotifyMin.Image = global::HFM.Properties.Resources.Minimize;
         this.mnuNotifyMin.Name = "mnuNotifyMin";
         this.mnuNotifyMin.Size = new System.Drawing.Size(128, 22);
         this.mnuNotifyMin.Text = "Mi&nimize";
         this.mnuNotifyMin.Click += new System.EventHandler(this.mnuNotifyMin_Click);
         // 
         // mnuNotifyMax
         // 
         this.mnuNotifyMax.Image = global::HFM.Properties.Resources.Maximize;
         this.mnuNotifyMax.Name = "mnuNotifyMax";
         this.mnuNotifyMax.Size = new System.Drawing.Size(128, 22);
         this.mnuNotifyMax.Text = "&Maximize";
         this.mnuNotifyMax.Click += new System.EventHandler(this.mnuNotifyMax_Click);
         // 
         // mnuNotifySep1
         // 
         this.mnuNotifySep1.Name = "mnuNotifySep1";
         this.mnuNotifySep1.Size = new System.Drawing.Size(125, 6);
         // 
         // mnuNotifyQuit
         // 
         this.mnuNotifyQuit.Image = global::HFM.Properties.Resources.Quit;
         this.mnuNotifyQuit.Name = "mnuNotifyQuit";
         this.mnuNotifyQuit.Size = new System.Drawing.Size(128, 22);
         this.mnuNotifyQuit.Text = "&Quit";
         this.mnuNotifyQuit.Click += new System.EventHandler(this.mnuFileQuit_Click);
         // 
         // notifyIcon
         // 
         this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
         this.notifyIcon.ContextMenuStrip = this.notifyMenu;
         this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
         this.notifyIcon.Text = "frmMainNew";
         this.notifyIcon.Visible = true;
         this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
         // 
         // AppMenu
         // 
         this.AppMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuClients,
            this.mnuView,
            this.mnuTools,
            this.mnuHelp});
         this.AppMenu.Location = new System.Drawing.Point(0, 0);
         this.AppMenu.Name = "AppMenu";
         this.AppMenu.Size = new System.Drawing.Size(988, 24);
         this.AppMenu.TabIndex = 4;
         this.AppMenu.Text = "App Menu";
         // 
         // mnuFile
         // 
         this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.mnuFileSave,
            this.mnuFileSaveas,
            this.mnuFileSep1,
            this.mnuFileQuit});
         this.mnuFile.Name = "mnuFile";
         this.mnuFile.Size = new System.Drawing.Size(35, 20);
         this.mnuFile.Text = "&File";
         // 
         // mnuFileNew
         // 
         this.mnuFileNew.Image = global::HFM.Properties.Resources.New;
         this.mnuFileNew.Name = "mnuFileNew";
         this.mnuFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
         this.mnuFileNew.Size = new System.Drawing.Size(260, 22);
         this.mnuFileNew.Text = "&New Configuration";
         this.mnuFileNew.ToolTipText = "Create a new configuration file";
         this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
         // 
         // mnuFileOpen
         // 
         this.mnuFileOpen.Image = global::HFM.Properties.Resources.Open;
         this.mnuFileOpen.Name = "mnuFileOpen";
         this.mnuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
         this.mnuFileOpen.Size = new System.Drawing.Size(260, 22);
         this.mnuFileOpen.Text = "&Open Configuration";
         this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
         // 
         // mnuFileSave
         // 
         this.mnuFileSave.Image = global::HFM.Properties.Resources.Save;
         this.mnuFileSave.Name = "mnuFileSave";
         this.mnuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
         this.mnuFileSave.Size = new System.Drawing.Size(260, 22);
         this.mnuFileSave.Text = "&Save Configuration";
         this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
         // 
         // mnuFileSaveas
         // 
         this.mnuFileSaveas.Image = global::HFM.Properties.Resources.SaveAs;
         this.mnuFileSaveas.Name = "mnuFileSaveas";
         this.mnuFileSaveas.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                     | System.Windows.Forms.Keys.S)));
         this.mnuFileSaveas.Size = new System.Drawing.Size(260, 22);
         this.mnuFileSaveas.Text = "Save Configuration &As";
         this.mnuFileSaveas.Click += new System.EventHandler(this.mnuFileSaveas_Click);
         // 
         // mnuFileSep1
         // 
         this.mnuFileSep1.Name = "mnuFileSep1";
         this.mnuFileSep1.Size = new System.Drawing.Size(257, 6);
         // 
         // mnuFileQuit
         // 
         this.mnuFileQuit.Image = global::HFM.Properties.Resources.Quit;
         this.mnuFileQuit.Name = "mnuFileQuit";
         this.mnuFileQuit.Size = new System.Drawing.Size(260, 22);
         this.mnuFileQuit.Text = "&Quit";
         this.mnuFileQuit.Click += new System.EventHandler(this.mnuFileQuit_Click);
         // 
         // mnuEdit
         // 
         this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditPreferences});
         this.mnuEdit.Name = "mnuEdit";
         this.mnuEdit.Size = new System.Drawing.Size(37, 20);
         this.mnuEdit.Text = "&Edit";
         // 
         // mnuEditPreferences
         // 
         this.mnuEditPreferences.Name = "mnuEditPreferences";
         this.mnuEditPreferences.Size = new System.Drawing.Size(143, 22);
         this.mnuEditPreferences.Text = "&Preferences";
         this.mnuEditPreferences.Click += new System.EventHandler(this.mnuEditPreferences_Click);
         // 
         // mnuClients
         // 
         this.mnuClients.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuClientsAdd,
            this.mnuClientsSep1,
            this.mnuClientsEdit,
            this.mnuClientsDelete,
            this.mnuClientsSep2,
            this.mnuClientsViewCachedLog,
            this.mnuClientsViewClientFiles,
            this.mnuClientsSep3,
            this.mnuClientsRefreshSelected,
            this.mnuClientsRefreshAll});
         this.mnuClients.Name = "mnuClients";
         this.mnuClients.Size = new System.Drawing.Size(51, 20);
         this.mnuClients.Text = "&Clients";
         // 
         // mnuClientsAdd
         // 
         this.mnuClientsAdd.Name = "mnuClientsAdd";
         this.mnuClientsAdd.Size = new System.Drawing.Size(221, 22);
         this.mnuClientsAdd.Text = "&Add Client";
         this.mnuClientsAdd.Click += new System.EventHandler(this.mnuClientsAdd_Click);
         // 
         // mnuClientsSep1
         // 
         this.mnuClientsSep1.Name = "mnuClientsSep1";
         this.mnuClientsSep1.Size = new System.Drawing.Size(218, 6);
         // 
         // mnuClientsEdit
         // 
         this.mnuClientsEdit.Name = "mnuClientsEdit";
         this.mnuClientsEdit.Size = new System.Drawing.Size(221, 22);
         this.mnuClientsEdit.Text = "&Edit Client";
         this.mnuClientsEdit.Click += new System.EventHandler(this.mnuClientsEdit_Click);
         // 
         // mnuClientsDelete
         // 
         this.mnuClientsDelete.Name = "mnuClientsDelete";
         this.mnuClientsDelete.Size = new System.Drawing.Size(221, 22);
         this.mnuClientsDelete.Text = "&Delete Client";
         this.mnuClientsDelete.Click += new System.EventHandler(this.mnuClientsDelete_Click);
         // 
         // mnuClientsSep2
         // 
         this.mnuClientsSep2.Name = "mnuClientsSep2";
         this.mnuClientsSep2.Size = new System.Drawing.Size(218, 6);
         // 
         // mnuClientsViewCachedLog
         // 
         this.mnuClientsViewCachedLog.Name = "mnuClientsViewCachedLog";
         this.mnuClientsViewCachedLog.Size = new System.Drawing.Size(221, 22);
         this.mnuClientsViewCachedLog.Text = "View Cached &Log File";
         this.mnuClientsViewCachedLog.Click += new System.EventHandler(this.mnuClientsViewCachedLog_Click);
         // 
         // mnuClientsViewClientFiles
         // 
         this.mnuClientsViewClientFiles.Name = "mnuClientsViewClientFiles";
         this.mnuClientsViewClientFiles.Size = new System.Drawing.Size(221, 22);
         this.mnuClientsViewClientFiles.Text = "View Client &Files (Local Only)";
         this.mnuClientsViewClientFiles.Click += new System.EventHandler(this.mnuClientsViewClientFiles_Click);
         // 
         // mnuClientsSep3
         // 
         this.mnuClientsSep3.Name = "mnuClientsSep3";
         this.mnuClientsSep3.Size = new System.Drawing.Size(218, 6);
         // 
         // mnuClientsRefreshSelected
         // 
         this.mnuClientsRefreshSelected.Name = "mnuClientsRefreshSelected";
         this.mnuClientsRefreshSelected.ShortcutKeys = System.Windows.Forms.Keys.F5;
         this.mnuClientsRefreshSelected.Size = new System.Drawing.Size(221, 22);
         this.mnuClientsRefreshSelected.Text = "Refresh &Selected";
         this.mnuClientsRefreshSelected.Click += new System.EventHandler(this.mnuClientsRefreshSelected_Click);
         // 
         // mnuClientsRefreshAll
         // 
         this.mnuClientsRefreshAll.Name = "mnuClientsRefreshAll";
         this.mnuClientsRefreshAll.ShortcutKeys = System.Windows.Forms.Keys.F6;
         this.mnuClientsRefreshAll.Size = new System.Drawing.Size(221, 22);
         this.mnuClientsRefreshAll.Text = "&Refresh All";
         this.mnuClientsRefreshAll.Click += new System.EventHandler(this.mnuClientsRefreshAll_Click);
         // 
         // mnuView
         // 
         this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewShowHideLog,
            this.mnuViewSep1,
            this.mnuViewToggleDateTime});
         this.mnuView.Name = "mnuView";
         this.mnuView.Size = new System.Drawing.Size(41, 20);
         this.mnuView.Text = "&View";
         // 
         // mnuViewShowHideLog
         // 
         this.mnuViewShowHideLog.Name = "mnuViewShowHideLog";
         this.mnuViewShowHideLog.ShortcutKeys = System.Windows.Forms.Keys.F8;
         this.mnuViewShowHideLog.Size = new System.Drawing.Size(215, 22);
         this.mnuViewShowHideLog.Text = "&Show/Hide Log File";
         this.mnuViewShowHideLog.Click += new System.EventHandler(this.mnuViewShowHideLog_Click);
         // 
         // mnuViewSep1
         // 
         this.mnuViewSep1.Name = "mnuViewSep1";
         this.mnuViewSep1.Size = new System.Drawing.Size(212, 6);
         // 
         // mnuViewToggleDateTime
         // 
         this.mnuViewToggleDateTime.Name = "mnuViewToggleDateTime";
         this.mnuViewToggleDateTime.ShortcutKeys = System.Windows.Forms.Keys.F9;
         this.mnuViewToggleDateTime.Size = new System.Drawing.Size(215, 22);
         this.mnuViewToggleDateTime.Text = "Toggle &Date/Time Style";
         this.mnuViewToggleDateTime.Click += new System.EventHandler(this.mnuViewToggleDateTime_Click);
         // 
         // mnuTools
         // 
         this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsMessages,
            this.mnuToolsDownloadProjects});
         this.mnuTools.Name = "mnuTools";
         this.mnuTools.Size = new System.Drawing.Size(44, 20);
         this.mnuTools.Text = "&Tools";
         // 
         // mnuToolsMessages
         // 
         this.mnuToolsMessages.Name = "mnuToolsMessages";
         this.mnuToolsMessages.Size = new System.Drawing.Size(246, 22);
         this.mnuToolsMessages.Text = "&Show/Hide Messages Window";
         this.mnuToolsMessages.Click += new System.EventHandler(this.mnuToolsMessages_Click);
         // 
         // mnuToolsDownloadProjects
         // 
         this.mnuToolsDownloadProjects.Name = "mnuToolsDownloadProjects";
         this.mnuToolsDownloadProjects.Size = new System.Drawing.Size(246, 22);
         this.mnuToolsDownloadProjects.Text = "Download Projects From Stanford";
         this.mnuToolsDownloadProjects.Click += new System.EventHandler(this.mnuToolsDownloadProjects_Click);
         // 
         // mnuHelp
         // 
         this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpContents,
            this.mnuHelpIndex,
            this.mnuHelpSep1,
            this.mnuHelpAbout});
         this.mnuHelp.Name = "mnuHelp";
         this.mnuHelp.Size = new System.Drawing.Size(40, 20);
         this.mnuHelp.Text = "&Help";
         // 
         // mnuHelpContents
         // 
         this.mnuHelpContents.Image = global::HFM.Properties.Resources.HelpContents;
         this.mnuHelpContents.Name = "mnuHelpContents";
         this.mnuHelpContents.Size = new System.Drawing.Size(129, 22);
         this.mnuHelpContents.Text = "&Contents";
         this.mnuHelpContents.Visible = false;
         this.mnuHelpContents.Click += new System.EventHandler(this.mnuHelpContents_Click);
         // 
         // mnuHelpIndex
         // 
         this.mnuHelpIndex.Name = "mnuHelpIndex";
         this.mnuHelpIndex.Size = new System.Drawing.Size(129, 22);
         this.mnuHelpIndex.Text = "&Index";
         this.mnuHelpIndex.Visible = false;
         this.mnuHelpIndex.Click += new System.EventHandler(this.mnuHelpIndex_Click);
         // 
         // mnuHelpSep1
         // 
         this.mnuHelpSep1.Name = "mnuHelpSep1";
         this.mnuHelpSep1.Size = new System.Drawing.Size(126, 6);
         this.mnuHelpSep1.Visible = false;
         // 
         // mnuHelpAbout
         // 
         this.mnuHelpAbout.Image = global::HFM.Properties.Resources.About;
         this.mnuHelpAbout.Name = "mnuHelpAbout";
         this.mnuHelpAbout.Size = new System.Drawing.Size(129, 22);
         this.mnuHelpAbout.Text = "&About";
         this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
         // 
         // openConfigDialog
         // 
         this.openConfigDialog.DefaultExt = "hfm";
         this.openConfigDialog.Filter = "HFM Configuration Files|*.hfm";
         this.openConfigDialog.RestoreDirectory = true;
         // 
         // saveConfigDialog
         // 
         this.saveConfigDialog.DefaultExt = "hfm";
         this.saveConfigDialog.Filter = "HFM Configuration Files|*.hfm";
         // 
         // txtLogFile
         // 
         this.txtLogFile.BackColor = System.Drawing.Color.White;
         this.txtLogFile.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtLogFile.Location = new System.Drawing.Point(0, 0);
         this.txtLogFile.Multiline = true;
         this.txtLogFile.Name = "txtLogFile";
         this.txtLogFile.ReadOnly = true;
         this.txtLogFile.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtLogFile.Size = new System.Drawing.Size(988, 356);
         this.txtLogFile.TabIndex = 1;
         // 
         // gridContextMenuStrip
         // 
         this.gridContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuContextClientsRefreshSelected,
            this.mnuContextClientsSep1,
            this.mnuContextClientsAdd,
            this.mnuContextClientsEdit,
            this.mnuContextClientsDelete,
            this.mnuContextClientsSep2,
            this.mnuContextClientsViewCachedLog,
            this.mnuContextClientsViewClientFiles});
         this.gridContextMenuStrip.Name = "contextMenuStrip1";
         this.gridContextMenuStrip.Size = new System.Drawing.Size(186, 148);
         // 
         // mnuContextClientsRefreshSelected
         // 
         this.mnuContextClientsRefreshSelected.Name = "mnuContextClientsRefreshSelected";
         this.mnuContextClientsRefreshSelected.Size = new System.Drawing.Size(185, 22);
         this.mnuContextClientsRefreshSelected.Text = "Refresh Selected";
         this.mnuContextClientsRefreshSelected.Click += new System.EventHandler(this.mnuClientsRefreshSelected_Click);
         // 
         // mnuContextClientsSep1
         // 
         this.mnuContextClientsSep1.Name = "mnuContextClientsSep1";
         this.mnuContextClientsSep1.Size = new System.Drawing.Size(182, 6);
         // 
         // mnuContextClientsAdd
         // 
         this.mnuContextClientsAdd.Name = "mnuContextClientsAdd";
         this.mnuContextClientsAdd.Size = new System.Drawing.Size(185, 22);
         this.mnuContextClientsAdd.Text = "Add Client";
         this.mnuContextClientsAdd.Click += new System.EventHandler(this.mnuClientsAdd_Click);
         // 
         // mnuContextClientsEdit
         // 
         this.mnuContextClientsEdit.Name = "mnuContextClientsEdit";
         this.mnuContextClientsEdit.Size = new System.Drawing.Size(185, 22);
         this.mnuContextClientsEdit.Text = "Edit Client";
         this.mnuContextClientsEdit.Click += new System.EventHandler(this.mnuClientsEdit_Click);
         // 
         // mnuContextClientsDelete
         // 
         this.mnuContextClientsDelete.Name = "mnuContextClientsDelete";
         this.mnuContextClientsDelete.Size = new System.Drawing.Size(185, 22);
         this.mnuContextClientsDelete.Text = "Delete Client";
         this.mnuContextClientsDelete.Click += new System.EventHandler(this.mnuClientsDelete_Click);
         // 
         // mnuContextClientsSep2
         // 
         this.mnuContextClientsSep2.Name = "mnuContextClientsSep2";
         this.mnuContextClientsSep2.Size = new System.Drawing.Size(182, 6);
         // 
         // mnuContextClientsViewCachedLog
         // 
         this.mnuContextClientsViewCachedLog.Name = "mnuContextClientsViewCachedLog";
         this.mnuContextClientsViewCachedLog.Size = new System.Drawing.Size(185, 22);
         this.mnuContextClientsViewCachedLog.Text = "View Cached Log File";
         this.mnuContextClientsViewCachedLog.Click += new System.EventHandler(this.mnuClientsViewCachedLog_Click);
         // 
         // mnuContextClientsViewClientFiles
         // 
         this.mnuContextClientsViewClientFiles.Name = "mnuContextClientsViewClientFiles";
         this.mnuContextClientsViewClientFiles.Size = new System.Drawing.Size(185, 22);
         this.mnuContextClientsViewClientFiles.Text = "View Client Files";
         this.mnuContextClientsViewClientFiles.Click += new System.EventHandler(this.mnuClientsViewClientFiles_Click);
         // 
         // webGenTimer
         // 
         this.webGenTimer.Interval = 900000;
         this.webGenTimer.Tick += new System.EventHandler(this.webGenTimer_Tick);
         // 
         // splitContainer1
         // 
         this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
         this.splitContainer1.IsSplitterFixed = true;
         this.splitContainer1.Location = new System.Drawing.Point(0, 24);
         this.splitContainer1.Name = "splitContainer1";
         this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
         // 
         // splitContainer1.Panel1
         // 
         this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
         this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
         // 
         // splitContainer1.Panel2
         // 
         this.splitContainer1.Panel2.Controls.Add(this.txtLogFile);
         this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this.splitContainer1.Size = new System.Drawing.Size(988, 720);
         this.splitContainer1.SplitterDistance = 360;
         this.splitContainer1.TabIndex = 6;
         // 
         // dataGridView1
         // 
         this.dataGridView1.AllowUserToAddRows = false;
         this.dataGridView1.AllowUserToDeleteRows = false;
         this.dataGridView1.AllowUserToOrderColumns = true;
         this.dataGridView1.AllowUserToResizeRows = false;
         dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
         this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
         this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
         dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
         dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
         dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
         dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
         dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
         dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
         this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
         this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
         dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
         dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
         dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
         dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
         dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
         this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle6;
         this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.dataGridView1.Location = new System.Drawing.Point(0, 0);
         this.dataGridView1.MultiSelect = false;
         this.dataGridView1.Name = "dataGridView1";
         this.dataGridView1.ReadOnly = true;
         this.dataGridView1.RowHeadersVisible = false;
         this.dataGridView1.RowTemplate.Height = 18;
         this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
         this.dataGridView1.ShowCellToolTips = false;
         this.dataGridView1.Size = new System.Drawing.Size(988, 360);
         this.dataGridView1.TabIndex = 0;
         this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
         this.dataGridView1.Sorted += new System.EventHandler(this.dataGridView1_Sorted);
         this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
         this.dataGridView1.ColumnDividerDoubleClick += new System.Windows.Forms.DataGridViewColumnDividerDoubleClickEventHandler(this.dataGridView1_ColumnDividerDoubleClick);
         this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
         // 
         // frmMain
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(988, 773);
         this.Controls.Add(this.splitContainer1);
         this.Controls.Add(this.AppMenu);
         this.Controls.Add(this.statusStrip);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.Name = "frmMain";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "HFM.NET";
         this.Shown += new System.EventHandler(this.frmMain_Shown);
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMainNew_FormClosing);
         this.Resize += new System.EventHandler(this.frmMain_Resize);
         this.statusStrip.ResumeLayout(false);
         this.statusStrip.PerformLayout();
         this.notifyMenu.ResumeLayout(false);
         this.AppMenu.ResumeLayout(false);
         this.AppMenu.PerformLayout();
         this.gridContextMenuStrip.ResumeLayout(false);
         this.splitContainer1.Panel1.ResumeLayout(false);
         this.splitContainer1.Panel2.ResumeLayout(false);
         this.splitContainer1.Panel2.PerformLayout();
         this.splitContainer1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Timer bgWorkTimer;
      private System.Windows.Forms.StatusStrip statusStrip;
      private System.Windows.Forms.ToolStripStatusLabel statusLabelLeft;
      private System.Windows.Forms.ToolStripStatusLabel statusLabelHosts;
      private System.Windows.Forms.ToolStripStatusLabel statusLabelPPW;
      private System.Windows.Forms.ToolStripStatusLabel statusLabelRight;
      private System.Windows.Forms.ContextMenuStrip notifyMenu;
      private System.Windows.Forms.ToolStripMenuItem mnuNotifyRst;
      private System.Windows.Forms.ToolStripMenuItem mnuNotifyMin;
      private System.Windows.Forms.ToolStripMenuItem mnuNotifyMax;
      private System.Windows.Forms.ToolStripSeparator mnuNotifySep1;
      private System.Windows.Forms.ToolStripMenuItem mnuNotifyQuit;
      private System.Windows.Forms.NotifyIcon notifyIcon;
      private System.Windows.Forms.MenuStrip AppMenu;
      private System.Windows.Forms.ToolStripMenuItem mnuFile;
      private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
      private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
      private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
      private System.Windows.Forms.ToolStripMenuItem mnuFileSaveas;
      private System.Windows.Forms.ToolStripSeparator mnuFileSep1;
      private System.Windows.Forms.ToolStripMenuItem mnuFileQuit;
      private System.Windows.Forms.ToolStripMenuItem mnuEdit;
      private System.Windows.Forms.ToolStripMenuItem mnuEditPreferences;
      private System.Windows.Forms.ToolStripMenuItem mnuHelp;
      private System.Windows.Forms.ToolStripMenuItem mnuHelpContents;
      private System.Windows.Forms.ToolStripMenuItem mnuHelpIndex;
      private System.Windows.Forms.ToolStripSeparator mnuHelpSep1;
      private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
      private System.Windows.Forms.OpenFileDialog openConfigDialog;
      private System.Windows.Forms.SaveFileDialog saveConfigDialog;
      private System.Windows.Forms.ToolStripMenuItem mnuClients;
      private System.Windows.Forms.ToolStripMenuItem mnuClientsAdd;
      private System.Windows.Forms.ToolStripSeparator mnuClientsSep1;
      private System.Windows.Forms.ToolStripMenuItem mnuClientsEdit;
      private System.Windows.Forms.ToolStripMenuItem mnuClientsDelete;
      private System.Windows.Forms.ToolStripSeparator mnuClientsSep2;
      private System.Windows.Forms.ToolStripMenuItem mnuClientsRefreshSelected;
      private System.Windows.Forms.ToolStripMenuItem mnuClientsRefreshAll;
      private System.Windows.Forms.TextBox txtLogFile;
      private System.Windows.Forms.ToolStripMenuItem mnuView;
      private System.Windows.Forms.ToolStripMenuItem mnuViewShowHideLog;
      private System.Windows.Forms.Timer webGenTimer;
      private System.Windows.Forms.ToolStripMenuItem mnuTools;
      private System.Windows.Forms.ToolStripMenuItem mnuToolsMessages;
      private System.Windows.Forms.ToolStripSeparator mnuClientsSep3;
      private System.Windows.Forms.ToolStripMenuItem mnuClientsViewCachedLog;
      private System.Windows.Forms.ToolStripMenuItem mnuToolsDownloadProjects;
      private System.Windows.Forms.ContextMenuStrip gridContextMenuStrip;
      private System.Windows.Forms.ToolStripMenuItem mnuContextClientsAdd;
      private System.Windows.Forms.ToolStripSeparator mnuContextClientsSep1;
      private System.Windows.Forms.ToolStripMenuItem mnuContextClientsRefreshSelected;
      private System.Windows.Forms.ToolStripMenuItem mnuContextClientsEdit;
      private System.Windows.Forms.ToolStripMenuItem mnuContextClientsDelete;
      private System.Windows.Forms.ToolStripSeparator mnuContextClientsSep2;
      private System.Windows.Forms.ToolStripMenuItem mnuContextClientsViewCachedLog;
      private System.Windows.Forms.ToolStripSeparator mnuViewSep1;
      private System.Windows.Forms.ToolStripMenuItem mnuViewToggleDateTime;
      private System.Windows.Forms.ToolStripMenuItem mnuClientsViewClientFiles;
      private System.Windows.Forms.ToolStripMenuItem mnuContextClientsViewClientFiles;
      private System.Windows.Forms.SplitContainer splitContainer1;
      private System.Windows.Forms.DataGridView dataGridView1;
   }
}
