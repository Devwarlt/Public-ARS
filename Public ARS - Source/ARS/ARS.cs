#region

using ARS.Assembly;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

#endregion

namespace ARS
{
    public class PublicARS : Form
	{
		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        internal static class ARSAssemblySettings
        {
            static string ARSdir;
            static string ARSusr;
            static int ARSsP;
            static int ARSwP;
            static int ARSrD;
            public static string ARSdirectory { get { return ARSdir; } set { ARSdir = value; } }
            public static string ARSuser { get { return ARSusr; } set { ARSusr = value; } }
            public static int ARSserverPort { get { return ARSsP; } set { ARSsP = value; } }
            public static int ARSwserverPort { get { return ARSwP; } set { ARSwP = value; } }
            public static int ARSrestartDelay { get { return ARSrD; } set { ARSrD = value; } }
            public static string ARSserverName = "server";
            public static string ARSwserverName = "wServer";
            public static string ARSAssemblyFile = "ARS";
            public static string ARSlocalhost = "127.0.0.1";
            public static string ARSwerfault = "WerFault";
            public static string ARSmysqld = "mysqld";
            public static string ARSpooledConnectionsError = "all pooled connections were";
            public static string ARSwserverError = "wServer.exe - Application Error";
            public static string ARSnull = "0";
        }

		private IContainer components = null;
		private System.Windows.Forms.Timer firstRegister;
		private ListBox dataBox;
        private Label arsTitle;
        private Button startButton;
        private Label versionLabel;
        private Label countdownLabel;
        private Label countdown;
        private System.Windows.Forms.Timer secondRegister;

		public PublicARS()
		{
			InitializeComponent();
		}

		private void loadARS(object sender, EventArgs e)
		{
            using (AssemblyAlgorithm ARSSettings = new AssemblyAlgorithm(ARSAssemblySettings.ARSAssemblyFile))
            {
                ARSAssemblySettings.ARSdirectory = ARSSettings.getDataTask<string>("DIR", "C");
                ARSAssemblySettings.ARSuser = ARSSettings.getDataTask<string>("USR", "Admin");
                ARSAssemblySettings.ARSserverPort = ARSSettings.getDataTask<int>("SERVER_PORT", "80");
                ARSAssemblySettings.ARSwserverPort = ARSSettings.getDataTask<int>("WSERVER_PORT", "2050");
                ARSAssemblySettings.ARSrestartDelay = ARSSettings.getDataTask<int>("RESTART_DELAY", "60");
                Process[] processes = Process.GetProcesses();
                Process[] array = processes;
			    if (!portCheckingHandler(ARSAssemblySettings.ARSserverPort))
			    {
				    for (int i = 0; i < array.Length; i++)
				    {
					    Process process = array[i];
					    if (process.ProcessName == ARSAssemblySettings.ARSserverName)
                            process.Kill();
                    }
                    dataBox.Items.Add(" [" + Convert.ToString(DateTime.Now) + "] Initializing first server.exe...");
                    Process.Start(ARSAssemblySettings.ARSdirectory + ":/Users/" + ARSAssemblySettings.ARSuser + "/Desktop/" + ARSAssemblySettings.ARSserverName + ".bat");
			    }
			    if (!portCheckingHandler(ARSAssemblySettings.ARSwserverPort))
			    {
				    for (int i = 0; i < array.Length; i++)
				    {
					    Process process = array[i];
					    if (process.ProcessName == ARSAssemblySettings.ARSwserverName)
                            process.Kill();
                    }
                    dataBox.Items.Add(" [" + Convert.ToString(DateTime.Now) + "] Initializing first wServer.exe...");
                    Process.Start(ARSAssemblySettings.ARSdirectory + ":/Users/" + ARSAssemblySettings.ARSuser + "/Desktop/" + ARSAssemblySettings.ARSwserverName + ".bat");
			    }
            }
		}

		private bool portCheckingHandler(int port)
		{
			bool result;
			using (TcpClient tcpClient = new TcpClient())
			{
				try
				{
					tcpClient.Connect(ARSAssemblySettings.ARSlocalhost, port);
				}
				catch (SocketException)
				{
					result = false;
					return result;
				}
				tcpClient.Close();
				result = true;
			}
			return result;
		}

