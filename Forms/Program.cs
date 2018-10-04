using DocumentSigner.Helpers;
using DocumentSigner.Properties;
using Microsoft.Owin.Hosting;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace DocumentSigner.Forms
{
    class Program : Form
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var app = new Program();

            try
            {
                app.StartWebServer();
                Application.Run(app);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Erro ao iniciar o serviço Web", ex, EventLogEntryType.Error);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;

            if (ex == null)
            {
                LogHelper.WriteLog("Erro não esperado " + Environment.NewLine + e.ExceptionObject.ToString(), EventLogEntryType.Error);
            }
            else
            {
                LogHelper.WriteLog("Erro não esperado", ex, EventLogEntryType.Error);
            }
        }

        private IDisposable WebServer;
        private NotifyIcon _TrayIcon;

        public Program()
        {
            var version = Assembly
                .GetExecutingAssembly()
                .GetName()
                .Version
                .ToString();
            version = version.Substring(0, version.LastIndexOf('.'));

            Visible = false;
            Width = 0;
            Height = 0;
            StartPosition = FormStartPosition.CenterScreen;

            _TrayIcon = new NotifyIcon();
            _TrayIcon.Icon = Resources.favicon;

            _TrayIcon.Visible = true;
            _TrayIcon.Text = "DocumentSigner v" + version + " - Desenvolvido por Fabiohvp";
        }

        public void StartWebServer()
        {
            var localhostAddress = ConfigurationManager.AppSettings[SettingsHelper.KEY_LOCALHOST_ADDRESS];

            try
            {
                WebServer = WebApp.Start<Startup>(url: localhostAddress);
                LogHelper.WriteLog("WebServer - Has started on " + localhostAddress, EventLogEntryType.SuccessAudit);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.            
            base.OnLoad(e);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (WebServer != null)
            {
                WebServer.Dispose();
            }

            LogHelper.WriteLog("WebServer - Has been closed", EventLogEntryType.SuccessAudit);

            if (isDisposing)
            {
                // Release the icon resource.
                _TrayIcon.Dispose();
                //_TrayMenu.Dispose();
            }

            base.Dispose(isDisposing);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Program));
            this.SuspendLayout();
            // 
            // Program
            // 
            resources.ApplyResources(this, "$this");
            this.Name = "Program";
            this.ResumeLayout(false);
        }
    }
}