		private void applicationsFatalErrorsHandler()
		{
			IntPtr hWnd = FindWindow(null, ARSAssemblySettings.ARSwserverError);
			if (hWnd.ToString() != ARSAssemblySettings.ARSnull)
			{
                SendMessage(hWnd, 16u, 0, 0);
				Process[] processesByName = Process.GetProcessesByName(ARSAssemblySettings.ARSwserverName);
				if (processesByName.GetLength(0) > 0)
                {
                    Thread.Sleep(
                        (int)TimeSpan.FromSeconds(5).TotalMilliseconds);
                    processesByName[0].Kill();
				}
            }
            IntPtr intPtr = FindWindow(null, ARSAssemblySettings.ARSpooledConnectionsError);
			if (hWnd.ToString() != ARSAssemblySettings.ARSnull)
			{
                SendMessage(hWnd, 16u, 0, 0);
				Process[] processesByName = Process.GetProcessesByName(ARSAssemblySettings.ARSmysqld);
				if (processesByName.GetLength(0) > 0)
                {
                    Thread.Sleep(
                        (int)TimeSpan.FromSeconds(5).TotalMilliseconds);
                    processesByName[0].Kill();
				}
			}
		}

        private void beginNetworkHandler(object sender, EventArgs e)
		{
            try
			{
				Process[] processes = Process.GetProcesses();
				Process[] array = processes;
                if (!portCheckingHandler(ARSAssemblySettings.ARSserverPort)) {
				    for (int i = 0; i < array.Length; i++)
				    {
					    Process process = array[i];
                        if (!this.portCheckingHandler(ARSAssemblySettings.ARSserverPort) && (process.ProcessName != ARSAssemblySettings.ARSserverName))
				        {
                            System.Threading.Thread.Sleep(
                                (int)System.TimeSpan.FromSeconds(15).TotalMilliseconds);
                            if (!this.portCheckingHandler(ARSAssemblySettings.ARSserverPort) && (process.ProcessName != ARSAssemblySettings.ARSserverName))
				            {
					            Process.Start(ARSAssemblySettings.ARSdirectory + ":/Users/" + ARSAssemblySettings.ARSuser + "/Desktop/" + ARSAssemblySettings.ARSserverName + ".bat");
                            }
                        }
                    }
                    dataBox.Items.Add(" [" + Convert.ToString(DateTime.Now) + "] New server.exe has been opened.");
                }
                if (!portCheckingHandler(ARSAssemblySettings.ARSwserverPort))
                {
				    for (int i = 0; i < array.Length; i++)
				    {
					    Process process = array[i];
                        if (!portCheckingHandler(ARSAssemblySettings.ARSwserverPort) && (process.ProcessName != ARSAssemblySettings.ARSwserverName))
				        {
                            Thread.Sleep(
                                (int)System.TimeSpan.FromSeconds(10).TotalMilliseconds);
                            if (!portCheckingHandler(ARSAssemblySettings.ARSwserverPort) && (process.ProcessName != ARSAssemblySettings.ARSwserverName))
				            {
					            Process.Start(ARSAssemblySettings.ARSdirectory + ":/Users/" + ARSAssemblySettings.ARSuser + "/Desktop/" + ARSAssemblySettings.ARSwserverName + ".bat");
                            }
                        }
                    }
                    dataBox.Items.Add(" [" + Convert.ToString(DateTime.Now) + "] New wServer.exe has been opened.");
                }
				Process[] arrayl = processes;
				for (int i = 0; i < arrayl.Length; i++)
				{
					Process process = arrayl[i];
					if (process.ProcessName == ARSAssemblySettings.ARSwerfault)
                    {
                        System.Threading.Thread.Sleep(
                            (int)TimeSpan.FromSeconds(10).TotalMilliseconds);
                        process.Kill();
                        dataBox.Items.Add(" [" + Convert.ToString(DateTime.Now) + "] wServer has been crashed!");
						if (process.ProcessName == ARSAssemblySettings.ARSwserverName)
                        {
                            System.Threading.Thread.Sleep(
                                (int)System.TimeSpan.FromSeconds(10).TotalMilliseconds);
                            process.Kill();
						}
					}
				}
			}
			catch (Exception ex)
			{
                dataBox.Items.Add(" [" + Convert.ToString(DateTime.Now) + "] FATAL ERROR! Error: " + Convert.ToString(ex));
			}
		}

		private void beginHandlers(object sender, EventArgs e)
		{
			applicationsFatalErrorsHandler();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PublicARS));
            this.firstRegister = new System.Windows.Forms.Timer(this.components);
            this.dataBox = new System.Windows.Forms.ListBox();
            this.secondRegister = new System.Windows.Forms.Timer(this.components);
            this.arsTitle = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.versionLabel = new System.Windows.Forms.Label();
            this.countdownLabel = new System.Windows.Forms.Label();
            this.countdown = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // firstRegister
            // 
            this.firstRegister.Enabled = true;
            this.firstRegister.Interval = 10000;
            this.firstRegister.Tick += new System.EventHandler(this.beginNetworkHandler);
            // 
            // dataBox
            // 
            this.dataBox.BackColor = System.Drawing.SystemColors.ControlText;
            this.dataBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dataBox.Cursor = System.Windows.Forms.Cursors.Cross;
            this.dataBox.ForeColor = System.Drawing.Color.Gold;
            this.dataBox.FormattingEnabled = true;
            this.dataBox.Location = new System.Drawing.Point(12, 26);
            this.dataBox.Name = "dataBox";
            this.dataBox.Size = new System.Drawing.Size(356, 119);
            this.dataBox.TabIndex = 0;
            this.dataBox.SelectedIndexChanged += new System.EventHandler(this.dataBox_SelectedIndexChanged);
            // 
            // secondRegister
            // 
            this.secondRegister.Enabled = true;
            this.secondRegister.Interval = 1000;
            this.secondRegister.Tick += new System.EventHandler(this.beginHandlers);
            // 
            // arsTitle
            // 
            this.arsTitle.AutoSize = true;
            this.arsTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.arsTitle.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.arsTitle.Location = new System.Drawing.Point(13, 7);
            this.arsTitle.Name = "arsTitle";
            this.arsTitle.Size = new System.Drawing.Size(141, 13);
            this.arsTitle.TabIndex = 4;
            this.arsTitle.Text = "Public ARS - Event log:";
            // 
            // startButton
            // 
            this.startButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.startButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.startButton.Enabled = false;
            this.startButton.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.startButton.Location = new System.Drawing.Point(293, 2);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 5;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = false;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.versionLabel.Location = new System.Drawing.Point(328, 148);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(40, 13);
            this.versionLabel.TabIndex = 6;
            this.versionLabel.Text = "1.0.0.0";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // countdownLabel
            // 
            this.countdownLabel.AutoSize = true;
            this.countdownLabel.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.countdownLabel.Enabled = false;
            this.countdownLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.countdownLabel.ForeColor = System.Drawing.Color.Gainsboro;
            this.countdownLabel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.countdownLabel.Location = new System.Drawing.Point(12, 148);
            this.countdownLabel.Name = "countdownLabel";
            this.countdownLabel.Size = new System.Drawing.Size(93, 13);
            this.countdownLabel.TabIndex = 2;
            this.countdownLabel.Text = "Time remaning:";
            this.countdownLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.countdownLabel.Click += new System.EventHandler(this.labelEvent);
            // 
            // countdown
            // 
            this.countdown.AutoSize = true;
            this.countdown.Enabled = false;
            this.countdown.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.countdown.Location = new System.Drawing.Point(102, 148);
            this.countdown.Name = "countdown";
            this.countdown.Size = new System.Drawing.Size(49, 13);
            this.countdown.TabIndex = 3;
            this.countdown.Text = "00:00:00";
            this.countdown.UseWaitCursor = true;
            // 
            // PublicARS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(380, 170);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.arsTitle);
            this.Controls.Add(this.countdown);
            this.Controls.Add(this.countdownLabel);
            this.Controls.Add(this.dataBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PublicARS";
            this.Text = "Public ARS by LoESoft";
            this.Load += new System.EventHandler(this.loadARS);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        private void dataBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void labelEvent(object sender, EventArgs e)
        {

        }
    }
}
